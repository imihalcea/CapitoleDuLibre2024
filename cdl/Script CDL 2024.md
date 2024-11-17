## 1. Web Server

## 1.1 Creation d'un projet webapi

```bash
# depuis le répertoire de travail `cdl`
dotnet new webapi -n AcquireServer
dotnet sln add AcquireServer/AcquireServer.csproj
```

### Remplacer le code par défaut de Program.cs par

```cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => 
    options.AddPolicy("AllowAllOrigins",b =>
        b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
    )
);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");

app.MapGet("/ping", ()=>"pong")
    .WithName("ping")
    .WithOpenApi();

app.Run();

public partial class Program { } // hack pour les tests api
```

### Vérifier que ça fonctionne

```bash
dotnet run --project AcquireServer/AcquireServer.csproj  
```

## 1.2 Ajouter un projet de tests

```bash
dotnet new nunit -n AcquireServerTests
cd AcquireServerTests/ 
dotnet sln add AcquireServerTests/AcquireServerTests.csproj 
```
Ajouter le package Microsoft.AspNetCore.Mvc.Testing 8.0.11
Referencer le projet AcquireServer dans AcquireServerTests
Editer le fichier AcquireServerTests.csproj

```xml
   <PreserveCompilationContext>true</PreserveCompilationContext>
    <GenerateDependencyFile>true</GenerateDependencyFile>
    <GenerateRuntimeConfigurationFiles>true</enerateRuntimeConfigurationFiles>
```


Ajouter le fichier TestUtils.cs

```cs
using System.Net.Http.Json;
using DataModel;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using static AcquireServerTests.TestUtils;

namespace AcquireServerTests;

public class WebApiTests
{
    private HttpClient _client;

    [SetUp]
    public void Setup()
    {
        var application = new WebApplicationFactory<Program>();
        _client = application.CreateClient();
    }

    [Test]
    public async Task SaveMeasures_ShouldReturnOk()
    {
        var deviceData = new DeviceData("123", new List<SensorMeasure>
        {
            new(T.T0, new SensorData("Probe1", 1.0f)),
            new(T.T0, new SensorData("Probe2", 2.0f)),
            new(T.T0, new SensorData("Probe3", 3.0f))
        });

        var response = await _client.PostAsJsonAsync("/measures", deviceData);

        response.EnsureSuccessStatusCode();
    }

    [Test]
    public async Task GetMeasuresByDevice_ShouldReturnMeasures()
    {
        var deviceData = new DeviceData("123", new List<SensorMeasure>
        {
            new(T.T0, new SensorData("Probe1", 1.0f)),
            new(T.T0, new SensorData("Probe2", 2.0f)),
            new(T.T0, new SensorData("Probe3", 3.0f))
        });

        await _client.PostAsJsonAsync("/measures", deviceData);

        var response = await _client.GetAsync("/measures/123");
        response.EnsureSuccessStatusCode();

        var measures = await response.Content.ReadFromJsonAsync<List<Measures>>();
        Assert.IsNotNull(measures);
        Assert.That(measures.Count, Is.EqualTo(1));
        Assert.That(measures[0].SensorsMeasures.Count, Is.EqualTo(3));
    }

    [Test]
    public async Task GetAllMeasures_ShouldReturnAllMeasures()
    {
        var deviceData1 = new DeviceData("123", new List<SensorMeasure>
        {
            new(T.T0, new SensorData("Probe1", 1.0f)),
            new(T.T0, new SensorData("Probe2", 2.0f)),
            new(T.T0, new SensorData("Probe3", 3.0f))
        });

        var deviceData2 = new DeviceData("456", new List<SensorMeasure>
        {
            new(T.T1, new SensorData("Probe4", 4.0f)),
            new(T.T1, new SensorData("Probe5", 5.0f)),
            new(T.T1, new SensorData("Probe6", 6.0f))
        });

        await _client.PostAsJsonAsync("/measures", deviceData1);
        await _client.PostAsJsonAsync("/measures", deviceData2);

        var response = await _client.GetAsync("/measures");
        response.EnsureSuccessStatusCode();

        var allMeasures = await response.Content.ReadFromJsonAsync<List<DeviceMeasures>>();
        Assert.IsNotNull(allMeasures);
        Assert.That(allMeasures.Count, Is.EqualTo(2));
    }
}
```

