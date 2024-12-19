using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class IndustrySpecialLuxury
    {
        public int Id { get; set; }
        public int SpecialType { get; set; }
        public double Price { get; set; }
        public int Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public DateTime? UpdateLast { get; set; }
        public string GroupLabelId { get; set; }
        public bool? IsAllowDiscountCompare { get; set; }
    }
}
