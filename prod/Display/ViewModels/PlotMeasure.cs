using System;
using System.Linq;
using DataModel;

namespace Display.ViewModels;

public class PlotMeasure
{
  public PlotMeasure(Measures measures)
  {
    At = measures.At;
    Temp = GetValue(measures, SensorIds.Temp);
    CpuLoad1 = GetValue(measures, SensorIds.CpuLoad1);
    CpuLoad2 = GetValue(measures, SensorIds.CpuLoad2);
    CpuLoad3 = GetValue(measures, SensorIds.CpuLoad3);
    NetTx = GetValue(measures, SensorIds.NetTx);
    NetRx = GetValue(measures, SensorIds.NetRx);
  }

  private static float? GetValue(Measures measures, string sensorId) =>
    measures.SensorsMeasures.FirstOrDefault(m => m.SensorId == sensorId)?.Value;

  public DateTime At { get; set; }
  public float? Temp { get; set; }
  public float? CpuLoad1 { get; set; }
  public float? CpuLoad2 { get; set; }
  public float? CpuLoad3 { get; set; }
  public float? NetTx { get; set; }
  public float? NetRx { get; set; }
}