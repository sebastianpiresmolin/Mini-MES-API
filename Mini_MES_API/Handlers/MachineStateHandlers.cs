using System.Text.Json;
using Mini_MES_API.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MQTTnet;
using MQTTnet.Client;


namespace Mini_MES_API.Handlers;

public class MachineStateHandlers
{
    public static IResult GetFactorySnapshot(FactorySnapshotStore store)
    {
        var snapshot = store.GetSnapshot();
        return Results.Content(snapshot, "application/json");
    }
    
    public static async Task<IResult> ShutdownMachine(
        int id,
        [FromServices] IMqttClient mqttClient
    )
    {
        if (!mqttClient.IsConnected)
        {
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("localhost", 1883)
                .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V311)
                .Build();
            await mqttClient.ConnectAsync(options);
        }

        var topic = $"factory/machine{id}/state";
        var payload = JsonSerializer.Serialize(new { state = "stopped" });

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();

        for (int i = 0; i < 10; i++)
        {
            await mqttClient.PublishAsync(message);
        }

        return Results.Ok(new { message = $"Shutdown published to {topic} 10 times." });
    }
    
    public static async Task<IResult> PowerUpMachine(
        int id,
        [FromServices] IMqttClient mqttClient
    )
    {
        if (!mqttClient.IsConnected)
        {
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("localhost", 1883)
                .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V311)
                .Build();
            await mqttClient.ConnectAsync(options);
        }

        var topic = $"factory/machine{id}/state";
        var payload = JsonSerializer.Serialize(new { state = "start" });

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();

        for (int i = 0; i < 10; i++)
        {
            await mqttClient.PublishAsync(message);
        }

        return Results.Ok(new { message = $"Power up published to {topic} 10 times." });
    }
}