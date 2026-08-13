using System;
using System.Collections.Generic;

namespace Vkotk.Models;

public partial class Product
{
    public int Idproduct { get; set; }

    public string? Nameproduct { get; set; }

    public string? Codeproduct { get; set; }

    public int? Countproduct { get; set; }

    public string? Commentproduct { get; set; }

    public DateOnly? Datecheck { get; set; }

    public virtual ICollection<Productimage> Productimages { get; set; } = new List<Productimage>();
}
