using ENTITIES.ViewModels.APP;
using ENTITIES.ViewModels.Client;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories;
using REPOSITORIES.IRepositories.Clients;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using static ENTITIES.ViewModels.B2C.AccountB2CViewModel.AcconutViewModel;

namespace API_CORE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private IConfiguration configuration;
        private IClientRepository clientRepository;
        private IAccountClientRepository accountClientRepository;
        public ClientController(IConfiguration _configuration, IClientRepository _clientRepository, IAccountClientRepository _accountClientRepository)
        {
            configuration = _configuration;
            clientRepository = _clientRepository;
            accountClientRepository = _accountClientRepository;
        }

        [HttpPost("push-detail.json")]
        public async Task<ActionResult> pushClient(string token)
        {
            try
            {
                //var j_param = new ClientViewModel()
                //{
                //    client_map_id = 1,
                //    address = "HN ADAVIGO HCM",
                //    client_name = "HN ADAVIGO HCM",
                //    email = "HNADAVIGO@gmail.com",
                //    client_type = (int)ClientType.AGENT,
                //    gender = (int)Gender.MALE,
                //    join_date = DateTime.Now,
                //    phone = "0942066299",
                //    sale_map_id = 1,
                //    taxno = "22222222222222222"
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:api_manual"]);

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    var clientViewModel = JsonConvert.DeserializeObject<ClientViewModel>(objParr[0].ToString());
                    bool isUpdate = false;
                    var result = clientRepository.InsertOrUpdate(clientViewModel, out isUpdate);
                    var message = "Insert thất bại";
                    if (result == -1)
                    {
                        if (isUpdate)
                        {
                            message = "Update thất bại";
                        }
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = message
                        });
                    }
                    message = "Insert thành công";
                    if (isUpdate)
                    {
                        message = "Update thành công";
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = message
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
                LogHelper.InsertLogTelegram("pushClient - ClientController: " + ex);
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }

        }

        //[HttpPost("get-token.json")]
        //public async Task<ActionResult> GetToken(ClientViewModel clientViewModel)
        //{
        //    try
        //    {
        //        var data_product = JsonConvert.SerializeObject(clientViewModel);
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
        //        return Ok(new { status = ResponseTypeString.Fail, message = "error: " + ex.ToString(), token = "" });
        //    }

        //}


        [HttpPost("update-account.json")]
        public async Task<ActionResult> UpdataAccount(string token, int source_payment_type)
        {
            try
            {
                #region Test
                var j_param = new Dictionary<string, object>
                {
                    { "client_id","171"},
                    { "client_name","Phạm Anh Hiếu"},
                    { "gender","1"},
                    { "BirthdayDay","12"},
                    { "BirthdayMonth","3"},
                    { "BirthdayYear","2021"},
                    { "Wards","1"},//phường xã
                    { "District","1"},//quận huyện
                    { "Province","1"},//tỉnh thành
                    { "Address","Vin"},//địa chỉ
                    { "Phone","001354"},//
                    
                };
                var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:api_manual"]);
                #endregion
                JArray objParr = null;
                string private_token_key = source_payment_type == ((int)SourcePaymentType.b2c) ? configuration["DataBaseConfig:key_api:b2c"] : configuration["DataBaseConfig:key_api:b2b"];
                if (CommonHelper.GetParamWithKey(token, out objParr, private_token_key))
                {
                    var ClientB2C = new ClientB2CViewModel
                    {

                        account_client_id = Convert.ToInt32(objParr[0]["client_id"]),
                        gender = Convert.ToInt32(objParr[0]["gender"]),
                        UpdateTime = DateTime.Now,
                        client_name = objParr[0]["client_name"].ToString(),
                        Birthday = Convert.ToDateTime(objParr[0]["BirthdayDay"] + "/" + objParr[0]["BirthdayMonth"] + "/" + objParr[0]["BirthdayYear"]),
                        Address = objParr[0]["Address"].ToString(),
                        Wards = objParr[0]["Wards"].ToString(),
                        District = objParr[0]["District"].ToString(),
                        ProvinceId = objParr[0]["Province"].ToString(),
                        Phone = objParr[0]["Phone"].ToString(),
                    };
                    List<int> client_type = source_payment_type == ((int)SourcePaymentType.b2c) ? new List<int>() { (int)ClientType.CUSTOMER } : new List<int>() { (int)ClientType.TIER_1_AGENT, (int)ClientType.TIER_2_AGENT, (int)ClientType.TIER_3_AGENT, (int)ClientType.AGENT };
                    var result = clientRepository.Updata(ClientB2C, client_type);
                    if (result != null)
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Updata thành công",

                        });
                    else
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "Updata không thành công",

                        });
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
                LogHelper.InsertLogTelegram("GetToken - AccountController: " + ex);
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString(), token = "" });
            }
        }

        [HttpPost("reset-password.json")]
        public async Task<ActionResult> ResetPassword(string token)
        {
            try
            {
                #region Test
                //var j_param = new ResetPasswordModel()
                //{
                //    email = "anhhieu1@gmail.com",
                //    password = "123456",
                //    re_password = "123456",
                //};
                //var data_reset = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_reset, configuration["DataBaseConfig:key_api:api_manual"]);
                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    var userParams = JsonConvert.DeserializeObject<ResetPasswordModel>(objParr[0].ToString());
                    var accountClient = accountClientRepository.GetByUsername(userParams.email);
                    string error = string.Empty;

                    if (accountClient == null)
                        error = "Email không tồn tại trên hệ thống";
                    if (userParams.password.Length < 6)
                        error = "Mật khẩu phải có tối thiểu 6 kí tự và tối đa 16 kí tự";
                    if (userParams.re_password.Length < 6)
                        error = "Nhập lại mật khẩu phải có tối thiểu 6 kí tự và tối đa 16 kí tự";
                    if (userParams.password != userParams.re_password)
                        error = "Mật khẩu và nhập lại mật khẩu không khớp. Vui lòng kiểm tra lại";
                    if(userParams.email!=null&&(userParams.password == null|| userParams.re_password == null))
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Mật khẩu và nhập lại mật khẩu không không đươc bỏ trống",
                            data= userParams
                        });
                    }

                    if (!string.IsNullOrEmpty(error))
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = error,
                            data = userParams
                        });

                    var resultUpdate = accountClientRepository.UpdatePassword(userParams.email, userParams.password);
                    if (resultUpdate < 0)
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Đặt lại mật khẩu không thành công",
                            data = userParams
                        });

                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Đặt lại mật khẩu thành công",
                        data = userParams
                    });
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
                LogHelper.InsertLogTelegram("reset-password.json - ClientController: " + ex);
                return Ok(new { status = ResponseTypeString.Fail, msg = "error: " + ex.ToString() });
            }

        }
      
    }
}
