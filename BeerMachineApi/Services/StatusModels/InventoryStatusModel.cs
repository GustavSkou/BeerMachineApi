namespace BeerMachineApi.Services.StatusModels;

public class InventoryStatusModel
{
    public int Barley { get; set; }
    public int Hops { get; set; }
    public int Malt { get; set; }
    public int Wheat { get; set; }
    public int Yeast { get; set; }
    public bool FillingInventory { get; set; }

    public InventoryStatusModel()
    {
        Barley = 0;
        Hops = 0;
        Malt = 0;
        Wheat = 0;
        Yeast = 0;
        FillingInventory = false;
    }

    public void UpdateModel(Opc.UaFx.Client.OpcClient session)
    {
        Barley = Convert.ToInt32(session.ReadNode(NodeIds.Barley).Value);
        Hops = Convert.ToInt32(session.ReadNode(NodeIds.Hops).Value);
        Malt = Convert.ToInt32(session.ReadNode(NodeIds.Malt).Value);
        Wheat = Convert.ToInt32(session.ReadNode(NodeIds.Wheat).Value);
        Yeast = Convert.ToInt32(session.ReadNode(NodeIds.Yeast).Value);
        FillingInventory = Convert.ToBoolean(session.ReadNode(NodeIds.Yeast).Value);
    }

    public override string ToString()
    {
        return base.ToString();
    }
}