using Mini_MES_API.Stores;
using Microsoft.AspNetCore.Http;

namespace Mini_MES_API.Handlers;

public class MachineStateHandlers
{
    public static IResult GetFactorySnapshot(FactorySnapshotStore store)
    {
        var snapshot = store.GetSnapshot();
        return Results.Content(snapshot, "application/json");
    }
}