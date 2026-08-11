using System;
using System.Collections.Generic;

namespace Vkotk.Models;

public partial class Imageproduct
{
    public int Idimage { get; set; }

    public string? Pathimage { get; set; }

    public virtual ICollection<Productimage> Productimages { get; set; } = new List<Productimage>();
}
