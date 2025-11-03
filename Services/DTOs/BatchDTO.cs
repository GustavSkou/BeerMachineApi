namespace BeerMachineApi.Services.DTOs;

public class BatchDTO
{
    public float Id { get; set; }
    public float Amount { get; set; }
    public float Speed { get; set; }
    public float Type { get; set; }

    public BatchDTO(float id, float amount, float speed, float type)
    {
        Id = id;
        Amount = amount;
        Speed = speed;
        Type = type;
    }
}