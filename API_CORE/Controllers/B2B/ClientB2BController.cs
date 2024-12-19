using Caching.RedisWorker;
using ENTITIES.Models;
using ENTITIES.ViewModels.B2B;
using ENTITIES.ViewModels.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories;
using REPOSITORIES.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.Controllers.B2B
{
    [Route("api/b2b/client")]
    [ApiController]
    public class ClientB2BController : Controller
    {
        private IConfiguration configuration;
        private IOrderRepository ordersRepository;
        private IClientRepository clientRepository;
        private IAccountB2BRepository accountB2BRepository;
        private IAccountRepository accountRepository;
        private IIdentifierServiceRepository identifierServiceRepository;
        private IAllCodeRepository allCodeRepository;
        private readonly RedisConn redisService;
        public ClientB2BController(IConfiguration _configuration, IOrderRepository _ordersRepository, RedisConn _redisService, IAccountRepository _accountRepository, IClientRepository _clientRepository, IAccountB2BRepository _accountB2BRepository, IIdentifierServiceRepository _identifierServiceRepository, IAllCodeRepository _allCodeRepository)
        {
            configuration = _configuration;
            redisService = _redisService;
            ordersRepository = _ordersRepository;
            accountRepository = _accountRepository;
            accountB2BRepository = _accountB2BRepository;
            identifierServiceRepository = _identifierServiceRepository;
            clientRepository = _clientRepository;
            allCodeRepository = _allCodeRepository;
        }

        [HttpPost("get-detail.json")]
        public async Task<ActionResult> GetClientDetail(string token)
        {
            try
            {
                #region Param Test:
                //var model = new
                //{
                //    account_client_id = "159",

                //};
                //token = CommonHelper.Encode(JsonConvert.SerializeObject(model), configuration["DataBaseConfig:key_api:b2b"]);
                #endregion

                JArray objParr = null;
                if (!CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Key không hợp lệ"
                    });
                }
                long account_client_id = Convert.ToInt64(objParr[0]["account_client_id"]);

                if (account_client_id <= 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Dữ liệu không hợp lệ"
                    });
                }
                var account_client = await accountB2BRepository.GetAccountClientById(account_client_id);
                if (account_client == null || account_client.Id <= 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "ID tài khoản không tồn tại"
                    });
                }
                var data = await accountB2BRepository.GetClientB2BDetailViewModel((long)account_client.ClientId);
                if (data != null)
                {
                    data.UserName = account_client.UserName;
                    data.Status = (int)account_client.Status;
                    data.GroupPermission = account_client.GroupPermission != null ?(int)account_client.GroupPermission:1;
                    data.Password = account_client.Password;
                    data.id = account_client.Id;
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Success",
                        data = data
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "FAILED",
                        data = data
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetClientDetail - ClientB2BController " + ex.ToString());
                return Ok(new { status = (int)ResponseType.ERROR, msg = "Error on Excution" });

            }
        }
        [HttpPost("update-detail.json")]
        public async Task<ActionResult> UpdateClientDetail(string token)
        {
            try
            {
                #region Param Test:
                //var model = new
                //{
                //    account_client_id = "159",
                //    name = "Cường",
                //    country = 0,
                //    provinced_id = "55",
                //    district_id = "45",
                //    ward_id = "33",
                //    address = "Địa chỉ New",
                //    account_number = "0123456789",
                //    account_name = "AccountName",
                //    bank_name = "BankName",
                //    indentifer_id ="ID-012345"
                //};
                //token = CommonHelper.Encode(JsonConvert.SerializeObject(model), configuration["DataBaseConfig:key_api:b2b"]);
                #endregion

                JArray objParr = null;
                if (!CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Key không hợp lệ"
                    });
                }
                long account_client_id = Convert.ToInt64(objParr[0]["account_client_id"]);
                string name = objParr[0]["name"].ToString();
                string provinced_id = objParr[0]["provinced_id"].ToString();
                string district_id = objParr[0]["district_id"].ToString();
                string ward_id = objParr[0]["ward_id"].ToString();
                string address = objParr[0]["address"].ToString();
                string account_number = objParr[0]["account_number"].ToString();
                string account_name = objParr[0]["account_name"].ToString();
                string bank_name = objParr[0]["bank_name"].ToString();
                string country = objParr[0]["country"].ToString();
                string indentifer_id = objParr[0]["indentifer_id"].ToString();

                if (account_client_id <= 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Dữ liệu không hợp lệ"
                    });
                }
                try
                {
                    var p = Convert.ToInt64(provinced_id);
                    p = Convert.ToInt64(district_id);
                    p = Convert.ToInt64(ward_id);
                }
                catch
                {

                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Dữ liệu không hợp lệ"
                    });
                }
                var account_client = await accountB2BRepository.GetAccountClientById(account_client_id);
                if (account_client == null || account_client.Id <= 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "ID tài khoản không tồn tại"
                    });
                }
                var client_detail = new ClientB2BDetailUpdateViewModel()
                {
                    account_name = account_name,
                    account_number = account_number,
                    address = address,
                    bank_name = bank_name,
                    country = country,
                    district_id = district_id,
                    name = name,
                    provinced_id = provinced_id,
                    ward_id = ward_id,
                    indentifer_no = indentifer_id,

                };
                var client_id = await accountB2BRepository.UpdateClientDetail(client_detail, (long)account_client.ClientId);
                if (client_id > 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Cập nhật thông tin thành công",
                        data = client_id
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Cập nhật thông tin thất bại, vui lòng kiểm tra lại",
                        data = client_id
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateClientDetail - ClientB2BController " + ex.ToString());
                return Ok(new { status = (int)ResponseType.ERROR, msg = "Error on Excution" });

            }
        }
        [HttpPost("inser-clientb2b.json")]
        public async Task<ActionResult> InsertClientB2B(string token)
        {
            try
            {
                #region Param Test:
                //var model = new
                //{
                //    account_client_id = "159",
                //    name = "Cường",
                //    country = 0,
                //    provinced_id = "55",
                //    district_id = "45",
                //    ward_id = "33",
                //    address = "Địa chỉ New",
                //    account_number = "0123456789",
                //    account_name = "AccountName",
                //    bank_name = "BankName",
                //    indentifer_id ="ID-012345"
                //};
                //token = CommonHelper.Encode(JsonConvert.SerializeObject(model), configuration["DataBaseConfig:key_api:b2b"]);
                #endregion

                JArray objParr = null;
                if (!CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Key không hợp lệ"
                    });
                }
                AccountB2BViewModel model = JsonConvert.DeserializeObject<AccountB2BViewModel>(objParr[0].ToString()); ;
                var account_client = await accountB2BRepository.GetAccountClientById(model.AccountId);
                var all_code = await allCodeRepository.GetAllCodeByType(AllCodeType.CLIENT_TYPE_ACCOUNT_NUMBER);
                var detail = all_code.FirstOrDefault(x => x.CodeValue == account_client.ClientType);
                var Number = Convert.ToInt32(detail.Description);
               var CountClient= clientRepository.CountClientByParentId((int)account_client.ClientId);
                if(CountClient >= Number)
                {
                    return Ok(new { status = (int)ResponseType.FAILED, msg = "Số lượng tài khoản của bạn đã đạt tối đa" });
                }
                model.ClientCode = await identifierServiceRepository.buildClientNo(8, model.ClientType);
                model.ParentId = (int)account_client.ClientId;
                if (model.Email != null)
                {
                    var is_check_email_exits =await accountB2BRepository.checkEmailExtisB2B(model.Email);
                    if (is_check_email_exits)
                    {
                        return Ok(new { status = (int)ResponseType.FAILED, msg = "Email này đã tồn tại trong hệ thống" });
                    }
                   
                }
                var is_check_username_exits = await accountB2BRepository.checkUserNameExtisB2B(model.UserName);
                if (is_check_username_exits)
                {
                    return Ok(new { status = (int)ResponseType.FAILED, msg = "Tên đăng nhập này đã tồn tại trong hệ thống" });
                }
                var client_id = await clientRepository.InsertClientB2b(model);
                if (client_id >= 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Thêm mới thành công",
                        data = client_id
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Thêm mới thất bại, vui lòng kiểm tra lại",

                    });
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertClient - ClientB2BController " + ex.ToString());
                return Ok(new { status = (int)ResponseType.ERROR, msg = "Error on Excution" });

            }
        }

    }
}
