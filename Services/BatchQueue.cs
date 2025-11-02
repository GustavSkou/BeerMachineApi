namespace BeerMachineApi.Services;

public class BatchQueue : Queue<BatchDTO>
{
    public BatchQueue Batches { get { return this; }}
}