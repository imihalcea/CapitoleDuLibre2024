using AcquireServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IStoreDeviceMeasures, InMemoryStorage>();
builder.Services.AddSingleton<DeviceMeasuresApi>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost("/measures", async (DeviceMeasuresApi api, DeviceData deviceData) =>
    {
        var result = await api.SaveMeasures(deviceData);
        return Results.Ok(result);
    })
    .WithName("SaveMeasures")
    .WithOpenApi();

app.MapGet("/measures/{serialNumber}", async (DeviceMeasuresApi api, string serialNumber) =>
{
    var result = await api.GetMeasuresByDevice(serialNumber);
    return Results.Ok(result);
})
    .WithName("GetMeasuresByDevice")
    .WithOpenApi();

app.MapGet("/measures", async (DeviceMeasuresApi api) =>
{
    var result = await api.GetAllMeasures();
    return Results.Ok(result);
}).WithName("GetAllMeasures")
    .WithOpenApi();

app.Run();


public partial class Program { }