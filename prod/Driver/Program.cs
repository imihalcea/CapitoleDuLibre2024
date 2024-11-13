using System.Net.Http.Json;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using DataModel;
using Driver;
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

var stream = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
    .Select( async _ => await acquisitionService.Read(DateTime.Now));


async void OnNext(Task<SensorMeasure[]> readSensors)
{
    Console.WriteLine($"[{DateTime.Now}] Sending data to server...");
    var measures = await readSensors;
    var data = new DeviceData(serialNumber, measures.ToList());
    await httpClient.PostAsJsonAsync("/measures", data);
}

var subscription = stream.Subscribe(
    OnNext,
    ex => Console.WriteLine($"Error: {ex.Message}"),
    () => Console.WriteLine("Completed"));

#if DEBUG
Console.WriteLine("Press any key to stop...");
Console.ReadKey();

subscription.Dispose();
#else
Task.Delay(-1).GetAwaiter().GetResult();
#endif
