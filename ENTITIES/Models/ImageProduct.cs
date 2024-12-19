using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class ImageProduct
    {
        public long Id { get; set; }
        public string Image { get; set; }
        public long? ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}
