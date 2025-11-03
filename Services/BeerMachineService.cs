using Opc.UaFx;
using Opc.UaFx.Client;

using BeerMachineApi.Services.DTOs;
using BeerMachineApi.Services.StatusModels;
using BeerMachineApi.Repository;

namespace BeerMachineApi.Services
{
    public class BeerMachineService : MachineCommands, IMachineService
    {
        private BeerMachineStatusModel _machineStatusModel;
        private BatchStatusModel _batchStatusModel;
        private OpcClient? _opcClient;
        private readonly string _serverURL;
        private Queue<Func<OpcClient>> _machineCommandQueue;
        private Queue<BatchDTO> _batchQueue;
        private readonly IServiceScopeFactory _scopeFactory;

        public BeerMachineService(BeerMachineStatusModel beerMachineStatusModel, BatchStatusModel batchStatusModel, IServiceScopeFactory scopeFactory, bool simulated = true)
        {
            _machineStatusModel = beerMachineStatusModel;
            _batchStatusModel = batchStatusModel;
            _batchQueue = new Queue<BatchDTO>();
            _machineCommandQueue = new Queue<Func<OpcClient>>();
            _scopeFactory = scopeFactory;

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

                // when there is a change to the amount of processed beers the method HandleDataChanged is called
                OpcSubscribeDataChange[] subscriptions = {
                    new OpcSubscribeDataChange(NodeIds.AdminProcessedCount, HandleProcessedChange)
                };

                OpcSubscription subscription = _opcClient.SubscribeNodes(subscriptions);

                while (true)
                {

                }
            }
        }

        private void HandleProcessedChange(object sender, OpcDataChangeReceivedEventArgs e)
        {
            _machineStatusModel.UpdateModel(_opcClient);
            _batchStatusModel.UpdateModel(_opcClient);

            if (_batchStatusModel.BatchId == null || _batchStatusModel.BatchId == 0)
            {

            }
            else
            {
                Console.Clear();
                // The 'sender' variable contains the OpcMonitoredItem with the NodeId
                OpcMonitoredItem item = (OpcMonitoredItem)sender;




                if (_batchStatusModel.BatchId != null)
                {
                    SaveTime(_batchStatusModel, _machineStatusModel, _scopeFactory); // Save time
                    UpdateBatchProducedAmount(_batchStatusModel, _scopeFactory); // Update the batch produced amount
                }




                if (_batchStatusModel.ProducedAmount == (int)_batchStatusModel.ToProduceAmount)
                {
                    UpdateBatchCompletedAt(_batchStatusModel, _scopeFactory); // update the batch to be completed

                    Thread.Sleep(500);
                    StopMachine(_opcClient);
                    Thread.Sleep(500);
                    ResetMachine(_opcClient);

                    if (_batchQueue.Count > 0) // if there is more batches in the queue
                    {
                        Thread.Sleep(500);
                        ExecuteCommand(new Command() { Type = "start" }); // start the next batch
                    }
                    else
                    {
                        Console.WriteLine("no batches queued");
                    }
                }
                Console.WriteLine($"Data Change {item.NodeId}: {e.Item.Value}\n{_machineStatusModel}\n{_batchStatusModel}");
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

                case "queue":


                default:
                    throw new Exception("status time does not exist");
            }
        }

        public void ExecuteCommand(Command command)
        {
            if (_opcClient == null) throw new Exception("OpcClient error");

            switch (command.Type.ToLower())
            {
                case "batch":
                    if (command.Parameters == null) throw new Exception("batch command parameters cannot be null");

                    _batchQueue.Enqueue(new BatchDTO(
                        command.Parameters["id"],
                        command.Parameters["amount"],
                        command.Parameters["speed"],
                        command.Parameters["type"]
                    ));
                    break;

                case "start":
                    BatchDTO batch = _batchQueue.Dequeue();
                    StartBatch(_opcClient, batch);
                    SaveBatch(batch, _scopeFactory);
                    break;

                case "reset":
                    ResetMachine(_opcClient);
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
}