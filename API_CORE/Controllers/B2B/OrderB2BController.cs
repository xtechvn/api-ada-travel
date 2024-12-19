using Caching.RedisWorker;
using ENTITIES.ViewModels.Order;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.Controllers.B2B
{
    [Route("api/b2b/order")]
    [ApiController]
    public class OrderB2BController : Controller
    {
        private IConfiguration configuration;
        private IOrderRepository ordersRepository;
        private IClientRepository clientRepository;
        private IAccountRepository accountRepository;
        private IAccountB2BRepository accountB2BRepository;
        private readonly RedisConn redisService;
        public OrderB2BController(IConfiguration _configuration, IOrderRepository _ordersRepository, RedisConn _redisService, IAccountRepository _accountRepository, IClientRepository _clientRepository, IAccountB2BRepository _accountB2BRepository)
        {
            configuration = _configuration;
            redisService = _redisService;
            ordersRepository = _ordersRepository;
            accountRepository = _accountRepository;
            clientRepository = _clientRepository;
            accountB2BRepository = _accountB2BRepository;
        }
        [HttpPost("history/hotel-list.json")]
        public async Task<ActionResult> GetOrderHistoryByID(string token)
        {
            #region Test

            //var j_param = new Dictionary<string, object>
            //    {
            //        {"account_client_id", "159"},
            //        {"page", "1"},
            //        {"size", "20"},
            //        {"start_date",DateTime.MinValue.ToString() },
            //        {"end_date",new DateTime(2023,04,10,23,59,59).ToString() }
            //    };
            //var data_product = JsonConvert.SerializeObject(j_param);
            //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);

            #endregion
            try
            {

              
                JArray objParr = null;
                bool response_queue = false;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    int page = Convert.ToInt32(objParr[0]["page"]);
                    int size = Convert.ToInt32(objParr[0]["size"]);
                    
                    DateTime start_date = Convert.ToDateTime(objParr[0]["start_date"]);
                    DateTime end_date = Convert.ToDateTime(objParr[0]["end_date"]);

                    int take = (size <= 0) ? 10 : size;
                    int skip = ((page - 1) <= 0) ? 0 : (page - 1) * take;
                    long account_client_id = Convert.ToInt64(objParr[0]["account_client_id"]);
                    int service_type = Convert.ToInt32(objParr[0]["service_type"]);

                    var account_client = await accountRepository.GetAccountClient(account_client_id);
                    if(account_client==null || account_client.Id <= 0 ||account_client.Status!=(int)AccountClientStatus.ACTIVE || account_client.ClientType==(int)ClientType.CUSTOMER || account_client.ClientType == (int)ClientType.ALL)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Tài khoản này đã bị khóa hoặc không tồn tại",
                            
                        });
                    }
                    var listid = await accountB2BRepository.GetListAccountClient((long)account_client.ClientId, -1, -1, -1, -1, null);
                    var list_account_clientId = string.Join(',', listid.Select(i => i.Id).ToList());
                    var list_order = new HotelBookingB2BPagingViewModel();
                    string cache_name = CacheType.ORDER_ACCOUNT_CLIENT_HOTEL + account_client.Id;
                    var j_data = await redisService.GetAsync(cache_name, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_order_client"]));
                    
                    if ((take+skip) < 200)
                    {
                        if (j_data != null && j_data.Trim() != "")
                        {
                            list_order = JsonConvert.DeserializeObject<HotelBookingB2BPagingViewModel>(j_data);
                            if(list_order.data==null || list_order.data.Count < 1) {
                                list_order = await ordersRepository.GetHotelOrderB2BPaging(200, 1, (long)account_client.ClientId, list_account_clientId, start_date,end_date);
                                redisService.Set(cache_name, JsonConvert.SerializeObject(list_order), DateTime.Now.AddMinutes(10), Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_order_client"]));
                            }
                        }
                        else
                        {
                            list_order = await ordersRepository.GetHotelOrderB2BPaging(200, 1, (long)account_client.ClientId, list_account_clientId, start_date, end_date);
                            redisService.Set(cache_name, JsonConvert.SerializeObject(list_order),DateTime.Now.AddMinutes(10),Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_order_client"]));
                        }
                        var total_order = list_order.total_record;
                        var list_order_pagnition = new List<HotelBookingB2BViewModel>();
                        list_order_pagnition = list_order.data.Skip(skip).Take(take).ToList();

                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Successfully",
                            data = list_order_pagnition,
                            current_page = page,
                            total_order = total_order
                        });
                    }
                    else
                    {
                        list_order = await ordersRepository.GetHotelOrderB2BPaging(size, page, (long)account_client.ClientId, list_account_clientId, start_date, end_date);
                        var total_order = list_order.total_record;
                    
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Successfully",
                            data = list_order.data,
                            current_page = page,
                            total_order = total_order
                        });
                    }

                    
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Key invalid!"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetOrderHistoryByID - OrderController - : " + ex);
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }

        }
        [HttpPost("history/hotel-order-detail.json")]
        public async Task<ActionResult> GetOrderDetailByID(string token)
        {
            #region Test
            //var j_param = new Dictionary<string, object>
            //    {
            //        {"order_id", "1463"},
            //        {"account_client_id", "159"},
            //    };
            //var data_product = JsonConvert.SerializeObject(j_param);
            //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
            #endregion
            try
            {

                JArray objParr = null;
                bool response_queue = false;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    long order_id = Convert.ToInt64(objParr[0]["order_id"]);
                    long account_client_id = Convert.ToInt64(objParr[0]["account_client_id"]);
                    long type = Convert.ToInt64(objParr[0]["type"]);
                    if (type == 1)
                    {
                        var accountclient = await accountRepository.GetByClientId(account_client_id);
                        account_client_id = accountclient.Id;
                    }
                    var account_client = await accountRepository.GetAccountClient(account_client_id);

                    var listid = await accountB2BRepository.GetListAccountClient((long)account_client.ClientId, -1, -1, -1, -1, null);
                    var list_account_clientId = string.Join(',', listid.Select(i => i.Id).ToList());

                    if (account_client == null || account_client.Id <= 0 || account_client.Status != (int)AccountClientStatus.ACTIVE || account_client.ClientType == (int)ClientType.CUSTOMER || account_client.ClientType == (int)ClientType.ALL)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Tài khoản này đã bị khóa hoặc không tồn tại",

                        });
                    }
                    var order = ordersRepository.getDetail(order_id);
                    if (list_account_clientId.Contains(order.AccountClientId.ToString()) != true)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Tài khoản không có quyền xem đơn hàng này",

                        });
                    }
                    string cache_name = CacheType.ORDER_DETAIL + order_id;
                    var j_data = await redisService.GetAsync(cache_name, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_order_client"]));
                    HotelB2BOrderDetailViewModel data =null;
                    if (j_data != null && j_data.Trim() != "")
                    {
                         data = JsonConvert.DeserializeObject<HotelB2BOrderDetailViewModel>(j_data);
                        if (data.order == null || data.order.OrderId<=0)
                        {
                            data = await ordersRepository.GetHotelOrderDetailB2B(order_id);
                            redisService.Set(cache_name, JsonConvert.SerializeObject(data), Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_order_client"]));
                        }
                    }
                    else
                    {
                         data = await ordersRepository.GetHotelOrderDetailB2B(order_id);
                        redisService.Set(cache_name, JsonConvert.SerializeObject(data), Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_order_client"]));
                    }
                    if(data==null)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Get Data Failed",
                            data = data
                        });
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Successfully",
                        data = data
                    });


                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Key invalid!"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetOrderHistoryByID - OrderController - : " + ex);
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }

        }

    }
}
