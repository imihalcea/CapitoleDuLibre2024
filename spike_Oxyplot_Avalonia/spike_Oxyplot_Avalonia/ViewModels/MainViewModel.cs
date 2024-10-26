using System.Collections.ObjectModel;
using System;
using System.Reactive.Linq;
using DynamicData;

namespace spike_Oxyplot_Avalonia.ViewModels;

public class MainViewModel : ViewModelBase
{
  public MainViewModel()
  {
    var measurements = new SourceList<Measurement>();
    measurements
      .Connect()
      .Bind(out _measurements)
      .Subscribe();

    ExecuteDataUpdateInParallelTask(measurements);
  }

  public ReadOnlyObservableCollection<Measurement> Measurements => _measurements;
  private readonly ReadOnlyObservableCollection<Measurement> _measurements;

  private static void ExecuteDataUpdateInParallelTask(SourceList<Measurement> measurements)
  {
    // Pourrait être remplacé par un accès http ?
    var random = new Random(385);
    var y = 0.0;
    var dy = 0.0;
    var i = 0;
    var n = 10;
    Observable
      .Interval(TimeSpan.FromSeconds(1))
      .Subscribe(_ =>
      {
        Console.WriteLine("Update data");
        for (int j = 0; j < n; j++)
        {
          dy += random.NextDouble() * 2 - 1;
          y += dy;
          measurements.Add(
            new Measurement
            {
              Time = 2.5 * i / (n - 1),
              Value = y / (n - 1),
              Maximum = (y / (n - 1)) + 5,
              Minimum = (y / (n - 1)) - 5
            });
          if(measurements.Count > 400)
            measurements.RemoveAt(0);
          i++;
        }
      });
  }
}