using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Media.Imaging;

namespace Vkotk.Models;

public partial class Imageproduct
{
    public int Idimage { get; set; }

    public string? Pathimage { get; set; }

    public virtual ICollection<Productimage> Productimages { get; set; } = new List<Productimage>();
    
    
    public Bitmap? Imageprod
    {
        get
        {
          var bitmap = new Bitmap(Path.Combine(Directory.GetCurrentDirectory(), $"Image\\{Pathimage.ToString()}"));
          return bitmap;
        }
    }
}
