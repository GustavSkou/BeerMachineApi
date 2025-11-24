namespace BeerMachineApi.Services;

public interface IMachineService
{
    /// <summary>
    /// initializes the OpcClient and keeps the connection open
    /// </summary>
    public void Start();

    /// <summary>
    /// Returns an object which properties represent the status of a model
    /// </summary>
    public object GetStatus(string type);

    /// <summary>
    /// Addes a Command to the CommandQueue, which will be processed in a given interval
    /// </summary>
    public void QueueCommand(Command command);

    /// <summary>
    /// Keeps trying to connect to the opc server until to gets connected
    /// </summary>
    public void TryToConnectToServer ();
}