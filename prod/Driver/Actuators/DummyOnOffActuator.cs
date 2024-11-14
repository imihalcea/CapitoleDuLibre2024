namespace Driver.Actuators;

public class DummyOnOffActuator: IOnOffActuator
{
    public void Dispose()
    {
        // TODO release managed resources here
    }

    public void On()
    {
        Console.WriteLine("Turning on");
    }

    public void Off()
    {
        Console.WriteLine("Turning off");
    }
}