using BeerMachineApi.Repository;
using BeerMachineApi.Services.DTOs;
using Microsoft.EntityFrameworkCore;

public class BatchHandler : EntityHandler, IBatchHandler
{
    public BatchHandler(IServiceScopeFactory scopeFactory) : base(scopeFactory) { }

    public async void SaveBatchAsync(BatchDTO batch)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MachineDbContext>();

        var nowUnspecified = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        var entity = new Batch
        {
            UserId = batch.UserId,
            Amount = (int)batch.Amount,
            AmountCompleted = 0,
            Failed = 0,
            TypeId = (int)batch.Type,
            StartedAt = nowUnspecified,
            CreatedAt = nowUnspecified,
            UpdatedAt = nowUnspecified
        };
        await db.Batches.AddAsync(entity);
        await db.SaveChangesAsync();
    }

    public async void SaveBatchChangesAsync(BatchDTO batchDTO)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MachineDbContext>();

        // Use the async EF method to avoid blocking threads
        var batch = await db.Batches.FirstOrDefaultAsync(b => b.Id == (int)batchDTO.Id);
        if (batch == null) return;

        var nowUnspecified = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        batch.AmountCompleted = batchDTO.ProducedAmount;
        batch.Failed = batchDTO.DefectiveAmount;
        batch.UpdatedAt = nowUnspecified;

        if (batch.Amount == batchDTO.ProducedAmount)
            batch.CompletedAt = nowUnspecified;

        await db.SaveChangesAsync();
    }
}
