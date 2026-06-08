using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.B2B
{
    public class FlightWarehouseBookingModel
    {
        public long Id { get; set; }
        public string BookingCode { get; set; }
        public int? TripType { get; set; }
        public string DeparturePoint { get; set; }
        public string ArrivalPoint { get; set; }
        public int? TotalTicket { get; set; }
        public int? IsRefundable { get; set; }
        public string CarryOnBaggage { get; set; }
        public string CheckedBaggage { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int? AgencyTotalTicket { get; set; }
        public int? FundType { get; set; }
        public int? AdaTotalTicket { get; set; }
        public int? TotalClosedTicket { get; set; }
        public int? RemainTicket { get; set; }
    }
}