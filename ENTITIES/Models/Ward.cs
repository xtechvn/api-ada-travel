using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class Ward
    {
        public int Id { get; set; }
        public string WardId { get; set; }
        public string Name { get; set; }
        public string NameNonUnicode { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public string DistrictId { get; set; }
        public short? Status { get; set; }
    }
}
