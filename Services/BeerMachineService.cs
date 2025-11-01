using Opc.UaFx;
using Opc.UaFx.Client;

using BeerMachineApi.Repository;

namespace BeerMachineApi.Services
{
    public class BeerMachineService : MachineCommands, IMachineService
    {
        private BeerMachineStatusModel _statusModel;
        private OpcClient? _opcSession;
        private readonly string _serverURL;

        public BeerMachineService(BeerMachineStatusModel beerMachineStatusModel, bool simulated = true)
        {
            _statusModel = beerMachineStatusModel;
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
            using (_opcSession = new OpcClient(_serverURL))
            {
                ConnectToServer(_opcSession);

                // when there is a change to the amount of processed beers the method HandleDataChanged is called
                //OpcSubscription subscription = _opcSession.SubscribeDataChange(NodeIds.AdminProcessedCount, HandleDataChange);

                OpcSubscribeDataChange[] subscriptions = {
                    new OpcSubscribeDataChange(NodeIds.AdminProcessedCount, HandleDataChange)
                };

                OpcSubscription subscription = _opcSession.SubscribeNodes(subscriptions);

                while (true) { }
            }
        }

        private void HandleDataChange(object sender, OpcDataChangeReceivedEventArgs e)
        {
            // The 'sender' variable contains the OpcMonitoredItem with the NodeId.
            OpcMonitoredItem item = (OpcMonitoredItem)sender;

            _statusModel.UpdateModel(_opcSession);
            Console.Clear();
            Console.WriteLine($"Data Change {item.NodeId}: {e.Item.Value}\n{_statusModel}");
        }

        public object GetStatus()
        {
            return _statusModel;
        }

        public void ExecuteCommand(Command command)
        {
            switch (command.Type.ToLower())
            {
                case "batch":
                    if (command.Parameters == null) throw new Exception("batch command parameters cannot be null");

                    float id = command.Parameters["id"];
                    float type = command.Parameters["type"];
                    float amount = command.Parameters["amount"];
                    float speed = command.Parameters["speed"];
                    WriteBatchToServer(_opcSession, id, type, amount, speed);
                    break;

                case "start":
                    StartBatch(_opcSession);
                    break;

                case "reset":
                    ResetMachine(_opcSession);
                    break;

                case "stop":
                    StopMachine(_opcSession);
                    break;

                case "connect":
                    ConnectToServer(_opcSession);
                    break;

                case "disconnect":
                    DisconnectFromServer(_opcSession);
                    break;

                default:
                    throw new Exception($"No command matching type {command.Type}");
            }
        }
    }
}