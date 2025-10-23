using Opc.UaFx;
using Opc.UaFx.Client;

namespace BeerMachine
{
    public class BeerMachineHandler
    {
        private OpcClient? _opcSession;

        //private string _serverURL = "opc.tcp://192.168.0.122:4840";  //Physical PLC
        private string _serverURL = "opc.tcp://127.0.0.1:4840";      //Simulated PLC

        public void Run()
        {
            using (_opcSession = new OpcClient(_serverURL))
            {
                ConnectToServer();
                ResetMachine();

                int batchId = 1;
                BeerType beerType = BeerType.AlcoholFree;
                int amount = 200;
                int machineSpeed = 150;
                WriteBatchToServer(new Batch(batchId, beerType, amount, machineSpeed));

                while (true)
                {
                    MachineStatusModel.Instance.UpdateModel(_opcSession);

                    Console.Clear();
                    Console.WriteLine(MachineStatusModel.Instance.ToString());
                    //Thread.Sleep(500);
                }
            }
        }

        public void WriteBatchToServer(Batch batch)
        {
            // Values are up casted to insure that the types algin with the expected data types

            OpcWriteNode[] commands = {
            new OpcWriteNode(NodeIds.CmdId, (float) batch.Id),
            new OpcWriteNode(NodeIds.CmdType, (float) batch.Type),
            new OpcWriteNode(NodeIds.CmdAmount, (float) batch.Amount),
            new OpcWriteNode(NodeIds.MachSpeed, (float) batch.Speed),
            new OpcWriteNode(NodeIds.CntrlCmd, 2),
            new OpcWriteNode(NodeIds.CmdChangeRequest, true )
        };
            if (_opcSession == null) throw new Exception();

            _opcSession.WriteNodes(commands);
        }

        public void ResetMachine()
        {
            OpcWriteNode[] commands = {
                new(NodeIds.CntrlCmd, 1),
                new(NodeIds.CmdChangeRequest, true)
            };
            _opcSession.WriteNodes(commands);
            Thread.Sleep(1000);
        }

        public void ConnectToServer()
        {
            _opcSession.Connect();
        }
        public void DisconnectFromServer()
        {
            _opcSession.Disconnect();
            _opcSession.Dispose(); //Clean up in case it wasn't automatically handled
        }
    }
}