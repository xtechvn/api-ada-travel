using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.B2B
{
    public class FlightWarehouseBookingDetail
    {
        public FlightWarehouseBookingModel Booking { get; set; }
        public List<FlightWarehouseSegmentModel> Segments { get; set; }
    }
}
