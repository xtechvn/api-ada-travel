using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class DebtStatistic
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int? ClientId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public double? Amount { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public int? Status { get; set; }
        public string Currency { get; set; }
        public string OrderIds { get; set; }
        public string DeclineReason { get; set; }
        public int? CreateBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? VerifyDate { get; set; }
        public int? UserVerify { get; set; }
        public bool? IsDelete { get; set; }
    }
}
