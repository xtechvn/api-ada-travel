using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class Invoice
    {
        public long Id { get; set; }
        public int? ClientId { get; set; }
        public int? PayType { get; set; }
        public int? BankingAccountId { get; set; }
        public string InvoiceCode { get; set; }
        public string InvoiceFromId { get; set; }
        public string InvoiceSignId { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? ExportDate { get; set; }
        public int? UserVerify { get; set; }
        public DateTime? VerifyDate { get; set; }
        public int? Status { get; set; }
        public bool? IsDelete { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Note { get; set; }
        public string AttactFile { get; set; }
    }
}
