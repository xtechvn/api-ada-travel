using Entities.ViewModels;
using ENTITIES.APPModels;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Utilities.Contants;
using WEB.API.Service.Queue;

namespace API_CORE.Service.Log
{
    public static class TeleLog
    {
     

        public static async Task<int> InsertLogTelegram(IConfiguration configuration,string log_content, string log_type = "", string log_source = "")
        {
            var rs = 1;
            var work_queue = new WorkQueueClient(configuration);
            var queue_setting = new QueueSettingViewModel
            {
                host = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Queue")["Host"],
                v_host = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Queue")["V_Host"],
                port = Convert.ToInt32(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Queue")["Port"]),
                username = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Queue")["Username"],
                password = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Queue")["Password"]
            };
            try
            {
                SystemLog systemLog = new SystemLog();
                systemLog.Log = log_content;
                string typeId = "";
                int source = (int)SystemLogSourceID.API_CORE;
                switch (log_type)
                {
                    case LogSourceTypeConstant.LOG_COMPARE_PRICE:
                        typeId = SystemLogTypeID.ERROR;
                        source = (int)SystemLogSourceID.BACKEND;
                        break;
                    //case LogSourceTypeConstant.LOG_B2B_ADAVIGO:
                    //    typeId = (int)SystemLogTypeID.All;
                    //    source = (int)SystemLogSourceID.FRONT_END_B2B;
                    //    break;
                }

                if (!string.IsNullOrEmpty(log_source))
                {
                    switch (log_source)
                    {
                        case LogSourceConstant.LOG_CMS_ADAVIGO:
                            source = (int)SystemLogSourceID.BACKEND;
                            break;
                    }
                }
                systemLog.Type = typeId;
                systemLog.SourceID = source;
                // Execute Push Queue
                var response_queue = work_queue.InsertQueueSimple(queue_setting, JsonConvert.SerializeObject(systemLog), QueueName.SystemLog);
                if (!response_queue)
                {
                    rs = -1;
                }
            }
            catch (Exception ex)
            {
                rs = -1;
                SystemLog systemLog = new SystemLog();
                systemLog.Log = ex.Message;
                systemLog.SourceID = (int)SystemLogSourceID.API_CORE;
                // Execute Push Queue
                work_queue.InsertQueueSimple(queue_setting, JsonConvert.SerializeObject(systemLog), QueueName.SystemLog);
            }
            return rs;
        }
    }
}
