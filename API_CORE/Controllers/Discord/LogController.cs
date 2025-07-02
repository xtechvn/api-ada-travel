using ENTITIES.Models;
using ENTITIES.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Nest;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Utilities.Contants;

namespace API_CORE.Controllers.Discord
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private IConfiguration _configuration;
        public LogController(IConfiguration configuration)
        {
            _configuration= configuration;
        }
        [HttpPost("InsertLog")]
        public async Task<IActionResult> InsertLog([FromBody] LogDiscordModel request)
        {
            try
            {
                HttpClient _httpClient = new HttpClient();
           
                string url_n8n = "https://n8n.adavigo.com/webhook/send-log";
                if (_configuration["config_value:send_log"] != null && _configuration["config_value:send_log"].ToString() != null)
                {
                    url_n8n = _configuration["config_value:send_log"].ToString();
                }
                var Content = new StringContent(JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8, "application/json");
                var Result =await _httpClient.PostAsync(url_n8n, Content);
            }
            catch(Exception ex)
            {
                
            }
            return Ok(new
            {
                status = (int)ResponseType.SUCCESS,
                msg = "SUCCESS!",
                body = ""
            });
        }
    }
}
