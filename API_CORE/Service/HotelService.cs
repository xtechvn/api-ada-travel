using ENTITIES.ViewModels.Payment;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Contants;

namespace API_CORE.Service
{
    public static class VietQRService 
    {

        public static async Task<string> GetVietQRCode(PaymentHotelModel model)
        {
            try
            {
                var data = await GetVietQRBankList();
                var selected_bank = data.Count > 0 ? data.FirstOrDefault(x => x.shortName.Trim().ToLower().Contains(model.short_name.Trim().ToLower())) : null;
                string bank_code = model.bank_code;
                if (selected_bank != null) bank_code = selected_bank.bin;
                var result = await GetVietQRCode(model.bank_account, bank_code, model.order_no, Convert.ToDouble(model.amount));
                var jsonData = JObject.Parse(result);
                var status = int.Parse(jsonData["code"].ToString());

                if (status == (int)ResponseType.SUCCESS)
                {
                    return jsonData["data"]["qrDataURL"].ToString();
                }
            }
            catch
            {
            }
            return null;
        }
        public static async Task<List<VietQRBankDetail>> GetVietQRBankList()
        {
            try
            {
                var options = new RestClientOptions("https://api.vietqr.io");
                var client = new RestClient(options);
                var request = new RestRequest("/v2/banks", Method.Get);
                RestResponse response = await client.ExecuteAsync(request);

                var jsonData = JObject.Parse(response.Content);
                var status = int.Parse(jsonData["code"].ToString());
                if (status == 0)
                {
                    return JsonConvert.DeserializeObject<List<VietQRBankDetail>>(jsonData["data"].ToString());
                }

            }
            catch
            {
            }
            return null;
        }

        private static async Task<string> GetVietQRCode(string account_number, string bank_code, string order_no, double amount)
        {
            try
            {
                var options = new RestClientOptions("https://api.vietqr.io");
                var client = new RestClient(options);
                var request = new RestRequest("/v2/generate", Method.Post);
                request.AddHeader("x-client-id", "ba09d2bf-7f59-442f-8c26-49a8d48e78d7");
                request.AddHeader("x-api-key", "a479a45c-47d5-41c1-9f83-990d65cd832a");
                request.AddHeader("Content-Type", "application/json");
                var body = "{ \"accountNo\": \""
                    + account_number
                    + "\", \"accountName\": \"CTCP TM VA DV QUOC TE DAI VIET\", \"acqId\": \""
                    + (bank_code.Length > 6 ? bank_code.Substring(0, 6) : bank_code)
                    + "\", \"addInfo\": \""
                    + order_no
                    + " THANH TOAN\", \"amount\": \"" + Math.Round(amount, 0)
                    + "\", \"template\": \"compact\" }";
                request.AddStringBody(body, DataFormat.Json);
                RestResponse response = await client.ExecuteAsync(request);
                return response.Content;

            }
            catch
            {

            }
            return null;
        }
       
    }
}
