using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Utilities;

namespace API_LOGIN.Controllers.LOGIN.Base
{
    public class UserCoreService
    {
        private IConfiguration configuration;

        public UserCoreService(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public async Task<string> getUrlLoginByCompany(string domain_company,int user_id, string user_name, string email)
        {
            try
            {
                string private_token_key = configuration["DataBaseConfig:key_api:api_manual"];
                string url = domain_company + "/login/token";//configuration["DataBaseConfig:domain_cms_core:domain_travel"] + "/login/token";
                var uri = new Uri(url);
                using (var httpClient = new HttpClient())
                {
                    var j_param = new Dictionary<string, string>
                    {
                        {"user_id", user_id.ToString()},
                        {"user_name", user_name},
                        {"email", email},
                        {"time", DateTime.Now.ToString()}
                    };
                    var data_json = JsonConvert.SerializeObject(j_param);
                    var token = CommonHelper.Encode(data_json, private_token_key);
                    // var response = ApiConnect.CreateHttpRequest(token, url).Result;

                    // var data_voucher = JObject.Parse(response.ToString());
                    // if (data_voucher["status"].ToString() == "0")
                    // {
                    // string url_home = data_voucher["url"].ToString();
                    // return url_home;
                    //  }
                    //token = "ShFCQVFLZ1pUG3tjegBeZGhtRhQFDBUwCA83NGRdFzwOEgZNGBUcXA5cAWtzQQM5HiBAK1BXVkRdXlcdU1YsY2QSEj8nJEZbVFhVQFdadWN2VQZxUktDWg4ESxE/eE80";
                    return url + "/" + token.Replace("+","-").Replace("/","_");
                }

               // return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;                
            }
        }


    }
}
