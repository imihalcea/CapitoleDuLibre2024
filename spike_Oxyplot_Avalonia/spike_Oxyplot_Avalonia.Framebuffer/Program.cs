using Avalonia;
using Avalonia.Media;

namespace spike_Oxyplot_Avalonia.Framebuffer;

class Program
{
  [STAThread]
  public static void Main(string[] args) => BuildAvaloniaApp()
    .StartLinuxFbDev(args);
  
  public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
      .WithInterFont()
      .With(new FontManagerOptions
      {
        DefaultFamilyName = "avares://Avalonia.Fonts.Inter/Assets#Inter"
      })
      .LogToTrace();
}