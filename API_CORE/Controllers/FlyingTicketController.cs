using API_CORE.Service.Price;
using Caching.RedisWorker;
using ENTITIES.APIModels;
using ENTITIES.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories;
using REPOSITORIES.IRepositories.Fly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.Controllers
{
    [Route("api")]
    [ApiController]
    public class FlyingTicketController : ControllerBase
    {
        private IConfiguration configuration;
        private IServicePiceRepository price_repository;
        private IProductFlyTicketServiceRepository productFlyTicketServiceRepository;
        private ICampaignRepository campaign_repository;
        private IGroupClassAirlinesDetailRepository groupClassAirlinesDetailRepository;
        private IAirlinesRepository airlinesRepository;
        private IGroupClassAirlinesRepository groupClassAirlinesRepository;
        private readonly RedisConn redisService;
        public FlyingTicketController(IConfiguration _configuration, IServicePiceRepository _price_repository, IGroupClassAirlinesDetailRepository _groupClassAirlinesDetailRepository,
            ICampaignRepository _campaign_repository, IProductFlyTicketServiceRepository _productFlyTicketServiceRepository,
            IAirlinesRepository _airlinesRepository, IGroupClassAirlinesRepository _groupClassAirlinesRepository, RedisConn _redisService)
        {
            configuration = _configuration;
            price_repository = _price_repository;
            campaign_repository = _campaign_repository;
            redisService = _redisService;
            productFlyTicketServiceRepository = _productFlyTicketServiceRepository;
            airlinesRepository = _airlinesRepository;
            groupClassAirlinesDetailRepository = _groupClassAirlinesDetailRepository;
            groupClassAirlinesRepository = _groupClassAirlinesRepository;
        }

        [EnableCors("MyApi")]
        [HttpPost("flyticket/get-price.json")]
        public async Task<IActionResult> GetAmountByPrice(string token)
        {
            try
            {
                var data = new List<FlightServicePriceModel>();

                #region Test
                //var j_param = new Dictionary<string, string>
                //        {
                //            {"price_range", "500000,1270000"},
                //            {"client_type","5" },
                //      };
                //var data_product = JsonConvert.SerializeObject(j_param);
                //  token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    string _price_list = string.Empty;
                    if (objParr[0]["price_range"] != null)
                    {
                        _price_list = !(string.IsNullOrEmpty(objParr[0]["price_range"].ToString())) ? objParr[0]["price_range"].ToString() : objParr[0]["price"].ToString();
                    }
                    else
                    {
                        _price_list = objParr[0]["price"].ToString();
                    }

                    var price_range = Array.ConvertAll(_price_list.Split(','), s => double.Parse(s));

                    int client_type = Convert.ToInt32(objParr[0]["client_type"]);
                    
                    if (price_range.Count() == 0)
                    {
                        return Ok(new { status = ((int)ResponseType.FAILED).ToString(), msg = "Data Invalid" });
                    }
                    //price_range = price_range.Distinct().ToArray();
                    // -- Read from Cache:
                    int db_index = Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]);
                    string cache_name = CacheName.B2C_FLY_PRICE + "_" + string.Join(",",price_range) + client_type;
                    var str = redisService.Get(cache_name, db_index);
                    if (str != null && str.Trim() != "")
                    {
                        List<FlightServicePriceModel> model = JsonConvert.DeserializeObject<List<FlightServicePriceModel>>(str);
                        // --Trả kết quả
                        return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), msg = "Success", data = model });
                    }

                    foreach (var _price in price_range)
                    {
                        if (_price <= 0)
                        {
                            continue;
                        }
                        var active_policy = await productFlyTicketServiceRepository.GetFlyPricePolicyActive();
                        data.Add(PricePolicyService.GetFlyTicketProfit(active_policy,_price));

                    }
                    //data = data.GroupBy(x => x.price).Select(x => x.First()).ToList();
                    redisService.Set(cache_name, JsonConvert.SerializeObject(data), db_index);

                    return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), msg = "Success", data = data });
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
                LogHelper.InsertLogTelegram("GetAmountByPrice - FlyingTicketController: " + ex);
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "Error On Excution. Vui lòng liên hệ IT" });
            }

        }

        [EnableCors("MyApi")]
        [HttpPost("fly/get-airline-by-code.json")]
        public async Task<IActionResult> getAirlineByCode(string token)
        {
            try
            {
                #region Test
                //var j_param = new Dictionary<string, string>
                //        {
                //            {"code", "-1"}
                //        };
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    string code = objParr[0]["code"].ToString();
                    string cache_name = CacheName.AIRLINES;
                    int db_index = Convert.ToInt32(configuration["Redis:Database:db_core"]);

                    var airlines = new Airlines();
                    var dataCache = new List<Airlines>();
                    var strDataCache = redisService.Get(cache_name, db_index);
                    if (!string.IsNullOrEmpty(strDataCache))
                        dataCache = JsonConvert.DeserializeObject<List<Airlines>>(strDataCache);

                    if (dataCache.Count == 0)
                    {
                        var listAirlines = airlinesRepository.GetAllData();
                        dataCache = listAirlines;
                        airlines = listAirlines.FirstOrDefault(n => n.Code.ToLower().Equals(code.ToLower()));
                        redisService.Set(cache_name, JsonConvert.SerializeObject(listAirlines), db_index);
                    }

                    if (code.Equals("-1") == true)
                    {
                        return Ok(new
                        {
                            data = dataCache,
                            status = (int)ResponseType.SUCCESS,
                            msg = "Successfully !!!"
                        });
                    }
                    else
                    {
                        airlines = dataCache.FirstOrDefault(n => n.Code.ToLower().Equals(code.ToLower()));
                    }
                    if (airlines == null)
                    {
                        LogHelper.InsertLogTelegram("Không lấy được danh sách hãng bay theo mã. Code: " + code + ". Token = " + token);
                        return Ok(new
                        {
                            data = airlines,
                            status = (int)ResponseType.FAILED,
                            msg = "Fail!"
                        });
                    }
                    return Ok(new
                    {
                        data = airlines,
                        status = (int)ResponseType.SUCCESS,
                        msg = "Successfully !!!"
                    });
                }
                else
                {

                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key không hợp lệ"
                    });
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("FlyingTicketController - fly/get-airline-by-code.json: " + ex);
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "Error On Excution. Vui lòng liên hệ IT" });
            }
        }

        [EnableCors("MyApi")]
        [HttpPost("fly/get-group-class.json")]
        public async Task<IActionResult> getGroupClass(string token)
        {
            try
            {
                #region Test
                //var j_param = new Dictionary<string, string>
                //        {
                //            {"air_line", "VN"},
                //            {"class_code", "Q"},
                //            {"fare_type", "Economy"}
                //        };
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    string air_line = objParr[0]["air_line"].ToString();
                    string class_code = objParr[0]["class_code"].ToString();
                    string fare_type = objParr[0]["fare_type"].ToString();

                    var groupClassAirline = new GroupClassAirlines();

                    int db_index = Convert.ToInt32(configuration["Redis:Database:db_core"]);
                    string cache_name = CacheName.GROUP_AIRLINES;

                    var listData = new List<GroupClassAirlines>();
                    var strDataCache = redisService.Get(cache_name, db_index);
                    if (!string.IsNullOrEmpty(strDataCache))
                        listData = JsonConvert.DeserializeObject<List<GroupClassAirlines>>(strDataCache);
                    if (listData.Count == 0)
                    {
                        listData = groupClassAirlinesRepository.GetAllData();
                        redisService.Set(cache_name, JsonConvert.SerializeObject(listData), db_index);
                    }
                    if (!string.IsNullOrEmpty(air_line) && (air_line.Equals("VU") || air_line.Equals("vu")))
                    {
                        groupClassAirline = listData.FirstOrDefault(n => n.Airline.ToLower().Equals(air_line.ToLower()));
                    }
                    else if (!string.IsNullOrEmpty(class_code) && class_code.ToLower().Contains("_eco") && (air_line == "VJ" || air_line == "vj"))
                    {
                        groupClassAirline = listData.FirstOrDefault(n => n.ClassCode.ToLower().Contains("_eco") && n.Airline.ToLower().Equals(air_line.ToLower()));
                    }
                    else if (!string.IsNullOrEmpty(class_code) && (class_code.Contains("_DLX") || class_code.Contains("_dlx")) && (air_line == "VJ" || air_line == "vj"))
                    {
                        groupClassAirline = listData.FirstOrDefault(n => n.ClassCode.ToLower().Contains("_dlx") && n.Airline.ToLower().Equals(air_line.ToLower()));
                    }
                    else if (!string.IsNullOrEmpty(class_code) && (class_code.ToUpper().Contains("_BOSS") || class_code.ToLower().Contains("_boss")) && (air_line == "VJ" || air_line == "vj"))
                    {
                        groupClassAirline = listData.FirstOrDefault(n => n.ClassCode.ToLower().Contains("_boss") && n.Airline.ToLower().Equals(air_line.ToLower()));
                    }
                    else if (!string.IsNullOrEmpty(class_code) && (class_code.ToUpper().Contains("_SBOSS") || class_code.ToLower().Contains("_sboss")) && (air_line == "VJ" || air_line == "vj"))
                    {
                        groupClassAirline = listData.FirstOrDefault(n => n.ClassCode.ToLower().Contains("_sboss") && n.Airline.ToLower().Equals(air_line.ToLower()));
                    }
                    else if (!string.IsNullOrEmpty(class_code) && (class_code.ToLower().Contains("_combo") || class_code.ToUpper().Contains("_COMBO")) && (air_line == "VJ" || air_line == "vj"))
                    {
                        groupClassAirline = listData.FirstOrDefault(n => n.ClassCode.ToLower().Contains("_combo") && n.Airline.ToLower().Equals(air_line.ToLower()));
                    }
                    else if (!string.IsNullOrEmpty(class_code) && (class_code.Contains(",")) && (air_line.ToLower() == "vn") && (fare_type.Contains("Economy") || fare_type.Contains("ECONOMY")))
                    {
                        groupClassAirline = listData.FirstOrDefault(n => n.ClassCode.Equals(",") && n.Airline.ToLower().Equals(air_line.ToLower()) && n.FareType.Contains("Economy"));
                    }
                    else if (!string.IsNullOrEmpty(class_code) && (class_code.Contains(",")) && (air_line.ToLower() == "vn") && (fare_type.Contains("Business") || fare_type.Contains("BUSINESS")))
                    {
                        groupClassAirline = listData.FirstOrDefault(n => n.ClassCode.Equals(",") && n.Airline.ToLower().Equals(air_line.ToLower()) && n.FareType.Contains("Business"));
                    }
                    else
                    {
                        groupClassAirline = listData.FirstOrDefault(n => n.Airline.ToLower().Equals(air_line.ToLower()) && n.ClassCode.ToLower().Equals(class_code.ToLower())
                           && n.FareType.ToLower().Equals(fare_type.ToLower())
                       );
                    }
                    if (groupClassAirline == null)
                    {
                        LogHelper.InsertLogTelegram("Không lấy được thông tin hạng vé theo hãng." +
                            " code = " + class_code + ". air_line = " + air_line + ". fare_type = " + fare_type + ". Token = " + token);
                        return Ok(new
                        {
                            data = groupClassAirline,
                            status = (int)ResponseType.FAILED,
                            msg = "Fail!"
                        });
                    }
                    groupClassAirline.Description = GetDetailGroupClass(groupClassAirline.Id);
                    return Ok(new
                    {
                        data = groupClassAirline,
                        status = (int)ResponseType.SUCCESS,
                        msg = "Successfully !!!"
                    });
                }
                else
                {

                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key không hợp lệ"
                    });
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("FlyingTicketController - fly//get-group-class.json: " + ex + ".             Token = " + token);
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "Error On Excution. Vui lòng liên hệ IT" });
            }
        }

        private string GetDetailGroupClass(int groupClassId)
        {
            string detail = String.Empty;
            GroupClassAirlinesDetail groupClassAirlinesDetail = groupClassAirlinesDetailRepository.GetDetailGroupClassAirlines(groupClassId);
            if (groupClassAirlinesDetail != null)
            {
                var listDetail = groupClassAirlinesDetail.Name.Split("-");
                foreach (var item in listDetail)
                {
                    if (item.ToLower().Contains("không"))
                    {
                        detail += "<li  style=\"padding-left: 0px !important;\"><img src=\"https://static-image.adavigo.com/uploads/images/webicons/ticket2.svg\"><span style=\"margin-left: 10px; \">" + item + "</span></li>";
                    }
                    else
                    {
                        detail += "<li  style=\"padding-left: 0px !important;\"><img src=\"https://static-image.adavigo.com/uploads/images/webicons/ticket.svg\"><span style=\"margin-left: 10px;\">" + item + "</span></li>";
                    }
                }
            }
            return detail;
        }

        [EnableCors("MyApi")]
        [HttpPost("fly/get-list-group-class.json")]
        public async Task<IActionResult> getListGroupClass(string token)
        {
            try
            {
                #region Test
                //token = "GjpjICtBERYILVRbWRATD2NtYyctMDIxHiIuJiRzeQliWHpbVBUHLBkgPScKGw1ncFEMBgsqSHErEyALUzhUQ1YoHT50AVwWVht4WCQZb1oXMQ47KTk9IV8VMBFpbzF3S1sfDwACHRAIIxZIfUlzBho4XEZAFwpZEDx2Sx4YZjFbHR0vICUWZ25PIREReXcQXgg9PScnORIVaUByAQZ7GBl5NgkVNiUmE0daExMxDCZfQSkmEGASKhorAAshE1oZG1AaUQFKaAkKCjxxCU06OBdEVxklJRICJzQKIRxhTGUKYUNSDywaF3QREUgMSU13ITcgKzYAHnE0PQIkIyoGex5oP3glURM5CysjC2AMWyV8SxkTIQ4tMhIvUlk2SmACES1gGxofUTxVawUhA1ZQXRYxNCgGDDsRdjo7LUICXDNPECo=";
                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    var datalist = JsonConvert.DeserializeObject<List<GroupClassAirlinesModel>>(objParr[0].ToString());

                    var groupClassAirline = new GroupClassAirlines();
                    List<GroupClassAirlines> listGroupClassAirlines = new List<GroupClassAirlines>();

                    int db_index = Convert.ToInt32(configuration["Redis:Database:db_core"]);
                    string cache_name = CacheName.GROUP_AIRLINES;

                    var listData = new List<GroupClassAirlines>();
                    var strDataCache = redisService.Get(cache_name, db_index);
                    if (!string.IsNullOrEmpty(strDataCache))
                        listData = JsonConvert.DeserializeObject<List<GroupClassAirlines>>(strDataCache);
                    if (listData.Count == 0)
                    {
                        listData = groupClassAirlinesRepository.GetAllData();
                        redisService.Set(cache_name, JsonConvert.SerializeObject(listData), db_index);
                    }
                    for (int i = 0; i < datalist.Count(); i++)
                    {
                        if (!string.IsNullOrEmpty(datalist[i].air_line) && (datalist[i].air_line.Equals("VU") || datalist[i].air_line.Equals("vu")))
                        {
                            groupClassAirline = listData.FirstOrDefault(n => n.Airline.ToLower().Equals(datalist[i].air_line.ToLower()));
                        }
                        else if (!string.IsNullOrEmpty(datalist[i].class_code) && (datalist[i].class_code.Contains("_ECO") || datalist[i].class_code.ToLower().Contains("_eco")) && (datalist[i].air_line == "VJ" || datalist[i].air_line == "vj"))
                        {
                            groupClassAirline = listData.FirstOrDefault(n => n.ClassCode.ToLower().Contains("_eco") && n.Airline.ToLower().Equals(datalist[i].air_line.ToLower()));
                        }
                        else if (!string.IsNullOrEmpty(datalist[i].class_code) && (datalist[i].class_code.Contains("_DLX") || datalist[i].class_code.ToLower().Contains("_dlx")) && (datalist[i].air_line == "VJ" || datalist[i].air_line == "vj"))
                        {
                            groupClassAirline = listData.FirstOrDefault(n => n.ClassCode.ToLower().Contains("_dlx") && n.Airline.ToLower().Equals(datalist[i].air_line.ToLower()));
                        }
                        else if (!string.IsNullOrEmpty(datalist[i].class_code) && (datalist[i].class_code.Contains("_BOSS") || datalist[i].class_code.ToLower().Contains("_boss")) && (datalist[i].air_line == "VJ" || datalist[i].air_line == "vj"))
                        {
                            groupClassAirline = listData.FirstOrDefault(n => n.ClassCode.ToLower().Contains("_boss") && n.Airline.ToLower().Equals(datalist[i].air_line.ToLower()));
                        }
                        else if (!string.IsNullOrEmpty(datalist[i].class_code) && (datalist[i].class_code.Contains("_SBOSS") || datalist[i].class_code.ToLower().Contains("_sboss")) && (datalist[i].air_line == "VJ" || datalist[i].air_line == "vj"))
                        {
                            groupClassAirline = listData.FirstOrDefault(n => n.ClassCode.ToLower().Contains("_sboss") && n.Airline.ToLower().Equals(datalist[i].air_line.ToLower()));
                        }
                        else if (!string.IsNullOrEmpty(datalist[i].class_code) && (datalist[i].class_code.ToLower().Contains("_combo") || datalist[i].class_code.Contains("_COMBO")) && (datalist[i].air_line == "VJ" || datalist[i].air_line == "vj"))
                        {
                            groupClassAirline = listData.FirstOrDefault(n => n.ClassCode.ToLower().Contains("_combo") && n.Airline.ToLower().Equals(datalist[i].air_line.ToLower()));
                        }
                        else if (!string.IsNullOrEmpty(datalist[i].class_code) && (datalist[i].class_code.Contains(",")) && (datalist[i].air_line.ToLower() == "vn") && (datalist[i].fare_type.Contains("Economy") || datalist[i].fare_type.Contains("ECONOMY")))
                        {
                            groupClassAirline = listData.FirstOrDefault(n => n.ClassCode.Equals(",") && n.Airline.ToLower().Equals(datalist[i].air_line.ToLower()) && n.FareType.Contains("Economy"));
                        }
                        else if (!string.IsNullOrEmpty(datalist[i].class_code) && (datalist[i].class_code.Contains(",")) && (datalist[i].air_line.ToLower() == "vn") && (datalist[i].fare_type.Contains("Business") || datalist[i].fare_type.Contains("BUSINESS")))
                        {
                            groupClassAirline = listData.FirstOrDefault(n => n.ClassCode.Equals(",") && n.Airline.ToLower().Equals(datalist[i].air_line.ToLower()) && n.FareType.Contains("Business"));
                        }
                        else
                        {
                            groupClassAirline = listData.FirstOrDefault(n => n.Airline.ToLower().Equals(datalist[i].air_line.ToLower()) && n.ClassCode.ToLower().Equals(datalist[i].class_code.ToLower())
                               && n.FareType.ToLower().Equals(datalist[i].fare_type.ToLower()));
                        }
                        if (groupClassAirline == null)
                        {
                            groupClassAirline = new GroupClassAirlines();
                            groupClassAirline.Airline = datalist[i].air_line;
                            groupClassAirline.ClassCode = datalist[i].class_code;
                            groupClassAirline.DetailEn = datalist[i].class_code;
                            groupClassAirline.DetailVi = datalist[i].class_code;
                            groupClassAirline.FareType = datalist[i].air_line;
                        }
                        listGroupClassAirlines.Add(groupClassAirline);
                    }
                    if (listGroupClassAirlines == null || listGroupClassAirlines.Count == 0)
                    {
                        LogHelper.InsertLogTelegram("Không lấy được thông tin danh sách hạng vé theo hãng. Token = " + token);
                        return Ok(new
                        {
                            data = listGroupClassAirlines,
                            status = (int)ResponseType.FAILED,
                            msg = "Fail!"
                        });
                    }
                    foreach (var item in listGroupClassAirlines)
                    {
                        if (item != null)
                            item.Description = GetDetailGroupClass(item.Id);
                    }
                    return Ok(new
                    {
                        data = listGroupClassAirlines,
                        status = (int)ResponseType.SUCCESS,
                        msg = "Successfully !!!"
                    });
                }
                else
                {

                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key không hợp lệ"
                    });
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("FlyingTicketController - fly/get-list-group-class.json: " + ex + ".             Token = " + token);
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "Error On Excution. Vui lòng liên hệ IT" });
            }
        }

    }
}
