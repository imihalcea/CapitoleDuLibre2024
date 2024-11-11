using System.Collections.Concurrent;
namespace AcquireServer;

using SerialNumber = string;
using SensorId = string;

public class InMemoryStorage : IStoreDeviceMeasures
{
    private readonly ConcurrentDictionary<SerialNumber, Dictionary<DateTime, List<SensorData>>> _data;
    
    public InMemoryStorage()
    {
        _data = new ConcurrentDictionary<SerialNumber, Dictionary<DateTime, List<SensorData>>>();
    }

    public void Save(DeviceData deviceData)
    {
        if (!_data.TryGetValue(deviceData.SerialNumber, out var measures))
        {
            measures = new Dictionary<DateTime, List<SensorData>>();
            _data.TryAdd(deviceData.SerialNumber, measures);
        }

        foreach (var measure in deviceData.Data)
        {
            if (!measures.TryGetValue(measure.At, out var values))
            {
                values = [];
                measures.TryAdd(measure.At, values);
            }

            values.Add(measure.SensorData);
        }
    }
    
    public List<Measures> MeasuresByDevice(SerialNumber serialNumber)
    {
        if (!_data.TryGetValue(serialNumber, out var measures))
        {
            return new List<Measures>();
        }

        return measures.Select(m => new Measures(m.Key, m.Value)).ToList();
    }
    
    public List<DeviceMeasures> AllMeasures()
    {
        return _data.Select(d => new DeviceMeasures(d.Key, d.Value.Select(m => new Measures(m.Key, m.Value)).ToList())).ToList();
    }
    
}

public interface IStoreDeviceMeasures
{
    public void Save(DeviceData deviceData);
    public List<Measures> MeasuresByDevice(SerialNumber serialNumber);
    public List<DeviceMeasures> AllMeasures();
}