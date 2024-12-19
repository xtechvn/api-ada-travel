using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class GroupProduct
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int? PositionId { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public int? OrderNo { get; set; }
        public string Path { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? Priority { get; set; }
        public string Description { get; set; }
        public bool IsShowHeader { get; set; }
        public bool IsShowFooter { get; set; }
        public string Code { get; set; }
    }
}
