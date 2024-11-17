namespace Driver.Actuators;

public interface IOnOffActuator : IDisposable
{
    void On();
    void Off();
}