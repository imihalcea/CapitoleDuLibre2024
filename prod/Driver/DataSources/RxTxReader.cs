using System.Collections.Concurrent;
using DataModel;

namespace Driver.DataSources;
using NetworkManager.DBus;
using Tmds.DBus.Protocol;
using Connection = Tmds.DBus.Protocol.Connection;




public class RxTxReader : IDataSource
{
    public RxTxReader(string interfaceName, uint refreshRateMs)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        _data = new ConcurrentDictionary<string, ulong>();
        var token = cancellationTokenSource.Token;
        _ = Task.Run(async () =>
        {
            var connection = new Connection(Address.System!);
            await connection.ConnectAsync();
            var service = new NetworkManagerService(connection, "org.freedesktop.NetworkManager");
            var networkManager = service.CreateNetworkManager("/org/freedesktop/NetworkManager");
            var devices = await networkManager.GetDevicesAsync();
            ObjectPath? targetInterface = null;
            foreach (var devicePath in devices)
            {
                var device = service.CreateDevice(devicePath);
                var deviceInterface = await device.GetInterfaceAsync();
                if (deviceInterface == interfaceName)
                {
                    Console.WriteLine($"Found device with interface '{interfaceName}'");
                    targetInterface = devicePath;
                }
            }
            if (targetInterface is null)
            {
                Console.WriteLine($"Device with interface '{interfaceName}' not found");
                return;
            }

            var statistics = service.CreateStatistics(targetInterface);
            await statistics.WatchPropertiesChangedAsync(
                (exception, change) =>
                {
                    _data["rx"] = change.Properties.RxBytes / 1024;
                    _data["tx"] = change.Properties.TxBytes / 1024;
                }, true, ObserverFlags.None);
            await statistics.SetRefreshRateMsAsync(refreshRateMs);
        }, token);
    }


    private readonly ConcurrentDictionary<string,ulong> _data;


    public async Task<SensorMeasure[]> Read(DateTime currentTime)
    {
        var result = new SensorMeasure[]
        {
            new SensorMeasure(currentTime, new SensorData(SensorIds.NetRx, _data.GetValueOrDefault("rx"))),
            new SensorMeasure(currentTime, new SensorData(SensorIds.NetTx, _data.GetValueOrDefault("tx")))
        };
        return await Task.FromResult(result);
    }

    public void Dispose()
    {
        // Nothing to dispose
    }
}