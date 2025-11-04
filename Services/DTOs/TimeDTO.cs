namespace BeerMachineApi.Services.DTOs;

public class TimeDTO
{
    public long Id { get; set; }

    public long BatchId { get; set; }

    public double Temperature { get; set; }

    public double Humidity { get; set; }

    public double Vibration { get; set; }

    public TimeDTO(int batchId, float temperature, float humidity, float vibration)
    {
        BatchId = batchId;
        Temperature = temperature;
        Humidity = humidity;
        Vibration = vibration;
    }
}