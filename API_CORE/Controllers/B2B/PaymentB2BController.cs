using API_CORE.Service;
using API_CORE.Service.Price;
using Caching.Elasticsearch;
using Caching.RedisWorker;
using ENTITIES.Models;
using ENTITIES.ViewModels.Hotel;
using ENTITIES.ViewModels.Payment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories;
using REPOSITORIES.IRepositories.Elasticsearch;
using REPOSITORIES.IRepositories.Hotel;
using REPOSITORIES.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Payments;
using Utilities;
using Utilities.Contants;
using static MongoDB.Driver.WriteConcern;

namespace API_CORE.Controllers.B2B
{
    [Route("api/b2b/payment")]
    [ApiController]
    public class PaymentB2BController : Controller
    {
        private IConfiguration configuration;
        private IHotelBookingMongoRepository hotelBookingMongoRepository;
        private IElasticsearchDataRepository elasticsearchDataRepository;
        private IESRepository<HotelESViewModel> _ESRepository;
        private HotelESRepository _hotelESRepository;
        private IHotelDetailRepository _hotelDetailRepository;
        private IAllotmentFundRepository _allotmentFundRepository;
        private readonly RedisConn redisService;
        private IDepositHistoryRepository _depositHistoryRepository;
        private IAccountRepository _accountRepository;
        private List<int> SERVICES_TYPE_B2B = new List<int>() { (int)ServicesType.VINHotelRent, (int)ServicesType.FlyingTicket };
        private List<BankingAccountQRModel> BANK_ACCOUNT = new List<BankingAccountQRModel>();

        public PaymentB2BController(IConfiguration _configuration, IHotelBookingMongoRepository _hotelBookingMongoRepository, 
            IElasticsearchDataRepository _elasticsearchDataRepository, IHotelDetailRepository hotelDetailRepository,
            RedisConn _redisService, IAllotmentFundRepository allotmentFundRepository, IDepositHistoryRepository depositHistoryRepository, IAccountRepository accountRepository)
        {
            configuration = _configuration;
            hotelBookingMongoRepository = _hotelBookingMongoRepository;
            elasticsearchDataRepository = _elasticsearchDataRepository;
            _hotelDetailRepository = hotelDetailRepository;
            _ESRepository = new ESRepository<HotelESViewModel>(configuration["DataBaseConfig:Elastic:Host"]);
            _hotelESRepository = new HotelESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
            redisService = _redisService;
            _allotmentFundRepository = allotmentFundRepository;
            _depositHistoryRepository = depositHistoryRepository;
            _accountRepository = accountRepository;
            BANK_ACCOUNT = new List<BankingAccountQRModel>
            {
                new BankingAccountQRModel()
                {
                    AccountName = "Công ty cổ phần Thương mại và Dịch vụ Quốc tế Đại Việt",
                    AccountNumber = "19131835226016",
                    BankId = "Techcombank",
                    Branch = "Đông Đô",
                    Bin="970407",
                    Image="https://static-image.adavigo.com/uploads/images/banklogo/TCB.png",
                    ClientId = null,
                    CreatedBy = 18,
                    CreatedDate = DateTime.Now,
                    Id = 1,
                    SupplierId = 604,
                    UpdatedBy = 18,
                    UpdatedDate = DateTime.Now,
                },
                new BankingAccountQRModel()
                {
                    AccountName = "Công ty cổ phần Thương mại và Dịch vụ Quốc tế Đại Việt",
                    AccountNumber = "371704070000023",
                    BankId = "HDBank",
                    Bin="970437",
                    Image="https://static-image.adavigo.com/uploads/images/banklogo/HDB.png",
                    Branch = "Hà Nội",
                    ClientId = null,
                    CreatedBy = 18,
                    CreatedDate = DateTime.Now,
                    Id = 2,
                    SupplierId = 604,
                    UpdatedBy = 18,
                    UpdatedDate = DateTime.Now,
                },
                new BankingAccountQRModel()
                {
                    AccountName = "Công ty cổ phần Thương mại và Dịch vụ Quốc tế Đại Việt",
                    AccountNumber = "113600558866",
                    BankId = "VietinBank",
                    Branch = "Tràng An",
                    Bin="970415",
                    Image="https://static-image.adavigo.com/uploads/images/banklogo/ICB.png",
                    ClientId = null,
                    CreatedBy = 18,
                    CreatedDate = DateTime.Now,
                    Id = 3,
                    SupplierId = 604,
                    UpdatedBy = 18,
                    UpdatedDate = DateTime.Now,
                },
            };
        }
        [HttpPost("check-fund-available")]
        public async Task<ActionResult> CheckFundAvailable(string token)
        {
            try
            {
                #region Test

                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    long client_id = Convert.ToInt64(objParr[0]["client_id"]);
                    int service_type = Convert.ToInt32(objParr[0]["service_type"]);
                    string booking_id= objParr[0]["booking_id"].ToString();
                    if (booking_id == null || booking_id.Trim()=="") {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Data invalid!"
                        });
                    }
                    var booking=await hotelBookingMongoRepository.getBookingByID(new string[] { booking_id });
                    var detail = await _accountRepository.GetAccountClient(client_id);

