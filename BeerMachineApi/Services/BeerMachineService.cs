using Opc.UaFx.Client;
using System.Collections.Concurrent;

using BeerMachineApi.Services.DTOs;
using BeerMachineApi.Services.StatusModels;
using BeerMachineApi.Repository;

namespace BeerMachineApi.Services;

public class BeerMachineService : MachineCommands, IMachineService
{
    public bool IsConnected
    {
        get { return _isConnected; }
    }

    public ConcurrentQueue<Command> CommandQueue
    {
        get { return _commandQueue; }
    }

    public OpcClient? OpcClient
    {
        get { return _opcClient; }
    }

    private bool _isConnected = false;
    private BeerMachineStatusModel _machineStatusModel;
    private BatchStatusModel _batchStatusModel;
    private InventoryStatusModel _inventoryStatusModel;
    private MaintenanceStatusModel _maintenanceStatusModel;
    private OpcClient? _opcClient;
    private readonly string _serverURL;
    private readonly ConcurrentQueue<Command> _commandQueue;
    private readonly Queue<BatchDTO> _batchQueue;
    private readonly IBatchHandler _iBatchHandler;
    private readonly ITimeHandler _iTimeHandler;

    public BeerMachineService(
        BeerMachineStatusModel beerMachineStatusModel,
        BatchStatusModel batchStatusModel,
        InventoryStatusModel inventoryStatusModel,
        MaintenanceStatusModel maintenanceStatusModel,
        IBatchHandler iBatchHandler,
        ITimeHandler iTimehandler,
        bool simulated = true
    )
    {
        _machineStatusModel = beerMachineStatusModel;
        _batchStatusModel = batchStatusModel;
        _inventoryStatusModel = inventoryStatusModel;
        _maintenanceStatusModel = maintenanceStatusModel;

        _iTimeHandler = iTimehandler;
        _iBatchHandler = iBatchHandler;

        _batchQueue = new Queue<BatchDTO>();
        _commandQueue = new ConcurrentQueue<Command>();

        Thread commandQueueThread = new Thread(RunCommandQueueProcessor);
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
            // Setup events
            _opcClient.Connecting += (sender, e) => { Console.WriteLine("Connecting to BeerMachine"); };
            _opcClient.Connected += (sender, e) => OnConnected();
            _opcClient.Disconnected += (sender, e) => OnDisconnected();

            while (true) { } // keep connection alive. Todo True should be replace by some cancellation token
        }
    }

    public void TryToConnectToServer()
    {
        try
        {
            ConnectToServer(_opcClient);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Connection to machine failed, retrying...");
            Thread.Sleep(1000);
            TryToConnectToServer();
        }
    }

    private void OnConnected()
    {
        Console.WriteLine("Connected to BeerMachine");
        _isConnected = true;

        OpcSubscribeDataChange[] subscriptions = GetSubscriptions();
        _opcClient.SubscribeNodes(subscriptions);

        Console.Write(_opcClient);

        // update models OnConnected
        _inventoryStatusModel.UpdateModel(_opcClient);
        _machineStatusModel.UpdateModel(_opcClient);
        _batchStatusModel.UpdateModel(_opcClient);
        _maintenanceStatusModel.UpdateModel(_opcClient);
    }

    private void OnDisconnected()
    {
        _isConnected = false;
    }

    /// <summary>
    /// The machine is stop and reset, if the queue is not empty the next batch will be started
    /// </summary>
    private void HandleBatchProcess()
    {
        QueueCommand(new Command { Type = "stop" });
        QueueCommand(new Command { Type = "reset" });

        // If there is more batches in the queue, the next should be started
        if (_batchQueue.Count > 0)
        {
            _commandQueue.Enqueue(new Command() { Type = "start" }); // start the next batch
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

            case "maintenance":
                return _maintenanceStatusModel;

            case "queue":
                return _batchQueue.ToArray();
            default:
                throw new Exception("status does not exist");
        }
    }

    public void QueueCommand(Command command)
    {
        _commandQueue.Enqueue(command);
    }

    private async Task ProcessCommand(Command command)
    {
        if (_opcClient == null) throw new Exception("OpcClient error");

        switch (command.Type.ToLower())
        {
            case "batch":
                if (command.Parameters == null)
                {
                    throw new BadHttpRequestException("when creating a batch, parameter cannot be null");
                }

                if (!command.Parameters.TryGetValue("amount", out int amount) ||
                    !command.Parameters.TryGetValue("speed", out int speed) ||
                    !command.Parameters.TryGetValue("type", out int type) ||
                    !command.Parameters.TryGetValue("user", out int user))
                {
                    throw new BadHttpRequestException("Parameter missing");
                }

                _batchQueue.Enqueue(new BatchDTO()
                {
                    Amount = amount,
                    Speed = speed,
                    Type = type,
                    UserId = user
                });
                Console.WriteLine("batch queued");
                break;

            case "start":
                if (_batchQueue.Count > 0)
                {

                    BatchDTO batch = _batchQueue.Dequeue();
                    _batchStatusModel.UserId = batch.UserId;
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

        _iBatchHandler.SaveBatchChangesAsync(_batchStatusModel.GetDTO());

        _iTimeHandler.SaveTimeAsync(new TimeDTO(
            (int)_batchStatusModel.BatchId,
            _machineStatusModel.Speed,
            _machineStatusModel.Temperature,
            _machineStatusModel.Humidity,
            _machineStatusModel.Vibration
        ));

        if (_batchStatusModel.IsBatchDone())
        {
            // save when batch is done, it will be marked  as completed in db
            _iBatchHandler.SaveBatchChangesAsync(_batchStatusModel.GetDTO());
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
        _maintenanceStatusModel.UpdateModel(_opcClient);
    }

    private OpcSubscribeDataChange[] GetSubscriptions()
    {
        return new OpcSubscribeDataChange[] {
            new OpcSubscribeDataChange(NodeIds.AdminProcessedCount, HandleProcessedChange), // on batch completed amount change
            new OpcSubscribeDataChange(NodeIds.Barley, HandleInventoryChange),              // inventory change
            new OpcSubscribeDataChange(NodeIds.Malt, HandleInventoryChange),
            new OpcSubscribeDataChange(NodeIds.Yeast, HandleInventoryChange),
            new OpcSubscribeDataChange(NodeIds.Wheat, HandleInventoryChange),
            new OpcSubscribeDataChange(NodeIds.Hops, HandleInventoryChange),
            new OpcSubscribeDataChange(NodeIds.MaintenanceCount, HandleMaintenanceChange)   // maintenance change
        };
    }

    private void RunCommandQueueProcessor()
    {
        // Go step by step though each command and send it to the machine
        while (true)
        {
            if (_commandQueue.TryDequeue(out Command? command))
            {
                ProcessCommand(command);
                Thread.Sleep(500);  // wait to ensure that command has been process by the machine
            }
            else
            {
                Thread.Sleep(100);
            }
        }
    }
}