Ajouter un fichier de test WebApiTests.cs

```cs
using System.Net.Http.Json;
using AcquireServer;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AcquireServerTests;

public class WebApiTests
{
    private HttpClient _client;

    [SetUp]
    public void Setup()
    {
        var application = new WebApplicationFactory<Program>();
        _client = application.CreateClient();
    }

    [Test]
    public async Task SaveMeasures_ShouldReturnOk()
    {
        var deviceData = new DeviceData("123", new List<SensorMeasure>
        {
            new SensorMeasure(T.T0, new SensorData("Probe1", 1.0f)),
            new SensorMeasure(T.T0, new SensorData("Probe2", 2.0f)),
            new SensorMeasure(T.T0, new SensorData("Probe3", 3.0f))
        });

        var response = await _client.PostAsJsonAsync("/measures", deviceData);

        response.EnsureSuccessStatusCode();
    }

    [Test]
    public async Task GetMeasuresByDevice_ShouldReturnMeasures()
    {
        var deviceData = new DeviceData("123", new List<SensorMeasure>
        {
            new SensorMeasure(T.T0, new SensorData("Probe1", 1.0f)),
            new SensorMeasure(T.T0, new SensorData("Probe2", 2.0f)),
            new SensorMeasure(T.T0, new SensorData("Probe3", 3.0f))
        });

        await _client.PostAsJsonAsync("/measures", deviceData);

        var response = await _client.GetAsync("/measures/123");
        response.EnsureSuccessStatusCode();

        var measures = await response.Content.ReadFromJsonAsync<List<Measures>>();
        Assert.IsNotNull(measures);
        Assert.That(measures.Count, Is.EqualTo(1));
        Assert.That(measures[0].SensorsMeasures.Count, Is.EqualTo(3));
    }

    [Test]
    public async Task GetAllMeasures_ShouldReturnAllMeasures()
    {
        var deviceData1 = new DeviceData("123", new List<SensorMeasure>
        {
            new SensorMeasure(T.T0, new SensorData("Probe1", 1.0f)),
            new SensorMeasure(T.T0, new SensorData("Probe2", 2.0f)),
            new SensorMeasure(T.T0, new SensorData("Probe3", 3.0f))
        });

        var deviceData2 = new DeviceData("456", new List<SensorMeasure>
        {
            new SensorMeasure(T.T1, new SensorData("Probe4", 4.0f)),
            new SensorMeasure(T.T1, new SensorData("Probe5", 5.0f)),
            new SensorMeasure(T.T1, new SensorData("Probe6", 6.0f))
        });

        await _client.PostAsJsonAsync("/measures", deviceData1);
        await _client.PostAsJsonAsync("/measures", deviceData2);

        var response = await _client.GetAsync("/measures");
        response.EnsureSuccessStatusCode();

        var allMeasures = await response.Content.ReadFromJsonAsync<List<DeviceMeasures>>();
        Assert.IsNotNull(allMeasures);
        Assert.That(allMeasures.Count, Is.EqualTo(2));
    }
}
```

### Jouer les tests


## 1.3 Implémentation des endpoints

### Ajouter les références au projet DataModel

## Ajouter un GlobalUsing dans le fichier GlobalUsings.cs

```cs
global using DataModel;
```

### Ajouter le composant de storage InMemoryStorage.cs

