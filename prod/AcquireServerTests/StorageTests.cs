using AcquireServer;

namespace AcquireServerTests;

public class Tests
{
    [Test]
    public void SaveDeviceData()
    {
        var sut = new InMemoryStorage();
        var deviceData = new DeviceData("123", [
            new SensorMeasure(T.T0, new SensorData("Probe1", 1.0f)),
            new SensorMeasure(T.T0, new SensorData("Probe2", 2.0f)),
            new SensorMeasure(T.T0, new SensorData("Probe3", 3.0f))
        ]); 
        
        sut.Save(deviceData);
        var actual = sut.MeasuresByDevice("123");
        
        var expected = new List<Measures>
        {
            new(T.T0, [
                new SensorData("Probe1", 1.0f),
                new SensorData("Probe2", 2.0f),
                new SensorData("Probe3", 3.0f)
            ])
        };
        
        Assert.That(actual[0].SensorsMeasures, Is.EquivalentTo(expected[0].SensorsMeasures));
    }
}

public static class T
{
    public static DateTime T0 => new(TimeSpan.Zero.Ticks);
    public static DateTime T1 => T0.AddSeconds(1);
    
    public static DateTime T2 => T1.AddSeconds(1);
}