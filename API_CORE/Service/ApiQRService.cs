using ENTITIES.ViewModels.Payment;
using Microsoft.IdentityModel.Protocols;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;
using Utilities.Contants;
using Utilities.ModelHelpers;
using Utilities;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace API_CORE.Service
{
    public class ApiQRService
    {
        public static async Task<string> GetVietQRCode(string account_number, string bank_code, string order_no, double amount)
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
                if (status == (int)ResponseType.SUCCESS)
                {
                    return JsonConvert.DeserializeObject<List<VietQRBankDetail>>(jsonData["data"].ToString());
                }

            }
            catch
            {
                throw;
            }
            return null;
        }
        public static async Task<string> UploadImageQRBase64(string order_no, string amount, string ImageData, string type)
        {
            string key_token_api = "wVALy5t0tXEgId5yMDNg06OwqpElC9I0sxTtri4JAlXluGipo6kKhv2LoeGQnfnyQlC07veTxb7zVqDVKwLXzS7Ngjh1V3SxWz69";
            string ImagePath = string.Empty;
            string tokenData = string.Empty;

            try
            {
                var objimage = GetImageSrcBase64Object(ImageData);
                var j_param = new Dictionary<string, string> {
                    { "order_no", order_no },
                    { "amount", amount },
                    { "type", type },
                    { "data_file", objimage.ImageData },
                    { "extend", objimage.ImageExtension }
                };

                using (HttpClient httpClient = new HttpClient())
                {
                    tokenData = CommonHelper.Encode(JsonConvert.SerializeObject(j_param), key_token_api);
                    var contentObj = new { token = tokenData };
                    var content = new StringContent(JsonConvert.SerializeObject(contentObj), Encoding.UTF8, "application/json");
                    var url = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("config_value")["ImageStatic"] + "/images/upload-payment-order";
                    var result = await httpClient.PostAsync(url, content);
                    dynamic resultContent = Newtonsoft.Json.Linq.JObject.Parse(result.Content.ReadAsStringAsync().Result);
                    if (resultContent.status == 0)
                    {
                        return resultContent.url_path;
                    }
                    else
                    {
                        LogHelper.InsertLogTelegram("UploadImageQRBase64. Result: " + resultContent.status + ". Message: " + resultContent.msg);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UploadImageQRBase64 - " + ex.Message.ToString() + " Token:" + tokenData);
            }
            return ImagePath;
        }
        public static ImageBase64 GetImageSrcBase64Object(string imgSrc)
        {
            try
            {
                if (!string.IsNullOrEmpty(imgSrc) && imgSrc.StartsWith("data:image"))
                {
                    var ImageBase64 = new ImageBase64();
                    var base64Data = imgSrc.Split(',')[0];
                    ImageBase64.ImageData = imgSrc.Split(',')[1];
                    ImageBase64.ImageExtension = base64Data.Split(';')[0].Split('/')[1];
                    return ImageBase64;
                }
            }
            catch (FormatException)
            {

            }
            return null;
        }

    }
}
