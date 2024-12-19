using API_CORE.Model;
using API_CORE.Service.Log;
using ENTITIES.ViewModels.Vinpreal;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace API_CORE.Service.Vin
{
    public class VinpearlLib
    {
        private IConfiguration configuration;
        public string token = "";
        public DateTime time_get_token= DateTime.Now;
        public VinpearlLib(IConfiguration _configuration)
        {
            configuration = _configuration;
            token= GetTokenAsync().Result.ToString();
        }
        public async Task ValidateToken()
        {
            try
            {
                if (time_get_token.AddHours(20) < DateTime.Now)
                {
                    token = GetTokenAsync().Result.ToString();
                    time_get_token = DateTime.Now;
                }
            }
            catch
            {

            }
        }

        public async Task<string> GetTokenAsync()
        {
            try
            {


                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var values = new Dictionary<string, string>
                  {
                      {"username",  configuration["config_api_vinpearl:USER_NAME_API_VIN"]},
                      {"organization",  configuration["config_api_vinpearl:ORGANIZATION"]},
                      {"password",configuration["config_api_vinpearl:PASSWORD_API_VIN"]}
                  };
                var content = new FormUrlEncodedContent(values);
                var urlGetToken = configuration["config_api_vinpearl:API_VIN_URL"] + configuration["config_api_vinpearl:enpoint:get_token"];
                var response = await httpClient.PostAsync(urlGetToken, content);
                var responseString = await response.Content.ReadAsStringAsync();
                Authentication authentication = JsonConvert.DeserializeObject<Authentication>(responseString);
                var token = string.Empty;
                if (!string.IsNullOrEmpty(authentication.authentication_token))
                {
                    token = authentication.authentication_token.Split(' ')[2];
                }
                if (string.IsNullOrEmpty(token)){
                    LogHelper.InsertLogTelegram("VinpearlLib - GetTokenAsync with [" + configuration["config_api_vinpearl:USER_NAME_API_VIN"] + "] [" + configuration["config_api_vinpearl:ORGANIZATION"] + "] error: " + responseString);
                }
                return token;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("VinpearlLib - GetTokenAsync with [" + configuration["config_api_vinpearl:USER_NAME_API_VIN"] + "] [" + configuration["config_api_vinpearl:ORGANIZATION"] + "] error: " + ex.ToString());
                return string.Empty;
            }
        }

        /// <summary>
        /// API lấy thông tin tất cả khách sạn của VIN
        /// </summary>
        /// <returns></returns>
        public async Task<string> getAllRoom(int page, int limit)
        {
            try
            {
                await ValidateToken();
                
                if (string.IsNullOrEmpty(token)) return string.Empty;

                string url_hotel = configuration["config_api_vinpearl:API_VIN_URL"] + configuration["config_api_vinpearl:enpoint:get_all_room"].Replace("{page}", page.ToString()).Replace("{limit}", limit.ToString());
                var uri = new System.Uri(url_hotel);
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        var result = await httpClient.GetStringAsync(uri);
                        return result;
                    }
                    catch (Exception ex)
                    {
                        await TeleLog.InsertLogTelegram(configuration,"url_hotel = " + ex.Message);
                        return string.Empty;
                    }
                }


            }
            catch (Exception ex)
            {
               await TeleLog.InsertLogTelegram(configuration,"getAllRoom = "  + ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// API lấy thông tin các phòng thuộc 1 khách sạn theo ngày đến ngày về. Đã bao gồm giá
        /// </summary>
        /// <returns>
        /// ID		ID gốc từ CMS nhập
        //   ID VIN      ID từ vin
        //   Name        Tên khách sạn
        /////// Thông tin danh sách phòng + giá theo từng đêm/////////
        //ID ID gốc từ CMS nhập
        //ID VIN      ID từ vin
        //Giá     Giá đã qua công thức giá
        /// </returns>
        public async Task<string> getHotelAvailability(string input_api_vin)
        {
            try
            {
                await ValidateToken();
                string url_hotel = configuration["config_api_vinpearl:API_VIN_URL"] + configuration["config_api_vinpearl:enpoint:get_hotel_availability"];
                var uri = new System.Uri(url_hotel);
                using (var httpClient = new HttpClient())
                {
                    /*
                    try
                    {
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        var content =  new StringContent(input_api_vin, Encoding.UTF8, "application/json");
                        var response = httpClient.PostAsync(uri, content).Result;
                        var responseString = await response.Content.ReadAsStringAsync();
                        return responseString;
                    }
                    catch (Exception ex)
                    {
                        return ex.ToString();
                    }
                    */
                    var client = new RestClient(configuration["config_api_vinpearl:API_VIN_URL"]);
                    var request = new RestRequest(configuration["config_api_vinpearl:enpoint:get_hotel_availability"],Method.Post);
                    request.AddHeader("Authorization", "Bearer " + token);
                    request.AddParameter("application/json", input_api_vin, ParameterType.RequestBody);
                    RestResponse response = client.Execute(request);
                    return response.Content;

                }
               

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public async Task<string> getRoomAvailability(string input_api_vin)
        {
            try
            {
                await ValidateToken();
                string url_hotel = configuration["config_api_vinpearl:API_VIN_URL"] + configuration["config_api_vinpearl:enpoint:get_room_availability"];
                var uri = new System.Uri(url_hotel);
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        /*
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        var content = new StringContent(input_api_vin, Encoding.UTF8, "application/json");
                        var response = httpClient.PostAsync(uri, content).Result;
                        var responseString = await response.Content.ReadAsStringAsync();
                        return responseString;*/
                        var client = new RestClient(configuration["config_api_vinpearl:API_VIN_URL"]);
                        var request = new RestRequest(configuration["config_api_vinpearl:enpoint:get_room_availability"], Method.Post);
                        request.AddHeader("Authorization", "Bearer " + token);
                        request.AddParameter("application/json", input_api_vin, ParameterType.RequestBody);
                        RestResponse response = client.Execute(request);
                        return response.Content;
                    }
                    catch (Exception ex)
                    {

                        return ex.ToString();
                    }
                }


            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        /// <summary>
        /// Api lấy ra tất cả khách sạn ko có API. Nhập tay       
        public async Task<string> getAllRoomManual()
        {
            try
            {

                string url_hotel = configuration["config_api_room_manual:enpoint:get_all_room"];
                var uri = new System.Uri(url_hotel);
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        var result = await httpClient.GetStringAsync(uri);

                        return result;
                    }
                    catch (Exception ex)
                    {
                        return ex.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// Api tìm kiếm các gói trong chi tiết phòng
        /// </summary>
        /// <returns>

        public async Task<string> getRoomDetailAvailability(string input_api_vin)
        {
            try
            {
                await ValidateToken();
                string url = configuration["config_api_vinpearl:API_VIN_URL"] + configuration["config_api_vinpearl:enpoint:get_room_detail_availability"];
                var uri = new System.Uri(url);
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        var content = new StringContent(input_api_vin, Encoding.UTF8, "application/json");
                        var response = httpClient.PostAsync(uri, content).Result;
                        var responseString = await response.Content.ReadAsStringAsync();
                        return responseString;
                   
                    }
                    catch (Exception ex)
                    {
                        return ex.ToString();
                    }
                }

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        /// <summary>
        /// Api lấy mã đặt chỗ
        /// </summary>
        /// <returns>
        public async Task<string> getVinpearlConfirmBooking(string input_api_vin)
        {
            try
            {
                await ValidateToken();
                string url = configuration["config_api_vinpearl:API_VIN_URL"] + configuration["config_api_vinpearl:enpoint:get_bookable_package_availability"];
                var uri = new System.Uri(url);
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        var content = new StringContent(input_api_vin, Encoding.UTF8, "application/json");
                        var response = httpClient.PostAsync(uri, content).Result;
                        var responseString = await response.Content.ReadAsStringAsync();
                        return responseString;

                    }
                    catch (Exception ex)
                    {
                        return ex.ToString();
                    }
                }

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// Api  CREATE BOOKING VIN
        /// </summary>
        /// <returns>
        public async Task<string> getVinpearlCreateBooking(string input_api_vin)
        {
            try
            {
                await ValidateToken();
                string url = configuration["config_api_vinpearl:API_VIN_URL"] + configuration["config_api_vinpearl:enpoint:get_create_booking"];
                var uri = new System.Uri(url);
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        var content = new StringContent(input_api_vin, Encoding.UTF8, "application/json");
                        var response = httpClient.PostAsync(uri, content).Result;
                        var responseString = await response.Content.ReadAsStringAsync();
                        return responseString;

                    }
                    catch (Exception ex)
                    {
                        return ex.ToString();
                    }
                }

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        /// <summary>
        /// Api  Guarantee Methods VIN
        /// </summary>
        /// <returns>
        public async Task<string> getGuaranteeMethods(String reservationid, string input_api_vin)
        {
            try
            {
                await ValidateToken();
                string url = configuration["config_api_vinpearl:API_VIN_URL"] + configuration["config_api_vinpearl:enpoint:get_guarantee_methods"].Replace("reservationID", reservationid);
                var uri = new System.Uri(url);
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        var content = new StringContent(input_api_vin, Encoding.UTF8, "application/json");
                        var response = httpClient.PostAsync(uri, content).Result;
                        var responseString = await response.Content.ReadAsStringAsync();
                        return responseString;

                    }
                    catch (Exception ex)
                    {
                        return ex.ToString();
                    }
                }

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        /// <summary>
        /// Api  Batch Commit VIN
        /// </summary>
        /// <returns>
        public async Task<string> getBatchCommit(string input_api_vin)
        {
            try
            {
                await ValidateToken();
                string url = configuration["config_api_vinpearl:API_VIN_URL"] + configuration["config_api_vinpearl:enpoint:get_batch_commit"];
                var uri = new System.Uri(url);
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        var content = new StringContent(input_api_vin, Encoding.UTF8, "application/json");
                        var response = httpClient.PostAsync(uri, content).Result;
                        var responseString = await response.Content.ReadAsStringAsync();
                        return responseString;

                    }
                    catch (Exception ex)
                    {
                        return ex.ToString();
                    }
                }

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
