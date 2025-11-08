using Opc.UaFx;
using Opc.UaFx.Client;

using BeerMachineApi.Services.DTOs;
using BeerMachineApi.Services.StatusModels;
using BeerMachineApi.Repository;
using System.Threading.Tasks;

namespace BeerMachineApi.Services;

public class BeerMachineService : MachineCommands, IMachineService
{
    private BeerMachineStatusModel _machineStatusModel;
    private BatchStatusModel _batchStatusModel;
    private InventoryStatusModel _inventoryStatusModel;
    private OpcClient? _opcClient;
    private readonly string _serverURL;
    private Queue<Func<OpcClient>> _machineCommandQueue;
    private Queue<BatchDTO> _batchQueue;
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
        _machineCommandQueue = new Queue<Func<OpcClient>>();

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
            ConnectToServer(_opcClient);

            _inventoryStatusModel.UpdateModel(_opcClient);

            OpcSubscribeDataChange[] subscriptions = {
                new OpcSubscribeDataChange(NodeIds.AdminProcessedCount, HandleProcessedChange),
                new OpcSubscribeDataChange(NodeIds.Barley, HandleInventoryChange),
                new OpcSubscribeDataChange(NodeIds.Malt, HandleInventoryChange),
                new OpcSubscribeDataChange(NodeIds.Yeast, HandleInventoryChange),
                new OpcSubscribeDataChange(NodeIds.Wheat, HandleInventoryChange),
                new OpcSubscribeDataChange(NodeIds.Hops, HandleInventoryChange)
            };

            OpcSubscription subscription = _opcClient.SubscribeNodes(subscriptions);

            while (true)
            {

            }
        }
    }

    private async void HandleProcessedChange(object sender, OpcDataChangeReceivedEventArgs e)
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
            _machineStatusModel.Vibration
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
            HandleBatchProcess();
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

    /// <summary>
    /// The machine is stop and reset, if the queue is not empty the next batch will be started
    /// </summary>
    private async void HandleBatchProcess()
    {
        Thread.Sleep(500);
        StopMachine(_opcClient);
        Thread.Sleep(500);
        ResetMachine(_opcClient);

        // If there is more batches in the queue, the next should be started
        if (_batchQueue.Count > 0)
        {
            Thread.Sleep(500);
            ExecuteCommand(new Command() { Type = "start" }); // start the next batch
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

            default:
                throw new Exception("status time does not exist");
        }
    }

    public async void ExecuteCommand(Command command)
    {
        if (_opcClient == null) throw new Exception("OpcClient error");

        switch (command.Type.ToLower())
        {
            case "batch":
                if (command.Parameters == null)
                    throw new BadHttpRequestException("when create a batch parameter cannot be null");

                _batchQueue.Enqueue(new BatchDTO()
                {
                    Id = command.Parameters["id"],
                    Amount = command.Parameters["amount"],
                    Speed = command.Parameters["speed"],
                    Type = command.Parameters["type"],
                    UserId = command.Parameters["user"]
                });
                break;

            case "start":
                if (_batchQueue.Count > 0)
                {
                    BatchDTO batch = _batchQueue.Dequeue();
                    StartBatch(_opcClient, batch);
                    _iBatchHandler.SaveBatchAsync(batch);
                }
                else
                {
                    throw new BadHttpRequestException("no queued batches");
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

            case "pause":

                break;

            default:
                throw new Exception($"No command matching type {command.Type}");
        }
    }
}