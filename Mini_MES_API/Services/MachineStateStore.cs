namespace Mini_MES_API.Services;

using System.Collections.Concurrent;
using System.Text.Json;

public class MachineStateStore
{
    private readonly ConcurrentDictionary<string, JsonElement> _state = new();

    public void UpdateSnapshot(string json)
    {
        var parsed = JsonDocument.Parse(json).RootElement;
        foreach (var machine in parsed.EnumerateObject())
        {
            _state[machine.Name] = machine.Value;
        }
    }

    public IReadOnlyDictionary<string, JsonElement> GetSnapshot() => _state;
}
