using Caching.RedisWorker;
using ENTITIES.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.Controllers.B2B
{
    [Route("api/b2b/location")]
    [ApiController]
    public class LocationB2BController : Controller
    {
        private IConfiguration configuration;
        private readonly RedisConn _redisService;
        private IAllCodeRepository allCodeRepository;


        public LocationB2BController(IConfiguration _configuration,IAllCodeRepository _allCodeRepository)
        {
            configuration = _configuration;
            _redisService = new RedisConn(_configuration);
            _redisService.Connect();
            allCodeRepository = _allCodeRepository;

        }

        [HttpPost("province.json")]
        public async Task<ActionResult> GetProvinceList(string token)
        {
            #region Test
            //string p = "{\"confirm\":\"1\"}";
            //token = CommonHelper.Encode(p, configuration["DataBaseConfig:key_api:b2b"]);
            #endregion

            try
            {
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    string cache_name = CacheType.PROVINCE;
                    var j_province = await _redisService.GetAsync(cache_name, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                    if (j_province != null)
                    {
                        var province = JsonConvert.DeserializeObject<List<Province>>(j_province);
                        return Ok(new { status = ResponseType.SUCCESS, data = province });
                    }
                    else
                    {
                        var province = await allCodeRepository.GetProvinceList();
                        if (province.Count() > 0)
                        {
                            _redisService.Set(cache_name, JsonConvert.SerializeObject(province), Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                        }
                        return Ok(new { status = province.Count() > 0 ? ResponseType.SUCCESS : ResponseType.FAILED, data = province });
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
                LogHelper.InsertLogTelegram("GetProvinceList - LocationB2BController error with token " + token + " :  " + ex.ToString());
                return Ok(new { status = ResponseType.EMPTY, msg = "Error On Excution" });
            }
        }
        [HttpPost("district.json")]
        public async Task<ActionResult> getDistrictListByProvinceId(string token)
        {
            try
            {
                #region Test
                // string p = "{\"provinceId\":\"51\"}";
               // token = CommonHelper.Encode(p, configuration["DataBaseConfig:key_api:b2b"]);
                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    string province_id = objParr[0]["provinceId"].ToString();
                    string cache_name = CacheType.DISTRICT + province_id;
                    var j_data = await _redisService.GetAsync(cache_name, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                    if (j_data != null)
                    {
                        var obj = JsonConvert.DeserializeObject<List<District>>(j_data);
                        return Ok(new { status = ResponseType.SUCCESS, data = obj });
                    }
                    else
                    {
                        var district = await allCodeRepository.GetDistrictListByProvinceId(province_id);
                        if (district.Count() > 0)
                        {
                            _redisService.Set(cache_name, JsonConvert.SerializeObject(district), Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                        }
                        return Ok(new { status = district.Count() > 0 ? ResponseType.SUCCESS : ResponseType.FAILED, data = district });
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
                LogHelper.InsertLogTelegram("getDistrictListByProvinceId - LocationB2BController error with token " + token + " :  " + ex.ToString());
                return Ok(new { status = ResponseType.EMPTY, msg = "Error On Excution" });
            }
        }
        [HttpPost("ward.json")]
        public async Task<ActionResult> GetWardListByDistrictId(string token)
        {
            try
            {
                #region Test
                //string p = "{\"districtId\":\"522\"}";
                //token = CommonHelper.Encode(p, configuration["DataBaseConfig:key_api:b2b"]);
                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    string district_id = objParr[0]["districtId"].ToString();
                    string cache_name = CacheType.WARD + district_id;
                    var j_data = await _redisService.GetAsync(cache_name, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                    if (j_data != null)
                    {
                        var obj = JsonConvert.DeserializeObject<List<Ward>>(j_data);
                        return Ok(new { status = ResponseType.SUCCESS, data = obj });
                    }
                    else
                    {
                        var ward = await allCodeRepository.GetWardListByDistrictId(district_id);
                        if (ward.Count() > 0)
                        {
                            _redisService.Set(cache_name, JsonConvert.SerializeObject(ward), Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                        }
                        return Ok(new { status = ward.Count() > 0 ? ResponseType.SUCCESS : ResponseType.FAILED, data = ward });
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
                LogHelper.InsertLogTelegram("GetWardListByDistrictId - LocationB2BController error with token "+token+" :  " + ex.ToString());
                return Ok(new { status = ResponseType.EMPTY, msg="Error On Excution" });
            }
        }

    }
}
