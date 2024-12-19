using API_CORE.Service.Log;
using APP.PUSH_LOG.Functions;
using Entities.ViewModels;
using ENTITIES.APPModels;
using ENTITIES.APPModels.ReadBankMessages;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using WEB.API.Service.Queue;

namespace API_CORE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogManagerController : ControllerBase
    {
        private IConfiguration configuration;
        private LogService logService;
        public LogManagerController(IConfiguration _configuration)
        {
            configuration = _configuration;
            logService = new LogService(_configuration);
        }

        [EnableCors("MyApi")]
        [HttpPost("insert-log.json")]
        public async Task<ActionResult> InsertLogTelegram(string token)
        {
            try
            {
                #region Test
                //var j_param = new LogModel()
                //{
                //    log_content = "Log CMS insert into user",
                    //log_type = LogSourceTypeConstant.LOG_COMPARE_PRICE
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:api_manual"]);
                #endregion

                JArray objParr = null;
                bool response_queue = false;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    LogModel logModel = JsonConvert.DeserializeObject<LogModel>(objParr[0].ToString());
                    var result = TeleLog.InsertLogTelegram(configuration,logModel.log_content, logModel.log_type, logModel.log_source);
                    if (response_queue)
                    {
                        return Ok(new { status = ResponseTypeString.Success, message = "Push Log Success" });
                    }
                    else
                    {
                        return Ok(new { status = ResponseTypeString.Success, message = "Push Log ERROR" });
                    }
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        message = "Key invalid!"
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { status = ResponseTypeString.Fail, message = "error: " + ex.ToString() });
            }

        }

        //[EnableCors("MyApi")]
        //[HttpPost("get-log-token.json")]
        //public async Task<ActionResult> GetLogToken(LogModel logModel)
        //{
        //    try
        //    {
        //        var data_product = JsonConvert.SerializeObject(logModel);
        //        string token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:api_manual"]);
        //        TeleLog.InsertLogTelegram("Test log");
        //        return Ok(new
        //        {
        //            status = ResponseTypeString.Success,
        //            message = "Get Token success",
        //            token = token
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new { status = ResponseTypeString.Fail, message = "error: " + ex.ToString(), token = "" });
        //    }

        //}

        [EnableCors("MyApi")]
        [HttpPost("insert-log-activity.json")]
        public async Task<ActionResult> InsertLogMongo(string token)
        {
            try
            {
                #region Test
                //var j_param = new LogModel()
                //{
                //    log_content = "Log CMS insert into user",
                //    log_type = LogSourceTypeConstant.LOG_COMPARE_PRICE
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:api_manual"]);
                #endregion

                JArray objParr = null;
                bool response_queue = false;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    LogModel logModel = JsonConvert.DeserializeObject<LogModel>(objParr[0].ToString());
                    SystemLog systemLog = new SystemLog();
                    string typeId = "ERROR";
                    switch (logModel.log_type)
                    {
                        case LogSourceTypeConstant.LOG_COMPARE_PRICE:
                            typeId = SystemLogTypeID.ERROR;
                            break;
                    }
                    systemLog.Log = logModel.log_content;
                    systemLog.Type = typeId;
                    systemLog.SourceID = (int)SystemLogSourceID.BACKEND;
                    // Execute Push Queue
                    var work_queue = new WorkQueueClient(configuration);
                    var queue_setting = new QueueSettingViewModel
                    {
                        host = configuration["Queue:Host"],
                        v_host = configuration["Queue:V_Host"],
                        port = Convert.ToInt32(configuration["Queue:Port"]),
                        username = configuration["Queue:Username"],
                        password = configuration["Queue:Password"]
                    };
                    response_queue = work_queue.InsertQueueSimple(queue_setting, JsonConvert.SerializeObject(systemLog), QueueName.SystemLog);
                    if (response_queue)
                    {
                        return Ok(new { status = ResponseTypeString.Success, msg = "Push Queue Success" });
                    }
                    else
                    {
                        return Ok(new { status = ResponseTypeString.Fail, msg = "Push Queue ERROR" });
                    }
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key invalid!"
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { status = ResponseTypeString.Fail, msg = "error: " + ex.ToString() });
            }

        }
        [EnableCors("MyApi")]
        [HttpPost("insert-app-log.json")]
        public async Task<ActionResult> InsertAppLog(string token)
        {
            #region Test
            //var j_param = new SystemLog()
            //{
            //     SourceID=(int)SystemLogSourceID.API_CORE,
            //     KeyID="Test",
            //     Log="Test Log",
            //     Type=SystemLogTypeID.WARNING
            //};
            //var data_product = JsonConvert.SerializeObject(j_param);
            //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:api_manual"]);
            #endregion
            try
            {
            
                JArray objParr = null;
                bool response_queue = false;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    SystemLog logModel = JsonConvert.DeserializeObject<SystemLog>(objParr[0].ToString());
                    response_queue = logService.InsertLog(logModel);
                    if (response_queue)
                    {
                        return Ok(new { status = ResponseTypeString.Success, msg = "Push Queue Success" });
                    }
                    else
                    {
                        return Ok(new { status = ResponseTypeString.Fail, msg = "Push Queue ERROR" });
                    }
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key invalid!"
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { status = ResponseTypeString.Fail, msg = "error: " + ex.ToString() });
            }

        }
        [EnableCors("MyApi")]
        [HttpPost("check-sms-exists.json")]
        public async Task<ActionResult> CheckBankSMS(string token)
        {
            try
            {

                JArray objParr = null;
                bool response_queue = false;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    List<BankMessageDetail> logModel = JsonConvert.DeserializeObject<List<BankMessageDetail>>(objParr[0].ToString());
                    // Execute Push Queue
                    if (logModel == null || logModel.Count < 1)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "No Data"
                        });
                    }
                    List<BankMessageDetail> new_text = new List<BankMessageDetail>();
                    foreach (var sms in logModel)
                    {
                        string str = MongoDBSMSAccess.CheckIfSMSExists(sms, configuration);

                        if (str == null)
                        {
                            new_text.Add(sms);
                        }
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Success",
                        data=new_text
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key invalid!"
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { status = ResponseTypeString.Fail, msg = "error: " + ex.ToString() });
            }

        }
        [EnableCors("MyApi")]
        [HttpPost("push-sms-mongo.json")]
        public async Task<ActionResult> PushSMSToMongo(string token)
        {
            try
            {

                JArray objParr = null;
                bool response_queue = false;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    BankMessageDetail detail = JsonConvert.DeserializeObject<BankMessageDetail>(objParr[0].ToString());
                    if (detail.OrderNo == null || detail.OrderNo.Trim() == "" || detail.MessageContent == null || detail.MessageContent.Trim() == ""
                      || detail.BankName == null || detail.BankName.Trim() == "" || detail.Amount <= 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Data không hợp lệ"
                        });
                    }
                    string str = MongoDBSMSAccess.CheckIfSMSExists(detail, configuration);
                    if (str == null)
                    {
                        MongoDBSMSAccess.InsertSMS(detail, configuration);
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key invalid!"
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { status = ResponseTypeString.Fail, msg = "error: " + ex.ToString() });
            }

        }

        [EnableCors("MyApi")]
        [HttpPost("insert-app-log-tele.json")]
        public async Task<ActionResult> InsertAppLogtele(string token)
        {
            try
            {
                #region Test
                var j_param = new SystemLog()
                {
                    Log = "test Log b2c",
                    
                };
                var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion
                JArray objParr = null;
                bool response_queue = false;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    SystemLog logModel = JsonConvert.DeserializeObject<SystemLog>(objParr[0].ToString());
                    logModel.SourceID = (int)SystemLogSourceID.FRONTEND_B2C;
                    logModel.Type = SystemLogTypeID.WARNING;
                    
                    // Execute Push Queue
                    var work_queue = new WorkQueueClient(configuration);
                    var queue_setting = new QueueSettingViewModel
                    {
                        host = configuration["Queue:Host"],
                        v_host = configuration["Queue:V_Host"],
                        port = Convert.ToInt32(configuration["Queue:Port"]),
                        username = configuration["Queue:Username"],
                        password = configuration["Queue:Password"]
                    };
                    response_queue = work_queue.InsertQueueSimple(queue_setting, JsonConvert.SerializeObject(logModel), QueueName.SystemLog);
                    if (response_queue)
                    {
                        MongoDBSMSAccess.InsertLogMongoB2C(configuration,JsonConvert.SerializeObject(logModel.Log),logModel.KeyID);
                        return Ok(new { status = (int)ResponseType.SUCCESS, msg = "Push Queue Success" });
                    }
                    else
                    {
                        return Ok(new { status = (int)ResponseType.ERROR, msg = "Push Queue ERROR" });
                    }
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key invalid!"
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { status = ResponseTypeString.Fail, msg = "error: " + ex.ToString() });
            }

        }
        [EnableCors("MyApi")]
        [HttpPost("logging.json")]
        public async Task<ActionResult> SetLog(string token)
        {
            try
            {
                #region Test
                //var j_param = new SystemLog()
                //{
                //    Log = "Test Log Order",
                //    KeyID = "Test Order",
                //    SourceID = (int)SystemLogSourceID.BACKEND,
                //    Type = SystemLogTypeID.ACTIVITY,
                //    ObjectType = "Test",
                //    CompanyType=0
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:api_manual"]);
                #endregion
                JArray objParr = null;
                bool response_queue = false;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    SystemLog logModel = JsonConvert.DeserializeObject<SystemLog>(objParr[0].ToString());
                 
                    var work_queue = new WorkQueueClient(configuration);
                    var queue_setting = new QueueSettingViewModel
                    {
                        host = configuration["Queue:Host"],
                        v_host = configuration["Queue:V_Host"],
                        port = Convert.ToInt32(configuration["Queue:Port"]),
                        username = configuration["Queue:Username"],
                        password = configuration["Queue:Password"]
                    };
                    response_queue = work_queue.InsertQueueSimple(queue_setting, JsonConvert.SerializeObject(logModel), QueueName.SystemLog);
                    if (response_queue)
                    {
                        return Ok(new { status = (int)ResponseType.SUCCESS, msg = "Push Queue Success" });
                    }
                    else
                    {
                        return Ok(new { status = (int)ResponseType.ERROR, msg = "Push Queue ERROR" });
                    }
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key invalid!"
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { status = ResponseTypeString.Fail, msg = "error: " + ex.ToString() });
            }

        }
        [EnableCors("MyApi")]
        [HttpPost("get-log.json")]
        public async Task<ActionResult> GetLog(string token)
        {
            try
            {
                #region Test
                var j_param = new SystemLog()
                {
                    KeyID = "1494",
                    SourceID = (int)SystemLogSourceID.BACKEND,
                    Type = SystemLogTypeID.ACTIVITY

                };
              //  var data_product = JsonConvert.SerializeObject(j_param);
              // token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:api_manual"]);
                #endregion
                JArray objParr = null;
                bool response_queue = false;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    SystemLog logModel = JsonConvert.DeserializeObject<SystemLog>(objParr[0].ToString());
                    int page = Convert.ToInt32(objParr[0]["page"]==null ? "1": objParr[0]["page"]);
                    int size = Convert.ToInt32(objParr[0]["size"] == null ? "20" : objParr[0]["size"]);
                    if (page <= 0) page = 1;
                    if (size <= 0) size = 20;
                    var data= await MongoDBSMSAccess.GetLogByFilter(logModel, configuration,page,size);
                    return Ok(
                        new { 
                            status = (int)ResponseType.SUCCESS, 
                            msg = "Success",
                            data=data
                        });

                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key invalid!"
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { status = ResponseTypeString.Fail, msg = "error: " + ex.ToString() });
            }

        }
    }
}
