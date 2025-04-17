using API_CORE.Controllers.MAIL.Base;
using API_CORE.Service.Vin;
using APP.PUSH_LOG.Functions;
using ENTITIES.APPModels.ReadBankMessages;
using ENTITIES.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repositories.IRepositories;
using REPOSITORIES.IRepositories;
using REPOSITORIES.IRepositories.Clients;
using REPOSITORIES.IRepositories.Fly;
using REPOSITORIES.IRepositories.Hotel;
using REPOSITORIES.IRepositories.Notify;
using REPOSITORIES.IRepositories.VinWonder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Payments;
using Utilities;
using Utilities.Contants;
using static iTextSharp.text.pdf.AcroFields;

namespace API_CORE.Controllers.APP
{

    [Route("api/app/payment")]
    [ApiController]
    public class ContractPayAppController : Controller
    {
        private IConfiguration _configuration;
        private IContractPayRepository _contractPayRepository;
        private IIdentifierServiceRepository identifierServiceRepository;
        private IOrderRepository orderRepository;
        private IDepositHistoryRepository iDepositHistoryRepository;
        private MailService _mail_service;
        private readonly VinWonderBookingService _vinWonderBookingService;
        private readonly IVinWonderBookingRepository _vinWonderBookingRepository;
        private readonly IContactClientRepository _contactClientRepository;
        private readonly IVoucherRepository voucherRepository;
        private readonly INotifyRepository notifyRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IDebtGuaranteeRepository _debtGuaranteeRepository;

