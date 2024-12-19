using System;
using System.Collections.Generic;
using System.Text;
using ENTITIES.Models;

namespace ENTITIES.ViewModels.HotelBooking
{
   public class HotelBookingViewModel : ENTITIES.Models.HotelBooking
    {
        public string SalerId_name { get; set; }
        public string RoomTypeName { get; set; }
        public double TotalRooms { get; set; }
        public double TotalDays { get; set; }
    }
}
