namespace DataModel;

using SerialNumber = string;
using SensorId = string;
using MeasureValue = float;

public record SensorData(SensorId SensorId, MeasureValue Value);

public record SensorMeasure(DateTime At, SensorData SensorData);

public record DeviceData(SerialNumber SerialNumber, List<SensorMeasure> Data);

public record Measures(DateTime At, List<SensorData> SensorsMeasures);

public record DeviceMeasures(SerialNumber SerialNumber, List<Measures> Measures);
