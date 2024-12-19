using ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Hotel
{
    public class HotelRoomGridModel : HotelRoom
    {
        public string BedRoomTypeName { get; set; }
        public int TotalRow { get; set; }
    }

}
