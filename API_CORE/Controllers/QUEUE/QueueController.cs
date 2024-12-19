using Entities.ViewModels;
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

namespace API_CORE.Controllers.QUEUE
{
    [Route("api")]
    [ApiController]
    public class QueueController : Controller
    {
        private IConfiguration configuration;
        public QueueController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        [HttpPost("queue/re-push-data.json")]
        public async Task<ActionResult> pushDataToQueue(string token)
        {
            try
            {
                JArray objParr = null;
                var j_param = new Dictionary<string, object>
                {
                    {"j_param_queue", "232323"},  // data json cần push queue              
                    {"queue_name", QueueName.CheckoutOrder.ToString()},
                };
                var data_product = JsonConvert.SerializeObject(j_param);

                if (!CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key không hợp lệ"
                    });
                }

                string j_param_queue = objParr[0]["j_param_queue"].ToString();
                string queue_name = objParr[0]["queue_name"].ToString();

                // setting queue
                var work_queue = new WorkQueueClient(configuration);
                var queue_setting = new QueueSettingViewModel
                {
                    host = configuration["Queue:Host"],
                    v_host = configuration["Queue:V_Host"],
                    port = Convert.ToInt32(configuration["Queue:Port"]),
                    username = configuration["Queue:Username"],
                    password = configuration["Queue:Password"]
                };

                // push queue
                var response_queue = work_queue.InsertQueueSimpleWithDurable(queue_setting, j_param_queue, QueueName.CheckoutOrder);

                return View();
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        [HttpPost("queue/insert-message.json")]
        public async Task<ActionResult> PushMessagetoQueue(string token)
        {
            try
            {
                JArray objParr = null;
                /*
                var j_param = new Dictionary<string, object>
                {
                    {"payment_type","1"},
                    {"account_client_id", "173"},
                    {"bank_code", "VPBank"},
                    {"booking_cart_id","64b374e9154ee7d1dc0183d6"},
                    {"order_no", "CVB23L133343"},
                    {"order_id", "33887"},
                    {"event_status","2"},// 1: thanh toan lai | 0: tao moi don hang
                    {"service_type", ((Int16)ServicesType.FlyingTicket).ToString()},
                    {"source_payment_type", "1"},
                    {"client_type","2" }
                };
                
                var data_product = JsonConvert.SerializeObject(j_param);
                var json_input = new Dictionary<string, object>
                {
                    {"j_param_queue",data_product},
                    {"queue_name", "queue_checkout_order"}
                    
                };
                token = CommonHelper.Encode(JsonConvert.SerializeObject(json_input), configuration["DataBaseConfig:key_api:b2c"]);
                */
                if (!CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key không hợp lệ"
                    });
                }

                string j_param_queue = objParr[0]["j_param_queue"].ToString();
                string queue_name = objParr[0]["queue_name"].ToString();

                // setting queue
                var work_queue = new WorkQueueClient(configuration);
                var queue_setting = new QueueSettingViewModel
                {
                    host = configuration["Queue:Host"],
                    v_host = configuration["Queue:V_Host"],
                    port = Convert.ToInt32(configuration["Queue:Port"]),
                    username = configuration["Queue:Username"],
                    password = configuration["Queue:Password"]
                };

                // push queue
                var response_queue = work_queue.InsertQueueSimpleWithDurable(queue_setting, j_param_queue, queue_name);

                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Successful"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg =ex.ToString()
                });
            }

        }       
        [HttpPost("queue/insert-message-qc.json")]
        public async Task<ActionResult> PushMessagetoQueue(string data_json,string queue_name)
        {
            try
            {
                JArray objParr = null;
                /*
                var j_param = new Dictionary<string, object>
                {
                    {"payment_type","1"},
                    {"account_client_id", "173"},
                    {"bank_code", "VPBank"},
                    {"booking_cart_id","64b374e9154ee7d1dc0183d6"},
                    {"order_no", "CVB23L133343"},
                    {"order_id", "33887"},
                    {"event_status","2"},// 1: thanh toan lai | 0: tao moi don hang
                    {"service_type", ((Int16)ServicesType.FlyingTicket).ToString()},
                    {"source_payment_type", "1"},
                    {"client_type","2" }
                };
                
                var data_product = JsonConvert.SerializeObject(j_param);
                var json_input = new Dictionary<string, object>
                {
                    {"j_param_queue",data_product},
                    {"queue_name", "queue_checkout_order"}
                    
                };
                token = CommonHelper.Encode(JsonConvert.SerializeObject(json_input), configuration["DataBaseConfig:key_api:b2c"]);
                
                if (!CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key không hợp lệ"
                    });
                }
                */
               string j_param_queue = data_json;
               // string queue_name = queue_name;

                // setting queue
                var work_queue = new WorkQueueClient(configuration);
                var queue_setting = new QueueSettingViewModel
                {
                    host = configuration["Queue:Host"],
                    v_host = configuration["Queue:V_Host"],
                    port = Convert.ToInt32(configuration["Queue:Port"]),
                    username = configuration["Queue:Username"],
                    password = configuration["Queue:Password"]
                };

                // push queue
                var response_queue = work_queue.InsertQueueSimpleWithDurable(queue_setting, j_param_queue, queue_name);

                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Successful"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg =ex.ToString()
                });
            }

        }
    }
}
