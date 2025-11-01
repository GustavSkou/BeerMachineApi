namespace BeerMachineApi.Services
{
    public static class NodeIds
    {
        // Command nodes
        public const string CmdId = "ns=6;s=::Program:Cube.Command.Parameter[0].Value";
        public const string CmdType = "ns=6;s=::Program:Cube.Command.Parameter[1].Value";
        public const string CmdAmount = "ns=6;s=::Program:Cube.Command.Parameter[2].Value";
        public const string MachSpeed = "ns=6;s=::Program:Cube.Command.MachSpeed";
        public const string CntrlCmd = "ns=6;s=::Program:Cube.Command.CntrlCmd";
        public const string CmdChangeRequest = "ns=6;s=::Program:Cube.Command.CmdChangeRequest";


        public const string StatusBatchId = "ns=6;s=::Program:Cube.Status.Parameter[0].Value";
        public const string StatusCurAmount = "ns=6;s=::Program:Cube.Status.Parameter[1].Value"; //amount to produce
        public const string StatusHumidity = "ns=6;s=::Program:Cube.Status.Parameter[2].Value";
        public const string StatusTemp = "ns=6;s=::Program:Cube.Status.Parameter[3].Value";
        public const string StatusMovement = "ns=6;s=::Program:Cube.Status.Parameter[4].Value"; // Vibration
        public const string StatusMachSpeed = "ns=6;s=::Program:Cube.Status.MachSpeed";


        public const string AdminDefectiveCount = "ns=6;s=::Program:Cube.Admin.ProdDefectiveCount";
        public const string AdminProcessedCount = "ns=6;s=::Program:Cube.Admin.ProdProcessedCount";
        public const string AdminStopReason = "ns=6;s=::Program:Cube.Admin.StopReason.Value";
        public const string AdminCurType = "ns=6;s=::Program:Cube.Admin.Parameter[0].Value";

    }
}