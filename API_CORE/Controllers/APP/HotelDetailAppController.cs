using ENTITIES.APPModels.PushHotel;
using ENTITIES.ViewModels.Hotel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories.Hotel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.Controllers.APP
{
    [Route("api/app/hotel")]
    [ApiController]
    public class HotelDetailAppController : Controller
    {
        private IConfiguration _configuration;
        private IHotelDetailRepository _hotelDetailRepository;
        public HotelDetailAppController(IConfiguration configuration, IHotelDetailRepository hotelDetailRepository)
        {
            _configuration = configuration;
            _hotelDetailRepository = hotelDetailRepository;
        }
        [HttpPost("insert.json")]
        public async Task<ActionResult> InsertData([FromBody] string token)
        {

            try
            {

                #region Test

                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:api_manual"]);
                #endregion
                string key = _configuration["DataBaseConfig:key_api:api_manual"];
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, key))
                {
                    var data = JsonConvert.DeserializeObject<HotelSummit>(objParr[0].ToString());
                    if(data==null || data.hotel_detail.hotel.HotelId==null || data.hotel_detail.hotel.HotelId.Trim() == "")
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "Data invalid!"
                        });
                    }
                    var id = await _hotelDetailRepository.SummitHotelDetail(data);
                    if (id > 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Success",
                            data = id,
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Failed"
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
                LogHelper.InsertLogTelegram("InsertData - HotelDetailAppController: " + ex);
                return Ok(new { status = (int)ResponseType.ERROR, msg = "Error on Excution!" });
            }
        }

    }
}
