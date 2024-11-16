// See
// - https://www.nuget.org/packages?q=dbus
// - https://github.com/tmds/Tmds.DBus
// Use
//  dotnet add package Tmds.DBus.Protocol
//  dotnet tool update -g Tmds.DBus.Tool
//  dotnet dbus list services --bus system
//  dotnet dbus list objects --bus system --service org.freedesktop.NetworkManager
//  dotnet dbus codegen --protocol-api --bus system --service org.freedesktop.NetworkManager

using NetworkManager.DBus;
using Tmds.DBus.Protocol;
using Connection = Tmds.DBus.Protocol.Connection;

var connection = new Connection(Address.System!);
await connection.ConnectAsync();
Console.WriteLine("Connected to system bus.");

var service = new NetworkManagerService(connection, "org.freedesktop.NetworkManager");
var networkManager = service.CreateNetworkManager("/org/freedesktop/NetworkManager");

foreach (var devicePath in await networkManager.GetDevicesAsync())
{
  var device = service.CreateDevice(devicePath);
  var interfaceName = await device.GetInterfaceAsync();
  if (await IsSoftwareDevice(device))
    continue;
  Console.WriteLine($"Found physical interface '{await device.GetInterfaceAsync()}'.");
  
  var statistics = service.CreateStatistics(devicePath);
  await statistics.WatchPropertiesChangedAsync(
    (exception, change) =>
    {
      Console.WriteLine($"{interfaceName} - Rx: {change.Properties.RxBytes/1024} Kb / Tx: {change.Properties.TxBytes/1024} Kb");
    }, true, ObserverFlags.None);
  await statistics.SetRefreshRateMsAsync(5000);

  // ListProps(await device.GetPropertiesAsync());
  // await device.WatchStateChangedAsync(
  //   (exception, change) =>
  //   {
  //     if (exception is null)
  //       Console.WriteLine($"Interface changed from '{
  //         Enum.GetName((NmDeviceState)change.OldState)
  //       }' to '{
  //         Enum.GetName((NmDeviceState)change.NewState)
  //       }'.");
  //   });
}

var disconnectReason = await connection.DisconnectedAsync();
if (disconnectReason is not null)
{
  Console.WriteLine("The connection was closed:");
  Console.WriteLine(disconnectReason);
  return 1;
}

return 0;

void ListProps<T>(T t)
{
  foreach(var property in t!.GetType().GetProperties())
    Console.WriteLine($"{property.Name}: {property.GetValue(t)}");
}

async Task<bool> IsSoftwareDevice(Device device)
{
  var capabilities = await device.GetCapabilitiesAsync();
  return (capabilities & (uint) NmDeviceCapabilities.IsSoftware) != 0;
}

[Flags]
// https://people.freedesktop.org/~lkundrak/nm-docs/nm-dbus-types.html#NMDeviceCapabilities
enum NmDeviceCapabilities
{
  None = 0x00000000,
  Supported = 0x00000001,
  CarrierDetect = 0x00000002,
  IsSoftware = 0x00000004
}

// https://people.freedesktop.org/~lkundrak/nm-docs/nm-dbus-types.html#NMDeviceState
enum NmDeviceState
{
  Unknown = 0,
  Unmanaged = 10,
  Unavailable = 20,
  Disconnected = 30,
  Prepare = 40,
  Config = 50,
  NeedAuth = 60,
  IpConfig = 70,
  IpCheck = 80,
  Secondaries = 90,
  Activated = 100,
  Deactivating = 110,
  Failed = 120,
}
