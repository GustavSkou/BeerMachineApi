using System;
using System.Collections.Generic;

namespace BeerMachineApi.Repository;

public partial class Batch
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long TypeId { get; set; }

    public int Amount { get; set; }

    public int Failed { get; set; }

    public int AmountCompleted { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Time> Times { get; set; } = new List<Time>();

    public virtual Type Type { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
