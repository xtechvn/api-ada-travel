using Entities.ViewModels;
using ENTITIES.APPModels;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using Utilities;
using Utilities.Contants;
using WEB.API.Service.Queue;

namespace API_CORE.Service.Log
{
    public class LogService
    {
        private readonly IConfiguration configuration;
        private readonly WorkQueueClient work_queue;
        private readonly QueueSettingViewModel queue_setting;
        public LogService(IConfiguration _configuration)
        {
            configuration = _configuration;
            work_queue = new WorkQueueClient(configuration);
            queue_setting = new QueueSettingViewModel
            {
                host = configuration["Queue:Host"],
                v_host = configuration["Queue:V_Host"],
                port = Convert.ToInt32(configuration["Queue:Port"]),
                username = configuration["Queue:Username"],
                password = configuration["Queue:Password"]
            };
        }
      
        public bool InsertLog(string log_content, string key_id ="error")
        {
            try
            {
                SystemLog logModel = new SystemLog()
                {
                    KeyID = key_id,
                    SourceID = (int)SystemLogSourceID.API_CORE,
                    Log = log_content,
                    Type = SystemLogTypeID.ERROR
                };
                // Execute Push Queue
                var response_queue = work_queue.InsertQueueSimple(queue_setting, JsonConvert.SerializeObject(logModel), QueueName.SystemLog);
                if (response_queue)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertLogAPITelegram - LogService: " + ex.ToString());
                return false;
            }
        }
        public bool InsertLog(SystemLog logModel)
        {
            try
            {
                // Execute Push Queue
                var response_queue = work_queue.InsertQueueSimple(queue_setting, JsonConvert.SerializeObject(logModel), QueueName.SystemLog);
                if (response_queue)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertLogAPITelegram - LogService: " + ex.ToString());
                return false;
            }
        }
    }
}
