namespace BeerMachineApi.Services;

public class Command
{
    public string Type { get; set; }
    public Dictionary<string, int>? Parameters { get; set; }
}