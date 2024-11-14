using System.Device.Gpio;

namespace Driver.Actuators;

public static class ActuatorsFactory
{
    public static IOnOffActuator OnOffActuator(int pin)
    {
        if (!IsGpioControllerAvailable())
            return new DummyOnOffActuator();
        return new OnOffActuator(pin);
    }
    
    //a bit of a hack, but it works for now
    private static bool IsGpioControllerAvailable()
    {
        try
        {
            using var _ = new GpioController();
            return true;
        }
        catch (Exception _)
        {
            Console.WriteLine("GpioController is not available");
            return false;
        }
        
    }
}