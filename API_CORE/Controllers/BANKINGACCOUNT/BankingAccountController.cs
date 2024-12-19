using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories;
using REPOSITORIES.IRepositories.BankingAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.Controllers.BANKINGACCOUNT
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankingAccountController : ControllerBase
    {
        private IConfiguration configuration;
        private IAccountRepository accountRepository;
        private IBankingAccountRepository bankingAccountRepository;
        public BankingAccountController(IConfiguration _configuration, IBankingAccountRepository _bankingAccountRepository, IAccountRepository _accountRepository)
        {
            bankingAccountRepository = _bankingAccountRepository;
            configuration = _configuration;
            accountRepository = _accountRepository;
        }
        [HttpPost("get-by-id.json")]
        public async Task<ActionResult> Getbyid(string token)
        {
            try
            {
                #region Test
                var j_param = new Dictionary<string, object>
                {
                    {"id", "19"},

                };
                var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    //-- Convert AccountClientId to ClientId
                    int id = (int)objParr[0]["id"];

                    var data = bankingAccountRepository.GetById(id);

                    if (data != null)
                    {

                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Successfully ",
                            data = data,
                        });
                    }
                }
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "ERROR "
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Getbyid- BankingAccountController" + ex.ToString());
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "Error On Excution. Vui lòng liên hệ IT" });
            }
        }
        [HttpPost("Insert.json")]
        public async Task<ActionResult> Insert(string token)
        {
            try
            {
                #region Test
                var j_param = new Dictionary<string, object>
                {
                    {"Id", "148"},
                    {"BankId", "148"},
                    {"AccountNumber", "148"},
                    {"AccountName", "148"},
                    {"Branch", "148"},
                    {"ClientId", "148"},

                };
                var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    //-- Convert AccountClientId to ClientId

                    var model = JsonConvert.DeserializeObject<ENTITIES.Models.BankingAccount>(objParr[0].ToString());
                    var account_client = await accountRepository.GetAccountClient((long)model.ClientId);
                    model.ClientId = Convert.ToInt32(account_client.ClientId);
                    model.CreatedBy = model.ClientId;
                    var data = bankingAccountRepository.InsertBankingAccount(model);

                    if (data > 0)
                    {

                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Successfully ",
                            data = data,
                        });
                    }
                }
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "ERROR "
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Getbyid- BankingAccountController" + ex.ToString());
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "Error On Excution. Vui lòng liên hệ IT" });
            }
        }
        [HttpPost("Update.json")]
        public async Task<ActionResult> Update(string token)
        {
            try
            {
                #region Test
                var j_param = new Dictionary<string, object>
                {
                    {"id", "148"},

                };
                var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    //-- Convert AccountClientId to ClientId

                    var model = JsonConvert.DeserializeObject<ENTITIES.Models.BankingAccount>(objParr[0].ToString());
                    var account_client = await accountRepository.GetAccountClient((long)model.ClientId);
                    model.ClientId = Convert.ToInt32(account_client.ClientId);
                    model.UpdatedBy = model.ClientId;
                    var data = bankingAccountRepository.UpdateBankingAccount(model);

                    if (data > 0)
                    {

                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Successfully ",
                            data = data,
                        });
                    }
                }
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "ERROR "
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Getbyid- BankingAccountController" + ex.ToString());
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "Error On Excution. Vui lòng liên hệ IT" });
            }
        }
    }
}
