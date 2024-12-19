using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class Permission
    {
        public Permission()
        {
            Action = new HashSet<Action>();
            RolePermission = new HashSet<RolePermission>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? SortOrder { get; set; }
        public int Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public virtual ICollection<Action> Action { get; set; }
        public virtual ICollection<RolePermission> RolePermission { get; set; }
    }
}