```cs
using System.Collections.Concurrent;
namespace AcquireServer;

using SerialNumber = string;
using SensorId = string;

public interface IStoreDeviceMeasures
{
    public void Save(DeviceData deviceData);
    public List<Measures> MeasuresByDevice(SerialNumber serialNumber);
    public List<DeviceMeasures> AllMeasures();
}

public class InMemoryStorage : IStoreDeviceMeasures
{
    private readonly ConcurrentDictionary<SerialNumber, Dictionary<DateTime, List<SensorData>>> _data;
    
    public InMemoryStorage()
    {
        _data = new ConcurrentDictionary<SerialNumber, Dictionary<DateTime, List<SensorData>>>();
    }

    public void Save(DeviceData deviceData)
    {
        if (!_data.TryGetValue(deviceData.SerialNumber, out var measures))
        {
            measures = new Dictionary<DateTime, List<SensorData>>();
            _data.TryAdd(deviceData.SerialNumber, measures);
        }

        foreach (var measure in deviceData.Data)
        {
            if (!measures.TryGetValue(measure.At, out var values))
            {
                values = [];
                measures.TryAdd(measure.At, values);
            }

            values.Add(measure.SensorData);
        }
    }
    
    public List<Measures> MeasuresByDevice(SerialNumber serialNumber)
    {
        if (!_data.TryGetValue(serialNumber, out var measures))
        {
            return new List<Measures>();
        }

        return measures.Select(m => new Measures(m.Key, m.Value)).ToList();
    }
    
    public List<DeviceMeasures> AllMeasures()
    {
        return _data.Select(d => new DeviceMeasures(d.Key, d.Value.Select(m => new Measures(m.Key, m.Value)).ToList())).ToList();
    }
    
}
```

### Ajouter la classe DeviceMeasuresApi.cs

```cs
namespace AcquireServer;

public class DeviceMeasuresApi(IStoreDeviceMeasures store)
{

    public async Task<bool> SaveMeasures(DeviceData deviceData)
    {
        store.Save(deviceData);
        return await Task.FromResult(true);
    }

    public Task<List<Measures>> GetMeasuresByDevice(string serialNumber)
    {
        var measuresByDevice = store.MeasuresByDevice(serialNumber);
        return Task.FromResult(measuresByDevice);
    }

    public Task<List<DeviceMeasures>> GetAllMeasures()
    {
        var deviceMeasuresList = store.AllMeasures();
        return Task.FromResult(deviceMeasuresList);
    }
    
}
```

### Definir les endpoints dans le fichier Program.cs

Config du container des dépendances

```cs
builder.Services.AddSingleton<IStoreDeviceMeasures, InMemoryStorage>();
builder.Services.AddSingleton<DeviceMeasuresApi>();
```
Ajout du endpoint d'enregistrement des mesures

```cs
app.MapPost("/measures", async (DeviceMeasuresApi api, DeviceData deviceData) =>
    {
        var result = await api.SaveMeasures(deviceData);
        return Results.Ok(result);
    })
    .WithName("SaveMeasures")
    .WithOpenApi();
```

Ajout du endpoint de récupération des mesures par device

```cs
app.MapGet("/measures/{serialNumber}", async (DeviceMeasuresApi api, string serialNumber) =>
{
    var result = await api.GetMeasuresByDevice(serialNumber);
    return Results.Ok(result);
})
    .WithName("GetMeasuresByDevice")
    .WithOpenApi();
```

Ajout du endpoint de récupération de toutes les mesures

```cs
app.MapGet("/measures", async (DeviceMeasuresApi api) =>
{
    var result = await api.GetAllMeasures();
    return Results.Ok(result);
}).WithName("GetAllMeasures")
    .WithOpenApi();
```

Jouer les tests oOo => ça passe \o/

## 1.4 Le script de déploiement sur le serveur 

Ajouter un fichier deploy.sh

