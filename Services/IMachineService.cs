namespace BeerMachineApi.Services;

public interface IMachineService
{
    public void Start();

    // return an object which properties represent the status of the machine
    public object GetStatus();
    public void ExecuteCommand(Command command);
}