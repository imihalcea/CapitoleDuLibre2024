namespace Driver;

public class CpuTempSensor : IDisposable
{
    private const string TempFilePath = "/sys/class/thermal/thermal_zone0/temp";
    private readonly FileStream _tempFileStream;
    private readonly StreamReader _tempStreamReader;
    private bool _disposed;

    public CpuTempSensor()
    {
        _tempFileStream = new FileStream(TempFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        _tempStreamReader = new StreamReader(_tempFileStream);
    }

    public double? Read()
    {
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
        if (_disposed)
            return;
        _disposed = true;
        _tempStreamReader?.Dispose();
        _tempFileStream?.Dispose();
    }
}
