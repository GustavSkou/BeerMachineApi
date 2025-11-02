namespace BeerMachineApi.Services;

public interface IMachineService
{
    public void Start();
    public object GetStatus(string type); // return an object which properties represent the status of a model
    public void ExecuteCommand(Command command);
}