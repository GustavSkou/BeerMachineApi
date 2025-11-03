using Microsoft.EntityFrameworkCore;
using BeerMachineApi.Services;
using BeerMachineApi.Repository;
using BeerMachineApi.Services.StatusModels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MachineDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// register IMachineService but inject IServiceScopeFactory instead of resolving MachineDbContext here
builder.Services.AddSingleton<IMachineService>(sp =>
{
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    return new BeerMachineService(
        new BeerMachineStatusModel(),
        new BatchStatusModel(),
        scopeFactory // pass factory, not DbContext
    );
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// resolve and start as before
var machineService = app.Services.GetRequiredService<IMachineService>();
Thread thread = new Thread(machineService.Start);
thread.Start();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();