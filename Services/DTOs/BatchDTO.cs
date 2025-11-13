namespace BeerMachineApi.Services.DTOs;
// should be "stupid", no methods only simple getter properties

/// <summary>
/// data transfer object, 
/// the object should be seen as a temporary and be used for transporting data between layers
/// </summary>
public class BatchDTO
{
    public float? Id { get; set; }
    public float Amount { get; set; }
    public float Speed { get; set; }
    public float Type { get; set; }
    public int UserId { get; set; }
    public int DefectiveAmount { get; set; }
    public int ProducedAmount { get; set; }

    public BatchDTO()
    {
    }
}