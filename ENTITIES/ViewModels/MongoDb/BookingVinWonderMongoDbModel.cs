using ENTITIES.ViewModels.Booking;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.MongoDb
{
    public class BookingVinWonderMongoDbModel: VinWonderBookingB2CModel
    {
        [BsonElement("_id")]
        public string _id { get; set; }
        public int account_client_id { get; set; }
        public int is_checkout { get; set; }
        public string voucher_name { get; set; }

        public DateTime create_date { get; set; }
        public void GenID()
        {
            _id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        }
    }
}
