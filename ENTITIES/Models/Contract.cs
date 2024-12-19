using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class Contract
    {
        public Contract()
        {
            Order = new HashSet<Order>();
        }

        public long ContractId { get; set; }
        public string ContractNo { get; set; }
        public DateTime ContractDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public int ClientId { get; set; }
        public int SalerId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateLast { get; set; }
        public byte VerifyStatus { get; set; }
        public int? UserIdCreate { get; set; }
        public int? UserIdUpdate { get; set; }
        public int? UserIdVerify { get; set; }
        public DateTime? VerifyDate { get; set; }
        public int? TotalVerify { get; set; }
        public int? ContractStatus { get; set; }
        public int? DebtType { get; set; }
        public int? UpdatedBy { get; set; }
        public string ServiceType { get; set; }
        public int? PolicyId { get; set; }
        public string Note { get; set; }
        public int? ClientType { get; set; }
        public int? PermisionType { get; set; }
        public bool? IsDelete { get; set; }

        public virtual ICollection<Order> Order { get; set; }
    }
}
