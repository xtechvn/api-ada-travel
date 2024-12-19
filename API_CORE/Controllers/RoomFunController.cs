using ENTITIES.APPModels.PushHotel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories;
using System;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomFunController : ControllerBase
    {
        private IConfiguration configuration;
        private IRoomFunRepository roomFunRepository;
        public RoomFunController(IConfiguration _configuration, IRoomFunRepository _roomFunRepository)
        {
            configuration = _configuration;
            roomFunRepository = _roomFunRepository;
        }
        [HttpPost("add-room-fun.json")]
        public async Task<IActionResult> AddOrUpdateRoomFun([FromBody]string token)
        {
            int status = (int)ResponseType.FAILED;
            try
            {
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_cms"]))
                {
                    HotelContract hotelContract = JsonConvert.DeserializeObject<HotelContract>(objParr[0]["data"].ToString());
                    if (hotelContract != null)
                    {
                        status = await roomFunRepository.CreateOrUpdateRoomFun(hotelContract);
                    }
                    return Ok(new
                    {
                        status = status,
                        msg="Success"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Key không hợp lệ"
                    });
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddOrUpdateRoomFun - RoomFunController: " + ex.ToString());
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "Error On Excution. Vui lòng liên hệ IT" });
            }

        }
    }
}
