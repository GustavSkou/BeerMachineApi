using Opc.UaFx;
using Opc.UaFx.Client;

namespace BeerMachineApi.Services
{
    public class BeerMachineService : MachineCommands, IMachineService
    {
        private BeerMachineStatusModel _machineStatusModel;
        private BatchStatusModel _batchStatusModel;
        private BatchQueue _batchQueue;
        private OpcClient? _opcClient;
        private readonly string _serverURL;

        public BeerMachineService(BeerMachineStatusModel beerMachineStatusModel, BatchStatusModel batchStatusModel, BatchQueue batchQueue, bool simulated = true)
        {
            _machineStatusModel = beerMachineStatusModel;
            _batchStatusModel = batchStatusModel;
            _batchQueue = batchQueue;

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
            // The 'sender' variable contains the OpcMonitoredItem with the NodeId.
            OpcMonitoredItem item = (OpcMonitoredItem)sender;

            _machineStatusModel.UpdateModel(_opcClient);
            _batchStatusModel.UpdateModel(_opcClient);

            Console.Clear();
            Console.WriteLine(_batchQueue.Count);
            if (_batchStatusModel.ProducedAmount == (int)_batchStatusModel.ToProduceAmount)
            {
                Thread.Sleep(500);
                StopMachine(_opcClient);
                Thread.Sleep(500);
                ResetMachine(_opcClient);

                if (_batchQueue.Count > 0)
                {
                    Thread.Sleep(500);
                    StartBatch(_opcClient, _batchQueue);
                }
                else
                {
                    Console.WriteLine("no batches queued");
                }
            }
            Console.WriteLine($"Data Change {item.NodeId}: {e.Item.Value}\n{_machineStatusModel}\n{_batchStatusModel}");
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
                    StartBatch(_opcClient, _batchQueue);
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

                default:
                    throw new Exception($"No command matching type {command.Type}");
            }
        }
    }
}