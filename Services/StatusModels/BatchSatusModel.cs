namespace BeerMachineApi.Services;

public class BatchStatusModel
{
    public float BatchId { get; set; }
    public float BeerType { get; set; }
    public float Speed { get; set; }
    public float ToProduceAmount { get; set; }
    public int ProducedAmount { get; set; }
    public int DefectiveAmount { get; set; }
    public float FailureRate
    {
        get
        {
            return (float)DefectiveAmount / (ProducedAmount == 0 ? 1 : ProducedAmount);
        }
    }

    public BatchStatusModel()
    {
        BatchId = 0;
        BeerType = 0;
        Speed = 0;
        ToProduceAmount = 0;
        ProducedAmount = 0;
        DefectiveAmount = 0;
    }

    public void UpdateModel(Opc.UaFx.Client.OpcClient session)
    {
        BatchId = Convert.ToSingle(session.ReadNode(NodeIds.StatusBatchId).Value);
        Speed = Convert.ToSingle(session.ReadNode(NodeIds.StatusMachSpeed).Value);

        ToProduceAmount = Convert.ToSingle(session.ReadNode(NodeIds.StatusCurAmount).Value);
        ProducedAmount = Convert.ToInt32(session.ReadNode(NodeIds.AdminProcessedCount).Value);
        DefectiveAmount = Convert.ToInt32(session.ReadNode(NodeIds.AdminDefectiveCount).Value);
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