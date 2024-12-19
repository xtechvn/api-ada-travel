using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.APIModels
{
    public class FlightServicePriceModel
    {
        public int price_id { get; set; }
        public double price { get; set; } // giá gốc
        public double amount { get; set; } // giá đã cộng lợi nhuận

        public double profit { get; set; } // tổng lợi nhuận

    }
    public class GroupClassAirlinesModel
    {
        public string air_line { get; set; }
        public string class_code { get; set; }
        public string fare_type { get; set; }

       

    }
}
