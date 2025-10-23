namespace BeerMachine
{
    public class MachineStatusModel
    {
        private MachineStatusModel()
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
        }

        private static MachineStatusModel _instance = new MachineStatusModel();
        public static MachineStatusModel Instance { get { return _instance; } }

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

        public override string ToString()
        {
            return $"{BatchId}, {Type}, {Speed}, {Ctrlcmd}, {Temperature}, {Vibration}, {Humidity}, {ToProduceAmount}, {ProducedAmount}, {DefectiveAmount}";
        }
    }


}