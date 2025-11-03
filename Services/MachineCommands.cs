using Opc.UaFx;
using Opc.UaFx.Client;
using BeerMachineApi.Services.DTOs;
using BeerMachineApi.Repository;
using BeerMachineApi.Services.StatusModels;
namespace BeerMachineApi.Services;

public class MachineCommands
{
    protected void StartBatch(OpcClient opcClient, BatchDTO batch)
    {
        WriteBatchToServer(opcClient, batch);

        OpcWriteNode[] commands = {
                new(NodeIds.CntrlCmd, 2),
                new(NodeIds.CmdChangeRequest, true)
        };

        opcClient.WriteNodes(commands);
    }

    protected void ResetMachine(OpcClient opcSession)
    {
        OpcWriteNode[] commands = {
                new(NodeIds.CntrlCmd, 1),
                new(NodeIds.CmdChangeRequest, true)
            };
        opcSession.WriteNodes(commands);
    }

    protected void StopMachine(OpcClient opcSession)
    {
        OpcWriteNode[] commands = {
                new(NodeIds.CntrlCmd, 3),
                new(NodeIds.CmdChangeRequest, true)
            };
        opcSession.WriteNodes(commands);
    }

    protected void ConnectToServer(OpcClient opcSession)
    {
        opcSession.Connect();
    }

    protected void DisconnectFromServer(OpcClient opcSession)
    {
        opcSession.Disconnect();
        opcSession.Dispose(); //Clean up in case it wasn't automatically handled
    }

    protected void WriteBatchToServer(OpcClient opcSession, BatchDTO batch)
    {
        OpcWriteNode[] commands = {
            new OpcWriteNode(NodeIds.CmdId, batch.Id),
            new OpcWriteNode(NodeIds.CmdType, batch.Type),
            new OpcWriteNode(NodeIds.CmdAmount, batch.Amount),
            new OpcWriteNode(NodeIds.CmdMachSpeed, batch.Speed)
        };
        opcSession.WriteNodes(commands);
    }

    protected async void SaveBatch(BatchDTO batch, IServiceScopeFactory scopeFactory)
    {
        using var scope = scopeFactory.CreateScope();
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

    protected void UpdateBatchProducedAmount(BatchStatusModel batchStatusModel, IServiceScopeFactory scopeFactory)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MachineDbContext>();

        var existing = db.Batches.FirstOrDefault(b => b.Id == (int)batchStatusModel.BatchId);
        if (existing == null) return;

        var nowUnspecified = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        existing.Failed = batchStatusModel.DefectiveAmount;
        existing.AmountCompleted = batchStatusModel.ProducedAmount;
        existing.UpdatedAt = nowUnspecified;
        db.SaveChanges();
    }

    protected void UpdateBatchCompletedAt(BatchStatusModel batchStatusModel, IServiceScopeFactory scopeFactory)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MachineDbContext>();

        var existing = db.Batches.FirstOrDefault(b => b.Id == (int)batchStatusModel.BatchId);
        if (existing == null) return;

        var nowUnspecified = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        existing.CompletedAt = nowUnspecified;
        existing.UpdatedAt = nowUnspecified;
        db.SaveChanges();
    }

    protected void SaveTime(BatchStatusModel batchStatusModel, BeerMachineStatusModel machineStatusModel, IServiceScopeFactory scopeFactory)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MachineDbContext>();

        var nowUnspecified = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        var timeEntity = new Time
        {
            BatchId = (int)batchStatusModel.BatchId,
            Temperature = machineStatusModel.Temperature,
            Humidity = machineStatusModel.Humidity,
            Vibration = machineStatusModel.Vibration,
            TimeStamp = nowUnspecified,
            CreatedAt = nowUnspecified,
            UpdatedAt = nowUnspecified
        };

        db.Times.Add(timeEntity);
        db.SaveChanges();
    }
}