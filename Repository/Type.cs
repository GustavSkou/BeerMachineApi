using System;
using System.Collections.Generic;

namespace BeerMachineApi.Repository;

public partial class Type
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Batch> Batches { get; set; } = new List<Batch>();
}
