namespace BeerMachineApi.Services.StatusModels;

public class MaintenanceStatusModel
{
    public MaintenanceStatusModel()
    {
        MaintenanceCount = 0;
        MaintenanceTrigger = 30000;
        MaintenanceState = false;
    }

    public System.UInt16 MaintenanceCount { get; set; }
    public System.UInt16 MaintenanceTrigger { get; set; }
    public bool MaintenanceState { get; set; }

    public void UpdateModel(Opc.UaFx.Client.OpcClient session)
    {
        MaintenanceCount = (System.UInt16)session.ReadNode(NodeIds.MaintenanceCount).Value;       // current amount 
        MaintenanceTrigger = (System.UInt16)session.ReadNode(NodeIds.MaintenanceTrigger).Value;   // max maintenance amount 

        if ((byte)session.ReadNode(NodeIds.MaintenanceState).Value == 20 || MaintenanceCount >= MaintenanceTrigger)
            MaintenanceState = true;   // during maintenance
        else
            MaintenanceState = false;
    }

    public override string ToString()
    {
        return $"{MaintenanceCount}, {MaintenanceState}";
    }
}
