using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class ContractHistory
    {
        public long Id { get; set; }
        public long ContractId { get; set; }
        public string Action { get; set; }
        public int? ActionBy { get; set; }
        public DateTime? ActionDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
