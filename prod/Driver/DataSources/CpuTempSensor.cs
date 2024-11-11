using DataModel;

namespace Driver.DataSources;

public class CpuTempSensor : IDisposable, IDataSource
{
    private const string TempFilePath = "/sys/class/thermal/thermal_zone0/temp";
    private readonly FileStream _tempFileStream;
    private readonly StreamReader _tempStreamReader;
    private bool _disposed;
    private readonly bool _enabled;

    public CpuTempSensor()
    {
        if (File.Exists(TempFilePath))
        {
            _enabled = true;
            _tempFileStream = new FileStream(TempFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _tempStreamReader = new StreamReader(_tempFileStream);
            
        }
    }

    public double? Read()
    {
        if (!_enabled)
            return null;
        
        if (_disposed)
            throw new ObjectDisposedException(nameof(CpuTempSensor));
        
        _tempFileStream.Seek(0, SeekOrigin.Begin);
        var tempStr = _tempStreamReader.ReadLine();

        if (!double.TryParse(tempStr, out var tempMilliCelsius))
            return null;
        
        var tempCelsius = tempMilliCelsius / 1000.0;
        return tempCelsius;

    }

    public void Dispose()
    {
        if (_disposed || !_enabled)
            return;
        _disposed = true;
        _tempStreamReader?.Dispose();
        _tempFileStream?.Dispose();
    }

    public async Task<SensorMeasure[]> Read(DateTime currentTime)
    {
        var temp = Read();
        if (temp == null) return await Task.FromResult(Array.Empty<SensorMeasure>());
        
        return await Task.FromResult(new[]
        {
            new SensorMeasure(currentTime, new SensorData(SensorIds.Temp, Convert.ToSingle(temp)))
        });
    }
}
