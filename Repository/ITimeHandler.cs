using BeerMachineApi.Services.DTOs;

public interface ITimeHandler
{
    public void SaveTimeAsync(TimeDTO time, IServiceScopeFactory scopeFactory);
}
