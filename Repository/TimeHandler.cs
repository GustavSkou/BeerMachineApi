using BeerMachineApi.Repository;
using BeerMachineApi.Services.DTOs;

public class TimeHandler : ITimeHandler
{
    public async void SaveTimeAsync(TimeDTO time, IServiceScopeFactory scopeFactory)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MachineDbContext>();

        var nowUnspecified = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        var timeEntity = new Time
        {
            BatchId = (int)time.Id,
            Temperature = time.Temperature,
            Humidity = time.Humidity,
            Vibration = time.Vibration,
            TimeStamp = nowUnspecified,
            CreatedAt = nowUnspecified,
            UpdatedAt = nowUnspecified,
        };

        db.Times.Add(timeEntity);
        await db.SaveChangesAsync();
    }
}