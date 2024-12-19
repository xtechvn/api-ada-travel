using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class ContractPay
    {
        public int PayId { get; set; }
        public string BillNo { get; set; }
        public string Note { get; set; }
        public double Amount { get; set; }
        public short PayStatus { get; set; }
        public DateTime? ExportDate { get; set; }
        public bool? IsDelete { get; set; }
        public short? Type { get; set; }
        public short? PayType { get; set; }
        public int? BankingAccountId { get; set; }
        public string Description { get; set; }
        public string AttatchmentFile { get; set; }
        public int? ClientId { get; set; }
        public int? SupplierId { get; set; }
        public int? DebtStatus { get; set; }
        public int? EmployeeId { get; set; }
        public int? ObjectType { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
