using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class Order
    {
        public Order()
        {
            Passenger = new HashSet<Passenger>();
        }

        public long OrderId { get; set; }
        public string OrderNo { get; set; }
        public byte? ServiceType { get; set; }
        public DateTime? CreateTime { get; set; }
        public double? Amount { get; set; }
        public int? PaymentStatus { get; set; }
        public long? ClientId { get; set; }
        public long? ContactClientId { get; set; }
        public byte? OrderStatus { get; set; }
        public long? ContractId { get; set; }
        public string SmsContent { get; set; }
        public int? PaymentType { get; set; }
        public string BankCode { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string PaymentNo { get; set; }
        public string ColorCode { get; set; }
        public double? Discount { get; set; }
        public double? Profit { get; set; }
        public DateTime? ExpriryDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ProductService { get; set; }
        public string Note { get; set; }
        public string UtmSource { get; set; }
        public DateTime? UpdateLast { get; set; }
        public long? SalerId { get; set; }
        public string SalerGroupId { get; set; }
        public long? UserUpdateId { get; set; }
        public short? SystemType { get; set; }
        public long? AccountClientId { get; set; }
        public long? CreatedBy { get; set; }
        public string Description { get; set; }
        public short? BranchCode { get; set; }
        public string BookingInfo { get; set; }
        public string Label { get; set; }
        public short? IsFinishPayment { get; set; }
        public int? PercentDecrease { get; set; }
        public int? VoucherId { get; set; }
        public double? Price { get; set; }
        public int? SupplierId { get; set; }
        public int? DepartmentId { get; set; }
        public string OperatorId { get; set; }
        public int? UserVerify { get; set; }
        public DateTime? VerifyDate { get; set; }
        public int? DebtStatus { get; set; }
        public string DebtNote { get; set; }
        public double? Commission { get; set; }
        public string UtmMedium { get; set; }
        public double? Refund { get; set; }
        public double? TotalFundCustomerCare { get; set; }
        public double? AmountFinalize { get; set; }
        public DateTime? FinalizeDate { get; set; }
        public bool? IsLock { get; set; }
        public DateTime? ClosingEndDate { get; set; }
        public bool? IsSalerDebtLimit { get; set; }

        public virtual ContactClient ContactClient { get; set; }
        public virtual Contract Contract { get; set; }
        public virtual ICollection<Passenger> Passenger { get; set; }
    }
}