```bash
#!/bin/bash

# Configuration
CONFIGURATION="Release"
DAEMON_EXE="AcquireServer"
DAEMON_FOLDER="/home/cdl"
DAEMON_PATH="$DAEMON_FOLDER/$DAEMON_EXE"
SERVICE_NAME="cdl2024-server.service"
SERVICE_PATH="/etc/systemd/system/$SERVICE_NAME"
REMOTE_USER="cdl"
REMOTE_HOST="opicdl"
ARCH="linux-arm"

# Environment variables
ASPNETCORE_URLS="http://0.0.0.0:8080"
ASPNETCORE_ENVIRONMENT="Development"
#ASPNETCORE_ENVIRONMENT="Production"

build_daemon() {
    echo "Building daemon executable..."
    dotnet publish -c "$CONFIGURATION" -r $ARCH --self-contained true -p:PublishSingleFile=true
}

# Function to upload the daemon executable
upload_daemon() {
    echo "Uploading daemon executable..."
    rsync -avz "./bin/$CONFIGURATION/net8.0/$ARCH/publish/$DAEMON_EXE" "$REMOTE_USER@$REMOTE_HOST:$DAEMON_PATH"
}

# Function to create the systemd service file
create_service() {
    echo "Creating systemd service file..."
    ssh "$REMOTE_USER@$REMOTE_HOST" "sudo tee $SERVICE_PATH > /dev/null" <<EOF
[Unit]
Description=CDL2024 AcquireServer Service
After=network.target

[Service]
ExecStart=$DAEMON_PATH
Restart=always
User=$REMOTE_USER
Environment="ASPNETCORE_URLS=$ASPNETCORE_URLS"
Environment="ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT"

[Install]
WantedBy=multi-user.target
EOF
}

# Function to reload systemd and restart the service
restart_service() {
    echo "Reloading systemd and restarting the service..."
    ssh "$REMOTE_USER@$REMOTE_HOST" "sudo systemctl daemon-reload"
    ssh "$REMOTE_USER@$REMOTE_HOST" "sudo systemctl restart $SERVICE_NAME"
    ssh "$REMOTE_USER@$REMOTE_HOST" "sudo systemctl enable $SERVICE_NAME"
}

# Function to verify the service status
verify_service() {
    echo "Verifying service status..."
    ssh "$REMOTE_USER@$REMOTE_HOST" "sudo systemctl status $SERVICE_NAME"
    ssh "$REMOTE_USER@$REMOTE_HOST" "sudo journalctl -fu $SERVICE_NAME"  
}

# Main script
build_daemon
upload_daemon
create_service
restart_service
verify_service
```

## 2. Native lib : CPUload

Ouvrir le projet Driver aller dans le fichier Program.cs



## 3. Display simple

### 3.1 Accès aux données du serveur


Executer Display.Desktop : on a une appli vide opérationnelle

On veut afficher les valeurs renvoyées par le serveur
On référence
- le data model
- la lib DynamicData 9.0.4

On copie/colle la classe utilitaire `CollectionUpdate` en expliquant qu'elle sert à mettre à jour une collection de données mais on ne s'attarde pas sur le détail.

On code une MainViewModel basique
```cs
public partial class MainViewModel : ViewModelBase
{
    private const int RefreshIntervalInSeconds = 1;

    [ObservableProperty] private ReadOnlyObservableCollection<float> _deviceMeasures;

    public MainViewModel()
    {
        var measures = new CollectionUpdate<float>(out _deviceMeasures);

        var client = new HttpClient();
        var apiRoute = Environment.GetEnvironmentVariable("MEASURES_API_ROUTE")
                       ?? throw new InvalidOperationException("Missing MEASURES_API_ROUTE");

        Observable
            .Interval(TimeSpan.FromSeconds(RefreshIntervalInSeconds))
            .Subscribe(async n =>
            {
                var response = await client.GetAsync(apiRoute);
                response.EnsureSuccessStatusCode();
                var allDevicesMeasures = await response.Content.ReadFromJsonAsync<List<DeviceMeasures>>();

                measures.UpdateWith(
                    allDevicesMeasures!.First().Measures.Select(m => m.SensorsMeasures.First().Value)
                );
            });
    }
}
```

On remplace le 
```xaml
<ItemsControl
	ItemsSource="{Binding DeviceMeasures}"
	HorizontalAlignment="Center" VerticalAlignment="Center"/>
```

On définit la route `MEASURES_API_ROUTE=http://192.168.42.4:8080/measures`

