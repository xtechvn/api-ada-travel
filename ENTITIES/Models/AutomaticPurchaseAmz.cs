using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class AutomaticPurchaseAmz
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public string OrderCode { get; set; }
        public string PurchaseUrl { get; set; }
        public string ProductCode { get; set; }
        public double Amount { get; set; }
        public int Quanity { get; set; }
        public DateTime CreateDate { get; set; }
        public int PurchaseStatus { get; set; }
        public string PurchaseMessage { get; set; }
        public string PurchasedOrderId { get; set; }
        public string PurchasedSellerName { get; set; }
        public string PurchasedSellerStoreUrl { get; set; }
        public DateTime UpdateLast { get; set; }
        public string ManualNote { get; set; }
        public string OrderedSuccessUrl { get; set; }
        public string OrderDetailUrl { get; set; }
        public string Screenshot { get; set; }
        public int? DeliveryStatus { get; set; }
        public DateTime? OrderEstimatedDeliveryDate { get; set; }
        public string DeliveryMessage { get; set; }
        public string OrderMappingId { get; set; }
        public long? AutoBuyMappingId { get; set; }
    }
}
