using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ENTITIES.ViewModels.MongoDb
{
    public class HotelPriceMongoDbModel
    {
        [BsonElement("_id")]
        public string _id { get; set; }
        public void GenID()
        {
            _id = ObjectId.GenerateNewId(DateTime.Now).ToString();
        }
        public int client_type { get; set; }
        public string hotel_id { get; set; }
        public DateTime arrival_date { get; set; }
        public DateTime departure_date { get; set; }
        public double min_price { get; set; }

    }
}
