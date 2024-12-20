﻿using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class Programs
    {
        public int Id { get; set; }
        public string ProgramCode { get; set; }
        public string ProgramName { get; set; }
        public int SupplierId { get; set; }
        public int ServiceType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public int? Status { get; set; }
        public int? UserVerify { get; set; }
        public DateTime? VerifyDate { get; set; }
        public string ServiceName { get; set; }
        public int? HotelId { get; set; }
        public DateTime? StayStartDate { get; set; }
        public DateTime? StayEndDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
