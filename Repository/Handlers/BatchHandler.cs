using BeerMachineApi.Repository;
using BeerMachineApi.Services.DTOs;

public class BatchHandler : EntityHandler, IBatchHandler
{
    public BatchHandler( IServiceScopeFactory scopeFactory ) : base ( scopeFactory ) { }

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
        db.SaveChanges();
    }

    public async void SaveBatchChangesAsync(BatchDTO batch)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MachineDbContext>();

        var existing = db.Batches.FirstOrDefault(b => b.Id == (int)batch.Id);
        if (existing == null) return;

        var nowUnspecified = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        existing.Failed = batch.DefectiveAmount;
        existing.AmountCompleted = batch.ProducedAmount;
        existing.AmountCompleted = batch.DefectiveAmount;
        existing.Failed = batch.DefectiveAmount;
        existing.UpdatedAt = nowUnspecified;

        if (batch.Amount == batch.ProducedAmount)
            existing.CompletedAt = nowUnspecified;

        await db.SaveChangesAsync();
    }
}
