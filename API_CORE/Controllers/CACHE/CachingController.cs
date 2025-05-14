using API_CORE.Controllers.ORDER.Base;
using Caching.RedisWorker;
using ENTITIES.ViewModels.Order;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.Controllers.CACHE
{
    [Route("api/[controller]")]
    [ApiController]
    public class CachingController : Controller
    {
        private IConfiguration configuration;
        private readonly RedisConn redisService;
        private IOrderRepository ordersRepository;
        private readonly IAccountB2BRepository accountB2BRepository;
        private readonly IAccountRepository accountRepository;
        public CachingController(IConfiguration _configuration, RedisConn _redisService, IOrderRepository _ordersRepository, IAccountB2BRepository _accountB2BRepository, IAccountRepository _accountRepository)
        {
            configuration = _configuration;
            redisService = _redisService;
            ordersRepository = _ordersRepository;
            accountB2BRepository = _accountB2BRepository;
            accountRepository = _accountRepository;
        }


        [HttpPost("clear.json")]
        public async Task<ActionResult> clearCacheByType(string token)
        {
            try
            {
                JArray objParr = null;
                string _msg = string.Empty;
                int db_cache = Convert.ToInt32(configuration["Redis:Database:db_core"]);

                #region
                var j_param = new Dictionary<string, object>
                {
                    {"cache_name", "order_client_id_102"},
                    {"cache_type","1"},
                    {"db_index",db_cache.ToString()},
                    {"data_load","{client_id:-1,source_type:2}"}
                };
                var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion

                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    string cache_name = objParr[0]["cache_name"].ToString(); // tên cache cần xóa
                    int cache_type = Convert.ToInt16(objParr[0]["cache_type"]); // phân biệt loại cache là xóa hết hay xóa xong rồi nạp
                    int db_index = Convert.ToInt16(objParr[0]["db_index"]); // phân vùng chứa cache
                    var data_load = JsonConvert.DeserializeObject<OrderViewCaching>(objParr[0]["data_load"].ToString()); // Là chuẩn json. Chưa các input để nạp cache khi cache_type = 1
                    switch (cache_type)
                    {
                        case CacheType.REMOVE_AND_RE_LOAD: // voi truong hop đặc biệt cần nạp cache sau khi xóa
                            if (cache_name.IndexOf("order_client_id_") >= 0)
                            {
                                // thuc hien xoa cache
                                redisService.clear(cache_name, db_index);
                                //thực hiện nạp cache
                                OrderService order = new OrderService(configuration, ordersRepository, redisService, accountB2BRepository, accountRepository);
                                var obj_lst_order = await order.getOrderByClientId(data_load.client_id, data_load.source_type);

                                // thuc hien xoa cache 
                                redisService.clear("order_client_id_-1", db_index);
                                //thực hiện nạp cache
                                var obj_lst_order2 = await order.getOrderByClientId(-1, data_load.source_type);
                            }

                            _msg = "Xóa và nạp cache thành công. Cache name = " + cache_name;
                            break;
                        default:
                            // Mac dinh xoa cache
                            redisService.clear(cache_name, db_index);
                            _msg = "Xóa  cache thành công. Cache name = " + cache_name;
                            break;
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = _msg
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
                LogHelper.InsertLogTelegram("cache/clear.json : " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = ex.ToString()
                });
            }

        }
        //xóa cache bankonepay
        [HttpPost("clear/bankonepay.json")]
        public async Task<ActionResult> clearBankonepay(string token)
        {
            try
            {
                JArray objParr = null;
                string _msg = string.Empty;
                int db_cache = Convert.ToInt32(configuration["Redis:Database:db_core"]);
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    string cache_name = CacheName.Bank_One_Pay; // tên cache cần xóa                    
                    int db_index = Convert.ToInt16(objParr[0]["db_index"]); // phân vùng chứa cache                   
                            // Mac dinh xoa cache
                            redisService.clear(cache_name, db_index);
                            _msg = "Xóa  cache thành công. Cache name = " + cache_name;
     
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = _msg
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
                LogHelper.InsertLogTelegram("cache/clear.json : " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = ex.ToString()
                });
            }
        }
        [HttpPost("clear_cache_by_key.json")]
        public async Task<ActionResult> clearCacheByKey(string token)
        {
            try
            {
                //  string j_param = "{'cache_key':'abcdef'}";
                //  token = CommonHelper.Encode(j_param, configuration["KEY_TOKEN_API"]);

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    string cache_key = objParr[0]["cache_key"].ToString();

                    #region Clear a cache
                    redisService.clear(cache_key, Convert.ToInt32(configuration["Redis:Database:db_core"]));
                    #endregion
                    return Ok(new { status = (int)ResponseType.SUCCESS, _token = token, msg = "Clear Successfully !!!", cache_key = cache_key });
                }
                else
                {
                    return Ok(new { status = (int)ResponseType.FAILED, _token = token, msg = "Token Error !!!" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("api-clear.json - clearCacheByKey " + ex.Message + " token=" + token.ToString());
                return Ok(new { status = (int)ResponseType.ERROR, _token = token, msg = "Sync error !!!" });
            }
        }
        /// <summary>
        /// Clear cache bài viết
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("sync-article.json")]
        public async Task<ActionResult> clearCacheArticle(string token)
        {
            try
            {
                string j_param = "{'article_id':'39','category_id':'35'}";
                //  token = CommonHelper.Encode(j_param, configuration["DataBaseConfig:key_api:api_manual"]);

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
                {

                    long article_id = Convert.ToInt64(objParr[0]["article_id"]);
                    var category_list_id = objParr[0]["category_id"].ToString().Split(",");
                    redisService.clear(CacheType.ARTICLE_ID + article_id, Convert.ToInt32(configuration["Redis:Database:db_core"]));
                    for (int i = 0; i <= category_list_id.Length - 1; i++)
                    {
                        int category_id = Convert.ToInt32(category_list_id[i]);
                        redisService.clear(CacheType.ARTICLE_CATEGORY_ID + category_id, Convert.ToInt32(configuration["Redis:Database:db_core"]));
                        redisService.clear(CacheType.CATEGORY_NEWS + "39", Convert.ToInt32(configuration["Redis:Database:db_core"]));
                        redisService.clear(CacheType.CATEGORY_NEWS + category_id, Convert.ToInt32(configuration["Redis:Database:db_core"]));
                        redisService.clear(CacheType.ARTICLE_B2C_MOST_VIEWED, Convert.ToInt32(configuration["Redis:Database:db_core"]));
                    }

                    return Ok(new { status = (int)ResponseType.SUCCESS, _token = token, msg = "Sync Successfully !!!", article_id = article_id, category_list_id = category_list_id });
                }
                else
                {
                    return Ok(new { status = (int)ResponseType.FAILED, _token = token, msg = "Token Error !!!" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("sync-article.json - clearCacheArticle " + ex.Message + " token=" + token.ToString());
                return Ok(new { status = (int)ResponseType.ERROR, _token = token, msg = "Sync error !!!" });
            }
        }
    }
}
