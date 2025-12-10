namespace BeerMachineApi.Services.StatusModels;

public class BeerMachineStatusModel : IStatusModel
{
    public BeerMachineStatusModel()
    {
        Speed = 0;
        Ctrlcmd = 0;
        Temperature = 0;
        Vibration = 0;
        Humidity = 0;
        StopReason = 0;
        StateCurrent = 2;
    }

    public float Speed { get; set; }
    public int Ctrlcmd { get; set; }

    public float Temperature { get; set; }
    public float Vibration { get; set; }
    public float Humidity { get; set; }

    public int StopReason { get; set; }
    public int StateCurrent { get; set; }

    public void UpdateModel(Opc.UaFx.Client.OpcClient session)
    {
        Ctrlcmd = (int)session.ReadNode(NodeIds.CntrlCmd).Value;
        Speed = (float)session.ReadNode(NodeIds.StatusCurMachSpeed).Value;
        Temperature = (float)session.ReadNode(NodeIds.StatusTemp).Value;
        Vibration = (float)session.ReadNode(NodeIds.StatusMovement).Value;
        Humidity = (float)session.ReadNode(NodeIds.StatusHumidity).Value;
        StopReason = (int)session.ReadNode(NodeIds.AdminStopReason).Value;
        StateCurrent = (int)session.ReadNode(NodeIds.StatusStateCurrent).Value;
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
