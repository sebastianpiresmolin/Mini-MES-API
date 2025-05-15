using Mini_MES_API.Handlers;
using Mini_MES_API.Services;
using Mini_MES_API.Stores;

namespace Mini_MES_API.Api;


public static class MachineStateEndpoints
{
    public static void MapMachineStateEndpoints(this WebApplication app)
    {
        app.MapGet("/factory/state", MachineStateHandlers.GetFactorySnapshot)
            .WithDescription("Get current state of all machines.")
            .WithOpenApi();
    }
}
