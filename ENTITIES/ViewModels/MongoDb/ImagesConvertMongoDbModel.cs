using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.MongoDb
{
    public class ImagesConvertMongoDbModel
    {
        [BsonElement("_id")]
        public string _id { get; set; }
        public string orginal_url { get; set; }
        public string converted_url { get; set; }
        public int? size { get; set; }

        public void GenID()
        {
            _id = ObjectId.GenerateNewId(DateTime.Now).ToString();
        }
    }
    
}