On exécute le Display.Desktop et on a une liste de nombres venus du serveur

### 3.2 Choix du device

On rajoute dans le MainViewModel
```cs
[ObservableProperty] private ReadOnlyObservableCollection<string> _devices;
[ObservableProperty] private string _displayedDevice = null!;
```

Dans le constructeur de MainViewModel, on rajoute l'init de la liste des devices
```cs
var devices = new CollectionUpdate<string>(out _devices);
```

et on gère le choix du device pour afficher les données
```cs
var allDevices = allDevicesMeasures!.ToDictionary(
	d => d.SerialNumber,
	d => d.Measures
);
if (!_devices.SequenceEqual(allDevices.Keys))
{
	var currentDevice = string.IsNullOrEmpty(DisplayedDevice) ? allDevices.Keys.First() : DisplayedDevice;
	devices.UpdateWith(allDevices.Keys);
	DisplayedDevice = currentDevice;
}

measures.UpdateWith(
	allDevices[DisplayedDevice].Select(m => m.SensorsMeasures.First().Value)
);
```

Le xaml devient un grid
```xaml
<Grid ColumnDefinitions="*" RowDefinitions="Auto,*">
	<ComboBox Grid.Column="0" Grid.Row="0"
			  HorizontalAlignment="Stretch"
			  ItemsSource="{Binding Devices}"
			  SelectedItem="{Binding DisplayedDevice}" />
	<ItemsControl Grid.Column="0" Grid.Row="1"
				  ItemsSource="{Binding DeviceMeasures}"
				  HorizontalAlignment="Center" VerticalAlignment="Center" />
</Grid>
```

On exécute et on peut changer de device

### 3.3 Affichage graphique

On introduit la classe `PlotMeasures` qui va être dans le modèle de vue (par opposition au modèle de données)

```cs
public class PlotMeasure
{
  public PlotMeasure(Measures measures)
  {
    At = measures.At;
    CpuLoad1 = GetValue(measures, SensorIds.CpuLoad1);
    CpuLoad2 = GetValue(measures, SensorIds.CpuLoad2);
    CpuLoad3 = GetValue(measures, SensorIds.CpuLoad3);
  }

  private static float? GetValue(Measures measures, string sensorId) =>
    measures.SensorsMeasures.FirstOrDefault(m => m.SensorId == sensorId)?.Value;

  public DateTime At { get; set; }
  public float? CpuLoad1 { get; set; }
  public float? CpuLoad2 { get; set; }
  public float? CpuLoad3 { get; set; }
}
```

Dans le MainViewModel, on met à jour pour utiliser ce PlotMeasure
```cs
private const int TimeWindowInSeconds = 5 * 60;
//---
[ObservableProperty] private ReadOnlyObservableCollection<PlotMeasure> _deviceMeasures;
//----
var measures = new CollectionUpdate<PlotMeasure>(out _deviceMeasures);
//----
measures.UpdateWith(allDevices[DisplayedDevice]
	.TakeLast(TimeWindowInSeconds)
	.Select(m => new PlotMeasure(m))
```

On référence Oxyplot.Avalonia_11_1 et on met à jour le xaml

```xaml
xmlns:oxy="clr-namespace:OxyPlot.Avalonia;assembly=OxyPlot.Avalonia"
```

```xaml
<oxy:Plot Grid.Column="0" Grid.Row="1">
	<oxy:Plot.Legends>
		<oxy:Legend />
	</oxy:Plot.Legends>
	<oxy:Plot.Axes>
		<oxy:LinearAxis Key="Load" Position="Left" MajorGridlineStyle="Dot" />
		<oxy:DateTimeAxis Position="Bottom" StringFormat="hh:mm:ss" MajorGridlineStyle="Dot" />
	</oxy:Plot.Axes>
	<oxy:Plot.Series>
		<oxy:LineSeries Title="CpuLoad1" DataFieldX="At" DataFieldY="CpuLoad1"
						ItemsSource="{Binding DeviceMeasures}"
						StrokeThickness="2" Color="Blue" />
		<oxy:LineSeries Title="CpuLoad2" DataFieldX="At" DataFieldY="CpuLoad2"
						ItemsSource="{Binding DeviceMeasures}"
						StrokeThickness="2" Color="DodgerBlue" />
		<oxy:LineSeries Title="CpuLoad3" DataFieldX="At" DataFieldY="CpuLoad3"
						ItemsSource="{Binding DeviceMeasures}"
						StrokeThickness="2" Color="DarkSlateBlue" />
	</oxy:Plot.Series>
</oxy:Plot>
```

