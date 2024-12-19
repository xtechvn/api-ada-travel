using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class Department
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentCode { get; set; }
        public int? ParentId { get; set; }
        public string FullParent { get; set; }
        public bool? IsDelete { get; set; }
        public int? Status { get; set; }
        public int? Sort { get; set; }
        public string Description { get; set; }
        public int? Branch { get; set; }
        public bool? IsReport { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
