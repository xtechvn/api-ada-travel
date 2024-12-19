using Entities.ViewModels;
using ENTITIES.APPModels.ReadBankMessages;
using ENTITIES.Models;
using ENTITIES.ViewModels.DepositHistory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.Controllers.DepositHistory
{
    [Route("api")]
    [ApiController]
    public class DepositHistoryController : Controller
    {
        private IConfiguration configuration;
        private IDepositHistoryRepository depositHistoryRepository;
        private IAccountRepository _accountRepository;
        private IClientRepository _clientRepository;
        private IIdentifierServiceRepository _identifierServiceRepository;
        private IPaymentRepository paymentRepository;
        private List<int> SERVICES_TYPE_B2B = new List<int>() { (int)ServicesType.VINHotelRent, (int)ServicesType.FlyingTicket };

        public DepositHistoryController(IConfiguration _configuration, IDepositHistoryRepository _depositHistoryRepository, IAccountRepository accountRepository, IIdentifierServiceRepository identifierServiceRepository, IPaymentRepository _paymentRepository,
            IClientRepository clientRepository)
        {
            configuration = _configuration;
            depositHistoryRepository = _depositHistoryRepository;
            _accountRepository = accountRepository;
            _identifierServiceRepository = identifierServiceRepository;
            paymentRepository = _paymentRepository;
            _clientRepository = clientRepository;

        }
        [HttpPost("b2b/deposit/history")]
        public async Task<ActionResult> getDepositHistory(string token)
        {
            try
            {
                //var j_param = new Dictionary<string, string>
                //        {
                //            {"skip", "1"},
                //            {"take", "15"},
                //            {"userid", "159"},
                //            {"startdate", "08/05/2023"},
                //            {"enddate", "10/05/2023"},
                //            {"servicetype", "-1"},
                //      };
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    int page = Convert.ToInt16(objParr[0]["page"]);
                    int size = Convert.ToInt16(objParr[0]["size"]);
                    int service_type = Convert.ToInt32(objParr[0]["service_type"]);
                    long account_client_id = Convert.ToInt64(objParr[0]["clientid"]);
                    if(objParr[0]["from_date"] ==null || objParr[0]["to_date"] == null)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Data invalid!",

                        });
                    }
                    DateTime from_date = Convert.ToDateTime(objParr[0]["from_date"]);
                    DateTime to_date = Convert.ToDateTime(objParr[0]["to_date"]);
                    if (from_date == DateTime.MinValue)
                    {
                        from_date = DateTime.Now.AddYears(-1);
                    }
                    if (to_date == DateTime.MinValue)
                    {
                        to_date = DateTime.Now;
                    }
                    if (size <= 0) size = 10;
                    if (page < 1) page = 1;
                    var detail = await _accountRepository.GetAccountClient(account_client_id);
                    if (detail != null)
                    {
                        List<int> service_type_list = SERVICES_TYPE_B2B;
                        if (service_type > 0)
                        {
                            service_type_list = new List<int>() { service_type };
                        }
                        var result = await depositHistoryRepository.getDepositHistory((long)detail.ClientId, string.Join(",", service_type_list),from_date,to_date, page,size);
                        if (result != null)
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                msg = "thành công",
                                data = result,
                            });
                        }
                    }
                }
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "Data invalid!",

                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getDepositHistory - DepositHistoryController: " + ex);
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "Error On Excution. Vui lòng liên hệ IT" });
            }

        }
        [HttpPost("b2b/deposit/amount")]
        public async Task<ActionResult> getAmountDeposit(string token)
        {
            try
            {
                var j_param = new Dictionary<string, string>
                        {
                            {"client_id", "10410"},
                            
                      };
                var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                   long clientid = Convert.ToInt16(objParr[0]["client_id"]);
                   var AcClient =await _accountRepository.GetAccountClient(clientid);
                    if (AcClient == null)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "data null",
                        });
                    }
                    var result =await depositHistoryRepository.GetAmountDepositB2B((long)AcClient.ClientId, clientid,  SERVICES_TYPE_B2B);
                    //var result = await depositHistoryRepository.amountDeposit((long)AcClient.ClientId);

                    if (result != null)
                    {
                       
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "thành công",
                            data = result,
                            
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "data null",
                        });
                    }
                }
                else
                {

                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key invalid!",

                    });
                }
            }
            catch(Exception ex)
            {
                LogHelper.InsertLogTelegram("getAmountDeposit - DepositHistoryController: " + ex);
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "Error On Excution. Vui lòng liên hệ IT" });
            }
        }
        [HttpPost("insert-deposithistory.json")]
        public async Task<ActionResult> CreateDepositHistory(string token)
        {
            try
            {

                //var j_param = new ENTITIES.Models.DepositHistory()
                //{

                //    Price = 1000000,
                //    TransType = 2,
                //    UserId = 159,
                //    ServiceType = 1,

                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    var depositHistory = JsonConvert.DeserializeObject<ENTITIES.Models.DepositHistory>(objParr[0].ToString());
                    if(depositHistory.ServiceType == (int)ServicesType.VINHotelRent|| depositHistory.ServiceType == (int)ServicesType.OthersHotelRent)
                    {
                        depositHistory.Title = "Nạp tiền khách sạn";
                    }
                    if (depositHistory.ServiceType == (int)ServicesType.FlyingTicket)
                    {
                        depositHistory.Title = "Nạp tiền vé máy bay";
                    }
                    if (depositHistory.ServiceType == (int)ServicesType.VehicleRent)
                    {
                        depositHistory.Title = "Nạp tiền thuê xe du lịch";
                    }
                    if (depositHistory.ServiceType == (int)ServicesType.Tourist)
                    {
                        depositHistory.Title = "Nạp tiền tour du lịch";
                    }
                    var account_client = await _accountRepository.GetAccountClient((long)depositHistory.UserId);
                    if (account_client != null)
                    {
                        depositHistory.ClientId = account_client.ClientId;
                        if (depositHistory.Price > 0)
                        {
                            depositHistory.TransNo =await _identifierServiceRepository.buildDepositNo((int)depositHistory.TransType);
                             var result = await depositHistoryRepository.CreateDepositHistory(depositHistory);

                            if (result != null)
                            {

                                return Ok(new
                                {
                                    status = (int)ResponseType.SUCCESS,
                                    msg = "thành công",
                                    data = depositHistory,
                                });
                            }
                            else
                            {
                                LogHelper.InsertLogTelegram("Nạp tiền không thành công: " + token);
                                return Ok(new
                                {
                                    status = (int)ResponseType.ERROR,
                                    msg = "không thành công",

                                });
                            }
                        }
                        else
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.ERROR,
                                msg = "Đơn giá phải > 0",

                            });
                        }
                        
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "User không tồn tại trong hệ thống",

                        });
                    }
                    
                }
                else
                {

                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key invalid!",

                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateDepositHistory - DepositHistoryController: " + ex);
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "Error On Excution. Vui lòng liên hệ IT" });
            }
        }
       

        private DateTime CheckDate(string dateTime)
        {
            DateTime _date = DateTime.MinValue;
            if (!string.IsNullOrEmpty(dateTime))
            {
                _date = DateTime.ParseExact(dateTime, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            }

            return _date != DateTime.MinValue ? _date : DateTime.MinValue;
        }
    }
}
