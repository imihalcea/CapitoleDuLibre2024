using System.Net.Http.Json;
using System.Reactive.Linq;
using DataModel;
using Driver;
using Driver.Actuators;
using Driver.DataSources;

var serialNumber = Environment.GetEnvironmentVariable("SERIAL_NUMBER") ?? "UNKNOWN";
var serverBaseUrl = Environment.GetEnvironmentVariable("SERVER_BASE_URL") ?? "http://localhost:5151";
var netInterfaceName = Environment.GetEnvironmentVariable("NET_INTERFACE_NAME") ?? "eth0";
var refreshRate = Environment.GetEnvironmentVariable("REFRESH_RATE") ?? "1000";


var acquisitionService = new AcquisitionService(
    new CpuInfoReader(),
    new CpuTempSensor(),
    new RxTxReader(netInterfaceName, Convert.ToUInt16(refreshRate))
);

var httpClient = new HttpClient { BaseAddress = new Uri(serverBaseUrl) };
var dataStore = new DataStore();

var controlService = new FanControlService(ActuatorsFactory.OnOffActuator(18), dataStore, SensorIds.Temp);

var acquisitionStream = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
    .Select( async _ => await acquisitionService.Read(DateTime.Now));

var controlStream = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
    .Do(_ => controlService.Control(42));

async void AcquisitionOnNext(Task<SensorMeasure[]> readSensors)
{
    Console.WriteLine($"[{DateTime.Now}] Sending data to server...");
    var measures = await readSensors;
    foreach (var measure in measures)
    {
        var (key, value) = measure.SensorData;
        dataStore.Update(key, value);
    }
    var data = new DeviceData(serialNumber, measures.ToList());
    await httpClient.PostAsJsonAsync("/measures", data);
}

var acquisitionSubscription = acquisitionStream.Subscribe(
    AcquisitionOnNext,
    ex => Console.WriteLine($"Error: {ex.Message}"),
    () => Console.WriteLine("Completed"));

var controlSubscription = controlStream.Subscribe(
    _ => Console.WriteLine("Controlling..."),
    ex => Console.WriteLine($"Error: {ex.Message}"),
    () => Console.WriteLine("Completed"));

#if DEBUG
Console.WriteLine("Press any key to stop...");
Console.ReadKey();

acquisitionSubscription.Dispose();
controlSubscription.Dispose();
#else
Task.Delay(-1).GetAwaiter().GetResult();
#endif