Et on rajoute le thème Oxyplot !!!
```xaml
<StyleInclude Source="avares://OxyPlot.Avalonia/Themes/Default.axaml"/>
```

On exécute le Display.Desktop et on a un bel affichage graphique


## 4. DBus : network & 5. FS : temperature

### Dans driver datasources on ajoute les fichiers:

- NetworkManager.DBus.cs
- RxTxReader.cs
- CpuTempSensor.cs

### Modifier Program.cs

```cs
var acquisitionService = new AcquisitionService(
    new CpuInfoReader(),
    new CpuTempSensor(),
    new RxTxReader(netInterfaceName, Convert.ToUInt16(refreshRate))
);
```

## 6. Display full

On met à jour le PlotMeasure avec les nouvelles valeurs
```cs
public class PlotMeasure
{
  public PlotMeasure(Measures measures)
  {
    At = measures.At;
    Temp = GetValue(measures, SensorIds.Temp);
    CpuLoad1 = GetValue(measures, SensorIds.CpuLoad1);
    CpuLoad2 = GetValue(measures, SensorIds.CpuLoad2);
    CpuLoad3 = GetValue(measures, SensorIds.CpuLoad3);
    NetTx = GetValue(measures, SensorIds.NetTx);
    NetRx = GetValue(measures, SensorIds.NetRx);
  }

  private static float? GetValue(Measures measures, string sensorId) =>
    measures.SensorsMeasures.FirstOrDefault(m => m.SensorId == sensorId)?.Value;

  public DateTime At { get; set; }
  public float? Temp { get; set; }
  public float? CpuLoad1 { get; set; }
  public float? CpuLoad2 { get; set; }
  public float? CpuLoad3 { get; set; }
  public float? NetTx { get; set; }
  public float? NetRx { get; set; }
}
```

On met à jour le xaml avec les 3 graphiques

