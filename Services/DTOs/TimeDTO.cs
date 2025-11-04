using BeerMachineApi.Services.DTOs;
public class TimeDTO
{
    public long Id { get; set; }

    public long BatchId { get; set; }

    public double Temperature { get; set; }

    public double Humidity { get; set; }

    public double Vibration { get; set; }

    public DateTime TimeStamp { get; set; }

    public TimeDTO(int batchId, float temperature, float humidity, float vibration, DateTime timeStamp)
    {
        BatchId = batchId;
        Temperature = temperature;
        Humidity = humidity;
        Vibration = vibration;
        TimeStamp = timeStamp;
    }
}