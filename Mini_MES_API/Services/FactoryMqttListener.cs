using Mini_MES_API.Stores;
using MQTTnet;
using MQTTnet.Client;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Mini_MES_API.Services;

public class FactoryMqttListener : BackgroundService
{
    private readonly FactorySnapshotStore _snapshotStore;
    private readonly ILogger<FactoryMqttListener> _logger;

    public FactoryMqttListener(FactorySnapshotStore snapshotStore, ILogger<FactoryMqttListener> logger)
    {
        _snapshotStore = snapshotStore;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new MqttFactory();
        var client = factory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer("mosquitto", 1883)
            .WithClientId($"mini-mes-api-{Guid.NewGuid()}")
            .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V311)
            .Build();

        client.ApplicationMessageReceivedAsync += async args =>
        {
            var topic = args.ApplicationMessage.Topic;
            var payload = args.ApplicationMessage.Payload;
            
            _logger.LogInformation(
                "Message received: Topic = {Topic}, Payload Present = {HasPayload}", 
                topic, payload != null && payload.Length > 0);
            
            if (payload != null && payload.Length > 0)
            {
                var payloadString = Encoding.UTF8.GetString(payload);
                _logger.LogInformation("Payload: {Payload}", payloadString);
                
                if (topic == "factory/state_snapshot")
                {
                    _snapshotStore.UpdateSnapshot(payloadString);
                }
            }
            else
            {
                _logger.LogWarning("Received empty payload on topic {Topic}", topic);
            }

            await Task.CompletedTask;
        };
        
        client.ConnectedAsync += async e =>
        {
            _logger.LogInformation("Connected to MQTT broker");
            var result = await client.SubscribeAsync("factory/state_snapshot");
            _logger.LogInformation("Subscribed to topics: {Result}", string.Join(", ", result.Items.Select(x => $"{x.TopicFilter.Topic} ({x.ResultCode})")));
        };
        
        client.DisconnectedAsync += async e =>
        {
            _logger.LogWarning("Disconnected from MQTT broker: {Reason}", e.Reason);
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Attempting to reconnect...");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); 
                    await client.ConnectAsync(options, stoppingToken);
                    _logger.LogInformation("Reconnected successfully");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to reconnect");
                }
            }
        };
        
        try
        {
            _logger.LogInformation("Connecting to MQTT broker...");
            await client.ConnectAsync(options, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to MQTT broker");
        }
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}