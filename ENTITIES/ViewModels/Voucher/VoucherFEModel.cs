using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Voucher
{
    public class VoucherFEModel
    {
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
        public int TotalRow { get; set; }
    }
}
