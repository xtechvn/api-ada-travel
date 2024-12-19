using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class AllotmentHistory
    {
        public int Id { get; set; }
        public double AmountMove { get; set; }
        public long AccountClientId { get; set; }
        public DateTime CreateDate { get; set; }
        public double AccountBalance { get; set; }
        public int FundTypeFrom { get; set; }
        public int AllotmentFundId { get; set; }
        public string Description { get; set; }
        public int? FundTypeTo { get; set; }
        public int? PaymentType { get; set; }

        public virtual AllotmentFund AllotmentFund { get; set; }
    }
}
