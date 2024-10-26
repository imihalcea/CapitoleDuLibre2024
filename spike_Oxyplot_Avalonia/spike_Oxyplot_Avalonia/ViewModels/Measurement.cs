namespace spike_Oxyplot_Avalonia.ViewModels;

public class Measurement
{
  public double Time { get; set; }
  public double Value { get; set; }
  public double Minimum { get; set; }
  public double Maximum { get; set; }

  public override string ToString()
  {
    return $"{Time:#0.0} {Value:##0.0}";
  }
}