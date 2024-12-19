using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class Product
    {
        public Product()
        {
            ImageProduct = new HashSet<ImageProduct>();
            OrderItem = new HashSet<OrderItem>();
            ProductHistory = new HashSet<ProductHistory>();
            PropertiesProduct = new HashSet<PropertiesProduct>();
        }

        public long Id { get; set; }
        public string ProductCode { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public int? Discount { get; set; }
        public double? Amount { get; set; }
        public string Rating { get; set; }
        public string Manufacturer { get; set; }
        public int LabelId { get; set; }
        public int? ReviewsCount { get; set; }
        public bool? IsPrimeEligible { get; set; }
        public int RateCurrent { get; set; }
        public string SellerId { get; set; }
        public string SellerName { get; set; }
        public DateTime? CreateOn { get; set; }
        public DateTime? UpdateLast { get; set; }
        public int? GroupProductId { get; set; }
        public string Description { get; set; }
        public string Information { get; set; }
        public string Variations { get; set; }
        public int? ProductMapId { get; set; }
        public string Path { get; set; }
        public string LinkSource { get; set; }
        public long? ParentId { get; set; }
        public double? ItemWeight { get; set; }
        public string UnitWeight { get; set; }

        public virtual ICollection<ImageProduct> ImageProduct { get; set; }
        public virtual ICollection<OrderItem> OrderItem { get; set; }
        public virtual ICollection<ProductHistory> ProductHistory { get; set; }
        public virtual ICollection<PropertiesProduct> PropertiesProduct { get; set; }
    }
}
