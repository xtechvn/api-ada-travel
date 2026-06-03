using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels
{
    public class TourItinerary
    {
        public long Id { get; set; }
        public long TourDepartureId { get; set; }
        public int RouteType { get; set; }//1 chieu di, 2 chieu ve
        public int TransportType { get; set; }//1 may bay, 2 tau hoa, 3 xe khach
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public string TransportProvider { get; set; }
        public DateTime? DepartureDate { get; set; }
        public string TransportCode { get; set; }
        public string Note { get; set; }
        public DateTime? BookingDeadline { get; set; }
        public string TransportTypeName { get; set; }
    

    }
}
