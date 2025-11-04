using BeerMachineApi.Services.DTOs;
public interface IBatchHandler
{
    public void SaveBatchAsync(BatchDTO batch, IServiceScopeFactory scopeFactory);
    public void SaveBatchChangesAsync(BatchDTO batch, IServiceScopeFactory scopeFactory);
}

