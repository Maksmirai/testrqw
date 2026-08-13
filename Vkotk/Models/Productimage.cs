using System;
using System.Collections.Generic;

namespace Vkotk.Models;

public partial class Productimage
{
    public int Idproductimage { get; set; }

    public int? Idproduct { get; set; }

    public int? Idimage { get; set; }

    public virtual Imageproduct? IdimageNavigation { get; set; }

    public virtual Product? IdproductNavigation { get; set; }
}
