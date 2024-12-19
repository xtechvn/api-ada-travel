using API_CORE.Service.Log;
using Caching.RedisWorker;
using ENTITIES.Models;
using Microsoft.AspNetCore.Cors;
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

namespace API_CORE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllCodeController : ControllerBase
    {
        private IConfiguration configuration;
        private IAllCodeRepository  allCodeRepository;
        private ITelegramRepository  telegramRepository;
        private readonly RedisConn _redisService;

        public AllCodeController(IConfiguration _configuration, IAllCodeRepository _allCodeRepository, ITelegramRepository _telegramRepository)
        {
            configuration = _configuration;
            allCodeRepository = _allCodeRepository;
            telegramRepository = _telegramRepository;
            _redisService = new RedisConn(_configuration);
            _redisService.Connect();
        }
        [EnableCors("MyApi")]
        [HttpPost("service/get-telegram-bot.json")]
        public async Task<ActionResult> GetTelegramBotList (string token)
        {
            
            try
            {
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    List<TelegramDetail> data = await telegramRepository.GetAllBotList();

                    return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), msg = "Success", data = data });
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
                TeleLog.InsertLogTelegram(configuration,ex.Message);
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "error: " + ex.ToString() });
            }
        }
        [EnableCors("MyApi")]
        [HttpPost("service/gen-api-token.json")]
        public async Task<ActionResult> GetTelegramBotList(string plaintext, string key)
        {

            try
            {
                if (key == "JBT3Fi4Qs4Q2sTL7FNaH4kBtIDx7uWZ8uF8Acvgfg9tRqEVZU9")
                {
                    
                    string token = CommonHelper.Encode(plaintext, configuration["DataBaseConfig:key_api:api_manual"]);
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Success",
                        token = token
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
                TeleLog.InsertLogTelegram(configuration,"service/gen-api-token.json: "+ ex.Message);
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "error: " + ex.ToString() });
            }
        }
        [HttpPost("service/clear-cache.json")]
        public async Task<ActionResult> ClearCacheName(string token)
        {

            try
            {
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    string cache_name = objParr[0]["cache_name"].ToString();
                    int dbindex = Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_core"]);
                    try
                    {
                        dbindex = Convert.ToInt32(objParr[0]["db_index"].ToString());
                    }
                    catch
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Key invalid!"
                        });
                    }
                    if(cache_name!=null && cache_name.Trim() != "")
                    {
                        _redisService.clear(cache_name, dbindex);
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Success: " + cache_name
                    });
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
                TeleLog.InsertLogTelegram(configuration,"service/gen-api-token.json: " + ex.Message);
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "error: " + ex.ToString() });
            }
        }
        [EnableCors("MyApi")]
        [HttpPost("service/get-allcode.json")]
        public async Task<ActionResult> GetAllCodeByType(string token)
        {

            try
            {
                //string p = "{\"type\":\"" + AllCodeType.PAYMENT_TYPE + "\"}";
                //token = CommonHelper.Encode(p, configuration["DataBaseConfig:key_api:api_manual"]);
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    string type = objParr[0]["type"].ToString();
                    List<AllCode> data = await allCodeRepository.GetAllCodeByType(type);

                    return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), msg = "Success", data = data });
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
                TeleLog.InsertLogTelegram(configuration,ex.Message);
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "error: " + ex.ToString() });
            }
        }
        [HttpPost("province.json")]
        public async Task<ActionResult> GetProvinceList(string token)
        {
            try
            {
                //string p = "{\"province\":\"0\"}";
                //token = CommonHelper.Encode(p, configuration["DataBaseConfig:key_api:api_manual"]);
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
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
                        return Ok(new { status = province.Count() > 0 ? ResponseType.SUCCESS : ResponseType.ERROR, data = province });
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
                LogHelper.InsertLogTelegram("[api==>>] GetProvinceList==> error:  " + ex.Message);
                return Ok(new { status = ResponseType.EMPTY, msg = ex.ToString() });
            }
        }
        [HttpPost("district.json")]
        public async Task<ActionResult> getDistrictListByProvinceId(string token)
        {
            try
            {
                //string p="{\"provinceId\":\"51\"}";
                //token = CommonHelper.Encode(p, configuration["DataBaseConfig:key_api:api_manual"]);
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
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
                        return Ok(new { status = district.Count() > 0 ? ResponseType.SUCCESS : ResponseType.ERROR, data = district });
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
                LogHelper.InsertLogTelegram("[api==>>] getDistrictListByProvinceId==> error:  " + ex.Message);
                return Ok(new { status = ResponseType.EMPTY, msg = ex.ToString() });
            }
        }
        [HttpPost("ward.json")]
        public async Task<ActionResult> GetWardListByDistrictId(string token)
        {
            try
            {
                //string p = "{\"districtId\":\"522\"}";
                //token = CommonHelper.Encode(p, configuration["DataBaseConfig:key_api:api_manual"]);
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
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
                        return Ok(new { status = ward.Count() > 0 ? ResponseType.SUCCESS : ResponseType.ERROR, data = ward });
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
                LogHelper.InsertLogTelegram("[api==>>] GetWardListByDistrictId==> error:  " + ex.Message);
                return Ok(new { status = ResponseType.EMPTY, msg = ex.ToString() });
            }
        }
    }
}
