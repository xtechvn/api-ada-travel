using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class Policy
    {
        public int PolicyId { get; set; }
        public string PolicyCode { get; set; }
        public string PolicyName { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public int? PermissionType { get; set; }
        public bool? IsPrivate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsDelete { get; set; }
    }
}
