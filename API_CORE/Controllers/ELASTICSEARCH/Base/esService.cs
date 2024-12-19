using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace API_CORE.Controllers.ELASTICSEARCH.Base
{
    public class esService
    {
        private IConfiguration configuration;
      

        public esService(IConfiguration _configuration)
        {
            configuration = _configuration; 
        }

        //cuonglv
        // Tìm kiếm thông tin  theo từ khóa trong nhiều cột
        /// <summary>
        /// ///
        /// </summary>
        /// <param name="keyword">từ khóa cần tìm kiếm</param>
        /// <param name="file_name">file chứa input cần tìm kiếm chuẩn json</param>        
        /// <returns></returns>
        public async Task<string> search(string keyword,bool search_type, string file_name)
        {
            try
            {
                string endpoint = string.Empty;                
                string url_es = configuration["DataBaseConfig:Elastic:Host"];
                var workingDirectory = Environment.CurrentDirectory;
              //  var currentDirectory = Directory.GetParent(workingDirectory);
                var query = workingDirectory + @"\QueryEs\" + file_name;

                var body_raw_input = File.ReadAllText(query);

                var j_input = JObject.Parse(body_raw_input);
                endpoint = j_input["endpoint"].ToString();
                body_raw_input = j_input["input_query"].ToString();

                


                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, url_es + endpoint);
                    var content = new StringContent(body_raw_input.Replace("{keyword}", keyword).Replace("{search_type}", search_type.ToString().ToLower()), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url_es + endpoint, content);   
                    return await response.Content.ReadAsStringAsync();
                }              
                return string.Empty;

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("API_CORE.Controllers.ELASTICSEARCH.Base - esService - searchMultiMatch: " + ex.ToString());
                return string.Empty;
            }
        }


    }
}
