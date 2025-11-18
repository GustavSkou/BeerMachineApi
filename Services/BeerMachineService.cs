using Opc.UaFx;
using Opc.UaFx.Client;

using BeerMachineApi.Services.DTOs;
using BeerMachineApi.Services.StatusModels;
using BeerMachineApi.Repository;
using System.Collections.Concurrent;

namespace BeerMachineApi.Services;

public class BeerMachineService : MachineCommands, IMachineService
{
    private BeerMachineStatusModel _machineStatusModel;
    private BatchStatusModel _batchStatusModel;
    private InventoryStatusModel _inventoryStatusModel;
    private OpcClient? _opcClient;
    private readonly string _serverURL;
    private readonly ConcurrentQueue<Command> _machineCommandQueue;
    private readonly Queue<BatchDTO> _batchQueue;
    private readonly IBatchHandler _iBatchHandler;
    private readonly ITimeHandler _iTimeHandler;

    public BeerMachineService(
        BeerMachineStatusModel beerMachineStatusModel,
        BatchStatusModel batchStatusModel,
        InventoryStatusModel inventoryStatusModel,
        IBatchHandler iBatchHandler,
        ITimeHandler iTimehandler,
        bool simulated = true
    )
    {
        _machineStatusModel = beerMachineStatusModel;
        _batchStatusModel = batchStatusModel;
        _inventoryStatusModel = inventoryStatusModel;

        _iTimeHandler = iTimehandler;
        _iBatchHandler = iBatchHandler;

        _batchQueue = new Queue<BatchDTO>();
        _machineCommandQueue = new ConcurrentQueue<Command>();

        Thread commandQueueThread = new Thread(() =>
        {
            while (true)
            {
                if (_machineCommandQueue.TryDequeue(out Command? command))
                {
                    ProcessCommand(command);
                    Thread.Sleep(500);  // wait to ensure that command has been process by the machine
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        });
        commandQueueThread.IsBackground = true;
        commandQueueThread.Start();

        switch (simulated)
        {
            case true:
                _serverURL = "opc.tcp://127.0.0.1:4840"; //Simulated PLC
                break;

            case false:
                _serverURL = "opc.tcp://192.168.0.122:4840"; //Physical PLC
                break;
        }
    }

    public void Start()
    {
        using (_opcClient = new OpcClient(_serverURL))
        {
            _opcClient.Connecting += (sender, e) => { Console.WriteLine("Connecting to BeerMachine"); };
            // When the opcClient is connected to the machine
            _opcClient.Connected += (sender, e) => OnConnected();

            Console.Clear();
            try
            {
                ConnectToServer(_opcClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection to machine failed, retrying...");
                Thread.Sleep(1000);
                Start();
            }

            while (true) { } // keep connection alive. Todo True should be replace by some cancellation token
        }
    }

    private void OnConnected()
    {
        Console.WriteLine("Connected to BeerMachine");

        OpcSubscribeDataChange[] subscriptions = GetSubscriptions();
        _opcClient.SubscribeNodes(subscriptions);

        _inventoryStatusModel.UpdateModel(_opcClient);
        _machineStatusModel.UpdateModel(_opcClient);
        _batchStatusModel.UpdateModel(_opcClient);
    }

    /// <summary>
    /// The machine is stop and reset, if the queue is not empty the next batch will be started
    /// </summary>
    private void HandleBatchProcess()
    {
        ExecuteCommand(new Command { Type = "stop" });
        ExecuteCommand(new Command { Type = "reset" });

        // If there is more batches in the queue, the next should be started
        if (_batchQueue.Count > 0)
        {
            _machineCommandQueue.Enqueue(new Command() { Type = "start" }); // start the next batch
        }
    }

    public object GetStatus(string type)
    {
        switch (type)
        {
            case "machine":
                return _machineStatusModel;

            case "batch":
                return _batchStatusModel;

            case "inventory":
                return _inventoryStatusModel;

            case "queue":
                return _batchQueue.ToArray();
            default:
                throw new Exception("status does not exist");
        }
    }

    public void ExecuteCommand(Command command)
    {
        _machineCommandQueue.Enqueue(command);
    }

    private async Task ProcessCommand(Command command)
    {
        if (_opcClient == null) throw new Exception("OpcClient error");

        switch (command.Type.ToLower())
        {
            case "batch":
                if (command.Parameters == null)
                    throw new BadHttpRequestException("when create a batch parameter cannot be null");

                _batchQueue.Enqueue(new BatchDTO()
                {
                    Amount = command.Parameters["amount"],
                    Speed = command.Parameters["speed"],
                    Type = command.Parameters["type"],
                    UserId = command.Parameters["user"]
                });
                Console.WriteLine("batch queued");
                break;

            case "start":
                if (_batchQueue.Count > 0)
                {
                    BatchDTO batch = _batchQueue.Dequeue();
                    batch.Id = (float)await _iBatchHandler.GetNextId(); // get the id from the db
                    StartBatch(_opcClient, batch);
                    _iBatchHandler.SaveBatchAsync(batch);
                }
                break;

            case "reset":
                ResetMachine(_opcClient); //make sure that if there is a batch running that it is save
                break;

            case "stop":
                StopMachine(_opcClient);
                break;

            case "connect":
                ConnectToServer(_opcClient);
                break;

            case "disconnect":
                DisconnectFromServer(_opcClient);
                break;

            case "abort":
                AbortMachine(_opcClient);
                break;

            case "pause":

                break;

            default:
                throw new Exception($"No command matching type {command.Type}");
        }
    }

    private void HandleProcessedChange(object sender, OpcDataChangeReceivedEventArgs e)
    {
        // The 'sender' variable contains the OpcMonitoredItem with the NodeId
        OpcMonitoredItem item = (OpcMonitoredItem)sender;

        _machineStatusModel.UpdateModel(_opcClient);
        _batchStatusModel.UpdateModel(_opcClient);

        if (_batchStatusModel.BatchId == null || _batchStatusModel.BatchId == 0)
            return;

        _iBatchHandler.SaveBatchChangesAsync(new BatchDTO()
        {
            Id = (float)_batchStatusModel.BatchId,
            ProducedAmount = _batchStatusModel.ProducedAmount,
            DefectiveAmount = _batchStatusModel.DefectiveAmount,
        });

        _iTimeHandler.SaveTimeAsync(new TimeDTO(
            (int)_batchStatusModel.BatchId,
            _machineStatusModel.Temperature,
            _machineStatusModel.Humidity,
            _machineStatusModel.Vibration,
            _machineStatusModel.Speed
        ));

        if (_batchStatusModel.IsBatchDone())
        {
            // save when batch is done, it will be marked  as completed in db
            _iBatchHandler.SaveBatchChangesAsync(new BatchDTO()
            {
                Id = (float)_batchStatusModel.BatchId,
                Amount = _batchStatusModel.ToProduceAmount,
                ProducedAmount = _batchStatusModel.ProducedAmount,
                DefectiveAmount = _batchStatusModel.DefectiveAmount,
            });
            HandleBatchProcess(); // Remove await since HandleBatchProcess is void
        }
        Console.Clear();
        Console.WriteLine($"Data Change {item.NodeId}: {e.Item.Value}\n{_machineStatusModel}\n{_batchStatusModel}");
    }

    private void HandleInventoryChange(object sender, OpcDataChangeReceivedEventArgs e)
    {
        _inventoryStatusModel.UpdateModel(_opcClient);
    }

    private void HandleMaintenanceChange(object sender, OpcDataChangeReceivedEventArgs e)
    {

    }

    private OpcSubscribeDataChange[] GetSubscriptions()
    {
        return new OpcSubscribeDataChange[] {
            new OpcSubscribeDataChange(NodeIds.AdminProcessedCount, HandleProcessedChange),
            new OpcSubscribeDataChange(NodeIds.Barley, HandleInventoryChange),
            new OpcSubscribeDataChange(NodeIds.Malt, HandleInventoryChange),
            new OpcSubscribeDataChange(NodeIds.Yeast, HandleInventoryChange),
            new OpcSubscribeDataChange(NodeIds.Wheat, HandleInventoryChange),
            new OpcSubscribeDataChange(NodeIds.Hops, HandleInventoryChange)
        };
    }
}