using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories.Elasticsearch;
using REPOSITORIES.IRepositories.Hotel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.Controllers.BOOKING.Hotel
{

    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : Controller
    {

        private IConfiguration configuration;
        private IHotelBookingMongoRepository hotelBookingMongoRepository;
        private IElasticsearchDataRepository elasticsearchDataRepository;

        public BookingController(IConfiguration _configuration, IHotelBookingMongoRepository _hotelBookingMongoRepository, IElasticsearchDataRepository _elasticsearchDataRepository)
        {
            configuration = _configuration;
            hotelBookingMongoRepository = _hotelBookingMongoRepository;
            elasticsearchDataRepository = _elasticsearchDataRepository;

        }
        [HttpPost("hotel/get-booking-by-session-id.json")]
        public async Task<ActionResult> getBookingBySessionId(string token, int source_booking_type)
        {

            try
            {
                JArray objParr = null;
                #region Test
                //var j_param = new Dictionary<string, string>
                //{
                //    {"session_id", "65a9d5700d01d2a5175e128c"},
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion

                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    string session_id = objParr[0]["session_id"].ToString();
                    var list_session_id = session_id.Split(',');
                    var result = await hotelBookingMongoRepository.getBookingByID(list_session_id);
                    if (result.Count > 0 && result != null)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Thành công",
                            data = result
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "không thành công",

                        });
                    }

                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key invalid!"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getBookingByBookingId - BookingControl: token+" + token + "\n " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "ERROR!",
                });
            }
        }
    }
}
