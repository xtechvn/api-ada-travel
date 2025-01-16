using API_CORE.Service.Log;
using API_CORE.Service.Price;
using API_CORE.Service.Vin;
using Caching.Elasticsearch;
using Caching.RedisWorker;
using ENTITIES.Models;
using ENTITIES.ViewModels;
using ENTITIES.ViewModels.B2B;
using ENTITIES.ViewModels.Hotel;
using ENTITIES.ViewModels.Hotels;
using ENTITIES.ViewModels.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repositories.IRepositories;
using REPOSITORIES.IRepositories;
using REPOSITORIES.IRepositories.Elasticsearch;
using REPOSITORIES.IRepositories.Hotel;
using REPOSITORIES.Repositories;
using REPOSITORIES.Repositories.Hotel;
using SixLabors.ImageSharp;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using static Utilities.DepositHistoryConstant;
using ClientType = Utilities.Contants.ClientType;

namespace API_CORE.Controllers.B2B
{
    [Route("api/b2b/home")]
    [ApiController]
    public class HomeB2BController : Controller
    {
        private IConfiguration configuration;
        private OrderESRepository _orderESRepository;
        private IAccountRepository accountRepository;
        private IOrderRepository ordersRepository;


        private string cache_order_client = "cache_order_client_";
        private readonly RedisConn redisService;
        public HomeB2BController(IConfiguration _configuration,  RedisConn _redisService, IAccountRepository _accountRepository, IOrderRepository _ordersRepository)
        {
            configuration = _configuration;
            redisService = _redisService;
            _orderESRepository = new OrderESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
            accountRepository = _accountRepository;
            ordersRepository=_ordersRepository;

        }
        [HttpPost("summary.json")]
        public async Task<ActionResult> HomeSummary(string token)
        {
            JArray objParr = null;
            HomeSummaryB2BResponseModel result = new HomeSummaryB2BResponseModel
            {
                list_order=new List<ENTITIES.ViewModels.ElasticSearch.OrderElasticsearchViewModel>(),
                list_order_checkin=new List<ENTITIES.ViewModels.ElasticSearch.OrderElasticsearchViewModel>(),
                list_order_checkout=new List<ENTITIES.ViewModels.ElasticSearch.OrderElasticsearchViewModel>(),
                list_order_payment=new List<ENTITIES.ViewModels.ElasticSearch.OrderElasticsearchViewModel>(),
                list_order_waiting_payment=new List<ENTITIES.ViewModels.ElasticSearch.OrderElasticsearchViewModel>(),
                total_amount=0,
                total_order_checkin=0,
                total_order_checkout=0,
                total_order_payment=0,
                total_order_waiting_payment=0,
                total_payment=0,
                total_waiting_payment = 0
            };
            try
            {
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    long account_client_id = Convert.ToInt64(objParr[0]["account_client_id"]);
                    var account_client = await accountRepository.GetAccountClient(account_client_id);
                    if (account_client == null || account_client.Id <= 0 || account_client.Status != (int)AccountClientStatus.ACTIVE || account_client.ClientType == (int)ClientType.CUSTOMER || account_client.ClientType == (int)ClientType.ALL)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Tài khoản này đã bị khóa hoặc không tồn tại",

                        });
                    }

                    //string cache_name = CacheName.B2B_SUMMARY + account_client_id;
                    //var str = redisService.Get(cache_name, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                    //if (str != null && str.Trim() != "")
                    //{
                    //    result = JsonConvert.DeserializeObject<HomeSummaryB2BResponseModel>(str);
                    //    //-- Trả kết quả
                    //    return Ok(new
                    //    {
                    //        status = (int)ResponseType.SUCCESS,
                    //        msg = "success",
                    //        data = result
                    //    });
                    //}
                    var fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,0,0,0); // Replace with your actual from date
                    var toDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59); // Replace with your actual to date
                    var order_status = new List<int>() { 0, 1, 2, 3, 4, 5, 6 };
                    var payment_status = new List<int>() { 0, 1, 2,3 };
                    var total_order_in_day = await _orderESRepository.GetListOrder((long)account_client.ClientId, order_status, payment_status, fromDate, toDate);
                   
                    if(total_order_in_day!=null && total_order_in_day.Count > 0)
                    {
                        result.list_order = total_order_in_day;
                        result.list_order_waiting_payment = total_order_in_day.Where(x => x.paymentstatus == null || x.paymentstatus == 0).ToList();
                        result.list_order_payment = total_order_in_day.Where(x => x.paymentstatus != null && (x.paymentstatus == 1 || x.paymentstatus == 2)).ToList();
                      
                        result.total_amount = total_order_in_day.Sum(x => x.amount!=null? (double)x.amount : 0);
                        result.total_waiting_payment = total_order_in_day.Where(x => x.paymentstatus == null || x.paymentstatus == 0).Sum(x => (x.amount != null ? (double)x.amount : 0));
                        result.total_payment = total_order_in_day.Where(x => x.paymentstatus != null && (x.paymentstatus == 1 || x.paymentstatus == 2)).Sum(x => (x.amount != null ? (double)x.amount : 0));
                        result.total_order_payment = total_order_in_day.Count(x => x.paymentstatus != null && (x.paymentstatus == 1 || x.paymentstatus == 2));
                        result.total_order_waiting_payment = total_order_in_day.Count(x => x.paymentstatus == null || x.paymentstatus == 0);
                    }
                    //var order_payment_in_day = await ordersRepository.GetListOrder((long)account_client.ClientId, fromDate, toDate, string.Join(",", order_status));
                    //if(order_payment_in_day!=null && order_payment_in_day.Count > 0)
                    //{
                    //    result.total_waiting_payment = order_payment_in_day.Sum(x => (x.Amount!=null? (double)x.Amount:0) - (x.Payment != null ? (double)x.Payment : 0));
                    //    result.total_payment = order_payment_in_day.Sum(x =>  (x.Payment != null ? (double)x.Payment : 0));
                    //    result.total_order_payment = order_payment_in_day.Count(x => x.PaymentStatus != null && (x.PaymentStatus == 1 || x.PaymentStatus == 2));
                    //    result.total_order_waiting_payment = order_payment_in_day.Count(x => x.PaymentStatus == null || x.PaymentStatus == 0);
                    //}
                  
                    var order_checkin = await _orderESRepository.GetListOrderCheckinNow((long)account_client.ClientId, order_status, fromDate, toDate);
                    var order_checkout = await _orderESRepository.GetListOrderCheckoutNow((long)account_client.ClientId, order_status, fromDate, toDate);
                    if(order_checkin!= null && order_checkin.Count > 0)
                    {
                        result.total_order_checkin = order_checkin.Count;
                        result.list_order_checkin = order_checkin;

                    }
                    if (order_checkout != null && order_checkout.Count > 0)
                    {
                        result.total_order_checkout = order_checkout.Count;
                        result.list_order_checkout = order_checkout;

                    }
                    //redisService.Set(cache_name, JsonConvert.SerializeObject(result),DateTime.Now.AddHours(4), Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_order_client"]));

                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "success",
                        data= result
                    });
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("HomeB2BController- Token: " + token + " - PushBookingToMongo: " + ex.ToString());

                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }
            return Ok(new { status = (int)ResponseType.FAILED, msg = "Data Invailid" });

        }

    }
}
