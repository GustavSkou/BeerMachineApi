using BeerMachineApi.Services;
public class Program
{
    static void Main(string[] args)
    {
        // Run the BeerMachineHandler on a thread so the process does not block the api

        IMachineService machineService = new BeerMachineService(new BeerMachineStatusModel());
        Thread thread = new Thread(machineService.Start);
        thread.Start();

        var builder = WebApplication.CreateBuilder(args); // build and run api
        builder.Services.AddControllers(); // Add services to the container.
        builder.Services.AddOpenApi(); // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddSingleton(machineService);
        var app = builder.Build();
        if (app.Environment.IsDevelopment()) // Configure the HTTP request pipeline.
        {
            app.MapOpenApi();
        }
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}