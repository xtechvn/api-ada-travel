using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class InvoiceSign
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? FromNumber { get; set; }
        public int? ToNumber { get; set; }
        public int? CurrentNumber { get; set; }
        public long? InvoiceFromId { get; set; }
        public bool? IsUsed { get; set; }
        public bool? IsDelete { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
