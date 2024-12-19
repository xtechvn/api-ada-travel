using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class PaymentAccount
    {
        public int Id { get; set; }
        public string AccountNumb { get; set; }
        public string AccountName { get; set; }
        public string BankName { get; set; }
        public string Branch { get; set; }
        public long ClientId { get; set; }
    }
}
