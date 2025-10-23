using Opc.UaFx;
using Opc.UaFx.Client;

namespace BeerMachine
{
    public class BeerMachineHandler
    {
        OpcClient? _accessPoint;

        //string _serverURL = "opc.tcp://192.168.0.122:4840";  //Physical PLC
        string _serverURL = "opc.tcp://127.0.0.1:4840";      //Simulated PLC

        public static void Start()
        {
            BeerMachineHandler prg = new BeerMachineHandler();

            using (prg._accessPoint = new OpcClient(prg._serverURL))
            {
                prg.ConnectToServer();
                Thread.Sleep(1000);
                prg.ResetMachine();
                Thread.Sleep(1000);

                int batchId = 1;
                BeerType beerType = BeerType.AlcoholFree;
                int amount = 200;
                int machineSpeed = 150;
                prg.WriteBatchToServer(new Batch(batchId, beerType, amount, machineSpeed));

                while (true)
                {
                    prg.UpdateMachineStatusModel();
                    Console.Clear();
                    Console.WriteLine(MachineStatusModel.Instance.ToString());
                    Thread.Sleep(500);
                }
            }
        }

        private void UpdateMachineStatusModel()
        {
            // All values are casted to the corresponding datatype for the OpcValue.
            // These datatype can be found in using UaExpert and inspecting the datatype, or by debuging and inspecting what "ReadNode()" returns 

            MachineStatusModel.Instance.BatchId = (float)_accessPoint.ReadNode(NodeIds.CmdId).Value;
            MachineStatusModel.Instance.Ctrlcmd = (int)_accessPoint.ReadNode(NodeIds.CntrlCmd).Value;
            MachineStatusModel.Instance.Type = (float)_accessPoint.ReadNode(NodeIds.CmdType).Value;
            MachineStatusModel.Instance.Speed = (float)_accessPoint.ReadNode(NodeIds.MachSpeed).Value;
            MachineStatusModel.Instance.Temperature = (float)_accessPoint.ReadNode(NodeIds.StatusTemp).Value;
            MachineStatusModel.Instance.Vibration = (float)_accessPoint.ReadNode(NodeIds.StatusMovement).Value;
            MachineStatusModel.Instance.Humidity = (float)_accessPoint.ReadNode(NodeIds.StatusHumidity).Value;
            MachineStatusModel.Instance.ToProduceAmount = (float)_accessPoint.ReadNode(NodeIds.CmdAmount).Value;
            MachineStatusModel.Instance.ProducedAmount = (int)_accessPoint.ReadNode(NodeIds.AdminProcessedCount).Value;
            MachineStatusModel.Instance.DefectiveAmount = (int)_accessPoint.ReadNode(NodeIds.AdminDefectiveCount).Value;
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
            if (_accessPoint == null) throw new Exception();

            _accessPoint.WriteNodes(commands);
        }

        public void ResetMachine()
        {
            OpcWriteNode[] commands = {
            new OpcWriteNode(NodeIds.CntrlCmd, 1),
            new OpcWriteNode(NodeIds.CmdChangeRequest, true)
        };
            _accessPoint.WriteNodes(commands);
        }

        public void ConnectToServer()
        {
            _accessPoint.Connect();
        }
        public void DisconnectFromServer()
        {
            _accessPoint.Disconnect();
            _accessPoint.Dispose(); //Clean up in case it wasn't automatically handled
        }
    }
}