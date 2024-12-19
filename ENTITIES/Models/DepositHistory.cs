using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class DepositHistory
    {
        public int Id { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateLast { get; set; }
        public long? UserId { get; set; }
        public string TransNo { get; set; }
        public string Title { get; set; }
        public double? Price { get; set; }
        public short? TransType { get; set; }
        public short? PaymentType { get; set; }
        public int? Status { get; set; }
        public string ImageScreen { get; set; }
        public short? ServiceType { get; set; }
        public string BankName { get; set; }
        public long? UserVerifyId { get; set; }
        public DateTime? VerifyDate { get; set; }
        public string NoteReject { get; set; }
        public long? ClientId { get; set; }
        public string BankAccount { get; set; }
        public bool? IsFinishPayment { get; set; }
    }
}
