using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class Label
    {
        public int Id { get; set; }
        public string StoreName { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string Icon { get; set; }
        public string PrefixOrderCode { get; set; }
        public string Domain { get; set; }
        public string DescExpire { get; set; }
        public short? Status { get; set; }
    }
}
