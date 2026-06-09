using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.B2B
{
    public class FlightWarehouseBookingViewModel
    {
        public string Id { get; set; }
        public string RouteName { get; set; }
        public string PriceDisplay { get; set; }
        public string AmountDL { get; set; }
        public string TotalTicket { get; set; }
        public string TotalDay { get; set; }

        public string FlightCodeGo { get; set; }
        public DateTime DepartureDate { get; set; }
        public string FlightCodeBack { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Airline { get; set; }
        public int TotalRow { get; set; }

        public string TripTypeName { get; set; }
        public string FlightPNRCodeGo { get; set; }
        public string FlightPNRCodeBack { get; set; }
        public int FundType { get; set; }

    }
}
