using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Hotel
{
    public class APIHotelPricePolicyRequest
    {
        public string hotel_id { get; set; }
        public DateTime from_date { get; set; }
        public DateTime to_date { get; set; }
        public int nights { get; set; }
        public long account_client_id { get; set; }
        public List<APIHotelPricePolicyRequestPriceDetail> rooms { get; set; }
    }
    public class APIHotelPricePolicyRequestPriceDetail
    {
        public string room_id { get; set; }
        public double price { get; set; }
       
    }
    public class APIHotelPricePolicyResponse
    {
        public string hotel_id { get; set; }
        public string room_id { get; set; }
        public string campaign_code { get; set; }
        public string contract_no { get; set; }
        public string package_id { get; set; }
        public string allotment_id { get; set; }
        public double price { get; set; }
        public double profit { get; set; }
        public double amount { get; set; }

    }
}
