using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class Province
    {
        public int Id { get; set; }
        public string ProvinceId { get; set; }
        public string Name { get; set; }
        public string NameNonUnicode { get; set; }
        public string Type { get; set; }
        public short? Status { get; set; }
    }
}
