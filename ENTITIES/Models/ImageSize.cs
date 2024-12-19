using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class ImageSize
    {
        public int Id { get; set; }
        public string PositionName { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
