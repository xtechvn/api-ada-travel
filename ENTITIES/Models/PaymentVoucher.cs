using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class PaymentVoucher
    {
        public long Id { get; set; }
        public string PaymentCode { get; set; }
        public int Type { get; set; }
        public int PaymentType { get; set; }
        public string RequestId { get; set; }
        public long SupplierId { get; set; }
        public decimal Amount { get; set; }
        public long BankingAccountId { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public int? ClientId { get; set; }
        public string AttachFiles { get; set; }
        public string BankAccount { get; set; }
        public string BankName { get; set; }
        public int? SourceAccount { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