        public ContractPayAppController(IConfiguration configuration, IContractPayRepository contractPayRepository, IIdentifierServiceRepository _identifierServiceRepository, IOrderRepository _ordersRepository,
             IDepositHistoryRepository _iDepositHistoryRepository,
            IClientRepository _clientRepository, IFlyBookingDetailRepository _flyBookingDetailRepository, IFlightSegmentRepository _flightSegmentRepository, IOrderRepository _orderRepository, IPassengerRepository _passengerRepository, IBagageRepository _bagageRepository,
             IAirPortCodeRepository _airPortCodeRepository, IWebHostEnvironment _webHostEnvironment, IAirlinesRepository _airlinesRepository, IAccountClientRepository _accountClientRepository,
           IHotelBookingRepositories _hotelBookingRepositories, IOtherBookingRepository otherBookingRepository, ITourRepository tourRepository, IAllCodeRepository allCodeRepository, IUserRepository userRepository,
           IVinWonderBookingRepository vinWonderBookingRepository, IContactClientRepository contactClientRepository, IVoucherRepository _voucherRepository, INotifyRepository _notifyRepository, IClientRepository clientRepository,
           IHotelBookingRoomExtraPackageRepository hotelBookingRoomExtraPackageRepository,IHotelBookingRoomRepository hotelBookingRoomRepository, IDebtGuaranteeRepository debtGuaranteeRepository)
        {
            _configuration = configuration;
            _contractPayRepository = contractPayRepository;
            identifierServiceRepository = _identifierServiceRepository;
            _orderRepository = _ordersRepository;
            iDepositHistoryRepository = _iDepositHistoryRepository;
            orderRepository = _orderRepository;
            _mail_service = new MailService(configuration, contactClientRepository, vinWonderBookingRepository, _clientRepository, _flyBookingDetailRepository,
                       _flightSegmentRepository, _orderRepository, _passengerRepository, _bagageRepository, _airPortCodeRepository, _webHostEnvironment, _airlinesRepository, _hotelBookingRepositories,
                       otherBookingRepository, tourRepository, allCodeRepository, userRepository, contractPayRepository, _voucherRepository, _notifyRepository, hotelBookingRoomExtraPackageRepository, hotelBookingRoomRepository);
            _vinWonderBookingService = new VinWonderBookingService(configuration, _vinWonderBookingRepository);
            _vinWonderBookingRepository = vinWonderBookingRepository;
            _contactClientRepository = contactClientRepository;
            _clientRepository = clientRepository;
            _debtGuaranteeRepository = debtGuaranteeRepository;
        }
        [HttpPost("update-order-payment-bank-transfer.json")]
        public async Task<ActionResult> UpdateOrderBankTransferPayment(string token)
        {

            //BankMessageDetail model = new BankMessageDetail()
            //{
            //    AccountNumber = "688989",
            //    Amount = 1900000,
            //    BankName = "MSB",
            //    BankTransferType = 0,
            //    BookingCode = "",
            //    CreatedTime = DateTime.Now,
            //    ImagePath = "",
            //    is_specify_transfer_to_order = 0,
            //    MessageContent = "688989-780960-A25D172813 FT25107870932852",
            //    OrderId = 0,
            //    OrderNo = "",
            //    ReceiveTime = DateTime.Now,
            //    StatusPush = false,
            //    TransferCode = "",
            //    TransferDescription = "688989-780960-A25D172813 FT25107870932852"
            //};
            //var data_product = JsonConvert.SerializeObject(model);
            //token = CommonHelper.Encode(data_product, _configuration["DataBaseConfig:key_api:api_manual"]);

            try
            {
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, _configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    string data = objParr[0].ToString();
                    BankMessageDetail detail = JsonConvert.DeserializeObject<BankMessageDetail>(data);
                    if (detail.MessageContent == null || detail.MessageContent.Trim() == ""
                        || detail.BankName == null || detail.BankName.Trim() == "" || detail.Amount <= 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Data không hợp lệ"
                        });
                    }
                    //LogHelper.InsertLogTelegram("UpdateOrderBankTransferPayment - ContractPayAppController - "
                    //                    + "Received [" + detail.MessageContent + "]  [" + detail.BankTransferType + "]");
                    switch (detail.BankTransferType)
                    {
                        case (int)BankMessageTransferType.CANNOT_DETECT:
                            {
                                string order_no = "";
                                Match m = Regex.Match(detail.MessageContent.ToUpper(), @"\b(O|A|P|D|CVB|BKS|CVW|KS|VB|TR|VW)\d{2}[A-Z]\d{5,6}\b", RegexOptions.IgnoreCase);
                                ENTITIES.Models.Order order = null;

                                if (m.Success && m.Value != null && m.Value.Trim() != "")
                                {
                                    order_no = m.Value;
                                    order= await orderRepository.GetOrderByOrderNo(order_no);
                                }
                               
                                if(order == null || order.OrderId<=0)
                                {
                                    Match m_order = Regex.Match(detail.MessageContent.ToUpper(), @"(O|A|P|D|CVB|BKS|CVW|KS|VB|TR|VW)\d{2}[A-Z]\d{5,6}", RegexOptions.IgnoreCase);
                                    if (m_order.Success && m_order.Value!=null && m_order.Value.Trim()!="")
                                    {
                                        order_no = m_order.Value;
                                        order = await orderRepository.GetOrderByOrderNo(order_no);
                                    }
                                }
                                if (order != null && order.OrderId > 0)
                                {
                                    detail.OrderNo = order.OrderNo;
                                    detail.OrderId = order.OrderId;
                                    detail.BankTransferType = (int)BankMessageTransferType.ORDER_PAYMENT;
                                }
                                else
                                {
                                    var deposit = await iDepositHistoryRepository.GetDepositHistoryByTransNo(order_no);
                                    if (deposit != null && deposit.Id > 0)
                                    {
                                        detail.OrderNo = deposit.TransNo;
                                        detail.OrderId = deposit.Id;
                                        detail.BankTransferType = (int)BankMessageTransferType.DEPOSIT_PAYMENT;
                                    }

                                }
                                if (detail.BankTransferType == (int)BankMessageTransferType.CANNOT_DETECT)
                                {
                                    return Ok(new
                                    {
                                        status = (int)ResponseType.FAILED,
                                        msg = "Cập nhật Payment thất bại. Content: " + detail.TransferDescription
                                    });
                                }
                                var contract_pay_code = await identifierServiceRepository.buildContractPay();
                                var payment_detail = await _contractPayRepository.UpdateOrderBankTransferPayment(detail, contract_pay_code);
                                if (payment_detail != null && payment_detail.OrderId > 0)
                                {
                                    return Ok(new
                                    {
                                        status = (int)ResponseType.SUCCESS,
                                        msg = "Cập nhật Payment thành công",
                                        data = payment_detail
                                    });
                                }
                                else
                                {
                                    return Ok(new
                                    {
                                        status = (int)ResponseType.FAILED,
                                        msg = "Cập nhật Payment thất bại. Code: " + detail.OrderNo
                                    });
                                }
                            }
                        case (int)BankMessageTransferType.ORDER_PAYMENT:
                        case (int)BankMessageTransferType.DEPOSIT_PAYMENT:
                            {
                                //LogHelper.InsertLogTelegram("UpdateOrderBankTransferPayment - ContractPayAppController - "
                                //         + "EXISTS [" + detail.MessageContent + "] [" + detail.BankTransferType + "][" + detail.OrderId + "][" + detail.OrderNo + "]");
                                var contract_pay_code = await identifierServiceRepository.buildContractPay();
                                var payment_detail = await _contractPayRepository.UpdateOrderBankTransferPayment(detail, contract_pay_code);
                                //var is_checkout = await iDepositHistoryRepository.BotVerifyTrans(detail.OrderNo);
                                if (payment_detail != null && payment_detail.OrderId > 0)
                                {
                                    return Ok(new
                                    {
                                        status = (int)ResponseType.SUCCESS,
                                        msg = "Cập nhật Payment thành công",
                                        data = payment_detail
                                    });
                                }
                                else
                                {
                                    return Ok(new
                                    {
                                        status = (int)ResponseType.FAILED,
                                        msg = "Cập nhật Payment thất bại. Code: " + detail.OrderNo
                                    });
                                }
                            }
                        case (int)BankMessageTransferType.LOCAL_COMPANY_TRANSFER:
                            {
                                return Ok(new
                                {
                                    status = (int)ResponseType.FAILED,
                                    msg = "Payment nội bộ, không thể cập nhật",
                                });
                            }
                    }


                }
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Key không hợp lệ"
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateOrderBankTransferPayment - ContractPayAppController - api/app/payment/update-order-payment-bank-transfer.json: " + ex.ToString());
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "Error On Excution. Vui lòng liên hệ IT" });
            }


        }
        [HttpPost("send-email-confirmed-payment.json")]
        public async Task<ActionResult> SendEmailConfirmedPaymentToOperator(string token)
        {
            string msg = "";
            try
            {
                JArray objParr = null;
                #region Test

                //var j_param = new Dictionary<string, string>
                //{
                //    {"order_id", "50669"},
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, _configuration["DataBaseConfig:key_api:api_manual"]);

                #endregion
                if (CommonHelper.GetParamWithKey(token, out objParr, _configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    long order_id = Convert.ToInt64(objParr[0]["order_id"]);
                    if (order_id > 0)
                    {
                        var DetailDebtGuarantee = _debtGuaranteeRepository.DetailDebtGuaranteebyOrderid((int)order_id);
                        if (DetailDebtGuarantee != null)
                        {
                            _debtGuaranteeRepository.UpdateDebtGuarantee(DetailDebtGuarantee.Id, (int)DebtGuaranteeStatus.HOAN_THANH, 2052);
                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                msg = "Đã gửi email"
                            });
                        }
                        var order = orderRepository.getDetail(order_id);
                        if(order!=null && order.ClientId!=null && order.ClientId > 0)
                        {
                           
                            if (order.OrderStatus == (int) OrderStatus.WAITING_FOR_OPERATOR )
                            {
                                var success = await _mail_service.SendSuccessPaymentToOperator(order_id);
                                return Ok(new
                                {
                                    status = (int)ResponseType.SUCCESS,
                                    msg = success
                                });
                            }
                            LogHelper.InsertLogTelegram("gửi Email Confirmed Payment To Operator không thành công OrderID=" + order_id);
                           
                        }
                        else
                        {
                            var success = await _mail_service.SendSuccessPaymentToOperator(order_id);
                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                msg = success
                            });
                        }
                     
                        
                    }
                    else
                    {
                        msg = "Failed to get OrderID: OrderID=" + order_id;

                    }
                }
                else
                {
                    msg = "Token Invalid: " + token;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SendEmailConfirmedPaymentToOperator - ContractPayAppController - send-email-confirmed-payment.json: " + ex.ToString());
            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = msg
            });
        }

        [HttpPost("vinwonder/confirm-booking.json")]
        public async Task<ActionResult> VinWonderConfirmBooking(string token)
        {

            try
            {
                JArray objParr = null;
                #region Test
                /*
                var j_param = new Dictionary<string, string>
                {
                    {"order_id", "12392"},
                };
                var data_product = JsonConvert.SerializeObject(j_param);
                token = CommonHelper.Encode(data_product, _configuration["DataBaseConfig:key_api:api_manual"]);
                */
                #endregion

                if (CommonHelper.GetParamWithKey(token, out objParr, _configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    long order_id = Convert.ToInt64(objParr[0]["order_id"]);
                    if (order_id <= 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Data invalid!"
                        });
                    }
                    List<object> result = new List<object>();
                    var booking = await _vinWonderBookingRepository.GetVinWonderBookingByOrderId(order_id);
                    if (booking != null && booking.Count > 0)
                    {
                        string vin_booking_token = await _vinWonderBookingService.GetToken();
                        var booking_mongo = _vinWonderBookingRepository.GetBookingById(booking.Select(x => x.AdavigoBookingId).ToArray());
                        if (booking_mongo != null && booking_mongo.Count > 0)
                        {
                            foreach (var data in booking_mongo)
                            {
                                if (data.requestVin != null && data.requestVin.Count > 0)
                                {
                                    foreach (var item in data.requestVin)
                                    {
                                        var push_content = await _vinWonderBookingService.ConfirmBooking(item, vin_booking_token);
                                        result.Add(push_content);
                                    }
                                }

                            }
                        }
                    }
                    MongoDBSMSAccess.InsertLogMongoDb(_configuration, JsonConvert.SerializeObject(result), "ConfirmBookingVinWonderAPI");
                    var order = orderRepository.getDetail(order_id);
                    string email = "";
                    if (order != null && order.ContactClientId != null && order.ContactClientId > 0)
                    {
                        var contact_client = _contactClientRepository.GetByContactClientId((long)order.ContactClientId);
                        email = contact_client.Email;
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "ConfirmBooking Success",
                        data = result,
                        email = email
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
                LogHelper.InsertLogTelegram("VinWonderConfirmBooking - ContractPayAppController - vinwonder/confirm-booking.json: token " + token + "\n " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "ERROR!",
                });
            }
        }
        [HttpPost("n8n/send-email-payment-success")]
        public async Task<ActionResult> N8NSendEmailSuccessPaymentToOperator(string token)
        {

            try
            {
                JArray objParr = null;
                #region Test
                /*
                var j_param = new Dictionary<string, string>
                {
                    {"order_id", "12392"},
                };
                var data_product = JsonConvert.SerializeObject(j_param);
                token = CommonHelper.Encode(data_product, _configuration["DataBaseConfig:key_api:api_manual"]);
                */
                #endregion

                if (CommonHelper.GetParamWithKey(token, out objParr, _configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    long order_id = Convert.ToInt64(objParr[0]["order_id"]);
                    if (order_id <= 0) {

                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Data Invalid!",
                            body = ""
                        });
                    }
                    var excute = await _mail_service.SendSuccessPaymentToOperator(order_id);
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Success",
                        body = excute
                    });
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("N8NSendEmailSuccessPaymentToOperator - ContractPayAppController - email/payment-email-body: token " + token + "\n " + ex);
               
            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = "ERROR!",
                body=""
            });
        }
        [HttpPost("n8n/telegram")]
        public async Task<ActionResult> N8NSendMessageTelegram(string token)
        {

            try
            {
                JArray objParr = null;
                #region Test
                /*
                var j_param = new Dictionary<string, string>
                {
                    {"order_id", "12392"},
                };
                var data_product = JsonConvert.SerializeObject(j_param);
                token = CommonHelper.Encode(data_product, _configuration["DataBaseConfig:key_api:api_manual"]);
                */
                #endregion

                if (CommonHelper.GetParamWithKey(token, out objParr, _configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    string bot_id = objParr[0]["bot_id"].ToString();
                    string group_id = objParr[0]["group_id"].ToString();
                    string message = objParr[0]["message"].ToString();
                    if(bot_id==null || bot_id.Trim()==""
                        || group_id == null || group_id.Trim() == ""
                         || message == null || message.Trim() == "")
                    {

                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Data Invalid!",
                            body = ""
                        });
                    }

                    TelegramBotClient alertMsgBot = new TelegramBotClient(bot_id);
                    var rs_push = await alertMsgBot.SendTextMessageAsync(group_id, message);
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Success",
                        body = (rs_push != null)
                    });
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("N8NSendMessageTelegram - ContractPayAppController - n8n/telegram: token " + token + "\n " + ex);

            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = "ERROR!",
                body = ""
            });
        }
       
    }
}
