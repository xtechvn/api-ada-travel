using API_CORE.Controllers.PAYMENT.Base;
using API_CORE.Controllers.PAYMENT.ONEPAY.Base;
using Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repositories.IRepositories;
using REPOSITORIES.IRepositories;
using REPOSITORIES.IRepositories.Fly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using WEB.API.Service.Queue;
using static MongoDB.Driver.WriteConcern;
using static Utilities.DepositHistoryConstant;

namespace API_CORE.Controllers.PAYMENT
{
    //Document OnePay : https://drive.google.com/open?id=1ONy_RROunTPeEf73hoDTyijPud0wdX9e  
    [Route("api/b2c/payment")]
    [ApiController]
    public class PaymentB2CController : Controller
    {
        private IConfiguration configuration;
        private IOrderRepository ordersRepository;
        private IIdentifierServiceRepository identifierServiceRepository;
        private IDepositHistoryRepository iDepositHistoryRepository;
        private ITourRepository tourRepository;

        public PaymentB2CController(IConfiguration _configuration, IOrderRepository _ordersRepository, 
            IIdentifierServiceRepository _identifierServiceRepository, IDepositHistoryRepository _iDepositHistoryRepository, ITourRepository _tourRepository)
        {
            configuration = _configuration;
            ordersRepository = _ordersRepository;
            identifierServiceRepository = _identifierServiceRepository;
            iDepositHistoryRepository = _iDepositHistoryRepository;
            tourRepository = _tourRepository;
        }


        [HttpPost("tour-booking")]
        public async Task<ActionResult> bookingTour(string token)
        {
            try
            {
                JArray objParr = null;

                string private_token_key = configuration["DataBaseConfig:key_api:b2c"];
                if (!CommonHelper.GetParamWithKey(token, out objParr, private_token_key))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key không hợp lệ"
                    });
                }
                else
                {
                    string booking_id = objParr[0]["booking_id"].ToString();
                    string order_no = objParr[0]["order_no"].ToString();
                    long order_id = Convert.ToInt64(objParr[0]["order_id"].ToString());
                    long account_client_id = Convert.ToInt64(objParr[0]["account_client_id"].ToString());
                    int payment_type = Convert.ToInt32(objParr[0]["payment_type"]);

                    int event_status =1;
                    if (order_no.Trim()!="" && order_id > 0)
                    {
                        event_status = 0;

                    }
                    else
                    {
                        // sinh mã đơn
                        order_no = await identifierServiceRepository.buildOrderNo((Int16)ServicesType.Tourist, (Int16)SourcePaymentType.b2c);

                        // Create new order no
                        order_id = await ordersRepository.CreateOrderNo(order_no);
                        if (order_id < 0)
                        {
                            LogHelper.InsertLogTelegram("payment / booking/tour.json error: Lỗi không tạo được tour  token = " + token);
                            return Ok(new
                            {
                                status = (int)ResponseType.FAILED,
                                msg = "Booking Tour thất bại. Vui lòng liên hệ với bộ phận chăm sóc khách hàng để được hỗ trợ"
                            });
                        }


                    }
                    var work_queue = new WorkQueueClient(configuration);
                   
                    #region Create new Order --> Push to Queue
                    var j_param_queue = new Dictionary<string, string>
                    {
                        {"order_no", order_no},
                        {"order_id", order_id.ToString()},
                        {"service_type", ((Int16)ServicesType.Tourist).ToString()},
                        {"booking_id",booking_id},
                        {"event_status",event_status.ToString()}, //1 thanh toan lai : 0 tạo mới
                        {"account_client_id",account_client_id.ToString()}, //1 thanh toan lai : 0 tạo mới

                    };

                    #region Thực hiện cập nhật lại orderno vào SQL và backup input queue. Mục đích: re-push queue nếu lỗi bot tạo đơn
                    await ordersRepository.BackupBookingInfo(order_id, JsonConvert.SerializeObject(j_param_queue));
                    #endregion
                    string url = "";
                  
                    var queue_setting = new QueueSettingViewModel
                    {
                        host = configuration["Queue:Host"],
                        v_host = configuration["Queue:V_Host"],
                        port = Convert.ToInt32(configuration["Queue:Port"]),
                        username = configuration["Queue:Username"],
                        password = configuration["Queue:Password"]
                    };
                    var response_queue = work_queue.InsertQueueSimpleWithDurable(queue_setting, JsonConvert.SerializeObject(j_param_queue), QueueName.CheckoutOrder);
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Booking thành công",
                        url = url,
                        order_no = order_no,
                        order_id = order_id,
                        content = order_no + " CHUYEN KHOAN"
                    });
                }
                #endregion   
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("payment /booking/tour.json" + ex.ToString() + "token =" + token);
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Transaction Error !!!"
                });
            }

        }
    }
}
