using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.MongoDb
{
    public class SystemLogMongDBRecruitmentModel
    {
        [BsonElement("_id")]
        public string _id { get; set; }
        public void GenID()
        {
            _id = ObjectId.GenerateNewId().ToString();
        }
        public DateTime CreatedTime { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string location { get; set; }
        public string area { get; set; }
        public string email { get; set; }
        public string note { get; set; }
        public string Path { get; set; }
    }
}
