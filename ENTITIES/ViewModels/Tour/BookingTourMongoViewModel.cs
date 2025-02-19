using ENTITIES.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Tour
{
    public class BookingTourMongoViewModel : TourBookingRequest
    {
        [BsonElement("_id")]
        public string _id { get; set; }
        public void GenID()
        {
            _id = ObjectId.GenerateNewId(DateTime.Now).ToString();
        }
        public long? order_id { get; set; } = -1;
        public TourProduct tour_product { get; set; }
        public TourProgramPackages packages { get; set; }
    }
}
