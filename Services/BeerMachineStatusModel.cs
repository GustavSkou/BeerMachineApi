namespace BeerMachineApi.Services
{
    public class BeerMachineStatusModel
    {
        public BeerMachineStatusModel()
        {
            BatchId = 0;
            Type = 0;
            Speed = 0;
            Ctrlcmd = 0;
            Temperature = 0;
            Vibration = 0;
            Humidity = 0;
            ToProduceAmount = 0;
            ProducedAmount = 0;
            DefectiveAmount = 0;
            StopReason = 0;
        }

        public float BatchId { get; set; }
        public float Type { get; set; }
        public float Speed { get; set; }
        public int Ctrlcmd { get; set; }

        public float Temperature { get; set; }
        public float Vibration { get; set; }
        public float Humidity { get; set; }

        public float ToProduceAmount { get; set; }
        public int ProducedAmount { get; set; }
        public int DefectiveAmount { get; set; }

        public int StopReason { get; set; }

        public void UpdateModel(Opc.UaFx.Client.OpcClient session)
        {
            BatchId = (float)session.ReadNode(NodeIds.CmdId).Value;
            Ctrlcmd = (int)session.ReadNode(NodeIds.CntrlCmd).Value;
            Type = (float)session.ReadNode(NodeIds.CmdType).Value;
            Speed = (float)session.ReadNode(NodeIds.MachSpeed).Value;
            Temperature = (float)session.ReadNode(NodeIds.StatusTemp).Value;
            Vibration = (float)session.ReadNode(NodeIds.StatusMovement).Value;
            Humidity = (float)session.ReadNode(NodeIds.StatusHumidity).Value;
            ToProduceAmount = (float)session.ReadNode(NodeIds.CmdAmount).Value;
            ProducedAmount = (int)session.ReadNode(NodeIds.AdminProcessedCount).Value;
            DefectiveAmount = (int)session.ReadNode(NodeIds.AdminDefectiveCount).Value;
            StopReason = (int)session.ReadNode(NodeIds.AdminStopReason).Value;
        }

        public override string ToString()
        {
            var propertyInfos = GetType().GetProperties();

            List<string> names = new(), values = new();

            const int space = -16;
            foreach (var property in propertyInfos)
            {
                names.Add($"{property.Name,space}");
                values.Add($"{property.GetValue(this),space}");
            }
            return $"{string.Join("", names)}\n{string.Join("", values)}";
        }
    }
}