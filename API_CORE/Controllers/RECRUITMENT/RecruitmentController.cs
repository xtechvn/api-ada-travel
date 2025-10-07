using Caching.RedisWorker;
using ENTITIES.ViewModels.Articles;
using ENTITIES.ViewModels.ArticleViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories;
using REPOSITORIES.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.Controllers.RECRUITMENT
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecruitmentController : ControllerBase {
        private readonly IRecruitmentRepository recruitmentRepository;
        public IConfiguration configuration;
        private readonly RedisConn _redisService;
        private readonly ITagRepository _tagRepository;
        private readonly IGroupProductRepository groupProductRepository;
        public RecruitmentController(IConfiguration config, IRecruitmentRepository _recruitmentRepository, RedisConn redisService, ITagRepository tagRepository, IGroupProductRepository _groupProductRepository)
        {
            configuration = config;
            recruitmentRepository = _recruitmentRepository;
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
                JArray objParr = null;
                if (!CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key ko hop le"
                    });
                }

                string db_type = "database";

                // ---- read input ----
                string rawCateIds = objParr[0]["category_ids"]?.ToString() ?? string.Empty;
                string title = objParr[0]["title"]?.ToString() ?? string.Empty;
                double min = Convert.ToDouble(objParr[0]["min"] ?? "-1");
                double max = Convert.ToDouble(objParr[0]["max"] ?? "-1");

                // ---- parse category_ids (CSV hoặc JSON array) ----
                List<int> categoryIds;
                if (!string.IsNullOrWhiteSpace(rawCateIds) && rawCateIds.TrimStart().StartsWith("["))
                {
                    // JSON array
                    categoryIds = JsonConvert.DeserializeObject<List<int>>(rawCateIds) ?? new List<int>();
                }
                else
                {
                    // CSV
                    categoryIds = (rawCateIds ?? "")
                                  .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                  .Select(s => { int v; return int.TryParse(s.Trim(), out v) ? (int?)v : null; })
                                  .Where(v => v.HasValue)
                                  .Select(v => v.Value)
                                  .ToList();
                }

                // nếu không có cate nào thì trả rỗng
                if (categoryIds.Count == 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        data = new List<ArticleFeModel>(),
                        msg = "No category selected."
                    });
                }

                // ---- chỉ cache khi KHÔNG có filter ----
                bool useCache = string.IsNullOrWhiteSpace(title) && min == -1 && max == -1;

                var list_article = new List<ArticleFeModel>();

                if (useCache)
                {
                    string cache_name = CacheType.Recruitment_CATEGORY_ID + string.Join("_", categoryIds);
                    var j_data = await _redisService.GetAsync(cache_name, Convert.ToInt32(configuration["Redis:Database:db_core"]));

                    if (j_data != null)
                    {
                        list_article = JsonConvert.DeserializeObject<List<ArticleFeModel>>(j_data) ?? new List<ArticleFeModel>();
                        db_type = "cache";
                    }
                    else
                    {
                        // gọi DB
                        list_article = await recruitmentRepository.getArticleListByCategoryId(
                            string.Join(",", categoryIds), title, min, max);

                        if (list_article?.Count > 0)
                        {
                            _redisService.Set(cache_name, JsonConvert.SerializeObject(list_article), Convert.ToInt32(configuration["Redis:Database:db_core"]));
                        }
                        db_type = "database";
                    }
                }
                else
                {
                    // Có filter → luôn query DB, KHÔNG dùng cache
                    list_article = await recruitmentRepository.getArticleListByCategoryId(
                        string.Join(",", categoryIds), title, min, max);
                    db_type = "database";
                }

                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = list_article ?? new List<ArticleFeModel>(),
                    msg = "Get " + db_type + " Successfully !!!"
                });
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



        [HttpPost("get-list-detail-by-categoryid.json")]
        public async Task<ActionResult> getListRecruitmentByCategoryId(string token)
        {
            try
            {
                // string j_param = "{'category_id':47,'page':1, 'size': 10}";
                // token = CommonHelper.Encode(j_param, configuration["DataBaseConfig:key_api:b2c"]);

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    string db_type = string.Empty;
                    int _category_id = Convert.ToInt32(objParr[0]["category_id"]);
                    //int page = Convert.ToInt32(objParr[0]["page"]);
                    //int size = Convert.ToInt32(objParr[0]["size"]);
                    //int take = (size <= 0) ? 10 : size;
                    //int skip = ((page - 1) <= 0) ? 0 : (page - 1) * take;
                    string cache_name = CacheType.Recruitment_CATEGORY_ID + _category_id;
                    string j_data = null;
                    try
                    {
                        j_data = await _redisService.GetAsync(cache_name, Convert.ToInt32(configuration["Redis:Database:db_core"]));
                    }
                    catch (Exception ex)
                    {
                        LogHelper.InsertLogTelegram("NewsController - getListArticleByCategoryId: " + ex + "\n Token: " + token);

                    }
                    List<ArticleGroupViewModel> group_product = null;

                    if (j_data != null)
                    {
                        group_product = JsonConvert.DeserializeObject<List<ArticleGroupViewModel>>(j_data);
                    }
                    else
                    {
                        group_product = await groupProductRepository.GetArticleCategoryByParentID(_category_id);
                        if (group_product.Count > 0)
                        {
                            try
                            {
                                _redisService.Set(cache_name, JsonConvert.SerializeObject(group_product), Convert.ToInt32(configuration["Redis:Database:db_core"]));
                            }
                            catch (Exception ex)
                            {
                                LogHelper.InsertLogTelegram("NewsController - GetAllCategory: " + ex + "\n Token: " + token);

                            }
                        }
                    }

                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Success",
                        categories = group_product
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
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
                    string cache_name = CacheType.Recruitment_ID + article_id;
                    var j_data = await _redisService.GetAsync(cache_name, Convert.ToInt32(configuration["Redis:Database:db_core"]));
                    var detail = new ArticleFeDetailModel();

                    if (j_data != null)
                    {
                        detail = JsonConvert.DeserializeObject<ArticleFeDetailModel>(j_data);
                        db_type = "cache";
                    }
                    else
                    {
                        detail = await recruitmentRepository.GetArticleDetailLite(article_id);
                        //detail.Tags = await _tagRepository.GetAllTagByArticleID(article_id);
                        if (detail != null)
                        {
                            _redisService.Set(cache_name, JsonConvert.SerializeObject(detail), Convert.ToInt32(configuration["Redis:Database:db_core"]));
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

                    detail = await recruitmentRepository.FindArticleByTitle(title, parent_cate_faq_id);

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
                string j_param = "{'category_id':'37','skip':1,'take':30}";
                token = CommonHelper.Encode(j_param, configuration["DataBaseConfig:key_api:b2c"]);

                JArray objParr = null;
                string msg = "";
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    string db_type = string.Empty;
                    int category_id = Convert.ToInt32(objParr[0]["category_id"]);
                    int skip = Convert.ToInt32(objParr[0]["skip"]);
                    int take = Convert.ToInt32(objParr[0]["take"]);
                    string cache_key = CacheType.CATEGORY_NEWS + category_id;
                    var j_data = await _redisService.GetAsync(cache_key, Convert.ToInt32(configuration["Redis:Database:db_core"]));
                    List<ArticleFeModel> data_list;
                    int total_count = -1;
                    if (j_data == null || j_data == "")
                    {
                        var group_product = await groupProductRepository.GetGroupProductNameAsync(category_id);
                        var data_100 = await recruitmentRepository.getArticleListByCategoryIdOrderByDate(category_id, 0, 100, group_product);
                        if (skip + take > 100)
                        {
                            var data = await recruitmentRepository.getArticleListByCategoryIdOrderByDate(category_id, skip, take, group_product);
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

                        _redisService.Set(cache_key, JsonConvert.SerializeObject(data_100), DateTime.Now.AddMinutes(15), Convert.ToInt32(configuration["Redis:Database:db_core"]));

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
                            var data = await recruitmentRepository.getArticleListByCategoryIdOrderByDate(category_id, skip, take, group_product);
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
