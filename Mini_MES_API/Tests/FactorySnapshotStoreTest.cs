/*using Xunit;
using Mini_MES_API.Stores;

public class FactorySnapshotStoreTests
{
    [Fact]
    public void UpdateSnapshot_ValidJson_ParsesAndStores()
    {
        // Arrange
        var store = new FactorySnapshotStore();
        string json = @"{
            ""machine1"": {
                ""sensors"": { ""temp"": 42.0 },
                ""state"": ""running""
            }
        }";

        // Act
        store.UpdateSnapshot(json);
        var snapshot = store.GetSnapshot();

        // Assert
        Assert.True(snapshot.ContainsKey("machine1"));
        Assert.Equal("running", snapshot["machine1"].state.ToString());
    }

    [Fact]
    public void UpdateSnapshot_InvalidJson_DoesNotCrash()
    {
        var store = new FactorySnapshotStore();

        var exception = Record.Exception(() =>
            store.UpdateSnapshot("not a json")
        );

        Assert.Null(exception); // Should swallow JSON exceptions
    }
}*/