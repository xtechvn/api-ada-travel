using API_CORE.Common;
using ENTITIES.ViewModels.Booking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Utilities;

namespace API_CORE.Controllers.PAYMENT.Base
{
    public class PaymentService
    {

        public PaymentService()
        {

        }

        public async Task<int> getDiscountBySessionId(List<BookingFlyMongoDbModel> obj_bk_fly, string domainName, long account_client_id, double total_order_amount_before, string private_token_key)
        {
            try
            {
                if (obj_bk_fly != null)
                {
                    var bk_detail = obj_bk_fly.First();
                    string voucher_name = bk_detail.voucher_name;
                    if (!string.IsNullOrEmpty(voucher_name))
                    {
                        // call lay ra discount

                        string url_vc = domainName + "/api/voucher/b2c/apply.json";
                        var uri = new System.Uri(url_vc);
                        using (var httpClient = new HttpClient())
                        {
                            var j_param = new Dictionary<string, string>
                            {
                                    {"voucher_name", voucher_name},
                                    {"user_id",account_client_id.ToString() },
                                    {"service_id","3" }, // ve mb
                                    {"total_order_amount_before",total_order_amount_before.ToString() },
                            };
                            var data_json = JsonConvert.SerializeObject(j_param);
                            var token = CommonHelper.Encode(data_json, private_token_key);
                            var response =  ApiConnect.CreateHttpRequest(token, url_vc).Result;

                            var data_voucher = JObject.Parse(response.ToString());
                            if (data_voucher["status"].ToString() == "0")
                            {
                                int discount = Convert.ToInt32(data_voucher["discount"]);
                                return discount;
                            }
                        }
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
