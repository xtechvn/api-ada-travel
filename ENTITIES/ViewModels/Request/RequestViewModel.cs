using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Request
{
    public class RequestViewModel
    {
        public string RequestId { get; set; }
        public string OrderNo { get; set; }
        public string RequestNo { get; set; }
        public string RequestStatus { get; set; }
        public string ClientName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string HotelName { get; set; }
        public double Amount { get; set; }
        public double Profit { get; set; }
        public long TotalRow { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public long ClientId { get; set; }
        public long BookingId { get; set; }
        public long OrderId { get; set; }
        public int Status { get; set; }
        public int numberOfRoom { get; set; }
    }
}
