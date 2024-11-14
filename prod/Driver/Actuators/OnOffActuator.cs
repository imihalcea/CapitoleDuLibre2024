using System.Device.Gpio;

namespace Driver.Actuators;

public class OnOffActuator: IOnOffActuator
{
    private readonly int _pin;
    private readonly GpioController _controller;
    private bool _disposed;
    
    public OnOffActuator(int pin)
    {
        _pin = pin;
        _controller = new GpioController();
        _controller.OpenPin(_pin, PinMode.Output);
        _disposed = false;
    }
    
    public void On()
    {
        _controller.Write(_pin, PinValue.High);
    }
    
    public void Off()
    {
        _controller.Write(_pin, PinValue.Low);
    }
    
    
    public void Dispose()
    {
        if (_disposed) return;
        Off();
        _controller.ClosePin(_pin);
        GC.SuppressFinalize(this);
        _disposed = true;
    }
}