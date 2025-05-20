using Microsoft.AspNetCore.Mvc;
using Mini_MES_API.Handlers;
using Mini_MES_API.Services;
using Mini_MES_API.Stores;
using MQTTnet.Client;

namespace Mini_MES_API.Api;


public static class MachineStateEndpoints
{
    public static void MapMachineStateEndpoints(this WebApplication app)
    {
        app.UseCors("AllowFrontend");
        app.MapGet("/factory/state", MachineStateHandlers.GetFactorySnapshot)
            .WithDescription("Get current state of all machines.")
            .WithOpenApi();
        
        app.UseCors("AllowFrontend");
        app.MapGet("/factory/{id}/poweroff", MachineStateHandlers.ShutdownMachine)
            .WithDescription("Shut down machine via MQTT.")
            .WithOpenApi();
    }
}
