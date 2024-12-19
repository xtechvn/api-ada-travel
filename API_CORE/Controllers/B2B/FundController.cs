using ENTITIES.Models;
using ENTITIES.ViewModels.AllotmentFunds;
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

namespace API_CORE.Controllers.B2B
{
    [Route("api/b2b/[controller]")]
    [ApiController]
    public class FundController : ControllerBase
    {
        private IConfiguration configuration;
        private IClientRepository clientRepository;
        private IAllotmentFundRepository allotmentFundRepository;

        public FundController(IConfiguration _configuration, IClientRepository _clientRepository, IAllotmentFundRepository _allotmentFundRepository)
        {
            configuration = _configuration;
            allotmentFundRepository = _allotmentFundRepository;

        }
        [HttpPost("transfer-fund.json")]
        public async Task<ActionResult> UpdateFundBalanceByTransfer(string token)
        {
            try
            {
                /*  var j_param = new Dictionary<string, string>
                          {
                              {"from_fund_type", "3"},
                              {"to_fund_type", "2"},
                              {"client_id", "182"},
                              {"amount_move", "277000"},
                          };
                  var data_product = JsonConvert.SerializeObject(j_param);
                  token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);*/
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    int from_fund_type = Convert.ToInt32(objParr[0]["from_fund_type"]);
                    int to_fund_type = Convert.ToInt32(objParr[0]["to_fund_type"]);
                    long client_id = Convert.ToInt64(objParr[0]["client_id"]);
                    double amount_move = Convert.ToDouble(objParr[0]["amount_move"]);
                    if (from_fund_type == to_fund_type || from_fund_type < 1 || to_fund_type < 1 || client_id < 1 || amount_move <= 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Data invalid!",
                        });
                    }
                    AllotmentFundTransferViewModel model = new AllotmentFundTransferViewModel()
                    {
                        AccountClientId = client_id,
                        FundType = from_fund_type,
                        CreateDate = DateTime.Now,
                        to_fund_type = to_fund_type,
                        amount_move = amount_move
                    };
                    long history_id = await allotmentFundRepository.UpdateFundBalanceByTransfer(model);
                    if (history_id > 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Success",
                            history_id = history_id
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Không thể chuyển tiền. Vui lòng liên hệ bộ phận IT",
                            history_id = history_id
                        });
                    }

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
                LogHelper.InsertLogTelegram("UpdateFundBalanceByTransfer - FundController: " + ex);
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "Error On Excution. Vui lòng liên hệ bộ phận IT" });
            }
        }
        

    }
}


