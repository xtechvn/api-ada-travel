using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class AllCode
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public short CodeValue { get; set; }
        public string Description { get; set; }
        public short? OrderNo { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
