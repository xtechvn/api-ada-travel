using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class Comments
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public string Content { get; set; }
        public string AttachFile { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string AttachFileName { get; set; }
        public int? UserType { get; set; }
    }
}
