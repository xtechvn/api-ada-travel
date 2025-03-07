﻿using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class ProvinceHotel
    {
        public int Id { get; set; }
        public int ProvinceId { get; set; }
        public int? TotalOrder { get; set; }
        public int? Order { get; set; }
        public short? Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
