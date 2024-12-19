using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class Price
    {
        public int PriceId { get; set; }
        public double Price1 { get; set; }
        public int UnitId { get; set; }
        public byte ServiceType { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Description { get; set; }
        public byte Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateLast { get; set; }
        public long UserUpdateId { get; set; }
        public int UserCreateId { get; set; }
    }
}
