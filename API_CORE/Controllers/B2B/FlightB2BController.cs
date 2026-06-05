using ENTITIES.ViewModels.Tour;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Utilities.Contants;
using Utilities;
using Microsoft.Extensions.Configuration;
using CACHING.Elasticsearch;
using Caching.RedisWorker;
using Repositories.IRepositories;
using REPOSITORIES.IRepositories;
using ENTITIES.ViewModels.B2B;
using Newtonsoft.Json;

namespace API_CORE.Controllers.B2B
{
    [Route("api/b2b/flight")]
    [ApiController]
    public class FlightB2BController : Controller
    {
        private IConfiguration configuration;
        private IFlightWarehouseRepository _flightWarehouseRepository;
        public FlightB2BController(IConfiguration _configuration, IFlightWarehouseRepository flightWarehouseRepository)
        {
            configuration = _configuration;
            _flightWarehouseRepository = flightWarehouseRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("get-list")]
        public async Task<ActionResult> GetList(string token)
        {
            #region Test
            //var j_param = new Dictionary<string, object>
            //    {
            //      
            //        {"tourtype", "1"},

            //    };
            //var data_product = JsonConvert.SerializeObject(j_param);
            //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
            #endregion
            try
            {
                var tour_type_list = new List<int>() { (int)TourType.Noi_Dia, (int)TourType.In_bound, (int)TourType.Out_bound };
                JArray objParr = null;

                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    var model = JsonConvert.DeserializeObject<GetListFlightWarehouseModel>(objParr[0].ToString());
                    var data =await _flightWarehouseRepository.GetListFlightWarehouse(model);
                    if(data != null && data.Count > 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "success",
                            data = data
                        });
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "No data"
                    });

                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Key invalid!"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetStartLocation - TourB2CController - [" + token + "] : " + ex.ToString());
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }

        }

    }
}
