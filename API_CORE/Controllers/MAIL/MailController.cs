using API_CORE.Service.Mail;
using APP.PUSH_LOG.Functions;
using Elasticsearch.Net;
using ENTITIES.Models;
using ENTITIES.ViewModels.MongoDb;
using ENTITIES.ViewModels.VinWonder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repositories.IRepositories;
using REPOSITORIES.IRepositories;
using REPOSITORIES.IRepositories.Clients;
using REPOSITORIES.IRepositories.Fly;
using REPOSITORIES.IRepositories.Notify;
using REPOSITORIES.IRepositories.VinWonder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.Controllers.Mail
{
    [Route("api/b2c/[controller]")]
    [ApiController]
    public class MailController : Controller
    {
        private IConfiguration configuration;
        private IClientRepository clientRepository;
        private IContactClientRepository contactClientRepository;
        private IFlyBookingDetailRepository flyBookingDetailRepository;
        private IFlightSegmentRepository flightSegmentRepository;
        private IOrderRepository orderRepository;
        private IPassengerRepository passengerRepository;
        private IBagageRepository bagageRepository;
        private IAirPortCodeRepository airPortCodeRepository;
        private IAirlinesRepository airlinesRepository;
        private IWebHostEnvironment webHostEnvironment;
        private IHotelBookingRepositories hotelBookingRepositories;
        private IAccountClientRepository accountClientRepository;
        private API_CORE.Controllers.MAIL.Base.MailService _mail_service;
        private readonly INotifyRepository notifyRepository;
        public MailController(IConfiguration _configuration, IContactClientRepository _contactClientRepository,
            IClientRepository _clientRepository, IFlyBookingDetailRepository _flyBookingDetailRepository,
             IFlightSegmentRepository _flightSegmentRepository, IOrderRepository _orderRepository, IVinWonderBookingRepository vinWonderBookingRepository,
             IPassengerRepository _passengerRepository, IBagageRepository _bagageRepository, IContractPayRepository contractPayRepository,
             IAirPortCodeRepository _airPortCodeRepository, IWebHostEnvironment _webHostEnvironment, IAirlinesRepository _airlinesRepository, IAccountClientRepository _accountClientRepository,
             IHotelBookingRepositories _hotelBookingRepositories, IOtherBookingRepository otherBookingRepository, ITourRepository tourRepository, IAllCodeRepository allCodeRepository,
             IUserRepository userRepository, IVoucherRepository _voucherRepository, INotifyRepository _notifyRepository, IHotelBookingRoomExtraPackageRepository hotelBookingRoomExtraPackageRepository, IHotelBookingRoomRepository hotelBookingRoomRepository)
        {
            configuration = _configuration;
            clientRepository = _clientRepository;
            contactClientRepository = _contactClientRepository;
            flyBookingDetailRepository = _flyBookingDetailRepository;
            flightSegmentRepository = _flightSegmentRepository;
            orderRepository = _orderRepository;
            passengerRepository = _passengerRepository;
            bagageRepository = _bagageRepository;
            airPortCodeRepository = _airPortCodeRepository;
            webHostEnvironment = _webHostEnvironment;
            airlinesRepository = _airlinesRepository;
            accountClientRepository = _accountClientRepository;
            hotelBookingRepositories = _hotelBookingRepositories;

            _mail_service = new API_CORE.Controllers.MAIL.Base.MailService(configuration, _contactClientRepository, vinWonderBookingRepository, _clientRepository, _flyBookingDetailRepository,
                      _flightSegmentRepository, _orderRepository, _passengerRepository, _bagageRepository, _airPortCodeRepository, _webHostEnvironment, _airlinesRepository, _hotelBookingRepositories,
                      otherBookingRepository, tourRepository, allCodeRepository, userRepository, contractPayRepository, _voucherRepository, _notifyRepository, hotelBookingRoomExtraPackageRepository, hotelBookingRoomRepository);
        }

        [HttpPost("send-mail.json")]
        public async Task<ActionResult> sendMail(string token)
        {
            try
            {
                //var order = new Order()
                //{
                //    Amount = 1000000,
                //    ClientId = 9,
                //    OrderNo = "F4039714",
                //    OrderId = 1,
                //    ServiceType = 1,
                //    CreateTime = DateTime.Now,
                //    ContactClientId = 2,
                //    OrderStatus = 1,
                //    ContractId = 3,
                //    SmsContent = "Đặt hàng thành công"
                //};
                //var j_param = new Dictionary<string, string>
                //        {
                //            {"template_type","1" },
                //            {"object", JsonConvert.SerializeObject(order) }
                //        };
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:api_manual"]);

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    int template_type = Convert.ToInt32(objParr[0]["template_type"]);
                    string objectStr = objParr[0]["object"].ToString();
                    MailService mailService = new MailService();
                    MailMessage message = new MailMessage();
                    //message.To.Add("thang.nguyenvan1@vti.com.vn");

                    var subject = string.Empty;
                    if (template_type == TemplateMailType.ORDER_TEMPLATE)
                    {
                        subject = "THÔNG TIN ĐƠN HÀNG";
                        var orderInfo = JsonConvert.DeserializeObject<Order>(objectStr);
                        Client client = clientRepository.GetDetail(orderInfo.ClientId.Value);
                        FlyBookingDetail flyBookingDetail = flyBookingDetailRepository.GetByOrderId(orderInfo.OrderId);
                        List<FlyBookingDetail> flyBookingDetailList = flyBookingDetailRepository.GetListByOrderId(orderInfo.OrderId);
                        ContactClient contactClient = contactClientRepository.GetByClientId(orderInfo.ClientId.Value);
                        FlightSegment flightSegment = flightSegmentRepository.GetByFlyBookingDetailId(flyBookingDetail != null ? flyBookingDetail.Id : 0);
                        if (!string.IsNullOrEmpty(client.Email))
                            message.To.Add(client.Email);
                        message.Body = mailService.GetValueOrderTemplate(orderInfo, client, flyBookingDetail,
                            contactClient, flightSegment);
                    }
                    message.Subject = subject;

                    //config send email
                    message.IsBodyHtml = true;
                    message.From = new MailAddress(configuration["MAIL_CONFIG:FROM_MAIL"]);
                    string sendEmailsFrom = configuration["MAIL_CONFIG:USERNAME"];
                    string sendEmailsFromPassword = configuration["MAIL_CONFIG:PASSWORD"];
                    SmtpClient smtp = new SmtpClient(configuration["MAIL_CONFIG:HOST"],
                        Convert.ToInt32(configuration["MAIL_CONFIG:PORT"]));
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Credentials = new NetworkCredential(sendEmailsFrom, sendEmailsFromPassword);
                    smtp.Timeout = 20000;
                    smtp.Send(message);

                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Gửi mail thành công"
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
                LogHelper.InsertLogTelegram("sendMail - MailController: " + ex);
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }

        }

        [HttpPost("send-mail-v1.json")]
        public async Task<ActionResult> sendMail_V1(string token)
        {
            try
            {
                //var order = new Order()
                //{
                //    Amount = 1000000,
                //    ClientId = 9,
                //    OrderNo = "F4039714",
                //    OrderId = 1,
                //    ServiceType = 1,
                //    CreateTime = DateTime.Now,
                //    ContactClientId = 2,
                //    OrderStatus = 1,
                //    ContractId = 3,
                //    SmsContent = "Đặt hàng thành công"
                //};
                //var j_param = new Dictionary<string, string>
                //        {
                //            {"template_type","1" },
                //            {"object", JsonConvert.SerializeObject(order) }
                //        };
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:api_manual"]);

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    int template_type = Convert.ToInt32(objParr[0]["template_type"]);
                    string objectStr = objParr[0]["object"].ToString();
                    var orderInfo = JsonConvert.DeserializeObject<Order>(objectStr);
                    //var orderInfo = orderRepository.getDetail(846);


                    var resulstSendMail = _mail_service.sendMail(template_type, JsonConvert.SerializeObject(orderInfo), "");
                    //mailService.sendMail(template_type, objectStr, "XÁC NHẬN THANH TOÁN");
                    if (!resulstSendMail)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Gửi mail thất bại"
                        });
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Gửi mail thành công"
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
                LogHelper.InsertLogTelegram("sendMail - MailController: " + ex);
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }

        }

        [HttpPost("send-mail-reset-password.json")]
        public async Task<ActionResult> sendMailChangePass(string token)
        {
            try
            {
                //var j_param = new Dictionary<string, string>
                //        {
                //            {"template_type","2" },
                //            {"email","sontest123@gmail.com" },
                //        };
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:api_manual"]);
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    int template_type = Convert.ToInt32(objParr[0]["template_type"]);

                    var email = objParr[0]["email"].ToString();
                    var accountClient = accountClientRepository.GetAccountClientByUserName(email, template_type);
                    if (template_type == 2)
                    {
                        if (accountClient != null)
                        {
                            var client = clientRepository.GetDetail((long)accountClient.ClientId);
                            if (client == null)
                            {
                                return Ok(new
                                {
                                    status = (int)ResponseType.ERROR,
                                    msg = "không tìm thấy Email của tài khoản"
                                });
                            }
                        }
                        else
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.ERROR,
                                msg = "Tài khoản không tồn tại, tài khoản " + email + " không tồn tại"
                            });
                        }

                    }
                    if (accountClient == null)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Gửi mail thất bại"
                        });
                    }
                    var sw = new Stopwatch();

                    sw.Start();
                    var resulstSendMail = _mail_service.sendMailChangePassword(template_type, email, (long)accountClient.ClientId, "");
                    sw.Stop();
                    var toal_time = sw.ElapsedMilliseconds;//tổng thồi gian thực hiện ms
                    if (!resulstSendMail)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Gửi mail thất bại"
                        });
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Gửi mail thành công",
                        time = toal_time,
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
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("sendMailChangePass - MailController: " + ex + " token:" + token);
                return null;
            }
        }

        [HttpPost("vinwonder/send-mail.json")]
        public async Task<ActionResult> sendMailVinWord(string token)
        {
            try
            {

                //var j_param = new Dictionary<string, string>
                //        {
                //            {"orderid","12326" },
                //            {"email","mn13795@gmail.com" },

                //        };
                //var data_product = JsonConvert.SerializeObject(j_param);
                ////token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {

                    var OrderId = Convert.ToInt32(objParr[0]["orderid"]);
                    var email = objParr[0]["email"].ToString();
                    //  var BookingCode = objParr[0]["booking_code"].ToString();
                    var url = JsonConvert.DeserializeObject<List<string>>(objParr[0]["url"].ToString());

                    var resulstSendMail = await _mail_service.sendMailVinWordbookingTC(OrderId, email, "", url);


                    if (!resulstSendMail)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Gửi mail thất bại"
                        });
                    }

                    return Ok(new
                    {

                        status = (int)ResponseType.SUCCESS,
                        msg = "Gửi mail thành công",

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
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("sendMailChangePass - MailController: " + ex);
                return null;
            }
        }
        [HttpPost("hotel/send-mail.json")]
        public async Task<ActionResult> sendMailHotel(string token)
        {
            try
            {

                //var j_param = new Dictionary<string, string>
                //        {
                //            {"orderid","12758" },
                //            {"email","anhhieuk51@gmail.com" },

                //        };
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {

                    var OrderId = Convert.ToInt32(objParr[0]["orderid"]);
                    var email = objParr[0]["email"].ToString();

                    var resulstSendMail = await _mail_service.sendMailHotelBooking(OrderId, email);


                    if (!resulstSendMail)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Gửi mail thất bại"
                        });
                    }

                    return Ok(new
                    {

                        status = (int)ResponseType.SUCCESS,
                        msg = "Gửi mail thành công",

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
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("sendMailChangePass - MailController: " + ex);
                return null;
            }
        }

        [HttpPost("recruitment/send-mail.json")]
        public async Task<ActionResult> sendMailRecruitment(string name, string phone, string location, string area, string email, string note, string Path)
        {
            try
            {
                var data = new SystemLogMongDBRecruitmentModel()
                {
                    name = name,
                    phone = phone,
                    location = location,
                    area = area,
                    email = email,
                    note = note,
                    Path = Path,
                    CreatedTime=DateTime.Now,
                };
               await MongoDBSMSAccess.InsertLogRecruitment(configuration, data);
                var resulstSendMail = await _mail_service.sendMailRecruitment(name, phone, location, area, email, note, Path);

                if (!resulstSendMail)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Gửi mail thất bại"
                    });
                }

                return Ok(new
                {

                    status = (int)ResponseType.SUCCESS,
                    msg = "Gửi mail thành công",

                });

                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("sendMailChangePass - MailController: " + ex);
                return null;
            }
        }
    }
}
