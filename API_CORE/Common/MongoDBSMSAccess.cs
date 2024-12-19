using ENTITIES.APPModels;
using ENTITIES.APPModels.ReadBankMessages;
using ENTITIES.APPModels.SystemLogs;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace APP.PUSH_LOG.Functions
{
    public static class MongoDBSMSAccess
    {

        public static async Task<BankMessageMongoModel> InsertSMS(BankMessageDetail item, IConfiguration _configuration)
        {
            try
            {
                MongoDbConfig config = new MongoDbConfig()
                {
                    host = _configuration["DataBaseConfig:MongoServer:Host"],
                    port = Convert.ToInt32(_configuration["DataBaseConfig:MongoServer:Port"]),
                    user_name = _configuration["DataBaseConfig:MongoServer:user"],
                    password = _configuration["DataBaseConfig:MongoServer:pwd"],
                    database_name = _configuration["DataBaseConfig:MongoServer:catalog_log"]
                };
                //-- "mongodb://user1:password1@localhost/test"
                string url = "mongodb://" + config.user_name + ":" + config.password + "@" + config.host + ":" + config.port+ "/?authSource=" + config.database_name;
                var client = new MongoClient(url);
                IMongoDatabase db = client.GetDatabase(config.database_name);
                var collection = _configuration["DataBaseConfig:MongoServer:sms_collection"];
                IMongoCollection<BankMessageMongoModel> log_collection = db.GetCollection<BankMessageMongoModel>(collection);
                var detail = new BankMessageMongoModel()
                {
                    Amount = item.Amount,
                    BankName = item.BankName,
                    OrderNo = item.OrderNo,
                    CreatedTime = item.CreatedTime,
                    MessageContent = item.MessageContent,
                    ReceiveTime = item.ReceiveTime,
                    StatusPush = item.StatusPush
                };
                detail.GenID();
                await log_collection.InsertOneAsync(detail);
                return detail;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("API_CORE - MongoDBSMSAccess - MongoDBLoggingAccess - Cannot Excute: " + ex.ToString());
                return null;
            }
        }
        public static string CheckIfSMSExists(BankMessageDetail item, IConfiguration _configuration)
        {
            try
            {
                MongoDbConfig config = new MongoDbConfig()
                {
                    host = _configuration["DataBaseConfig:MongoServer:Host"],
                    port = Convert.ToInt32(_configuration["DataBaseConfig:MongoServer:Port"]),
                    user_name = _configuration["DataBaseConfig:MongoServer:user"],
                    password = _configuration["DataBaseConfig:MongoServer:pwd"],
                    database_name = _configuration["DataBaseConfig:MongoServer:catalog_log"]
                };
                //-- "mongodb://user1:password1@localhost/test"
                string url = "mongodb://" + config.user_name + ":" + config.password + "@" + config.host + ":" + config.port + "/?authSource=" + config.database_name;
                var client = new MongoClient(url);
                IMongoDatabase db = client.GetDatabase(config.database_name);
                var collection = _configuration["DataBaseConfig:MongoServer:sms_collection"];
                IMongoCollection<BankMessageMongoModel> log_collection = db.GetCollection<BankMessageMongoModel>(collection);
                var filter = Builders<BankMessageMongoModel>.Filter;
                var filterDefinition = filter.Empty;
                filterDefinition &= Builders<BankMessageMongoModel>.Filter.Eq(x => x.OrderNo, item.OrderNo);
                filterDefinition &= Builders<BankMessageMongoModel>.Filter.Eq(x => x.BankName, item.BankName);
                filterDefinition &= Builders<BankMessageMongoModel>.Filter.Eq(x => x.Amount, item.Amount);
                filterDefinition &= Builders<BankMessageMongoModel>.Filter.Eq(x => x.ReceiveTime, item.ReceiveTime);
                filterDefinition &= Builders<BankMessageMongoModel>.Filter.Eq(x => x.MessageContent, item.MessageContent);
                var model = log_collection.Find(filterDefinition).FirstOrDefault();
                if (model != null && model._id != null && model._id.Trim() != "")
                    return model._id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("API_CORE - MongoDBSMSAccess - CheckIfSMSExists - Cannot Excute: " + ex.ToString());
                return ex.ToString();
            }
            return null;
        }
        public static async Task<List<BankMessageMongoModel>> GetAllItem(int page,int page_size, IConfiguration _configuration)
        {
            try
            {
                MongoDbConfig config = new MongoDbConfig()
                {
                    host = _configuration["DataBaseConfig:MongoServer:Host"],
                    port = Convert.ToInt32(_configuration["DataBaseConfig:MongoServer:Port"]),
                    user_name = _configuration["DataBaseConfig:MongoServer:user"],
                    password = _configuration["DataBaseConfig:MongoServer:pwd"],
                    database_name = _configuration["DataBaseConfig:MongoServer:catalog_log"]
                };
                //-- "mongodb://user1:password1@localhost/test"
                string url = "mongodb://" + config.user_name + ":" + config.password + "@" + config.host + ":" + config.port + "/?authSource=" + config.database_name;
                var client = new MongoClient(url);
                IMongoDatabase db = client.GetDatabase(config.database_name);
                var collection = _configuration["DataBaseConfig:MongoServer:sms_collection"];
                IMongoCollection<BankMessageMongoModel> log_collection = db.GetCollection<BankMessageMongoModel>(collection);
                var filter = Builders<BankMessageMongoModel>.Filter;
                var filterDefinition = filter.Empty;
              
                var model = await log_collection.Find(filterDefinition).Skip((page-1)*page_size).Limit(page_size).ToListAsync();
                return model;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("API_CORE - MongoDBSMSAccess - CheckIfSMSExists - Cannot Excute: " + ex.ToString());
            }
            return null;
        }
        public static async Task<bool> InsertLogMongoDb(IConfiguration _configuration,string log,string key_id)
        {
            try
            {
                MongoDbConfig config = new MongoDbConfig()
                {
                    host = _configuration["DataBaseConfig:MongoServer:Host"],
                    port = Convert.ToInt32(_configuration["DataBaseConfig:MongoServer:Port"]),
                    user_name = _configuration["DataBaseConfig:MongoServer:user"],
                    password = _configuration["DataBaseConfig:MongoServer:pwd"],
                    database_name = _configuration["DataBaseConfig:MongoServer:catalog_log"]
                };
                //-- "mongodb://user1:password1@localhost/test"
                string url = "mongodb://" + config.user_name + ":" + config.password + "@" + config.host + ":" + config.port + "/?authSource=" + config.database_name;
                var client = new MongoClient(url);
                IMongoDatabase db = client.GetDatabase(config.database_name);
                IMongoCollection<SystemLogMongDBModel> log_collection = db.GetCollection<SystemLogMongDBModel>("API_CORE");
                SystemLogMongDBModel model = new SystemLogMongDBModel()
                {
                    CreatedTime = DateTime.Now,
                    KeyID = key_id,
                    Log = log,
                    SourceID = (int)SystemLogSourceID.API_CORE,
                    Type = SystemLogTypeID.ACTIVITY
                };
                model.GenID();
                await log_collection.InsertOneAsync(model);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertLogMongoDb - MongoDBSMSAccess - Cannot Excute: " + ex.ToString());
            }
            return false;
        }
        public static async Task<bool> InsertLogMongoB2C(IConfiguration _configuration, string log, string key_id)
        {
            try
            {
                MongoDbConfig config = new MongoDbConfig()
                {
                    host = _configuration["DataBaseConfig:MongoServer:Host"],
                    port = Convert.ToInt32(_configuration["DataBaseConfig:MongoServer:Port"]),
                    user_name = _configuration["DataBaseConfig:MongoServer:user"],
                    password = _configuration["DataBaseConfig:MongoServer:pwd"],
                    database_name = _configuration["DataBaseConfig:MongoServer:catalog_log"]
                };
                //-- "mongodb://user1:password1@localhost/test"
                string url = "mongodb://" + config.user_name + ":" + config.password + "@" + config.host + ":" + config.port + "/?authSource=" + config.database_name;
                var client = new MongoClient(url);
                IMongoDatabase db = client.GetDatabase(config.database_name);
                IMongoCollection<SystemLogMongDBModel> log_collection = db.GetCollection<SystemLogMongDBModel>("FRONTEND_B2C");
                SystemLogMongDBModel model = new SystemLogMongDBModel()
                {
                    CreatedTime = DateTime.Now,
                    KeyID = key_id,
                    Log = log,
                    SourceID = (int)SystemLogSourceID.FRONTEND_B2C,
                    Type = SystemLogTypeID.WARNING
                };
                model.GenID();
                await log_collection.InsertOneAsync(model);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertLogMongoB2C - MongoDBSMSAccess - Cannot Excute: " + ex.ToString());
            }
            return false;
        }
        public static async Task<List<SystemLogMongDBModel>> GetLogByFilter(SystemLog filter_object, IConfiguration _configuration,int page=1, int size=10)
        {
            try
            {
                MongoDbConfig config = new MongoDbConfig()
                {
                    host = _configuration["DataBaseConfig:MongoServer:Host"],
                    port = Convert.ToInt32(_configuration["DataBaseConfig:MongoServer:Port"]),
                    user_name = _configuration["DataBaseConfig:MongoServer:user"],
                    password = _configuration["DataBaseConfig:MongoServer:pwd"],
                    database_name = _configuration["DataBaseConfig:MongoServer:catalog_log"]
                };
                //-- "mongodb://user1:password1@localhost/test"
                string url = "mongodb://" + config.user_name + ":" + config.password + "@" + config.host + ":" + config.port + "/?authSource=" + config.database_name;
                var client = new MongoClient(url);
                IMongoDatabase db = client.GetDatabase(config.database_name);
                string collection = Enum.GetName(typeof(SystemLogSourceID), filter_object.SourceID);
                IMongoCollection<SystemLogMongDBModel> log_collection = db.GetCollection<SystemLogMongDBModel>(collection);
                var filter = Builders<SystemLogMongDBModel>.Filter;
                var filterDefinition = filter.Empty;
                filterDefinition &= Builders<SystemLogMongDBModel>.Filter.Eq(x => x.SourceID, filter_object.SourceID);
                filterDefinition &= Builders<SystemLogMongDBModel>.Filter.Regex(x => x.Type, new BsonRegularExpression(".*" + filter_object.Type + ".*"));
                if (filter_object.ObjectType!=null && filter_object.ObjectType.Trim()!="")
                {
                    filterDefinition &= Builders<SystemLogMongDBModel>.Filter.Regex(x=>x.ObjectType, new BsonRegularExpression(".*"+filter_object.ObjectType+".*"));

                }
                filterDefinition &= Builders<SystemLogMongDBModel>.Filter.Regex(x=>x.KeyID, new BsonRegularExpression(".*"+filter_object.KeyID + "*"));

                var model = await log_collection.Find(filterDefinition).Skip((page - 1) * size).Limit(size).ToListAsync();
                return model;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("API_CORE - GetLogByFilter - Cannot Excute: " + ex.ToString());
            }
            return null;
        }
    }
}
