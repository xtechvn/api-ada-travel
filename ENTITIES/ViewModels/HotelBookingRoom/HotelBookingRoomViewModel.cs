using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.HotelBookingRoom
{
    public class HotelBookingRoomViewModel
    {
        public string Id { get; set; }
        public string HotelBookingRoomId { get; set; }
        public string HotelBookingId { get; set; }
        public string RoomTypeName { get; set; }
        public double TotalAmount { get; set; }
        public string PackageIncludes { get; set; }
        public string numberOfAdult { get; set; }
        public string numberOfChild { get; set; }
        public string numberOfInfant { get; set; }
        public string ExtraPackageAmount { get; set; }
        public double SumAmount { get; set; }
        public double SumUnitPrice { get; set; }
        public double Profit { get; set; }

        public double TotalUnitPrice { get; set; }

        public List<HotelBookingRoomRatesViewModel> HotelBookingRoomRates { get; set; }
    }
}
