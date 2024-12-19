﻿using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class PaymentRequestDetail
    {
        public long Id { get; set; }
        public long RequestId { get; set; }
        public long OrderId { get; set; }
        public long? ServiceId { get; set; }
        public int? Type { get; set; }
        public decimal Amount { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string ServiceCode { get; set; }
    }
}
