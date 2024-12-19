using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class Voucher
    {
        public Voucher()
        {
            VoucherLogActivity = new HashSet<VoucherLogActivity>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime? Cdate { get; set; }
        public DateTime? Udate { get; set; }
        public DateTime? EDate { get; set; }
        public int LimitUse { get; set; }
        public decimal? PriceSales { get; set; }
        public string Unit { get; set; }
        public int? RuleType { get; set; }
        public string GroupUserPriority { get; set; }
        public bool? IsPublic { get; set; }
        public string Description { get; set; }
        public bool? IsLimitVoucher { get; set; }
        public double? LimitTotalDiscount { get; set; }
        public string StoreApply { get; set; }
        public bool? IsMaxPriceProduct { get; set; }
        public double? MinTotalAmount { get; set; }
        public int? CampaignId { get; set; }
        public short? ProjectType { get; set; }

        public virtual ICollection<VoucherLogActivity> VoucherLogActivity { get; set; }
    }
}
