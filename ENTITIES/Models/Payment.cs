using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class Payment
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long ClientId { get; set; }
        public double Amount { get; set; }
        public int PaymentType { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string TransferContent { get; set; }
        public short? DepositPaymentType { get; set; }
        public string BotPaymentScreenShot { get; set; }
        public string ImageScreenShot { get; set; }
        public string BankName { get; set; }
    }
}
