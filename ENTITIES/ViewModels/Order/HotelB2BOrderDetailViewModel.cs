using ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Order
{
    public class HotelB2BOrderDetailViewModel
    { 
        public Models.Order order { get; set; }
        public ENTITIES.Models.HotelBooking booking { get; set; }
        public ContactClient contact { get; set; }
        public List<HotelB2BOrderDetailRooms> rooms { get; set; }
        public List<HotelBookingRoomExtraPackages> extras { get; set; }
    }
    public class HotelB2BOrderDetailRooms
    {
        public HotelBookingRooms detail { get; set; }
        public List<HotelBookingRoomRates> rates { get; set; }
        public List<HotelGuest> guest { get; set; }
        public List<HotelBookingRoomExtraPackages> packages { get; set; }
    }
}
