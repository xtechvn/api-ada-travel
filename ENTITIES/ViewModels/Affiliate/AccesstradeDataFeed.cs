using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Affiliate
{
    [System.Serializable]
    public class AccesstradeDataFeed
    {
        public string sku { get; set; }
        public string name { get; set; }
        public string id { get; set; }
        public int price { get; set; }
        public int retail_Price { get; set; }
        public string url { get; set; }
        public string image_url { get; set; }
        public string category_id { get; set; }
        public string category_name { get; set; }

    }
    [System.Serializable]
    public class AdpiaDataFeed
    {
        public string product_name { get; set; }
        public string product_id { get; set; }
        public string category { get; set; }
        public double price { get; set; }
        public double discount { get; set; }
        public string link { get; set; }
        public string image { get; set; }
        public string url { get; set; }

        public class MyAffiliateLinkViewModel
        {
            [BsonElement("_id")]
            public string _id { get; set; }

            [BsonElement("client_id")]
            public long client_id { get; set; }

            [BsonElement("create_date")]
            public DateTime create_date { get; set; }

            [BsonElement("update_time")]
            public DateTime update_time { get; set; }

            [BsonElement("link_aff")]
            public string link_aff { get; set; }

            public void GenID()
            {
                _id = ObjectId.GenerateNewId().ToString();
            }
        }
    }
}
