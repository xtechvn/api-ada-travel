using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class LocationProduct
    {
        public long LocationProductId { get; set; }
        public string ProductCode { get; set; }
        public int GroupProductId { get; set; }
        public int OrderNo { get; set; }
        public DateTime CreateOn { get; set; }
        public DateTime UpdateLast { get; set; }
        public int UserId { get; set; }
    }
}
