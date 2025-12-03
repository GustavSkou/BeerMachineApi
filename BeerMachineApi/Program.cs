using Microsoft.EntityFrameworkCore;
using BeerMachineApi.Services;
using BeerMachineApi.Repository;
using BeerMachineApi.Services.StatusModels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MachineDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton<IMachineService, BeerMachineService>();

builder.Services.AddTransient<BeerMachineStatusModel>();
builder.Services.AddTransient<BatchStatusModel>();
builder.Services.AddTransient<InventoryStatusModel>();
builder.Services.AddTransient<MaintenanceStatusModel>();

builder.Services.AddTransient<IBatchHandler, BatchHandler>();
builder.Services.AddTransient<ITimeHandler, TimeHandler>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// start beermachine service
var machineService = app.Services.GetRequiredService<IMachineService>();
Thread machineServiceThread = new Thread(machineService.Start);
machineServiceThread.Start();

Thread thread = new Thread(machineService.TryToConnectToServer);
thread.Start();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();