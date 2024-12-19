using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class PropertiesProduct
    {
        public int Id { get; set; }
        public long ProductId { get; set; }
        public int PropertiesProductId { get; set; }
        public DateTime? CreateOn { get; set; }
        public string Value { get; set; }

        public virtual Product Product { get; set; }
    }
}
