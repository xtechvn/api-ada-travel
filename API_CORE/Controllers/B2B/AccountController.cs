using ENTITIES.APPModels;
using ENTITIES.ViewModels.Client;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.Controllers.B2B
{
    [Route("api/b2b/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private IConfiguration configuration;
        private IClientRepository clientRepository;
        private IAccountB2BRepository accountB2BRepository;

        public AccountController(IConfiguration _configuration, IClientRepository _clientRepository, IAccountB2BRepository _accountB2BRepository)
        {
            configuration = _configuration;
            clientRepository = _clientRepository;
            accountB2BRepository = _accountB2BRepository;
        }

        [EnableCors("MyApi")]
        [HttpPost("check-login.json")]
        public async Task<ActionResult> GetAccountB2B(string token)
        {
            try
            {
                #region Test
                //var j_param = new LoginModel()
                //{
                //    UserName = "binhdt@adavigo.com",
                //    Password = "e10adc3949ba59abbe56e057f20f883e"
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:api_manual"]);
                #endregion

                JArray objParr = null;
                bool response_queue = false;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
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
                    var accountInfo = clientRepository.GetClientByUserAndPassword(login.UserName, login.Password, (int)ClientType.CUSTOMER, 1);
                    if (accountInfo == null)
                    {
                        return Ok(new
                        {
                            data = accountInfo,
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
                LogHelper.InsertLogTelegram("GetAccountB2B - ClientController: " + ex);
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }

        }


        //[EnableCors("MyApi")]
        //[HttpPost("get-token-login.json")]
        //public async Task<ActionResult> GetTokenB2B(LoginModel loginModel)
        //{
        //    try
        //    {
        //        var data_product = JsonConvert.SerializeObject(loginModel);
        //        string token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:api_manual"]);
        //        return Ok(new
        //        {
        //            status = ResponseTypeString.Success,
        //            message = "Get Token success",
        //            token = token
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.InsertLogTelegram("GetTokenB2B - ClientController: " + ex);
        //        return Ok(new { status = ResponseTypeString.Fail, message = "error: " + ex.ToString(), token = "" });
        //    }

        //}
        [HttpPost("change-password.json")]
        public async Task<ActionResult> ChangeAccountPassword(string token)
        {
            try
            {
                #region Param Test:
                //var model = new ClientInfoViewModel
                //{
                //    account_client_id = 173,
                //    password_old = "e10adc3949ba59abbe56e057f20f883e",
                //    password_new = "e10adc3949ba59abbe56e057f20f883e1",
                //    client_type = 2,
                //    confirm_password_new = "e10adc3949ba59abbe56e057f20f883e1"
                //};
                //token = CommonHelper.Encode(JsonConvert.SerializeObject(model), configuration["DataBaseConfig:key_api:b2b"]);
                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    var client_model = JsonConvert.DeserializeObject<ClientInfoViewModel>(objParr[0].ToString());
                    if (client_model.password_old.Trim() == client_model.password_new.Trim() && client_model.password_new.Trim() == client_model.confirm_password_new.Trim())
                    {
                        return Ok(new { status = (int)ResponseType.FAILED, msg = "Mật khẩu cũ không được trùng với mật khẩu mới" });
                    }
                    var account_client = await accountB2BRepository.GetAccountClientById(client_model.account_client_id);
                    if (account_client == null || client_model.password_old.Trim() != account_client.Password.Trim())
                    {
                        return Ok(new { status = (int)ResponseType.FAILED, msg = "Mật khẩu cũ không chính xác" });
                    }
                    else if (client_model.password_old == null || client_model.password_old.Trim() == "" || client_model.account_client_id < 0 ||
                        client_model.password_new == null || client_model.password_new.Trim() == "" || client_model.confirm_password_new == null || client_model.confirm_password_new.Trim() == ""
                        || client_model.password_new != client_model.confirm_password_new || client_model.client_type <= 0)
                    {
                        return Ok(new { status = (int)ResponseType.FAILED, msg = "Thông tin không chính xác, vui lòng kiểm tra lại" });

                    }
                    var address_result = await clientRepository.UpdateAccountPassword(client_model.password_old, client_model.password_new, client_model.confirm_password_new, client_model.account_client_id, client_model.client_type);
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
                LogHelper.InsertLogTelegram("ChangeAccountPassword - B2B.AccountController " + ex);
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });

            }
        }
        /*
        [HttpPost("update-infomation.json")]
        public async Task<ActionResult> UpdateAccountInfomation(string token, int source_payment_type)
        {
            try
            {
                #region Param Test:
                //var model = new ClientInfoViewModel
                //{
                //    client_id = 196,
                //    account_client_id = 173,
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
                //token = CommonHelper.Encode(JsonConvert.SerializeObject(model), configuration["DataBaseConfig:key_api:b2b"]);
                #endregion

                #region check key
                JArray objParr = null;
                string private_token_key = source_payment_type == ((int)SourcePaymentType.b2c) ? configuration["DataBaseConfig:key_api:b2c"] : configuration["DataBaseConfig:key_api:b2b"];
                if (!CommonHelper.GetParamWithKey(token, out objParr, private_token_key))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key không hợp lệ"
                    });
                }

                #endregion
                var client_model = JsonConvert.DeserializeObject<ClientInfoViewModel>(objParr[0].ToString());
                if (client_model.account_client_id <= 0 || client_model.client_id <= 0 || client_model.email == null || client_model.email.Trim() == ""
                    || client_model.phone == null || client_model.phone.Trim() == "" || client_model.name == null || client_model.name.Trim() == ""
                    || client_model.birthday_year <= 0 || client_model.birthday_month <= 0 || client_model.birthday_day <= 0
                    || client_model.province_id <= 0 || client_model.district_id <= 0 || client_model.ward_id <= 0
                    || client_model.address == null || client_model.address.Trim() == "")
                {
                    return Ok(new { status = (int)ResponseType.FAILED, msg = "Thông tin không chính xác, vui lòng kiểm tra lại" });

                }
                var address_result = await clientRepository.UpdateAccountInfomationB2B(client_model);
                if (address_result > 0)
                {
                    return Ok(new { status = (int)ResponseType.SUCCESS, msg = "Cập nhật thông tin tài khoản thành công" });
                }
                else
                {
                    return Ok(new { status = (int)ResponseType.FAILED, msg = "Cập nhật thông tin tài khoản thất bại" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateAccountInfomation - B2B.AccountController " + ex);
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });

            }
        }*/

        [HttpPost("list-account-client.json")]
        public async Task<ActionResult> GetListAccountClient(string token)
        {
            try
            {
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    var client_model = JsonConvert.DeserializeObject<ClientInfoViewModel>(objParr[0].ToString());
                    string TextSearch = objParr[0]["TextSearch"].ToString();
                    long Status = Convert.ToInt32(objParr[0]["Status"].ToString());
                    long AccountId = Convert.ToInt32(objParr[0]["ClientId"].ToString());
                    long GroupPermission = Convert.ToInt32(objParr[0]["GroupPermission"].ToString());
                    long PageIndex = Convert.ToInt32(objParr[0]["PageIndex"].ToString());
                    long PageSize = Convert.ToInt32(objParr[0]["PageSize"].ToString());
                    var account_client = await accountB2BRepository.GetAccountClientById(AccountId);
                    var data = await accountB2BRepository.GetListAccountClient((long)account_client.ClientId, GroupPermission, Status, PageIndex, PageSize, TextSearch);

                    if (data != null && data.Count > 0)
                    {
                        return Ok(new { status = (int)ResponseType.SUCCESS, data = data });
                    }
                    return Ok(new { status = (int)ResponseType.FAILED, msg = "data = NULL" });
                }
                else
                {
                    return Ok(new { status = (int)ResponseType.FAILED, msg = "Token invalid!" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListAccountClient - B2B.AccountController " + ex);
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }
        }
        [HttpPost("update-account-client.json")]
        public async Task<ActionResult> UpdateAccountClient(string token)
        {
            try
            {
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    var AccountClient_model = JsonConvert.DeserializeObject<AccountClientB2BViewModel>(objParr[0].ToString());

                    var account_client = await accountB2BRepository.GetAccountClientById(AccountClient_model.id);
                    if(AccountClient_model.Email != null)
                    {
                        var is_check_Email_Client = await accountB2BRepository.checkEmailClient(AccountClient_model.Email, (long)account_client.ClientId);
                        if (is_check_Email_Client)
                        {
                            return Ok(new { status = (int)ResponseType.FAILED, msg = "Email đã tồn tại" });
                        }
                    }
                    
                    var data = await accountB2BRepository.UpdataAccountClientB2B(AccountClient_model);

                    if (data >= 0)
                    {
                        return Ok(new { status = (int)ResponseType.SUCCESS, msg = "data = Cập nhật thành công", data = data });
                    }
                    return Ok(new { status = (int)ResponseType.FAILED, msg = "data = NULL" });
                }
                else
                {
                    return Ok(new { status = (int)ResponseType.FAILED, msg = "Token invalid!" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListAccountClient - B2B.AccountController " + ex);
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }
        }
    }

}


