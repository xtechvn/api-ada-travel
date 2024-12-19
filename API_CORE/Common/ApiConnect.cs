using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace API_CORE.Common
{
    public static class ApiConnect
    {
        public static async Task<string> CreateHttpRequest(string token, string url_api)
        {
            try
            {
                string responseFromServer = string.Empty;
                string status = string.Empty;
                var httpClientHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
                };

                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("token", token),
                    });

                    var response_api = await httpClient.PostAsync(url_api, content);

                    // Nhan ket qua tra ve                            
                    responseFromServer = response_api.Content.ReadAsStringAsync().Result;

                }

                return responseFromServer;
            }
            catch (Exception ex)
            {
               // LogHelper.InsertLogTelegram(token_tele, group_id, "[API NOT CONNECT] CreateHttpRequest error: " + ex.ToString() + " token =" + token + " url_api = " + url_api);
                return string.Empty;
            }
        }

    }
}
