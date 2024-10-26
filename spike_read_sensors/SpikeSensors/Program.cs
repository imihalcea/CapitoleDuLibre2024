using SpikeSensors;
using System.Reactive.Linq;


using var cpuTempReader = new CpuTempReader();

var stream = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
    .Select(_ => (DateTime.Now, cpuTempReader.Read()));

var subscription = stream.Subscribe(
    x => Console.WriteLine($"[{x.Item1}] CPU Temperature: {x.Item2}°C"),
    ex => Console.WriteLine($"Error: {ex.Message}"),
    () => Console.WriteLine("Completed"));

Console.WriteLine("Press any key to stop...");
Console.ReadKey();

subscription.Dispose();