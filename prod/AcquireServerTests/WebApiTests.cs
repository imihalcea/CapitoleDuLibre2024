using System.Net.Http.Json;
using AcquireServer;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AcquireServerTests;

public class WebApiTests
{
    private HttpClient _client;

    [SetUp]
    public void Setup()
    {
        var application = new WebApplicationFactory<Program>();
        _client = application.CreateClient();
    }

    [Test]
    public async Task SaveMeasures_ShouldReturnOk()
    {
        var deviceData = new DeviceData("123", new List<SensorMeasure>
        {
            new SensorMeasure(T.T0, new SensorData("Probe1", 1.0f)),
            new SensorMeasure(T.T0, new SensorData("Probe2", 2.0f)),
            new SensorMeasure(T.T0, new SensorData("Probe3", 3.0f))
        });

        var response = await _client.PostAsJsonAsync("/measures", deviceData);

        response.EnsureSuccessStatusCode();
    }

    [Test]
    public async Task GetMeasuresByDevice_ShouldReturnMeasures()
    {
        var deviceData = new DeviceData("123", new List<SensorMeasure>
        {
            new SensorMeasure(T.T0, new SensorData("Probe1", 1.0f)),
            new SensorMeasure(T.T0, new SensorData("Probe2", 2.0f)),
            new SensorMeasure(T.T0, new SensorData("Probe3", 3.0f))
        });

        await _client.PostAsJsonAsync("/measures", deviceData);

        var response = await _client.GetAsync("/measures/123");
        response.EnsureSuccessStatusCode();

        var measures = await response.Content.ReadFromJsonAsync<List<Measures>>();
        Assert.IsNotNull(measures);
        Assert.That(measures.Count, Is.EqualTo(1));
        Assert.That(measures[0].SensorsMeasures.Count, Is.EqualTo(3));
    }

    [Test]
    public async Task GetAllMeasures_ShouldReturnAllMeasures()
    {
        var deviceData1 = new DeviceData("123", new List<SensorMeasure>
        {
            new SensorMeasure(T.T0, new SensorData("Probe1", 1.0f)),
            new SensorMeasure(T.T0, new SensorData("Probe2", 2.0f)),
            new SensorMeasure(T.T0, new SensorData("Probe3", 3.0f))
        });

        var deviceData2 = new DeviceData("456", new List<SensorMeasure>
        {
            new SensorMeasure(T.T1, new SensorData("Probe4", 4.0f)),
            new SensorMeasure(T.T1, new SensorData("Probe5", 5.0f)),
            new SensorMeasure(T.T1, new SensorData("Probe6", 6.0f))
        });

        await _client.PostAsJsonAsync("/measures", deviceData1);
        await _client.PostAsJsonAsync("/measures", deviceData2);

        var response = await _client.GetAsync("/measures");
        response.EnsureSuccessStatusCode();

        var allMeasures = await response.Content.ReadFromJsonAsync<List<DeviceMeasures>>();
        Assert.IsNotNull(allMeasures);
        Assert.That(allMeasures.Count, Is.EqualTo(2));
    }
}