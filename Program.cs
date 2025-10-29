using BeerMachineApi;
public class Program
{
    static void Main(string[] args)
    {
        // Run the BeerMachineHandler on a thread so the process does not block the api

        BeerMachineApi.Models.IMachineService machineService = new BeerMachineService(new BeerMachineApi.Models.BeerMachineStatusModel());
        Thread thread = new Thread(machineService.Start);
        thread.Start();

        // build and run api
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddSingleton(machineService);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}