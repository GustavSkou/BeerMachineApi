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

    protected void SaveBatch(BatchDTO batch, MachineDbContext _dbContext)
    {
        var entity = new Batch
        {
            UserId = 1, // this should be set to batchDTO user
            Amount = (int)batch.Amount,
            TypeId = (int)batch.Type,
            StartedAt = DateTime.UtcNow
        };
        _dbContext.Batches.Add(entity);
        _dbContext.SaveChanges();
    }

    protected void UpdateBatchProducedAmount(BatchStatusModel batchStatusModel, MachineDbContext dbContext)
    {
        var existing = dbContext.Batches.FirstOrDefault(b => b.Id == batchStatusModel.BatchId);
        if (existing == null) return; // or throw new InvalidOperationException($"Batch {batch.Id} not found");

        existing.Failed = batchStatusModel.DefectiveAmount;
        existing.AmountCompleted = batchStatusModel.ProducedAmount;
        dbContext.SaveChanges();
    }

    protected void UpdateBatchCompletedAt(BatchStatusModel batchStatusModel, MachineDbContext dbContext)
    {
        var existing = dbContext.Batches.FirstOrDefault(b => b.Id == batchStatusModel.BatchId);
        if (existing == null) return; // or throw new InvalidOperationException($"Batch {batch.Id} not found");

        existing.CompletedAt = DateTime.UtcNow;

        dbContext.SaveChanges();
    }

    protected void SaveTime(BatchStatusModel batchStatusModel, BeerMachineStatusModel machineStatusModel, MachineDbContext dbContext)
    {
        var timeEntity = new Time
        {
            BatchId = (int)batchStatusModel.BatchId,
            Temperature = machineStatusModel.Temperature,
            Humidity = machineStatusModel.Humidity,
            Vibration = machineStatusModel.Vibration,
            TimeStamp = DateTime.UtcNow
        };
        dbContext.Times.Add(timeEntity);
        dbContext.SaveChanges();
    }


}