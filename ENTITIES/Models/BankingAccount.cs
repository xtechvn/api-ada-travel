using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class BankingAccount
    {
        public int Id { get; set; }
        public string BankId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string Branch { get; set; }
        public int? SupplierId { get; set; }
        public int? ClientId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsDisplayWebsite { get; set; }
    }
}
