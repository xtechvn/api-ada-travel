using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class OrderProgress
    {
        public long Id { get; set; }
        public string OrderNo { get; set; }
        public short OrderStatus { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
