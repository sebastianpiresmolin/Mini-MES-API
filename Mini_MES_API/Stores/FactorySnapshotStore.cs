namespace Mini_MES_API.Stores;

public class FactorySnapshotStore
{
    private readonly object _lock = new();
    private string _latestJson = "{}";

    public void UpdateSnapshot(string json)
    {
        lock (_lock)
        {
            _latestJson = json;
        }
    }

    public string GetSnapshot()
    {
        lock (_lock)
        {
            return _latestJson;
        }
    }
}