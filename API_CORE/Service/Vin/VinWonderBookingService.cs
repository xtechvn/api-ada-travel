using APP.PUSH_LOG.Functions;
using ENTITIES.ViewModels.Booking;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories.VinWonder;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Utilities;

namespace API_CORE.Service.Vin
{
    public class VinWonderBookingService
    {
        private IConfiguration configuration;
        private readonly IVinWonderBookingRepository _vinWonderBookingRepository;

        public VinWonderBookingService(IConfiguration _configuration, IVinWonderBookingRepository vinWonderBookingRepository)
        {
            configuration = _configuration;
            _vinWonderBookingRepository = vinWonderBookingRepository;
        }
        public async Task<string> GetToken()
        {
            string token = "";
            try
            {
                HttpClient httpClient = new HttpClient();
                var urlGetToken = configuration["config_api_vinpearl:VinWonder:Domain"] + configuration["config_api_vinpearl:VinWonder:GetToken"]+ "?username=" + configuration["config_api_vinpearl:VinWonder:Username"]+ "&password="+ configuration["config_api_vinpearl:VinWonder:Password"];
                var response = await httpClient.GetAsync(urlGetToken);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var data = JObject.Parse(responseString);
                    token = data["Data"]["Token"].ToString();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetToken - VinWonderBookingService: " + ex.ToString());
            }
            return token;
        }
        public async Task<object> ConfirmBooking(VinWonderBookingB2Request input_api_vin,string token)
        {
            try
            {
                var client = new RestClient(configuration["config_api_vinpearl:VinWonder:Domain"]);
                var request = new RestRequest(configuration["config_api_vinpearl:VinWonder:ConfirmBooking"], Method.Post);
                request.AddHeader("Authorization", "Bearer " + token);
                request.AddParameter("application/json", JsonConvert.SerializeObject(input_api_vin), ParameterType.RequestBody);
                RestResponse response = client.Execute(request);

                return JsonConvert.DeserializeObject<object>(response.Content);
                MongoDBSMSAccess.InsertLogMongoDb(configuration, response.Content, "ConfirmBooking");

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ConfirmBooking - VinWonderBookingService: " + ex.ToString());
            }
            return null;
        }

        public async Task<List<object>> ConfirmBookingByOrderID(long order_id)
        {
            List<object> result = new List<object>();
            try
            {
                var booking = await _vinWonderBookingRepository.GetVinWonderBookingByOrderId(order_id);
                if (booking != null && booking.Count > 0)
                {
                    string vin_booking_token = await GetToken();
                    var booking_mongo = _vinWonderBookingRepository.GetBookingById(booking.Select(x => x.AdavigoBookingId).ToArray());
                    if (booking_mongo != null && booking_mongo.Count > 0)
                    {
                        foreach (var data in booking_mongo)
                        {
                            if (data.requestVin != null && data.requestVin.Count > 0)
                            {
                                foreach (var item in data.requestVin)
                                {
                                    var push_content = await ConfirmBooking(item, vin_booking_token);
                                    result.Add(push_content);
                                }
                            }

                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ConfirmBookingByOrderID - VinWonderBookingService: OrderID= " + order_id + "\n " + ex);
                return new List<object>();
            }
        }
    }
}
