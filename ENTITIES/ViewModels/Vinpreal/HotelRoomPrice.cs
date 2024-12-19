using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Vinpreal
{
    public class HotelRoomPrice
    {
        public string hotel_id { get; set; }
        public string room_id { get; set; }
        public string rate_plan_id { get; set; }
        public string rate_plan_code { get; set; }
        public string allotment_id { get; set; }
        public string allotment_name { get; set; }
        public double amount { get; set; }
        public double profit { get; set; }
        public int profit_unit_id { get; set; }
        public long price_detail_id { get; set; }
    } 
}
