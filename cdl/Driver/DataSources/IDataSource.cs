using DataModel;

namespace Driver.DataSources;

public interface IDataSource : IDisposable
{
    public Task<SensorMeasure[]> Read(DateTime currentTime);
}