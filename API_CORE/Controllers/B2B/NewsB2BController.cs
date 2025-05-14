using Caching.RedisWorker;
using DAL.MongoDB;
using ENTITIES.ViewModels.Articles;
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
    [Route("api/b2b/news")]
    [ApiController]
    public class NewsB2BController : ControllerBase
    {
        private readonly IArticleRepository articleRepository;
        public IConfiguration configuration;
        private readonly RedisConn _redisService;
        private readonly ITagRepository _tagRepository;
        private readonly IGroupProductRepository groupProductRepository;
        public NewsB2BController(IConfiguration config, IArticleRepository _articleRepository, ITagRepository tagRepository, IGroupProductRepository _groupProductRepository)
        {
            configuration = config;
            articleRepository = _articleRepository;
            _redisService = new RedisConn(config);
            _redisService.Connect();
            _tagRepository = tagRepository;
            groupProductRepository = _groupProductRepository;

        }

        [HttpPost("get-detail.json")]
        public async Task<ActionResult> GetArticleDetailLite(string token)
        {
            try
            {
                //string j_param = "{'article_id':71}";


                //token = CommonHelper.Encode(j_param, configuration["DataBaseConfig:key_api:b2b"]);

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    string db_type = string.Empty;
                    long article_id = Convert.ToInt64(objParr[0]["article_id"]);
                    string cache_name = CacheType.ARTICLE_ID + article_id;
                    string j_data = null;
                    try
                    {
                        j_data = await _redisService.GetAsync(cache_name, Convert.ToInt32(configuration["Redis:Database:db_core"]));
                    }
                    catch (Exception ex)
                    {
                        LogHelper.InsertLogTelegram("NewsController - GetArticleDetailLite: " + ex + "\n Token: " + token);

                    }
                    var detail = new ArticleFeDetailModel();

                    if (j_data != null)
                    {
                        detail = JsonConvert.DeserializeObject<ArticleFeDetailModel>(j_data);
                        db_type = "cache";
                    }
                    else
                    {
                        detail = await articleRepository.GetArticleDetailLite(article_id);
                        if (detail != null)
                        {
                            var tags = await _tagRepository.GetAllTagByArticleID(article_id);
                            detail.Tags = tags == null ? new List<string>() : tags;
                            try
                            {
                                _redisService.Set(cache_name, JsonConvert.SerializeObject(detail), Convert.ToInt32(configuration["Redis:Database:db_core"]));
                            }
                            catch (Exception ex)
                            {
                                LogHelper.InsertLogTelegram("NewsController - GetArticleDetailLite: " + ex + "\n Token: " + token);

                            }
                            db_type = "database";
                        }
                        else
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.FAILED,
                                data = detail,
                                msg = "Article ID: " + article_id + " not found.",
                                _token = token
                            });
                        }

                    }
                    //-- Update view_count:
                    var view_count = new NewsViewCount()
                    {
                        articleID = article_id,
                        pageview = 1
                    };
                    NewsMongoService services = new NewsMongoService(configuration);
                    services.AddNewOrReplace(view_count);

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
    }
}
