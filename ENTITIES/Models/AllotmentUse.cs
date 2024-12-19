using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class AllotmentUse
    {
        public int Id { get; set; }
        public long DataId { get; set; }
        public DateTime CreateDate { get; set; }
        public double AmountUse { get; set; }
        public int AllomentFundId { get; set; }
        public long AccountClientId { get; set; }
        public short ServiceType { get; set; }
        public long ClientId { get; set; }

        public virtual AllotmentFund AllomentFund { get; set; }
    }
}
