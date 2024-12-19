using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class ProductHistory
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public int UserId { get; set; }
        public short? UserType { get; set; }
        public int? Price { get; set; }
        public int? Rate { get; set; }
        public DateTime? CreateOn { get; set; }

        public virtual Product Product { get; set; }
    }
}
