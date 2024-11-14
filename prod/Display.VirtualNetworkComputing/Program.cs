using Avalonia;
using Avalonia.Controls;

namespace Display.VirtualNetworkComputing;

class Program
{
  [STAThread]
  public static void Main(string[] args) => BuildAvaloniaApp()
    .StartWithHeadlessVncPlatform("0.0.0.0", 5900, args, ShutdownMode.OnMainWindowClose);
  
  public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
      .UseSkia()
      .WithInterFont()
      .LogToTrace();

}