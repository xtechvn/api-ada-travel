using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class AttachFile
    {
        public long Id { get; set; }
        public long DataId { get; set; }
        public int UserId { get; set; }
        public int? Type { get; set; }
        public string Path { get; set; }
        public string Ext { get; set; }
        public double? Capacity { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
