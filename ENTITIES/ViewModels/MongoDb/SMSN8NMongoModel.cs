using ENTITIES.ViewModels.APP.ContractPay;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.MongoDb
{
    public class SMSN8NMongoModel : APIRequestGenericModel
    {
        [BsonElement("_id")]
        public string _id { get; set; }
        public string n8n_status { get; set; }
        public string n8n_response { get; set; }

        public void GenID()
        {
            _id = ObjectId.GenerateNewId(DateTime.Now).ToString();
        }
    }

}