```xaml
<Grid ColumnDefinitions="*" RowDefinitions="Auto,*,*,*">
	<ComboBox Grid.Column="0" Grid.Row="0"
			  HorizontalAlignment="Stretch"
			  ItemsSource="{Binding PlotDevices}"
			  SelectedItem="{Binding DisplayedDevice}" />

	<oxy:Plot Grid.Column="0" Grid.Row="1">
		<oxy:Plot.Legends>
			<oxy:Legend />
		</oxy:Plot.Legends>
		<oxy:Plot.Axes>
			<oxy:LinearAxis Position="Left" MajorGridlineStyle="Dot" />
			<oxy:DateTimeAxis Position="Bottom" StringFormat="hh:mm:ss" MajorGridlineStyle="Dot" />
		</oxy:Plot.Axes>
		<oxy:Plot.Series>
			<oxy:LineSeries Title="Temp" DataFieldX="At" DataFieldY="Temp"
							ItemsSource="{Binding PlotMeasures}"
							LineStyle="Solid" StrokeThickness="2"
							Color="Red" />
		</oxy:Plot.Series>
	</oxy:Plot>

	<oxy:Plot Grid.Column="0" Grid.Row="2">
		<oxy:Plot.Legends>
			<oxy:Legend />
		</oxy:Plot.Legends>
		<oxy:Plot.Axes>
			<oxy:LinearAxis Key="Load" Position="Left" MajorGridlineStyle="Dot" />
			<oxy:DateTimeAxis Position="Bottom" StringFormat="hh:mm:ss" MajorGridlineStyle="Dot" />
		</oxy:Plot.Axes>
		<oxy:Plot.Series>
			<oxy:LineSeries Title="CpuLoad1" DataFieldX="At" DataFieldY="CpuLoad1"
							ItemsSource="{Binding PlotMeasures}"
							StrokeThickness="2" Color="Blue" />
			<oxy:LineSeries Title="CpuLoad2" DataFieldX="At" DataFieldY="CpuLoad2"
							ItemsSource="{Binding PlotMeasures}"
							StrokeThickness="2" Color="DodgerBlue" />
			<oxy:LineSeries Title="CpuLoad3" DataFieldX="At" DataFieldY="CpuLoad3"
							ItemsSource="{Binding PlotMeasures}"
							StrokeThickness="2" Color="DarkSlateBlue" />
		</oxy:Plot.Series>
	</oxy:Plot>

	<oxy:Plot Grid.Column="0" Grid.Row="3">
		<oxy:Plot.Legends>
			<oxy:Legend />
		</oxy:Plot.Legends>
		<oxy:Plot.Axes>
			<oxy:LinearAxis Key="Net" Position="Left" MajorGridlineStyle="Dot" />
			<oxy:DateTimeAxis Position="Bottom" StringFormat="hh:mm:ss" MajorGridlineStyle="Dot" />
		</oxy:Plot.Axes>
		<oxy:Plot.Series>
			<oxy:LineSeries Title="NetTx" DataFieldX="At" DataFieldY="NetTx"
							ItemsSource="{Binding PlotMeasures}"
							LineStyle="Solid" StrokeThickness="2"
							Color="Green" />
			<oxy:LineSeries Title="NetRx" DataFieldX="At" DataFieldY="NetRx"
							ItemsSource="{Binding PlotMeasures}"
							LineStyle="Solid" StrokeThickness="2"
							Color="Goldenrod" />
		</oxy:Plot.Series>
	</oxy:Plot>
</Grid>
```

On exécute le Display.Desktop et on a l'affichage graphique complet

## 7. GPIO : fan control

### Expliquer le package System.Device.Gpio

### Montrer le fichier OnOffActuator.cs

### Ajouter le fichier FanControlService.cs

```cs
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
```

### Modifier Program.cs

Ajouter le control Stream

```cs
var controlStream = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
    .Do(_ => controlService.Control(42));

var controlSubscription = controlStream.Subscribe(
    _ => Console.WriteLine("Controlling..."),
    ex => Console.WriteLine($"Error: {ex.Message}"),
    () => Console.WriteLine("Completed"));
```

## 8. Display embedded

On charge l'appli console `Display.DirectRenderingManager` 

On explique que Avalonia.LinuxFrameBuffer permet de faire du Linux sans serveur X ou Wayland :
- soit en Framebuffer avec un rendu logiciel
- soit en DirectRenderingManager avec le GPU

On modifie le programme principal :

```cs
class Program
{
  [STAThread]
  public static void Main(string[] args) => BuildAvaloniaApp()
    .StartLinuxDrm(args, card: "/dev/dri/card1", scaling: 1.0);
  
  public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
      .WithInterFont()
      .With(new FontManagerOptions
      {
        DefaultFamilyName = "avares://Avalonia.Fonts.Inter/Assets#Inter"
      })
      .LogToTrace();

}
```

## 9. Display VNC

**Si on a le temps**

On change l'appli console `Display.VirtualNetworkComputing`

On modifie le programme principal :

```cs
class Program
{
  [STAThread]
  public static void Main(string[] args) => BuildAvaloniaApp()
    .StartWithHeadlessVncPlatform("0.0.0.0", 5900, args, ShutdownMode.OnMainWindowClose);
  
  public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
      .UseSkia()
      .WithInterFont()
      .LogToTrace();

}
```


## 10. Display Wasm

Il est déjà dispo sur le serveur [http://192.168.42.4](http://192.168.42.4)

On montre le code du main Program qui est similaire à ce que l'on a déjà vu





