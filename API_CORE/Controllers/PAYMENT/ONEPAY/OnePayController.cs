using API_CORE.Common.OnePay;
using Caching.RedisWorker;
using ENTITIES.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.Controllers.PAYMENT.ONEPAY
{
    [Route("api/[controller]")]
    [ApiController]

    //Document OnePay : https://drive.google.com/open?id=1ONy_RROunTPeEf73hoDTyijPud0wdX9e    

    public class OnePayController : Controller
    {
        private IConfiguration configuration;
        private IBankOnePayRepository bankOnePayRepository;
        private readonly RedisConn redisService;
        public OnePayController(IConfiguration _configuration,IBankOnePayRepository _bankOnePayRepository, RedisConn _redisService)
        {
            configuration = _configuration;
            bankOnePayRepository = _bankOnePayRepository;
            redisService = _redisService;
        }

        /// <summary>
        /// Kết quả trả về từ ONEPAY
        /// IPN
        /// Enpoint sẽ được setup từ ONEPAY
        /// https://localhost:44396/api/onepay/ipn/reveiver-data.json?vpc_Command=pay&vpc_Amount=10000&vpc_Card=970425&vpc_CardNum=970425xxx0001&vpc_CardUid=INS-khrh_mBsMfjgUAB_AQAqKg&vpc_MerchTxnRef=6373&vpc_Merchant=TESTONEPAY&vpc_Message=Approved&vpc_OrderInfo=HNTESTbookingbycuonglv&vpc_PayChannel=WEB&vpc_TransactionNo=PAY-maZaCQgoSEOVQR_VI8Kfsg&vpc_TxnResponseCode=0&vpc_Version=2&vpc_BinCountry=VN&vpc_Locale=vn&vpc_PaymentTime=20220930T151849Z&vpc_SecureHash=DD95D2D8F83E042999D51B757D530A730620BA09C942E39763CF2A9FD493E858
        /// </summary>
        /// <returns></returns>
        [HttpGet("ipn/reveiver-data.json")]
        public async Task<ActionResult> reveiverData()
        {
            try
            {
                string hashvalidateResult = string.Empty;
                string vpc_result = string.Empty;
                var query_string = HttpContext.Request.Query;
                var query_string_collect = new NameValueCollection();

                string amount = query_string["vpc_Amount"];
                string localed = query_string["vpc_Locale"];
                string command = query_string["vpc_Command"];
                string version = query_string["vpc_Version"];
                string cardType = query_string["vpc_Card"];
                string orderInfo = query_string["vpc_OrderInfo"];
                string merchantID = query_string["vpc_Merchant"];
                string authorizeID = query_string["vpc_AuthorizeId"];
                string merchTxnRef = query_string["vpc_MerchTxnRef"];
                string transactionNo = query_string["vpc_TransactionNo"];
                string acqResponseCode = query_string["vpc_AcqResponseCode"];
                string message = query_string["vpc_Message"];
                string vpc_TxnResponseCode = query_string["vpc_TxnResponseCode"];

                query_string_collect.Add("vpc_Amount", amount);
                query_string_collect.Add("vpc_Locale", localed);
                query_string_collect.Add("vpc_Command", command);
                query_string_collect.Add("vpc_Version", version);
                query_string_collect.Add("vpc_Card", cardType);
                query_string_collect.Add("vpc_OrderInfo", orderInfo);
                query_string_collect.Add("vpc_Merchant", merchantID);
                query_string_collect.Add("vpc_AuthorizeId", authorizeID);
                query_string_collect.Add("vpc_MerchTxnRef", merchTxnRef);
                query_string_collect.Add("vpc_TransactionNo", transactionNo);
                query_string_collect.Add("vpc_AcqResponseCode", acqResponseCode);
                query_string_collect.Add("vpc_Message", message);
                query_string_collect.Add("vpc_TxnResponseCode", vpc_TxnResponseCode);

                // Khoi tao lop thu vien
                VPCRequest conn = new VPCRequest(configuration["config_onepay:virtual_payment_client_url"]);
                conn.SetSecureSecret(configuration["config_onepay:hash_key"]);
                // Xu ly tham so tra ve va kiem tra chuoi du lieu ma hoa
                hashvalidateResult = conn.Process3PartyResponse(query_string_collect);
                // Lay gia tri tham so tra ve tu cong thanh toan
               
                string txnResponseCode = vpc_TxnResponseCode; // Mã lỗi trả về. Lưu lại để trace trong doc: https://drive.google.com/drive/folders/1uZsmlTJK2bJE7bWX5IK1ZQY6sl53Ty80                

                // Sua lai ham check chuoi ma hoa du lieu
                //if (recive_SecureHash !=doSecureHash)
                if (hashvalidateResult == "CORRECTED" && txnResponseCode.Trim() == "0")
                {
                    vpc_result = "Transaction was paid successful";
                }
                else if (hashvalidateResult == "INVALIDATED" && txnResponseCode.Trim() == "0")
                {
                    vpc_result = "Transaction is pending";
                }
                else
                {
                    vpc_result = "Transaction was not paid successful";
                }          


                return Ok(new
                {
                    status = txnResponseCode,
                    msg = vpc_result                    
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = (int)ResponseType.ERROR           
                });
            }
        }


        ///// <summary>
        ///// Kết quả trả về từ ONEPAY
        ///// IPN
        ///// Enpoint sẽ được setup từ ONEPAY
        ///// https://mtf.onepay.vn/paygate/vpcpay.op?AgainLink=https%3a%2f%2fadavigo.com&Title=Adavigo&vpc_AccessCode=6BEB2546&vpc_Amount=10000&vpc_Command=pay&vpc_Locale=vn&vpc_MerchTxnRef=6373&vpc_Merchant=TESTONEPAY&vpc_OrderInfo=HNTESTbookingbycuonglv&vpc_ReturnURL=https%3a%2f%2flocalhost%3a44396%2fapi%2fonepay%2fipn%2freveiver-data.json&vpc_TicketNo=10.36.68.92&vpc_Version=2&vpc_SecureHash=B621CF56C5005D4D1020A7413B0AF5CA8CC9B1AA1A88EFD75ADCD9166E14D8F5
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet("send-data.json")]
        //public async Task<ActionResult> sendData()
        //{
        //    try
        //    {
        //        string return_url = "https://localhost:44396/api/onepay/ipn/reveiver-data.json";
        //        var r = new Random();
        //        var onppay = new OnePayService(configuration,"cuonglv",r.Next(2000,6000),"HNTEST",10000, return_url, 43535,"dia chi","0942066299","cuonglv8@fpt.com","");
        //        var send = onppay.sendPaymentToOnePay();


        //        return Ok(new
        //        {
        //            status = (int)ResponseType.SUCCESS,
        //            msg = send
        //        }); 
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new
        //        {
        //            status = (int)ResponseType.ERROR
        //        });
        //    }
        //}
       
        [HttpPost("get-all-bankonepay-data.json")]
        public async Task<ActionResult> getAllData(string token)
        {
            try
            {
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
                {

                    int db_index = Convert.ToInt32(configuration["Redis:Database:db_core"]);
                    string cache_name = CacheName.Bank_One_Pay;
                    var obj_lst_OnePay = new List<BankOnePay>();
                    var strDataCache = redisService.Get(cache_name, db_index);
                    // Kiểm tra có data trong cache ko
                    if (!string.IsNullOrEmpty(strDataCache))
                        // nếu có trả ra luôn object 
                        obj_lst_OnePay = JsonConvert.DeserializeObject<List<BankOnePay>>(strDataCache);
                    else
                    {
                        // nếu chưa thì vào db lấy
                        obj_lst_OnePay = await bankOnePayRepository.GetAllBankOnePay();
                        if (obj_lst_OnePay == null)
                        {
                            LogHelper.InsertLogTelegram("Không lấy được danh sách ngân hàng");
                            return null;
                        }
                        else
                        {
                            // Gán vào Cache
                            redisService.Set(cache_name, JsonConvert.SerializeObject(obj_lst_OnePay), db_index);
                        }
                    }
                    
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "SUCCESS",
                        data = obj_lst_OnePay
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key không hợp lệ",
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("OnePay - OnePayController:" + ex.Message);
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "error: " + ex.ToString() });
            }
            
        }
    }
}
