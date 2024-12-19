using Caching.RedisWorker;
using ENTITIES.ViewModels.ArticleViewModels;
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

namespace API_CORE.Controllers.NEWS
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase {
        private readonly IArticleRepository articleRepository;
        public IConfiguration configuration;
        private readonly RedisConn _redisService;
        private readonly ITagRepository _tagRepository;
        private readonly IGroupProductRepository groupProductRepository;
        public NewsController(IConfiguration config, IArticleRepository _articleRepository, RedisConn redisService, ITagRepository tagRepository, IGroupProductRepository _groupProductRepository)
        {
            configuration = config;
            articleRepository = _articleRepository;
            _redisService = redisService;
            _tagRepository = tagRepository;
            groupProductRepository = _groupProductRepository;
        }

        /// <summary>
        /// Lấy ra bài viết theo 1 chuyên mục
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("get-list-by-categoryid.json")]
        public async Task<ActionResult> getListArticleByCategoryId(string token)
        {
            try
            {
                // string j_param = "{'category_id':50}";
                // token = CommonHelper.Encode(j_param, configuration["DataBaseConfig:key_api:b2c"]);
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    string db_type = string.Empty;
                    int _category_id = Convert.ToInt32(objParr[0]["category_id"]);
                    string cache_name = CacheType.ARTICLE_CATEGORY_ID + _category_id;
                    var j_data = await _redisService.GetAsync(cache_name, Convert.ToInt32(configuration["Redis:Database:db_common"]));
                    var list_article = new List<ArticleFeModel>();

                    if (j_data != null)
                    {
                        list_article = JsonConvert.DeserializeObject<List<ArticleFeModel>>(j_data);
                        db_type = "cache";
                    }
                    else
                    {
                        list_article = await articleRepository.getArticleListByCategoryId(_category_id);
                        if (list_article.Count() > 0)
                        {
                            _redisService.Set(cache_name, JsonConvert.SerializeObject(list_article), Convert.ToInt32(configuration["Redis:Database:db_common"]));
                        }
                        db_type = "database";
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        data_list = list_article,
                        category_id = _category_id,
                        msg = "Get " + db_type + " Successfully !!!"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key ko hop le"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("NewsController - getListArticleByCategoryId: " + ex + "\n Token: " + token);
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Error: " + ex.ToString(),
                });
            }
        }
       
        [HttpPost("get-detail.json")]
        public async Task<ActionResult> GetArticleDetailLite(string token)
        {
            try
            {
               // string j_param = "{'article_id':71}";


               // token = CommonHelper.Encode(j_param, configuration["DataBaseConfig:key_api:b2c"]);

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    string db_type = string.Empty;
                    long article_id = Convert.ToInt64(objParr[0]["article_id"]);
                    string cache_name = CacheType.ARTICLE_ID + article_id;
                    var j_data = await _redisService.GetAsync(cache_name, Convert.ToInt32(configuration["Redis:Database:db_common"]));
                    var detail = new ArticleFeDetailModel();

                    if (j_data != null)
                    {
                        detail = JsonConvert.DeserializeObject<ArticleFeDetailModel>(j_data);
                        db_type = "cache";
                    }
                    else
                    {
                        detail = await articleRepository.GetArticleDetailLite(article_id);
                        detail.Tags = await _tagRepository.GetAllTagByArticleID(article_id);
                        if (detail != null)
                        {
                            _redisService.Set(cache_name, JsonConvert.SerializeObject(detail), Convert.ToInt32(configuration["Redis:Database:db_common"]));
                            db_type = "database";
                        }

                    }
                  
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        data = detail,
                        msg = "Get " + db_type + " Successfully !!!",
                        _token = token
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key ko hop le"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("get-detail.json - NewsController " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "[api/article/detail] = " + ex.ToString(),
                    _token = token
                });
            }
        }

        [HttpPost("find-article.json")]
        public async Task<ActionResult> FindArticleByTitle(string token)
        {
            try
            {
                // string j_param = "{'title':'54544544444','parent_cate_faq_id':279}";
                // token = CommonHelper.Encode(j_param, configuration["KEY_TOKEN_API"]);

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    string db_type = "database";
                    string title = (objParr[0]["title"]).ToString().Trim();
                    int parent_cate_faq_id = Convert.ToInt32(objParr[0]["parent_cate_faq_id"]);

                    var detail = new List<ArticleRelationModel>();

                    detail = await articleRepository.FindArticleByTitle(title, parent_cate_faq_id);

                    return Ok(new
                    {
                        status = detail.Count() > 0 ? (int)ResponseType.SUCCESS : (int)ResponseType.EMPTY,
                        data_list = detail.Count() > 0 ? detail : null,
                        msg = "Get " + db_type + " Successfully !!!",
                        _token = token
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key ko hop le"
                    });
                }
            }
            catch (Exception ex)
            {

                LogHelper.InsertLogTelegram("find-article.json - NewsController " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "find-article.json = " + ex.ToString(),
                    _token = token
                });
            }
        }

        /// <summary>
        /// Lấy ra bài viết theo 1 chuyên mục, skip+take, sắp xếp theo ngày gần nhất
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("get-list-by-categoryid-order.json")]
        public async Task<ActionResult> getListArticleByCategoryIdOrderByDate(string token)
        {
            try
            {
                JArray objParr = null;
                string msg = "";
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["KEY_TOKEN_API"]))
                {
                    string db_type = string.Empty;
                    int category_id = Convert.ToInt32(objParr[0]["category_id"]);
                    int skip = Convert.ToInt32(objParr[0]["skip"]);
                    int take = Convert.ToInt32(objParr[0]["take"]);
                    string cache_key = CacheType.CATEGORY_NEWS + category_id;
                    var j_data = await _redisService.GetAsync(cache_key, Convert.ToInt32(configuration["Redis:Database:db_common"]));
                    List<ArticleFeModel> data_list;
                    int total_count = -1;
                    if (j_data == null || j_data == "")
                    {
                        var group_product = await groupProductRepository.GetGroupProductNameAsync(category_id);
                        var data_100 = await articleRepository.getArticleListByCategoryIdOrderByDate(category_id, 0, 100, group_product);
                        if (skip + take > 100)
                        {
                            var data = await articleRepository.getArticleListByCategoryIdOrderByDate(category_id, skip, take, group_product);
                            data_list = data.list_article_fe;
                            total_count = data.total_item_count;
                        }
                        else
                        {
                            data_list = data_100.list_article_fe.Skip(skip).Take(take).ToList();
                            total_count = data_100.total_item_count;

                        }
                        //-- If is home Category, Add Pinned Article:
                        if (category_id == 401)
                        {

                        }

                        _redisService.Set(cache_key, JsonConvert.SerializeObject(data_100), DateTime.Now.AddMinutes(15), Convert.ToInt32(configuration["Redis:Database:db_common"]));

                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            data_list = data_list,
                            total_item = total_count
                        });

                        //return Content(JsonConvert.SerializeObject(data_list));
                    }
                    else
                    {
                        var group_product = await groupProductRepository.GetGroupProductNameAsync(category_id);

                        if (skip + take > 100)
                        {
                            var data = await articleRepository.getArticleListByCategoryIdOrderByDate(category_id, skip, take, group_product);
                            data_list = data.list_article_fe;
                            total_count = data.total_item_count;
                        }
                        else
                        {
                            var data_100 = JsonConvert.DeserializeObject<ArticleFEModelPagnition>(j_data);
                            data_list = data_100.list_article_fe.Skip(skip).Take(take).ToList();
                            total_count = data_100.total_item_count;
                        }

                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            data_list = data_list,
                            total_item = total_count
                        });
                        // return Content(JsonConvert.SerializeObject(data_list));
                    }

                }
                else
                {
                    msg = "Key ko hop le";
                }
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = msg
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("get-list-by-categoryid-order.json - NewsController " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "Error on Excution.",
                    _token = token
                });
            }
        }
    }
}