                    if (booking != null && booking.Count>0) {
                        double amount = booking.Sum(x => x.total_amount);
                        var result = await _depositHistoryRepository.GetAmountDepositB2B((long)detail.ClientId, client_id, SERVICES_TYPE_B2B);
                        if(result !=null)
                        {
                            var data = result.Where(x => x.service_type == service_type).FirstOrDefault();
                            if (data != null && amount <= data.account_blance)
                            {
                                return Ok(new
                                {
                                    status = (int)ResponseType.SUCCESS,
                                    msg = "Success",
                                    data = true
                                });
                            }
                            else
                            {
                                return Ok(new
                                {
                                    status = (int)ResponseType.SUCCESS,
                                    msg = "Số dư tài khoản không đủ để thanh toán đơn hàng",
                                    data = false
                                });
                            }
                        }
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Không tìm thấy dữ liệu đơn hàng, vui lòng liên hệ bộ phận CSKH",
                        data = false
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
                LogHelper.InsertLogTelegram("GetHotelLocations - HotelB2BController: " + ex);
                return Ok(new { status = ResponseType.FAILED, msg = "error: " + ex.ToString() });
            }
        }
        [HttpPost("confirm-fund-payment")]
        public async Task<ActionResult> ConfirmPaymentOrderByFund(string token)
        {
            try
            {
                #region Test

                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    long order_id = Convert.ToInt64(objParr[0]["order_id"]);
                    long client_id = Convert.ToInt64(objParr[0]["client_id"]);
                    int client_type = Convert.ToInt32(objParr[0]["clientType"]);
                    int service_type = Convert.ToInt32(objParr[0]["service_type"]);
                    string booking_id = objParr[0]["booking_id"].ToString();
                    if (booking_id == null || booking_id.Trim() == "")
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Data invalid!"
                        });
                    }
                    var booking = await hotelBookingMongoRepository.getBookingByID(new string[] { booking_id });
                    var detail = await _accountRepository.GetAccountClient(client_id);

                    if (booking != null && booking.Count > 0)
                    {
                        double amount = booking.Sum(x => x.total_amount);
                        var funds = await _depositHistoryRepository.GetAmountDepositB2B((long)detail.ClientId, client_id, SERVICES_TYPE_B2B);
                        var fund= funds.Where(x => x.service_type == service_type).First();
                        var allotment_use = new AllotmentUse()
                        {
                            AccountClientId=client_id,
                            AllomentFundId= fund.id,
                            AmountUse=amount,
                            ClientId= (long)detail.ClientId,
                            DataId= order_id,
                            ServiceType=(short)service_type,
                            CreateDate=DateTime.Now, 
                        };
                        allotment_use.Id = _allotmentFundRepository.AddAllotmentUse(allotment_use);
                        if (allotment_use.Id > 0)
                        {
                            await _allotmentFundRepository.UpdateFundBalanceByAllotmentUse(allotment_use);
                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                msg = "Success",
                                data = allotment_use.Id
                            });
                        }
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "FAILED"
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
                LogHelper.InsertLogTelegram("GetHotelLocations - HotelB2BController: " + ex);
                return Ok(new { status = ResponseType.FAILED, msg = "error: " + ex.ToString() });
            }
        }
        [HttpPost("qr-code")]
        public async Task<ActionResult> GenerateQRCode(string token)
        {
            try
            {
                #region Test
                //var j_param = new
                //{
                //    order_id = 50230,
                //    amount = 1500000,
                //    id = 1,
                //    order_no = "BKS24F134400"
                //};
                
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    long order_id = Convert.ToInt32(objParr[0]["order_id"]);
                    double amount = Convert.ToDouble(objParr[0]["amount"]);
                    int id = Convert.ToInt32(objParr[0]["id"]);
                    string order_no = objParr[0]["order_no"].ToString();

                    if (id <=0 || order_id <= 0|| order_no==null|| order_no.Trim()=="")
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Data invalid!"
                        });
                    }
                    var selected = BANK_ACCOUNT.FirstOrDefault(x => x.Id == id);
                    if(selected==null || selected.Id <= 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Data invalid!"
                        });
                    }
                    var url = await VietQRService.GetVietQRCode(new PaymentHotelModel()
                    {
                        amount=Convert.ToDecimal(amount),
                        bank_account=selected.AccountNumber,
                        bank_code=selected.Bin,
                        bank_name=selected.AccountName,
                        booking_id="",
                        event_status=0,
                        order_id=order_id,
                        order_no=order_no,
                        payment_type=1,
                        short_name=selected.BankId
                    });
                    if (url == null || url.Trim()=="")
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Cannot Get QR Code"
                        });
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Success",
                        data= url
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
                LogHelper.InsertLogTelegram("GenerateQRCode - HotelB2BController: " + ex);
                return Ok(new { status = ResponseType.FAILED, msg = "error: " + ex.ToString() });
            }
        }
        [HttpPost("bank-transfer-list")]
        public async Task<ActionResult> GetBanksList(string token)
        {
            try
            {
                #region Test

                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Success",
                        data = BANK_ACCOUNT.Select(x=> new
                        {
                            account_name=x.AccountName,
                            account_number=x.AccountNumber,
                            code=x.Bin,
                            id = x.Id,
                            image= x.Image,
                            branch = x.Branch
                        })
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
                LogHelper.InsertLogTelegram("GenerateQRCode - HotelB2BController: " + ex);
                return Ok(new { status = ResponseType.FAILED, msg = "error: " + ex.ToString() });
            }
        }
    }
}
