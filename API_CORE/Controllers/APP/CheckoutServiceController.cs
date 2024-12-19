using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repositories.IRepositories;
using REPOSITORIES.IRepositories;
using REPOSITORIES.IRepositories.Elasticsearch;
using REPOSITORIES.IRepositories.Fly;
using REPOSITORIES.IRepositories.Hotel;
using REPOSITORIES.IRepositories.VinWonder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.Controllers.APP
{
    [Route("api/app/checkout")]
    public class CheckoutServiceController : Controller
    {
        private IConfiguration configuration;
        private IFlyBookingMongoRepository bookingRepository;
        private IOrderRepository orderRepository;
        private IAccountRepository accountRepository;
        private IHotelBookingMongoRepository hotelBookingMongoRepository;
        private IVinWonderBookingRepository _vinWonderBookingRepository;
        private IElasticsearchDataRepository elasticsearchDataRepository;
        private ITourRepository _tourRepository;
        public CheckoutServiceController(IConfiguration _configuration, IFlyBookingMongoRepository _bookingRepository, IOrderRepository _ordersRepository, IAccountRepository _accountRepository,
            IHotelBookingMongoRepository _hotelBookingMongoRepository, IElasticsearchDataRepository _elasticsearchDataRepository, IVinWonderBookingRepository vinWonderBookingRepository,
            ITourRepository tourRepository)
        {
            configuration = _configuration;
            bookingRepository = _bookingRepository;
            orderRepository = _ordersRepository;
            accountRepository = _accountRepository;
            hotelBookingMongoRepository = _hotelBookingMongoRepository;
            elasticsearchDataRepository = _elasticsearchDataRepository;
            _vinWonderBookingRepository = vinWonderBookingRepository;
            _tourRepository = tourRepository;
        }


        [HttpPost("hotel/get-booking-by-session-id.json")]
        public async Task<ActionResult> getHotelBookingBySessionId(string token, int source_booking_type)
        {

            try
            {
                JArray objParr = null;
                #region Test
                var j_param = new Dictionary<string, string>
                {
                    {"session_id", "6427e6a707399c934e46266c"},
                };
                var data_product = JsonConvert.SerializeObject(j_param);
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
                LogHelper.InsertLogTelegram("getBookingByBookingId - CheckoutServiceController: token+" + token + "\n " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "ERROR!",
                });
            }
        }
        [HttpPost("vinwonder/get-booking-by-id.json")]
        public async Task<ActionResult> getVinWonderBookingBySessionId(string token)
        {

            try
            {
                JArray objParr = null;
                #region Test
                var j_param = new Dictionary<string, string>
                {
                    {"id", "6427e6a707399c934e46266c"},
                };
                var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:api_manual"]);
                #endregion

                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    string session_id = objParr[0]["id"].ToString();
                    var session_list = session_id.Split(",");
                    var result =  _vinWonderBookingRepository.GetBookingById(session_list);
                    if ( result != null && result.Count>0)
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
                LogHelper.InsertLogTelegram("getVinWonderBookingBySessionId - CheckoutServiceController: token+" + token + "\n " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "ERROR!",
                });
            }
        }
        [HttpPost("tour/get-booking-by-id.json")]
        public async Task<ActionResult> getTourBySessionId(string token)
        {

            try
            {
                JArray objParr = null;
                #region Test
                var j_param = new Dictionary<string, string>
                {
                    {"session_id", "6427e6a707399c934e46266c"},
                };
                var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion

                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    string session_id = objParr[0]["session_id"].ToString();
                    var list_session_id = session_id.Split(',');
                    var result = await _tourRepository.getBookingByID(list_session_id);
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
                LogHelper.InsertLogTelegram("tour/get-booking-by-id.json -getTourBySessionId- CheckoutServiceController: token+" + token + "\n " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "ERROR!",
                });
            }
        }
    }
}
