using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class AutomaticPurchaseHistory
    {
        public long Id { get; set; }
        public long AutomaticPurchaseId { get; set; }
        public long UserExcution { get; set; }
        public int PurchaseStatus { get; set; }
        public int? DeliveryStatus { get; set; }
        public DateTime CreateDate { get; set; }
        public string PurchaseLog { get; set; }
    }
}
