using ENTITIES.APPModels;
using ENTITIES.ViewModels.APP.Client;
using ENTITIES.ViewModels.Client;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using WEB.API.Service.Queue;
using static ENTITIES.ViewModels.B2C.AccountB2CViewModel;

namespace API_CORE.Controllers.B2C
{
    [Route("api/b2c/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private IConfiguration configuration;
        private IClientRepository clientRepository;
        private IAccountB2CRepository accountB2CRepository;
        private IAccountRepository accountRepository;
        private IContactClientRepository contactClientRepository;
        private IFlyBookingDetailRepository flyBookingDetailRepository;
        private IFlightSegmentRepository flightSegmentRepository;
        private IOrderRepository orderRepository;
        private IPassengerRepository passengerRepository;
        private IBagageRepository bagageRepository;
        private IAirPortCodeRepository airPortCodeRepository;
        private IAirlinesRepository airlinesRepository;
        private IHotelBookingRepositories hotelBookingRepositories;
        private IWebHostEnvironment webHostEnvironment;
        private API_CORE.Controllers.MAIL.Base.MailService _mail_service;
        private IIdentifierServiceRepository identifierServiceRepository;
        private readonly INotifyRepository notifyRepository;
        private readonly WorkQueueClient _workQueueClient;

        public AccountController(IConfiguration _configuration, IClientRepository _clientRepository, IAccountB2CRepository _accountB2CRepository, IAccountRepository _accountRepository
            ,IContactClientRepository _contactClientRepository, IVinWonderBookingRepository vinWonderBookingRepository,
        IFlyBookingDetailRepository _flyBookingDetailRepository, IIdentifierServiceRepository _identifierServiceRepository,
             IFlightSegmentRepository _flightSegmentRepository, IOrderRepository _orderRepository,
             IPassengerRepository _passengerRepository, IBagageRepository _bagageRepository,IContractPayRepository contractPayRepository,
             IAirPortCodeRepository _airPortCodeRepository, IWebHostEnvironment _webHostEnvironment, IAirlinesRepository _airlinesRepository,
              IHotelBookingRepositories _hotelBookingRepositories, IOtherBookingRepository otherBookingRepository, ITourRepository tourRepository, IAllCodeRepository allCodeRepository, 
              IUserRepository userRepository, IVoucherRepository _voucherRepository, INotifyRepository _notifyRepository,IHotelBookingRoomExtraPackageRepository hotelBookingRoomExtraPackageRepository,IHotelBookingRoomRepository hotelBookingRoomRepository
            )
        {
            configuration = _configuration;
            clientRepository = _clientRepository;
            accountB2CRepository = _accountB2CRepository;
            accountRepository = _accountRepository;
            contactClientRepository = _contactClientRepository;
            flyBookingDetailRepository = _flyBookingDetailRepository;
            flightSegmentRepository = _flightSegmentRepository;
            orderRepository = _orderRepository;
            passengerRepository = _passengerRepository;
            bagageRepository = _bagageRepository;
            airPortCodeRepository = _airPortCodeRepository;
            webHostEnvironment = _webHostEnvironment;
            airlinesRepository = _airlinesRepository;
            hotelBookingRepositories = _hotelBookingRepositories;
            identifierServiceRepository = _identifierServiceRepository;
            _mail_service = new API_CORE.Controllers.MAIL.Base.MailService(configuration, _contactClientRepository,vinWonderBookingRepository ,_clientRepository, _flyBookingDetailRepository,
                      _flightSegmentRepository, _orderRepository, _passengerRepository, _bagageRepository, _airPortCodeRepository, _webHostEnvironment, _airlinesRepository, _hotelBookingRepositories,
                      otherBookingRepository, tourRepository, allCodeRepository, userRepository, contractPayRepository,_voucherRepository, _notifyRepository,hotelBookingRoomExtraPackageRepository,hotelBookingRoomRepository);
            _workQueueClient = new WorkQueueClient(configuration);
        }
        [EnableCors("MyApi")]
        [HttpPost("login.json")]
        public async Task<ActionResult> GetAccountB2C(string token)
        {
            try
            {
                #region Test
                var j_param = new LoginModel()
                {
                    UserName = "dinhthanhbinh2305@gmail.com",
                    Password = "Binh2305"
                };
                var data_product = JsonConvert.SerializeObject(j_param);
               //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion

                JArray objParr = null;
                bool response_queue = false;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    LoginModel login = JsonConvert.DeserializeObject<LoginModel>(objParr[0].ToString());
                    
                    if (string.IsNullOrEmpty(login.UserName))
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Vui lòng nhập tài khoản"
                        });
                    }
                    if (string.IsNullOrEmpty(login.Password))
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Vui lòng nhập mật khẩu"
                        });
                    }
                    var accountInfo = clientRepository.GetClientByUserAndPassword(login.UserName, login.Password, (int)ClientType.CUSTOMER,0);
                    if (accountInfo == null)
                    {
                        return Ok(new
                        {
                            data = accountInfo,
                            status = (int)ResponseType.FAILED,
                            msg = "Thông tin đăng nhập không đúng"
                        });
                    }
                    if (accountInfo.ClientType != (int)ClientType.CUSTOMER)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Thông tin đăng nhập không đúng"
                        });
                    }
                    if (accountInfo.Status == (int)UserConstant.UserStatus.INACTIVE)
                    {
                        accountInfo = null;
                        return Ok(new
                        {
                            data = accountInfo,
                            status = (int)ResponseType.FAILED,
                            msg = "Tài khoản đã bị khóa"
                        });
                    }
                    return Ok(new
                    {
                        data = accountInfo,
                        status = (int)ResponseType.SUCCESS,
                        msg = " Đăng nhập thành công"
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
                LogHelper.InsertLogTelegram("GetAccountB2C - ClientController: " + ex);
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }
        }
        [EnableCors("MyApi")]
        [HttpPost("insert.json")]
        public async Task<ActionResult> InsertAccountB2C(string token)
        {
            try
            {
                //#region Test
                //var j_param = new AccountB2C()
                //{
                //    ClientName = "5000000",
                //    Email = "thuytruong2006@gmail.com",
                //    Phone = "123",
                //    Password = "123",
                //    PasswordBackup = "123",
                //    isReceiverInfoEmail = false

                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                //#endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    var regex = new Regex(@"^[0-9]+$");
                    AccountB2C accountB2C = JsonConvert.DeserializeObject<AccountB2C>(objParr[0].ToString());
                    if (string.IsNullOrEmpty(accountB2C.ClientName))
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Tên khách hàng không được bỏ trống",
                        });
                    }
                    if (string.IsNullOrEmpty(accountB2C.Email))
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Email không được bỏ trống",
                        });
                    }
                    if (string.IsNullOrEmpty(accountB2C.Password))
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Password không được bỏ trống",
                        });
                    }
                    if (string.IsNullOrEmpty(accountB2C.Phone))
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Số điện thoại không được bỏ trống",
                        });
                    }
                    Regex regexemail = new Regex(@"^(\s*)([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)(\s*)$");
                    if (!regexemail.IsMatch(accountB2C.Email))
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "nhập đúng định dạng Email",
                        });
                    }
                    //cuonglv sua lai code
                    var is_check_email_exits = await accountB2CRepository.checkEmailExtisB2c(accountB2C.Email.ToLower());
                    if (is_check_email_exits)
                    {
                        return Ok(new { status = (int)ResponseType.FAILED, msg = "Email này đã tồn tại trong hệ thống" });
                    }
                    
                   
                    if (!regex.IsMatch(accountB2C.Phone))
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Phone chỉ nhập ký tự số",
                        });
                    }

                    if (accountB2C.Password.Length > 32)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Mật khẩu tối đa 32 ký tự",
                        });
                    }
                    if (!accountB2C.Password.Equals(accountB2C.PasswordBackup))
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Vui lòng nhập đúng mật khẩu đã đặt",
                        });
                    }

                    string backup_Password_md5 = accountB2C.Password;
                    string backup_PasswordBackup_md5 = accountB2C.PasswordBackup;
                    accountB2C.Password = backup_Password_md5;
                    accountB2C.PasswordBackup = backup_PasswordBackup_md5;
                    accountB2C.ClientCode = await identifierServiceRepository.buildClientNo(8, (int)ClientType.CUSTOMER);
                    //accountB2C.ClientType = (int)SourcePaymentType.b2c;
                    var AddAccount = accountB2CRepository.AddAccountB2C(accountB2C);
                    var clienrdetail = clientRepository.GetClientDetailByEmail(accountB2C.Email);
                    _workQueueClient.SyncES(clienrdetail.Id, configuration["DataBaseConfig:Elastic:SP:sp_GetClient"], configuration["DataBaseConfig:Elastic:Index:Client"], ProjectType.ADAVIGO_CMS);
                    int template_type = 1;

                   
                    var resulstSendMail = _mail_service.sendMailInsertUser(template_type, accountB2C.Email, "");
                        return Ok(new { status = (int)ResponseType.SUCCESS, msg = "Đăng ký thành công" });
                  
                }
                else
                {
                    return Ok(new { status = (int)ResponseType.FAILED, msg = "Đăng ký không thành công" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertAccountB2C - ClientController: " + ex);
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }
        }
        [HttpPost("change-password.json")]
        public async Task<ActionResult> ChangeAccountPassword(string token)
        {
            try
            {
                #region Param Test:
                //var model = new ClientInfoViewModel
                //{
                //    account_client_id = 173,
                //    password_old = "e10adc3949ba59abbe56e057f20f883e1",
                //    password_new = "e10adc3949ba59abbe56e057f20f883e",
                //    confirm_password_new = "e10adc3949ba59abbe56e057f20f883e"
                //};
                //token = CommonHelper.Encode(JsonConvert.SerializeObject(model), configuration["DataBaseConfig:key_api:b2c"]);
                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    var client_model = JsonConvert.DeserializeObject<ClientInfoViewModel>(objParr[0].ToString());
                    var account_client = await accountRepository.GetAccountClient(client_model.account_client_id);
                    if (client_model.password_old.Trim() == client_model.password_new.Trim() && client_model.password_new.Trim() == client_model.confirm_password_new.Trim())
                    {
                        return Ok(new { status = (int)ResponseType.FAILED, msg = "Mật khẩu cũ không được trùng với mật khẩu mới" });
                    }
                    else 
                    if(client_model.password_old==null || client_model.password_old.Trim()==""|| client_model.account_client_id <0 ||
                        client_model.password_new==null || client_model.password_new.Trim()==""|| client_model.confirm_password_new==null || client_model.confirm_password_new.Trim()==""
                        || client_model.password_new != client_model.confirm_password_new)
                    {
                        return Ok(new { status = (int)ResponseType.FAILED, msg = "Thông tin không chính xác, vui lòng kiểm tra lại" });

                    }else
                     if (account_client.Password != client_model.password_old)
                    {
                        return Ok(new { status = (int)ResponseType.FAILED, msg = "Mật khẩu cũ không chính xác" });
                    }
                    var address_result = await clientRepository.UpdateAccountPassword(client_model.password_old,client_model.password_new,client_model.confirm_password_new,client_model.account_client_id, (int)ClientType.CUSTOMER);
                    if (address_result > 0)
                    {
                        return Ok(new { status = (int)ResponseType.SUCCESS, msg = "Đổi mật khẩu thành công" });
                    }
                    else
                    {
                        return Ok(new { status = (int)ResponseType.FAILED, msg = "Đổi mật khẩu thất bại" });
                    }
                }
                else
                {
                    return Ok(new { status = (int)ResponseType.FAILED, msg = "Token invalid!" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ChangeAccountPassword - B2C.AccountController " + ex);
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });

            }
        }


        //[EnableCors("MyApi")]
        //[HttpPost("get-token.json")]
        //public async Task<ActionResult> GetToken(AccountB2C accountB2C)
        //{
        //    try
        //    {
        //        var j_param = new AccountB2C
        //        {
        //            ClientName = accountB2C.ClientName,
        //            Email = accountB2C.Email,
        //            Phone = accountB2C.Phone,
        //            Password = accountB2C.Password,
        //            PasswordBackup = accountB2C.PasswordBackup,
        //            isReceiverInfoEmail = accountB2C.isReceiverInfoEmail
        //        };
        //        var data_product = JsonConvert.SerializeObject(j_param);
        //        string token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
        //        return Ok(new
        //        {
        //            status = (int)ResponseType.SUCCESS,
        //            msg = "Get Token success",
        //            token = token
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.InsertLogTelegram("GetToken - AccountController: " + ex);
        //        return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString(), token = "" });
        //    }
        //}

        //[EnableCors("MyApi")]
        //[HttpPost("token.json")]
        //public async Task<ActionResult> GetTokenlogin(LoginModel LoginModel)
        //{
        //    try
        //    {
        //        var j_param = new LoginModel
        //        {
        //            UserName = LoginModel.UserName,
        //            Password = LoginModel.Password
        //        };
        //        var data_product = JsonConvert.SerializeObject(j_param);
        //        string token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:api_manual"]);
        //        return Ok(new
        //        {
        //            status = (int)ResponseType.SUCCESS,
        //            msg = "Get Token success",
        //            token = token
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.InsertLogTelegram("GetTokenlogin - AccountController: " + ex);
        //        return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString(), token = "" });
        //    }
        //}
        [HttpPost("update-infomation.json")]
        public async Task<ActionResult> UpdateAccountInfomation(string token)
        {
            try
            {
                #region Param Test:
                //var model = new ClientInfoViewModel
                //{
                //    client_id = 196,
                //    email = "abc1@gmail.com",
                //    phone = "0123456789",
                //    name = "Nguyen minh",
                //    birthday_year = 2023,
                //    birthday_month = 02,
                //    birthday_day = 13,
                //    province_id = 45,
                //    district_id = 192,
                //    ward_id = 475,
                //    address = "So 01 ngach 02",
                //};
                //token = CommonHelper.Encode(JsonConvert.SerializeObject(model), configuration["DataBaseConfig:key_api:b2c"]);
                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    var client_model = JsonConvert.DeserializeObject<ClientInfoViewModel>(objParr[0].ToString());
                    if ( client_model.client_id <= 0 || client_model.email == null || client_model.email.Trim() == ""
                        || client_model.phone == null || client_model.phone.Trim() == "" || client_model.name == null || client_model.name.Trim() == ""
                        || client_model.birthday_year <= 0 || client_model.birthday_month <= 0 || client_model.birthday_day <= 0
                        || client_model.province_id <= 0 || client_model.district_id <= 0 || client_model.ward_id <= 0
                        || client_model.address == null || client_model.address.Trim() == "")
                    {
                        return Ok(new { status = (int)ResponseType.FAILED, msg = "Thông tin không chính xác, vui lòng kiểm tra lại" });

                    }
                    var address_result = await clientRepository.UpdateAccountInfomationB2C(client_model);
                    if (address_result > 0)
                    {
                        return Ok(new { status = (int)ResponseType.SUCCESS, msg = "Cập nhật thông tin tài khoản thành công" });
                    }
                    else
                    {
                        return Ok(new { status = (int)ResponseType.FAILED, msg = "Cập nhật thông tin tài khoản thất bại" });
                    }
                }
                else
                {
                    return Ok(new { status = (int)ResponseType.FAILED, msg = "Token invalid!" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateAccountInfomation - B2B.AccountController " + ex);
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });

            }
        }

        [HttpPost("register-aff.json")]
        public async Task<ActionResult> registerAffiliate(string token)
        {
            try
            {
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    var client_id = Convert.ToInt64(objParr[0]["client_id"]);
                    var account_client = await accountRepository.GetAccountClient(client_id);

                 
                    string s_format = string.Format(String.Format("{0,5:00000}", (long)account_client.ClientId));
                    string ReferralId = "ADA"+ s_format;
                   
                    var result = await clientRepository.registerAffiliate((long)account_client.ClientId, ReferralId);
                    if (result > 0)
                    {
                        return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), msg = "Success", referralid= ReferralId });
                    }
                    else
                    {
                        string displayUrl = UriHelper.GetDisplayUrl(Request);
                        return Ok(new { status = ((int)ResponseType.FAILED).ToString(), msg = "update registerAffiliate error displayUrl = " + displayUrl });
                    }
                }
                else
                {
                    return Ok(new { status = ((int)ResponseType.EXISTS).ToString(), _token = token, msg = "token valid !!!" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("[API]--> register-aff.json - ClientController " + ex);
                return Ok(new
                {
                    status = ((int)ResponseType.FAILED).ToString(),
                    msg = ex.ToString()
                });
            }
        }
      

    }
}
