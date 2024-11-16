namespace AcquireServer;

public class DeviceMeasuresApi(IStoreDeviceMeasures store)
{

    public async Task<bool> SaveMeasures(DeviceData deviceData)
    {
        store.Save(deviceData);
        return await Task.FromResult(true);
    }

    public Task<List<Measures>> GetMeasuresByDevice(string serialNumber)
    {
        var measuresByDevice = store.MeasuresByDevice(serialNumber);
        return Task.FromResult(measuresByDevice);
    }

    public Task<List<DeviceMeasures>> GetAllMeasures()
    {
        var deviceMeasuresList = store.AllMeasures();
        return Task.FromResult(deviceMeasuresList);
    }
    
}