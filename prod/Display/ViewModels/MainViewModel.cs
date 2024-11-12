using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using DataModel;

namespace Display.ViewModels;

public partial class MainViewModel : ViewModelBase
{
  private const int TimeWindowInSeconds = 5 * 60;
  private const int RefreshIntervalInSeconds = 1;

  [ObservableProperty] private ReadOnlyObservableCollection<string> _plotDevices;
  [ObservableProperty] private string _displayedDevice = null!;
  [ObservableProperty] private ReadOnlyObservableCollection<PlotMeasure> _plotMeasures;
    
  public MainViewModel()
  {
    var devices = new CollectionUpdate<string>(out _plotDevices);
    var measures = new CollectionUpdate<PlotMeasure>(out _plotMeasures);

    var client = new HttpClient();
    var apiRoute = Environment.GetEnvironmentVariable("MEASURES_API_ROUTE")
                   ?? throw new InvalidOperationException("Missing MEASURES_API_ROUTE");

    Observable
      .Interval(TimeSpan.FromSeconds(RefreshIntervalInSeconds))
      .Subscribe(async n =>
      {
        var response = await client.GetAsync(apiRoute);
        response.EnsureSuccessStatusCode();
        var allDevicesMeasures = await response.Content.ReadFromJsonAsync<List<DeviceMeasures>>();

        var allDevices = allDevicesMeasures!.ToDictionary(
          d => d.SerialNumber,
          d => d.Measures
        );
        if (!_plotDevices.SequenceEqual(allDevices.Keys))
        {
          var currentDevice = string.IsNullOrEmpty(DisplayedDevice) ? allDevices.Keys.First() : DisplayedDevice;
          devices.UpdateWith(allDevices.Keys);
          DisplayedDevice = currentDevice;
        }

        measures.UpdateWith(allDevices[DisplayedDevice]
          .TakeLast(TimeWindowInSeconds)
          .Select(m => new PlotMeasure(m))
        );
      });
    
  }
}