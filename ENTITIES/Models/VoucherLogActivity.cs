using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class VoucherLogActivity
    {
        public int Id { get; set; }
        public int? VoucherId { get; set; }
        public int? OrderId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? Status { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int? StoreId { get; set; }
        public double? PriceSaleVnd { get; set; }
        public int? CartId { get; set; }

        public virtual Voucher Voucher { get; set; }
    }
}
