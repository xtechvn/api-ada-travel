using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class Request
    {
        public int RequestId { get; set; }
        public string RequestNo { get; set; }
        public int? RoomTypeId { get; set; }
        public int? PackageId { get; set; }
        public int? BookingId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public double? Price { get; set; }
        public string Note { get; set; }
        public int? Status { get; set; }
        public int? SalerId { get; set; }
        public long? OrderId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? ClientId { get; set; }
        public int? HotelId { get; set; }
        public int? VoucherId { get; set; }
        public string VoucherName { get; set; }
        public double? Amount { get; set; }
        public double? Discount { get; set; }
    }
}
