using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class ProductOtherLink
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string LinkOrder { get; set; }
        public string Note { get; set; }
        public double? PriceCheckout { get; set; }
        public double? PriceDyamic { get; set; }
        public int? LabelId { get; set; }
        public string ProductOrderId { get; set; }
    }
}
