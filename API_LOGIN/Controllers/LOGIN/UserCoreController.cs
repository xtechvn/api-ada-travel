using API_LOGIN.Controllers.LOGIN.Base;
using API_LOGIN.ViewModels;
using ENTITIES.ViewModels.User;
using Google.Authenticator;
using Microsoft.AspNetCore.Cors;
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

namespace API_LOGIN.Controllers
{
    [Route("api/authent")]
    [ApiController]
    public class UserCoreController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private IUserCoreRepository _userCoreRepository;
        public UserCoreController(IConfiguration configuration, IUserCoreRepository userCoreRepository)
        {
            _configuration = configuration;
            _userCoreRepository = userCoreRepository;
        }

        /// <summary>
        /// Kiểm tra thông tin đăng nhập
        /// </summary>
        /// <returns></returns>
        [EnableCors("MyApi")]
        [HttpPost("login.json")]
        public async Task<ActionResult> login(string username, string password)
        {
            try
            {
                
                if ((string.IsNullOrEmpty(username) || (string.IsNullOrEmpty(password))))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.NOT_EXISTS,
                        msg = "Thông tin đăng nhập không đúng. Xin vui lòng thử lại"
                    });
                }
                password = EncodeHelpers.MD5Hash(password);
                var user_detail = await _userCoreRepository.checkAuthent(username, password);
                if (user_detail != null)
                {
                    string domain_permission = _configuration["DataBaseConfig:authent2MA:domain"];
                   
                    #region Môi trường QC ngắt nhập token. Mặc định vào TRAVEL
                    if (domain_permission.IndexOf("qc-be") >=0)
                    {
                        var user_core_service = new UserCoreService(_configuration);
                        string domain_redirect_travel = _configuration["DataBaseConfig:domain_cms_core:domain_travel"];
                        string url_redirect = await user_core_service.getUrlLoginByCompany(domain_redirect_travel, user_detail.Id, username, user_detail.Email);
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            is_active_authent_2fa = true,
                            url_redirect = url_redirect,
                            skip_token = true
                        });
                    }
                    #endregion

                    string private_key = "aDaviGo_Dai_viet" + DateTime.Now.Month;
                    var j_param = new Dictionary<string, string>
                        {
                            {"username",username},
                            {"password",password }
                        };
                    var data_product = JsonConvert.SerializeObject(j_param);
                    string token = CommonHelper.Encode(data_product, private_key);

                    // User đã active
                    if (user_detail.IsActive2Fa)
                    {
                        return Ok(new { status = (int)ResponseType.SUCCESS, skip_token = false, token = token, is_active_authent_2fa = true, comapy_type = user_detail.CompanyType }); // done step 1
                    }
                    else
                    {
                        // user chưa active. Gen qr code                        
                        string barcode_image_url = string.Empty;
                        string manual_entry_key = string.Empty;

                        string UserUniqueKey = (username + _configuration["DataBaseConfig:authent2MA:google_auth_key"]);

                        //Two Factor Authentication Setup
                        var TwoFacAuth = new TwoFactorAuthenticator();
                        var setupInfo = TwoFacAuth.GenerateSetupCode(_configuration["DataBaseConfig:authent2MA:domain"], username, CommonHelper.ConvertSecretToBytes(UserUniqueKey, false), 50);
                        barcode_image_url = setupInfo.QrCodeSetupImageUrl;
                        manual_entry_key = setupInfo.ManualEntryKey;
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            is_active_authent_2fa = false,
                            barcode_image_url = barcode_image_url,
                            manual_entry_key = manual_entry_key,
                            token = token,
                            comapy_type = user_detail.CompanyType,
                            skip_token = false
                        });
                    }
                }
                else
                {
                    return Ok(new { status = (int)ResponseType.FAILED, msg = "Thông tin đăng nhập không đúng. Xin vui lòng thử lại" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UserCoreController - login: " + ex.ToString());
                return Ok(new { status = (int)ResponseType.ERROR, msg = "Thông tin đăng nhập không đúng. Xin vui lòng thử lại" });
            }
        }

        /// <summary>
        /// Lấy ra thông tin chi tiết
        /// </summary>
        /// <returns></returns>
        [EnableCors("MyApi")]
        [HttpPost("get-detail.json")]
        public async Task<ActionResult> getDetail(string token)
        {
            try
            {
                JArray objParr = null;
                #region Test

                var j_param = new Dictionary<string, string>
                {
                    {"user_id", "18"},
                    {"username","" },
                    {"email","" }
                };

                var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, _configuration["databaseconfig:key_api:api_manual"]);
                #endregion

                if (CommonHelper.GetParamWithKey(token, out objParr, _configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    long user_id = Convert.ToInt64(objParr[0]["user_id"]);
                    string username = (objParr[0]["username"]).ToString();
                    string email = (objParr[0]["email"]).ToString();

                    var user_detail = await _userCoreRepository.getDetail(user_id, username, email);

                    return Ok(new
                    {
                        status = user_detail.Count > 0 ? ((int)ResponseType.SUCCESS) : (int)ResponseType.EMPTY,
                        data = user_detail
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = ((int)ResponseType.ERROR)
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UserCoreController - getDetail: " + ex.ToString());
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "error: " + ex.ToString() });
            }
        }

        /// <summary>
        /// Tạo mới hoặc sửa thông tin user bên db user
        /// 0: tạo mới | 1: sửa
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [EnableCors("MyApi")]
        [HttpPost("upsert_user.json")]
        public async Task<ActionResult> upsertUser(string token)
        {
            try
            {
                #region Test
                var j_param = new UserMasterViewModel()
                {
                    Id = 7,
                    UserName = "cuonglv2",
                    FullName = "Lê Văn Cường",
                    Password = "e10adc3949ba59abbe56e057f20f883e",
                    ResetPassword = "e10adc3949ba59abbe56e057f20f883e",
                    Phone = "0942066299",
                    BirthDay = DateTime.Now,
                    Gender = 1,
                    Email = "cuonglv8@fpt.com.vn",
                    Avata = "",
                    Address = "Số 14 ngõ ao sen 5 Hà Đông - Hà Nội",
                    Status = 0,// 0: BÌnh thường
                    Note = "User được khởi tạo từ công ty {detect theo comapny Type}",
                    CreatedBy = 1, // id của user nào tạo
                    ModifiedBy = 1, // id của user nào update
                    CompanyType = "0,1", // loại công ty. 0: Travel | 1: Phú Quốc | 2: Đại Việt
                    CompanyDeactiveType = "2"
                };
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, _configuration["DataBaseConfig:key_api:api_manual"]);
                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, _configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    var user_detail = JsonConvert.DeserializeObject<UserMasterViewModel>(objParr[0].ToString());
                    var user_result = await _userCoreRepository.upsertUser(user_detail);

                    // Tự động insert User vào db Company tương ứng theo Company Type
                    //var arr_company = user_detail.CompanyType.Split(",");
                    //var arr_company_deactive = user_detail.CompanyDeactiveType.Split(",");
                    //foreach (var com_type in arr_company)
                    //{

                    //    _userCorePQRepository.upsertUser(user_detail);
                    //}

                    if (user_result <= 0)
                    {
                        return Ok(new { status = (int)ResponseType.FAILED, msg = "Cập nhật thất bại", user_id = user_result });

                    }



                    return Ok(new { status = (int)ResponseType.SUCCESS, msg = "Cập nhật thành công", user_id = user_result });
                }
                else
                {
                    return Ok(new { status = (int)ResponseType.ERROR, msg = "Sai key" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UserCoreController - upsert_user.json: " + ex.ToString());
                throw;
            }
        }

        [EnableCors("MyApi")]
        [HttpPost("change_password.json")]
        public async Task<ActionResult> change_password(string token)
        {
            try
            {
                #region Test

                var j_param = new Dictionary<string, string>
                {
                    {"username", "cuonglv"},
                    {"password","e10adc3949ba59abbe56e057f20f883e" }
                };
                var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, _configuration["DataBaseConfig:key_api:api_manual"]);
                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, _configuration["DataBaseConfig:key_api:api_manual"]))
                {

                    string username = (objParr[0]["username"]).ToString();
                    string password = (objParr[0]["password"]).ToString();

                    var user_id = await _userCoreRepository.changePassword(username, password);

                    if (user_id <= 0)
                    {
                        return Ok(new { status = (int)ResponseType.FAILED, msg = "Cập nhật thất bại", user_id = user_id });

                    }
                    return Ok(new { status = (int)ResponseType.SUCCESS, msg = "Cập nhật mật khẩu thành công", user_id = user_id });
                }
                else
                {
                    return Ok(new { status = (int)ResponseType.ERROR, msg = "Sai key" });
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// Xác thực 2 lớp google Authent
        /// </summary>
        /// <returns></returns>
        [EnableCors("MyApi")]
        [HttpPost("verify_code.json")]
        public async Task<ActionResult> verifyCode(string code_verify, string token_authent)
        {
            try
            {
                string private_key = "aDaviGo_Dai_viet" + DateTime.Now.Month;
                JArray objParr = null;
                string domain_redirect = string.Empty;
                string url_redirect = string.Empty;
                #region Test

                //var j_param = new Dictionary<string, string>
                //    {
                //        {"username", "cuonglv"},
                //        {"password","e10adc3949ba59abbe56e057f20f883e" }
                //    };

                //var data_product = JsonConvert.SerializeObject(j_param);
                //string token = CommonHelper.Encode(data_product, _configuration["databaseconfig:key_api:api_manual"]);
                #endregion
                if (CommonHelper.GetParamWithKey(token_authent, out objParr, private_key))
                {
                    var TwoFacAuth = new TwoFactorAuthenticator();
                    string username = (objParr[0]["username"]).ToString();
                    string password = (objParr[0]["password"]).ToString();
                    // string domain = _configuration["DataBaseConfig:authent2MA:domain"];
                    string UserUniqueKey = (username + _configuration["DataBaseConfig:authent2MA:google_auth_key"]);

                    bool isValid = TwoFacAuth.ValidateTwoFactorPIN(UserUniqueKey, code_verify, false);
                    if (isValid)
                    {
                        //HttpCookie TwoFCookie = new HttpCookie("TwoFCookie");
                        //string UserCode = Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes(UserUniqueKey)));
                        //Session["IsValidTwoFactorAuthentication"] = true;

                        var user_detail = await _userCoreRepository.checkAuthent(username, password);
                        if (user_detail != null)
                        {
                            var arr_company = user_detail.CompanyType.Split(",").Select(int.Parse).ToList(); // Chuyển chuỗi thành danh sách int

                            var obj_company = new List<CompanyViewModel>()
                            {
                                new CompanyViewModel { id = 0, name = "Adavigo Travel", domain = _configuration["DataBaseConfig:domain_cms_core:domain_travel"] },
                                new CompanyViewModel { id = 1, name = "Adavigo Phú Quốc", domain = _configuration["DataBaseConfig:domain_cms_core:domain_phu_quoc"] },
                                new CompanyViewModel { id = 2, name = "Adavigo Đại Việt", domain = _configuration["DataBaseConfig:domain_cms_core:domain_dai_viet"] }
                            };
                            var filteredCompanies = obj_company.Where(company => arr_company.Contains(company.id)).ToList();
                            if (filteredCompanies.Count == 1)
                            {
                                domain_redirect = filteredCompanies[0].domain;

                                var user_core_service = new UserCoreService(_configuration);
                                url_redirect = await user_core_service.getUrlLoginByCompany(domain_redirect, user_detail.Id, username, user_detail.Email);
                            }
                            #region Mã hóa token authent khi xác thực 2 lớp thành công                            
                            var j_param = new Dictionary<string, string>
                            {
                                {"username",username},
                                {"password",password },
                                {"is_verify_2MA", "1" } // 1: xác thực thành công
                            };
                            var data_product = JsonConvert.SerializeObject(j_param);
                            string token = CommonHelper.Encode(data_product, private_key);
                            #endregion

                            // Cập nhật case load mã xác thực lần đầu để lần sau ko bật lên nữa
                            if (!user_detail.IsActive2Fa)
                            {
                                await _userCoreRepository.updateActive2Fa(user_detail.Id);
                            }

                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                msg = "xac thuc token thanh cong",
                                company_list = filteredCompanies, // Những công ty mà User này được truy cập vào
                                token_2ma = token,
                                url_redirect = url_redirect
                            });
                        }
                        else
                        {
                            LogHelper.InsertLogTelegram("UserCoreController - verifyCode: user nhap sai mat khau trong luc xac thuc token. kha nang cao la hacker");
                            return Ok(new { status = (int)ResponseType.EMPTY, msg = "Mã xác thực không đúng" });
                        }
                    }
                }
                return Ok(new { status = (int)ResponseType.EMPTY, msg = "Mã xác thực không đúng" });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UserCoreController - verifyCode: " + ex.ToString());
                return Ok(new { status = (int)ResponseType.ERROR, msg = "Mã xác thực không đúng" });
            }
        }

        /// <summary>
        /// Lựa chọn công ty nếu có
        /// </summary>
        /// <returns></returns>
        //[HttpPost("authent/location.json")]
        //public async Task<ActionResult> location()
        //{
        //    return Ok(new
        //    {
        //        status = is_public_noti ? (int)ResponseType.SUCCESS : (int)ResponseType.EMPTY,
        //        msg = is_public_noti ? "Thông tin notify của user_id" + user_id + " đã public thành công" : "Hiện tại không có notify nào của user này"
        //    });
        //}
        [EnableCors("MyApi")]
        [HttpPost("get-qr-core.json")]
        public ActionResult genQrCodeByUser(string token)
        {
            try
            {
                JArray objParr;
                #region Test
                //var j_param = new Dictionary<string, string>
                //{
                //    {"username","cuonglv" }
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                // token = CommonHelper.Encode(data_product, _configuration["DataBaseConfig:key_api:api_manual"]);
                #endregion

                if (CommonHelper.GetParamWithKey(token, out objParr, _configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    string username = objParr[0]["username"].ToString().Replace("\"", "");
                    string barcode_image_url = string.Empty;
                    string manual_entry_key = string.Empty;
                    string UserUniqueKey = (username + _configuration["DataBaseConfig:authent2MA:google_auth_key"]);
                    //Two Factor Authentication Setup
                    var TwoFacAuth = new TwoFactorAuthenticator();
                    var setupInfo = TwoFacAuth.GenerateSetupCode(_configuration["DataBaseConfig:authent2MA:domain"], username, CommonHelper.ConvertSecretToBytes(UserUniqueKey, false), 50);

                    barcode_image_url = setupInfo.QrCodeSetupImageUrl;
                    manual_entry_key = setupInfo.ManualEntryKey;

                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        barcode_image_url = barcode_image_url,
                        manual_entry_key = manual_entry_key
                    });

                }
                return Ok(new { status = (int)ResponseType.ERROR, msg = "Sai key" });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UserCoreController - genQrCodeByUser: " + ex.ToString());
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "error: " + ex.ToString() });
            }
        }

        [EnableCors("MyApi")]
        [HttpPost("send_company_choose.json")]
        public async Task<ActionResult> sendCompanyChoose(string token, int company_choose)
        {
            try
            {
                JArray objParr;
                string url_redirect = String.Empty;
                string domain_redirect = String.Empty;
                string private_key = "aDaviGo_Dai_viet" + DateTime.Now.Month;

                if (CommonHelper.GetParamWithKey(token, out objParr, private_key))
                {
                    string user_name = objParr[0]["username"].ToString().Replace("\"", "");
                    var is_verify_2MA = Convert.ToInt16(objParr[0]["is_verify_2MA"].ToString().Replace("\"", ""));
                    if (is_verify_2MA == 1)
                    {
                        var user_core_service = new UserCoreService(_configuration);
                        var user_detail = await _userCoreRepository.getDetail(-1, user_name, "");

                        switch (company_choose)
                        {
                            case CompanyType.TRAVEL:
                                domain_redirect = _configuration["DataBaseConfig:domain_cms_core:domain_travel"];
                                break;
                            case CompanyType.PHU_QUOC:
                                domain_redirect = _configuration["DataBaseConfig:domain_cms_core:domain_phu_quoc"];
                                break;
                            case CompanyType.DAI_VIET:
                                domain_redirect = _configuration["DataBaseConfig:domain_cms_core:domain_dai_viet"];
                                break;
                            default:
                                return Ok(new { status = (int)ResponseType.EMPTY, msg = "Domain khong xac dinh" });
                        }
                        url_redirect = await user_core_service.getUrlLoginByCompany(domain_redirect, user_detail[0].Id, user_name, user_detail[0].Email);
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            url_redirect = url_redirect
                        });
                    }
                }
                return Ok(new { status = (int)ResponseType.ERROR, msg = "Xác thực thất bại" });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UserCoreController - sendCompanyChoose: " + ex.ToString());
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "error: " + ex.ToString() });
            }
        }

      

    }
}
