namespace DataModel;

using SerialNumber = string;
using SensorId = string;
using MeasureValue = float;

public record SensorData(SensorId SensorId, MeasureValue Value);

public record SensorMeasure(DateTime At, SensorData SensorData);

public record DeviceData(SerialNumber SerialNumber, List<SensorMeasure> Data);

public record Measures(DateTime At, List<SensorData> SensorsMeasures);

public record DeviceMeasures(SerialNumber SerialNumber, List<Measures> Measures);

public class SensorIds
{
    public SensorId Temp => "temp";
    public SensorId CpuLoad1 => "load_1";
    public SensorId CpuLoad2 => "load_2";
    public SensorId CpuLoad3 => "load_3";
    public SensorId NetTx => "net_tx";
    public SensorId NetRx => "net_rx";
    
    public List<SensorId> All => [Temp, CpuLoad1, CpuLoad2, CpuLoad3, NetTx, NetRx];
    
}