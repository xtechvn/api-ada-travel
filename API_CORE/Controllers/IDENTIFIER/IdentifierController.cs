using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.Controllers.IDENTIFIER
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentifierController : ControllerBase
    {
        private IConfiguration configuration;
        private IIdentifierServiceRepository identifierServiceRepository;
        public IdentifierController(IConfiguration _configuration, IIdentifierServiceRepository _identifierServiceRepository)
        {
            configuration = _configuration;
            identifierServiceRepository = _identifierServiceRepository;
        }

        [HttpPost("get-code.json")]
        public async Task<ActionResult> getIdentifier(string token)
        {
            try
            {
                string private_token_key = configuration["DataBaseConfig:key_api:api_manual"];

                #region TEST
                JArray objParr = null;
                JObject jsonObject = new JObject(
                    new JProperty("code_type", "8"),
                    new JProperty("client_type", "2")
                );
                // PHIEU CHI
               // JObject jsonObject = new JObject(
               //    new JProperty("code_type", "2")
               //);
                var j_param = new Dictionary<string, object>
                 {
                     { "key",jsonObject}
                 };
                var data_product = JsonConvert.SerializeObject(j_param);
              //  token = CommonHelper.Encode(data_product, private_token_key);
                #endregion

                if (CommonHelper.GetParamWithKey(token, out objParr, private_token_key))
                {
                    string code = string.Empty;
                    var j_obj = JObject.Parse(objParr[0]["key"].ToString());
                    int code_type = Convert.ToInt32(j_obj["code_type"]);

                    int service_type = -1;
                    switch (code_type)
                    {
                        case IdentifierType.SERVICE:
                            service_type = Convert.ToInt32(j_obj["service_type"]);
                            code = await identifierServiceRepository.buildServiceNo(service_type);
                            break;
                        case IdentifierType.PHIEU_THU:                            
                            code = await identifierServiceRepository.buildContractPay();
                            break;
                        case IdentifierType.CONTRACT:
                            code = await identifierServiceRepository.buildContractNo();
                            break;
                        case IdentifierType.PHIEU_CHI:
                            code = await identifierServiceRepository.BuildPaymentVoucher();
                            break;
                        case IdentifierType.PHIEU_YEU_CAU_CHI:
                            code = await identifierServiceRepository.BuildPaymentRequest();
                            break;
                        case IdentifierType.PHIEU_YEU_CAU_XUAT_HOA_DON:
                            code = await identifierServiceRepository.BuildExportBillNo();
                            break;
                        case IdentifierType.HOA_DON:
                            code = await identifierServiceRepository.BuildRule1("BILL",code_type);
                            break;
                        case IdentifierType.CLIENT:
                            int client_type = Convert.ToInt32(j_obj["client_type"]);
                            code = await identifierServiceRepository.buildClientNo(code_type, client_type);
                            break;
                        case IdentifierType.BANG_KE:
                            code = await identifierServiceRepository.BuildRule1("BK", code_type);
                            break;
                    }

                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        code = code
                    });

                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key không hợp lệ"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("IdentifierController - getIdentifier: " + ex + "--token" + token);
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "IdentifierController - getIdentifier: " + ex
                });
            }
        }

    }
}
