using Avalonia;
using Avalonia.Media;

namespace Display.DirectRenderingManager;

class Program
{
  [STAThread]
  public static void Main(string[] args) => BuildAvaloniaApp()
    .StartLinuxDrm(args, card: "/dev/dri/card1", scaling: 1.0);
  
  public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
      .WithInterFont()
      .With(new FontManagerOptions
      {
        DefaultFamilyName = "avares://Avalonia.Fonts.Inter/Assets#Inter"
      })
      .LogToTrace();

}