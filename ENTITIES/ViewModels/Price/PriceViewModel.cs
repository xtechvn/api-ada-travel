using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Price
{
   public class PriceViewModel
    {
        public string hotel_id { get; set; }
        public string room_id { get; set; }
        public int price_id { get; set; }
        public double price { get; set; }
        public int service_type { get; set; }
        public DateTime from_date { get; set; }
        public DateTime to_date { get; set; }
        public double profit { get; set; }
        public int client_type_id { get; set; }
        public double amount { get; set; } // Giá về tay
        public int unit_id { get; set; }
        public string pakage_id { get; set; }
    }
}
