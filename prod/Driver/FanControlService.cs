using Driver.Actuators;

namespace Driver;

public class FanControlService(IOnOffActuator fan, DataStore dataStore, string tempSensorId)
{
    public void Control(int tempSetPoint)
    {
        var temp = dataStore.Get(tempSensorId);
        if (temp == null)
            return;
        
        if (temp > tempSetPoint)
            fan.On();
        else
            fan.Off();
    }
    
}