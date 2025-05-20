using Mini_MES_API.Stores;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Packets;
using MQTTnet.Protocol;

namespace Mini_MES_API.Services;

public class FactoryMqttListener : BackgroundService
{
    private readonly FactorySnapshotStore _snapshotStore;

    public FactoryMqttListener(FactorySnapshotStore snapshotStore)
    {
        _snapshotStore = snapshotStore;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new MqttFactory();
        var client = factory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost", 1883)
            .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V311)
            .Build();

        client.ApplicationMessageReceivedAsync += async args =>
        {
            var topic = args.ApplicationMessage.Topic;
            var payload = args.ApplicationMessage.ConvertPayloadToString();

            if (topic == "factory/state_snapshot")
            {
                _snapshotStore.UpdateSnapshot(payload);
            }

            await Task.CompletedTask;
        };
        
        client.ConnectedAsync += async e =>
        {
            await client.SubscribeAsync("factory/state_snapshot");
        };
        
        client.DisconnectedAsync += async e =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); 
                    await client.ConnectAsync(options, stoppingToken);
                    break;
                }
                catch
                {
                    // ignored
                }
            }
        };
        
        await client.ConnectAsync(options, stoppingToken);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}