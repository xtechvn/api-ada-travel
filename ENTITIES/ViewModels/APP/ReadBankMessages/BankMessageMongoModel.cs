using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.APPModels.ReadBankMessages
{
    public class BankMessageMongoModel : BankMessageDetail
    {
        [BsonElement("_id")]
        public string _id { get; set; }
        public string token { get; set; }
        public void GenID()
        {
            _id = ObjectId.GenerateNewId().ToString();
        }
    }
}
