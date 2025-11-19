using System;
using System.Collections.Generic;

namespace BeerMachineApi.Repository;

public partial class Time
{
    public long Id { get; set; }

    public long BatchId { get; set; }

    public double Temperature { get; set; }

    public double Humidity { get; set; }

    public double Vibration { get; set; }

    public double Speed { get; set; }

    public DateTime TimeStamp { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Batch Batch { get; set; } = null!;
}
