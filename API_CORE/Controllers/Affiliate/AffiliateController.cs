using Entities.ViewModels;
using ENTITIES.ViewModels.Order;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.Controllers.Affiliate
{
    [Route("api/[controller]")]
    [ApiController]
    public class AffiliateController : ControllerBase
    {
        private IConfiguration configuration;
        private IOrderRepository ordersRepository;
        private IAccountRepository accountRepository;
        private IClientRepository clientRepository;
        private IContractPayRepository contractPayRepository;
        public AffiliateController(IConfiguration _configuration, IOrderRepository _ordersRepository, IAccountRepository _accountRepository,
            IClientRepository _clientRepository, IContractPayRepository _contractPayRepository)
        {
            configuration = _configuration;
            ordersRepository = _ordersRepository;
            accountRepository = _accountRepository;
            clientRepository = _clientRepository;
            contractPayRepository = _contractPayRepository;
        }

        [HttpPost("order/get-order-by-utmsource.json")]
        public async Task<ActionResult> getListOrder(string token)
        {

            try
            {
                #region Test
                var j_param = new Dictionary<string, object>
                {
                    {"account_client_id", "148"},

                };
                var data_product = JsonConvert.SerializeObject(j_param);


                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    //-- Convert AccountClientId to ClientId
                    long account_client_id = (long)objParr[0]["account_client_id"];
                    string StartDateFrom = objParr[0]["StartDateFrom"].ToString();
                    string StartDateTo = objParr[0]["StartDateTo"].ToString();
                    string EndDateFrom = objParr[0]["EndDateFrom"].ToString();
                    string EndDateTo = objParr[0]["EndDateTo"].ToString();
                    int PageIndex = (int)objParr[0]["PageIndex"];
                    int pageSize = (int)objParr[0]["pageSize"];
                    List<int> Status = new List<int>();
                    var statusOrder = objParr[0]["Status"].ToString();
                    if (statusOrder != null&& statusOrder!="")
                    {
                        Status.Add(Convert.ToInt32(statusOrder));
                    }
                    var account_client = await accountRepository.GetAccountClient(account_client_id);
                    long client_id = (long)account_client.ClientId;
                    var client = clientRepository.GetDetail(client_id);

                    var model = new OrderViewSearchModel();
                    if (client.ReferralId != null)
                    {
                        model.UtmMedium = client.ReferralId;
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Successfully ",
                            data = new GenericViewModel<OrderaffViewModel>(),
                        });
                    }
                    
                    model.Status = Status;
                    model.StartDateFrom = StartDateFrom == "" ? DateTime.MinValue : DateTime.ParseExact(StartDateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    model.StartDateTo = StartDateTo == "" ? DateTime.MinValue : DateTime.ParseExact(StartDateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture) ;
                    model.EndDateFrom = EndDateFrom == "" ? DateTime.MinValue : DateTime.ParseExact(EndDateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture) ;
                    model.EndDateTo = EndDateTo == "" ? DateTime.MinValue : DateTime.ParseExact(EndDateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture) ;
                    var data = await ordersRepository.GetListOrrderbyUtmSource(model, PageIndex, pageSize);

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

                LogHelper.InsertLogTelegram("getOrder- AffiliateController- order/get-order-by-utmsource.json: " + ex.ToString());
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "Error On Excution. Vui lòng liên hệ IT" });
            }
        }
        [HttpPost("list-bankingaccount-by-accountclientid.json")]
        public async Task<ActionResult> getListBankingAccount(string token)
        {

            try
            {
#region Test
                var j_param = new Dictionary<string, object>
                {
                    {"account_client_id", "148"},

                };
                var data_product = JsonConvert.SerializeObject(j_param);


                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
#endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    //-- Convert AccountClientId to ClientId
                    int account_client_id = (int)objParr[0]["account_client_id"];

                    var data = await contractPayRepository.GetListBankingAccountByAccountClientId(account_client_id);

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

                LogHelper.InsertLogTelegram("getOrderDetail- OrderController- order/get-order-detail.json: " + ex.ToString());
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "Error On Excution. Vui lòng liên hệ IT" });
            }
        }

    }
}
