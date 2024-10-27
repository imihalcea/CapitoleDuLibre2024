using DriverGpio;

Console.WriteLine("Enter the pin number to control the actuator:");
var pin = int.Parse(Console.ReadLine() ?? "6");

Console.WriteLine("Commands 0=Off; 1=On; 2=Exit");
Console.WriteLine("Enter the command:");

var actuator = new OnOffActuator(pin);
var input = Console.ReadLine();
var order = int.Parse(input ?? "2");
while (order != 2)
{
    switch (order)
    {
        case 0:
            actuator.Off();
            break;
        case 1:
            actuator.On();
            break;
    }

    input = Console.ReadLine();
    order = int.Parse(input ?? "2");
}
actuator.Dispose();