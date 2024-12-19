using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class PriceProductLevel
    {
        public int PriceId { get; set; }
        public double Offset { get; set; }
        public double Limit { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string LabelId { get; set; }
        public double Price { get; set; }
        public int? FeeType { get; set; }
        public double? Discount { get; set; }
        public string Note { get; set; }
    }
}
