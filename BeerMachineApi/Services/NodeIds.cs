namespace BeerMachineApi.Services
{
    public static class NodeIds
    {
        //Cube.Command
        public const string CmdId = "ns=6;s=::Program:Cube.Command.Parameter[0].Value"; //Float
        public const string CmdType = "ns=6;s=::Program:Cube.Command.Parameter[1].Value"; //Float
        public const string CmdAmount = "ns=6;s=::Program:Cube.Command.Parameter[2].Value"; //Float
        public const string CmdMachSpeed = "ns=6;s=::Program:Cube.Command.MachSpeed"; //Float
        public const string CntrlCmd = "ns=6;s=::Program:Cube.Command.CntrlCmd"; //Int32
        public const string CmdChangeRequest = "ns=6;s=::Program:Cube.Command.CmdChangeRequest"; //Boolean
        //Cube.Status
        public const string StatusBatchId = "ns=6;s=::Program:Cube.Status.Parameter[0].Value"; //Float      current batch id
        public const string StatusCurAmount = "ns=6;s=::Program:Cube.Status.Parameter[1].Value"; //Float    amount to produce in current batch
        public const string StatusHumidity = "ns=6;s=::Program:Cube.Status.Parameter[2].Value"; //Float
        public const string StatusTemp = "ns=6;s=::Program:Cube.Status.Parameter[3].Value"; //Float
        public const string StatusMovement = "ns=6;s=::Program:Cube.Status.Parameter[4].Value"; //Float     Vibration
        public const string StatusMachSpeed = "ns=6;s=::Program:Cube.Status.MachSpeed"; //Float
        public const string StatusCurMachSpeed = "ns=6;s=::Program:Cube.Status.CurMachSpeed";//Float
        public const string StatusStateCurrent = "ns=6;s=::Program:Cube.Status.StateCurrent"; //Int32
        //Cube.Admin
        public const string AdminDefectiveCount = "ns=6;s=::Program:Cube.Admin.ProdDefectiveCount"; //Int32
        public const string AdminProcessedCount = "ns=6;s=::Program:Cube.Admin.ProdProcessedCount"; //Int32
        public const string AdminStopReason = "ns=6;s=::Program:Cube.Admin.StopReason.Value"; //Int32
        public const string AdminCurType = "ns=6;s=::Program:Cube.Admin.Parameter[0].Value"; //Float
        //Data

        //Inventory
        public const string FillingInventory = "ns=6;s=::Program:FillingInventory"; //Boolean
        public const string Barley = "ns=6;s=::Program:Inventory.Barley"; //Float
        public const string Hops = "ns=6;s=::Program:Inventory.Hops"; //Float
        public const string Malt = "ns=6;s=::Program:Inventory.Malt"; //Float
        public const string Wheat = "ns=6;s=::Program:Inventory.Wheat"; //Float
        public const string Yeast = "ns=6;s=::Program:Inventory.Yeast"; //Float
        // Maintenance
        public const string MaintenanceCount = "ns=6;s=::Program:Maintenance.Counter"; //Uint16
        public const string MaintenanceState = "ns=6;s=::Program:Maintenance.State"; //Byte
        public const string MaintenanceTrigger = "ns=6;s=::Program:Maintenance.Trigger"; //Uint16
        // Product
        public const string ProductBad = "ns=6;s=::Program:product.bad"; //Uint16
        public const string ProductGood = "ns=6;s=::Program:product.good"; //Uint16
        public const string ProduceTargetAmount = "ns=6;s=::Program:product.produce_amount"; //Uint16
        public const string ProducedAmount = "ns=6;s=::Program:product.produced"; //Uint16

        public const string batch_id = "ns=6;s=::Program:batch_id"; //Uint16
    }
}