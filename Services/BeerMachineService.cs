using Opc.UaFx;
using Opc.UaFx.Client;

using BeerMachineApi.Models;
using Org.BouncyCastle.Crypto.Engines;

namespace BeerMachineApi.Services
{
    public class BeerMachineService : IMachineService
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
                ConnectToServer();
                Thread.Sleep(500);
                StopMachine();
                Thread.Sleep(500);
                ResetMachine();

                while (true)
                {
                    _statusModel.UpdateModel(_opcSession);
                    Console.Clear();
                    Console.WriteLine(_statusModel.ToString());
                }
            }
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
                    WriteBatchToServer(new Batch(id, (BeerType)type, amount, speed));
                    break;

                case "start":
                    StartBatch();
                    break;

                case "reset":
                    ResetMachine();
                    break;

                case "stop":
                    StopMachine();
                    break;

                case "connect":
                    ConnectToServer();
                    break;

                case "disconnect":
                    DisconnectFromServer();
                    break;

                default:
                    throw new Exception($"No command matching type {command.Type}");
            }
        }
        //stop->reset->start
        private void WriteBatchToServer(Batch batch)
        {
            // Values are up casted to insure that the types algin with the expected data types

            OpcWriteNode[] commands = {
            new OpcWriteNode(NodeIds.CmdId, batch.Id),
            new OpcWriteNode(NodeIds.CmdType, (float) batch.Type),
            new OpcWriteNode(NodeIds.CmdAmount, batch.Amount),
            new OpcWriteNode(NodeIds.MachSpeed, batch.Speed)
        };
            if (_opcSession == null) throw new Exception();

            _opcSession.WriteNodes(commands);
        }

        private void StartBatch()
        {
            OpcWriteNode[] commands = {
                new(NodeIds.CntrlCmd, 2),
                new(NodeIds.CmdChangeRequest, true)
            };
            _opcSession.WriteNodes(commands);
            Thread.Sleep(1000);
        }

        private void ResetMachine()
        {
            OpcWriteNode[] commands = {
                new(NodeIds.CntrlCmd, 1),
                new(NodeIds.CmdChangeRequest, true)
            };
            _opcSession.WriteNodes(commands);
            Thread.Sleep(1000);
        }

        private void StopMachine()
        {
            OpcWriteNode[] commands = {
                new(NodeIds.CntrlCmd, 3),
                new(NodeIds.CmdChangeRequest, true)
            };
            _opcSession.WriteNodes(commands);
            Thread.Sleep(1000);
        }

        private void ConnectToServer()
        {
            _opcSession.Connect();
        }
        private void DisconnectFromServer()
        {
            _opcSession.Disconnect();
            _opcSession.Dispose(); //Clean up in case it wasn't automatically handled
        }
    }
}