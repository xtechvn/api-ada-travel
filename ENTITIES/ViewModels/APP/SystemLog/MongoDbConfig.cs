using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.APPModels.SystemLogs
{
    public class MongoDbConfig
    {
        public string host { get; set; }
        public int port { get; set; }
        public string user_name { get; set; }
        public string password { get; set; }
        public string database_name { get; set; }
    }
}
