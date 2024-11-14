using System.Collections.Concurrent;

namespace Driver;

public class DataStore
{
    private readonly ConcurrentDictionary<string, float> _sensorData = new();
    
    
    public void Update(string key, float value)
    {
        _sensorData[key] = value;
    }
    
    public float? Get(string key)
    {
        return _sensorData.TryGetValue(key, out var value) ? value : null;
    }
}