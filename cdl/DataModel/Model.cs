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
    public static SensorId Temp => "temp";
    public static SensorId CpuLoad1 => "load_1";
    public static SensorId CpuLoad2 => "load_2";
    public static SensorId CpuLoad3 => "load_3";
    public static SensorId NetTx => "net_tx";
    public static SensorId NetRx => "net_rx";
    
    public static List<SensorId> All => [Temp, CpuLoad1, CpuLoad2, CpuLoad3, NetTx, NetRx];
    
}