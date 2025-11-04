namespace BeerMachineApi.Repository;
using BeerMachineApi.Services.DTOs;

public interface IBatchHandler
{
    public void SaveBatchAsync(BatchDTO batch);
    public void SaveBatchChangesAsync(BatchDTO batch);
}