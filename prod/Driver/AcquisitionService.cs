using DataModel;
using Driver.DataSources;

namespace Driver;

public class AcquisitionService(params IDataSource[] sources)
{
    public async Task<SensorMeasure[]> Read(DateTime currentTime)
    {
        var tasks = sources.Select(s => s.Read(currentTime));
        var results = await Task.WhenAll(tasks);
        var measures = results.SelectMany(r => r);
        return measures.ToArray();
    }
}