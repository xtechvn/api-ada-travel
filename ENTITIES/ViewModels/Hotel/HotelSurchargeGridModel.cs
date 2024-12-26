using ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Hotel
{
    public class HotelSurchargeGridModel : HotelSurcharge
    {
        public string UserName { get; set; }
        public int TotalRow { get; set; }
    }
}
