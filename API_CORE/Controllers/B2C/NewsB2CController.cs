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
    [Route("api/b2c/news")]
    [ApiController]
    public class NewsB2CController : ControllerBase
    {
        private readonly IArticleRepository articleRepository;
        public IConfiguration configuration;
        private readonly RedisConn _redisService;
        private readonly ITagRepository _tagRepository;
        private readonly IGroupProductRepository groupProductRepository;
        public NewsB2CController(IConfiguration config, IArticleRepository _articleRepository, ITagRepository tagRepository, IGroupProductRepository _groupProductRepository)
        {
            configuration = config;
            articleRepository = _articleRepository;
            _redisService = new RedisConn(config);
            _redisService.Connect();
            _tagRepository = tagRepository;
            groupProductRepository = _groupProductRepository;

        }

        /// <summary>
        /// Lấy ra tất cả các chuyên mục thuộc B2C
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("get-category.json")]
        public async Task<ActionResult> GetAllCategory(string token)
        {
            try
            {
                //string j_param = "{'confirm':1}";
                //token = CommonHelper.Encode(j_param, configuration["DataBaseConfig:key_api:b2c"]);
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    int _category_id = Convert.ToInt32(objParr[0]["category_id"]);
                    string cache_name = CacheType.ARTICLE_B2C_CATEGORY_MENU;
                    string j_data = null;
                    try
                    {
                        j_data = await _redisService.GetAsync(cache_name, Convert.ToInt32(configuration["Redis:Database:db_common"]));
                    }
                    catch (Exception ex)
                    {
                        LogHelper.InsertLogTelegram("NewsController - GetMostViewedArticle: " + ex + "\n Token: " + token);

                    }
                    List<ArticleGroupViewModel> group_product = null;

                    if (j_data != null)
                    {
                        group_product = JsonConvert.DeserializeObject<List<ArticleGroupViewModel>>(j_data);
                    }
                    else
                    {
                        group_product = await groupProductRepository.GetArticleCategoryByParentID(Convert.ToInt64(configuration["config_value:default_b2c_news_root_group"]));
                        if (group_product.Count > 0)
                        {
                            try
                            {
                                _redisService.Set(cache_name, JsonConvert.SerializeObject(group_product), Convert.ToInt32(configuration["Redis:Database:db_common"]));
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
                        status = (int)ResponseType.ERROR,
                        msg = "Key ko hop le"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("NewsController - GetAllCategory: " + ex + "\n Token: " + token);
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Error: " + ex.ToString(),
                });
            }
        }

        [HttpPost("get-most-viewed-article.json")]
        public async Task<ActionResult> GetMostViewedArticle(string token)
        {
            try
            {
                int status = (int)ResponseType.FAILED;
                string msg = "No Item Found";
                var data_list = new List<ArticleFeModel>();
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    string cache_name = CacheType.ARTICLE_B2C_MOST_VIEWED;
                    string j_data = null;
                    try
                    {
                        j_data = await _redisService.GetAsync(cache_name, Convert.ToInt32(configuration["Redis:Database:db_common"]));
                    }
                    catch (Exception ex)
                    {
                        LogHelper.InsertLogTelegram("NewsController - GetMostViewedArticle: " + ex + "\n Token: " + token);

                    }
                    var detail = new ArticleFeModel();

                    if (j_data != null)
                    {
                        data_list = JsonConvert.DeserializeObject<List<ArticleFeModel>>(j_data);
                        msg = "Get From Cache Success";

                    }
                    else
                    {
                        NewsMongoService services = new NewsMongoService(configuration);
                        var list = await services.GetMostViewedArticle();
                        if (list != null && list.Count > 0)
                        {
                            foreach (var item in list)
                            {
                                var article = await articleRepository.GetMostViewedArticle(item.articleID);
                                if (article != null) data_list.Add(article);
                            }
                            try
                            {
                                _redisService.Set(cache_name, JsonConvert.SerializeObject(data_list), DateTime.Now.AddMinutes(5), Convert.ToInt32(configuration["Redis:Database:db_common"]));
                            }
                            catch (Exception ex)
                            {
                                LogHelper.InsertLogTelegram("NewsController - GetMostViewedArticle: " + ex + "\n Token: " + token);

                            }
                            status = (int)ResponseType.SUCCESS;
                            msg = "Get from DB Success";
                        }
                    }
                    return Ok(new { status = (int)ResponseType.SUCCESS, msg = msg, data = data_list });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Token không hợp lệ"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("NewsController - GetMostViewedArticle: " + ex + " token = " + token);
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "Error on Excution"
                });
            }
        }
        [HttpPost("get-detail.json")]
        public async Task<ActionResult> GetArticleDetailLite(string token)
        {
            try
            {
                //string j_param = "{'article_id':71}";


                //token = CommonHelper.Encode(j_param, configuration["DataBaseConfig:key_api:b2c"]);

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    string db_type = string.Empty;
                    long article_id = Convert.ToInt64(objParr[0]["article_id"]);
                    string cache_name = CacheType.ARTICLE_ID + article_id;
                    string j_data = null;
                    try
                    {
                        j_data = await _redisService.GetAsync(cache_name, Convert.ToInt32(configuration["Redis:Database:db_common"]));
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
                                _redisService.Set(cache_name, JsonConvert.SerializeObject(detail), Convert.ToInt32(configuration["Redis:Database:db_common"]));
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

        [HttpPost("find-article.json")]
        public async Task<ActionResult> FindArticleByTitle(string token)
        {
            try
            {
                //string j_param = "{'title':'KHÁM','parent_id':39,'size':10, 'page': 1}";
                //token = CommonHelper.Encode(j_param, configuration["DataBaseConfig:key_api:b2c"]);

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    string db_type = "database";
                    string title = (objParr[0]["title"]).ToString().Trim();
                    int parent_cate_faq_id = Convert.ToInt32(objParr[0]["parent_id"]);
                    int page = Convert.ToInt32(objParr[0]["page"]);
                    int size = Convert.ToInt32(objParr[0]["size"]);
                    int take = (size <= 0) ? 10 : size;
                    int skip = ((page - 1) <= 0) ? 0 : (page - 1) * take;
                    string cache_key = CacheType.CATEGORY_SEARCH + EncodeHelpers.MD5Hash(title.Trim().ToLower() + parent_cate_faq_id);
                    string j_data = null;
                    try
                    {
                        j_data = await _redisService.GetAsync(cache_key, Convert.ToInt32(configuration["Redis:Database:db_common"]));
                    }
                    catch (Exception ex)
                    {
                        LogHelper.InsertLogTelegram("NewsController - FindArticleByTitle: " + ex + "\n Token: " + token);

                    }
                    int total_count = -1;
                    int total_page = 1;
                    List<ArticleRelationModel> data_list;

                    if (j_data != null && j_data != "")
                    {
                        var data = JsonConvert.DeserializeObject<List<ArticleRelationModel>>(j_data);
                        data_list = (skip + take > data.Count) ? data.Skip(skip).ToList() : data.Skip(skip).Take(take).ToList();
                        total_count = data.Count;
                        total_page = Convert.ToInt32(total_count / take);
                        if (total_page < ((float)total_count / take))
                        {
                            total_page++;
                        }
                        try
                        {
                            _redisService.Set(cache_key, JsonConvert.SerializeObject(data), DateTime.Now.AddMinutes(15), Convert.ToInt32(configuration["Redis:Database:db_common"]));
                        }
                        catch (Exception ex)
                        {
                            LogHelper.InsertLogTelegram("NewsController - FindArticleByTitle: " + ex + "\n Token: " + token);

                        }
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            data = data_list,
                            total_item = total_count,
                            total_page = total_page

                        });

                    }
                    else
                    {
                        var data = await articleRepository.FindArticleByTitle(title, parent_cate_faq_id);
                        if (data != null && data.Count>0)
                        {
                            data_list = (skip + take > data.Count) ? data.Skip(skip).ToList() : data.Skip(skip).Take(take).ToList();
                            total_count = data.Count;
                            total_page = Convert.ToInt32(total_count / take);
                            if (total_page < ((float)total_count / take))
                            {
                                total_page++;
                            }
                            _redisService.Set(cache_key, JsonConvert.SerializeObject(data), DateTime.Now.AddMinutes(15), Convert.ToInt32(configuration["Redis:Database:db_common"]));
                        }
                        else
                        {
                            data_list = new List<ArticleRelationModel>();
                            total_count = 0;
                            total_page = 0;
                        }
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            data = data_list,
                            total_item = total_count,
                            total_page = total_page

                        });
                    }


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
               // string j_param = "{'category_id':50,'page':1, 'size': 8}";
               // token = CommonHelper.Encode(j_param, configuration["DataBaseConfig:key_api:b2c"]);

                JArray objParr = null;
                string msg = "";
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    string db_type = string.Empty;
                    int category_id = Convert.ToInt32(objParr[0]["category_id"]);
                    int page = Convert.ToInt32(objParr[0]["page"]);
                    int size = Convert.ToInt32(objParr[0]["size"]);
                    int take = (size <= 0) ? 10 : size;
                    int skip = ((page - 1) <= 0) ? 0 : (page - 1) * take;
                    string cache_key = CacheType.CATEGORY_NEWS + category_id;
                    string j_data = null;
                    try
                    {
                        j_data = await _redisService.GetAsync(cache_key, Convert.ToInt32(configuration["Redis:Database:db_common"]));
                    }
                    catch (Exception ex)
                    {
                        LogHelper.InsertLogTelegram("NewsController - getListArticleByCategoryIdOrderByDate: " + ex + "\n Token: " + token);

                    }
                    List<ArticleFeModel> data_list;
                    List<ArticleFeModel> pinned_article;
                    List<ArticleFeModel> video_article;
                    int total_count = -1;
                    int total_page = 1;
                    if (j_data == null || j_data == "")
                    {
                        var group_product = await groupProductRepository.GetGroupProductNameAsync(category_id);
                        var data_100 = await articleRepository.getArticleListByCategoryIdOrderByDate(category_id, 0, 100, group_product);
                        if (skip + take > 100)
                        {
                            var data = await articleRepository.getArticleListByCategoryIdOrderByDate(category_id, skip, take, group_product);
                            data_list = data.list_article_fe;
                            total_count = data.total_item_count;
                            pinned_article = data.list_article_pinned;
                            total_page = Convert.ToInt32(total_count / take);
                            if (total_page < ((float)total_count / take))
                            {
                                total_page++;
                            }
                        }
                        else
                        {
                            data_list = data_100.list_article_fe.Skip(skip).Take(take).ToList();
                            total_count = data_100.total_item_count;
                            pinned_article = data_100.list_article_pinned;
                            total_page = Convert.ToInt32(total_count / take);
                            if (total_page < ((float)total_count / take))
                            {
                                total_page++;
                            }
                        }


                        try
                        {
                            _redisService.Set(cache_key, JsonConvert.SerializeObject(data_100), DateTime.Now.AddMinutes(15), Convert.ToInt32(configuration["Redis:Database:db_common"]));
                        }
                        catch (Exception ex)
                        {
                            LogHelper.InsertLogTelegram("NewsController - getListArticleByCategoryIdOrderByDate: " + ex + "\n Token: " + token);

                        }
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            data = data_list,
                            pinned = pinned_article,
                            total_item = total_count,
                            total_page = total_page

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
                            pinned_article = data.list_article_pinned;
                            total_page = Convert.ToInt32(total_count / take);
                            if (total_page < ((float)total_count / take))
                            {
                                total_page++;
                            }
                        }
                        else
                        {
                            var data_100 = JsonConvert.DeserializeObject<ArticleFEModelPagnition>(j_data);
                            data_list = data_100.list_article_fe.Skip(skip).Take(take).ToList();
                            total_count = data_100.total_item_count;
                            pinned_article = data_100.list_article_pinned;
                            total_page = Convert.ToInt32(total_count / take);
                            if (total_page < ((float)total_count / take))
                            {
                                total_page++;
                            }
                        }

                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            data = data_list,
                            pinned = pinned_article,
                            total_item = total_count,
                            total_page = total_page
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
        /// <summary>
        /// Lấy ra bài viết theo tag, skip+take, sắp xếp theo ngày gần nhất
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("get-list-by-tag-order.json")]
        public async Task<ActionResult> getListArticleByTagsOrder(string token)
        {

            try
            {
                //string j_param = "{'tag':'#adavigo','size':10, 'page': 1}";
                //token = CommonHelper.Encode(j_param, configuration["DataBaseConfig:key_api:b2c"]);

                JArray objParr = null;
                string msg = "";
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    string db_type = string.Empty;
                    string tag = objParr[0]["tag"].ToString();
                    int page = Convert.ToInt32(objParr[0]["page"]);
                    int size = Convert.ToInt32(objParr[0]["size"]);
                    int take = (size <= 0) ? 10 : size;
                    int skip = ((page - 1) <= 0) ? 0 : (page - 1) * take;
                    string cache_key = CacheType.CATEGORY_TAG + EncodeHelpers.MD5Hash(tag.Trim().ToLower());
                    string j_data = null;
                    try
                    {
                        j_data = await _redisService.GetAsync(cache_key, Convert.ToInt32(configuration["Redis:Database:db_common"]));
                    }
                    catch (Exception ex)
                    {
                        LogHelper.InsertLogTelegram("NewsController - getListArticleByTagsOrder: " + ex + "\n Token: " + token);

                    }
                    List<ArticleFeModel> data_list;
                    List<ArticleFeModel> pinned_article;
                    int total_count = -1;
                    int total_page = 1;
                    if (j_data == null || j_data == "")
                    {
                        var data_100 = await articleRepository.getArticleListByTags(tag, 0, 100);
                        if (skip + take > 100)
                        {
                            var data = await articleRepository.getArticleListByTags(tag, skip, take);
                            data_list = data.list_article_fe;
                            total_count = data.total_item_count;
                            total_page = Convert.ToInt32(total_count / take);
                            if (total_page < ((float)total_count / take))
                            {
                                total_page++;
                            }
                        }
                        else
                        {
                            data_list = data_100.list_article_fe.Skip(skip).Take(take).ToList();
                            total_count = data_100.total_item_count;
                            total_page = Convert.ToInt32(total_count / take);
                            if (total_page < ((float)total_count / take))
                            {
                                total_page++;
                            }
                        }

                        try
                        {
                            _redisService.Set(cache_key, JsonConvert.SerializeObject(data_100), DateTime.Now.AddMinutes(15), Convert.ToInt32(configuration["Redis:Database:db_common"]));
                        }
                        catch (Exception ex)
                        {
                            LogHelper.InsertLogTelegram("NewsController - getListArticleByTagsOrder: " + ex + "\n Token: " + token);

                        }
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            data = data_list,
                            total_item = total_count,
                            total_page = total_page

                        });

                    }
                    else
                    {
                        var data_100 = JsonConvert.DeserializeObject<ArticleFEModelPagnition>(j_data);
                        if (skip + take > 100)
                        {
                            var data = await articleRepository.getArticleListByTags(tag, skip, take);
                            data_list = data.list_article_fe;
                            total_count = data.total_item_count;
                            total_page = Convert.ToInt32(total_count / take);
                            if (total_page < ((float)total_count / take))
                            {
                                total_page++;
                            }
                        }
                        else
                        {
                            data_list = data_100.list_article_fe.Skip(skip).Take(take).ToList();
                            total_count = data_100.total_item_count;
                            total_page = Convert.ToInt32(total_count / take);
                            if (total_page < ((float)total_count / take))
                            {
                                total_page++;
                            }
                        }
                        _redisService.Set(cache_key, JsonConvert.SerializeObject(data_100), DateTime.Now.AddMinutes(15), Convert.ToInt32(configuration["Redis:Database:db_common"]));

                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            data = data_list,
                            total_item = total_count,
                            total_page = total_page
                        });
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
        /// <summary>
        /// Lấy ra tất cả các chuyên mục thuộc footer B2C
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("get-footer-category.json")]
        public async Task<ActionResult> GetFooterAllCategory(string token)
        {
            try
            {
                //string j_param = "{'confirm':1}";
                //token = CommonHelper.Encode(j_param, configuration["DataBaseConfig:key_api:b2c"]);
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    int _category_id = Convert.ToInt32(objParr[0]["category_id"]);
                    string cache_name = CacheType.ARTICLE_B2C_FOOTER_MENU;
                    string j_data = null;
                    try
                    {
                        j_data = await _redisService.GetAsync(cache_name, Convert.ToInt32(configuration["Redis:Database:db_common"]));
                    }
                    catch (Exception ex)
                    {
                        LogHelper.InsertLogTelegram("NewsController - GetFooterAllCategory: " + ex + "\n Token: " + token);

                    }
                    List<ArticleGroupViewModel> group_product = null;

                    if (j_data != null)
                    {
                        group_product = JsonConvert.DeserializeObject<List<ArticleGroupViewModel>>(j_data);
                    }
                    else
                    {
                        group_product = await groupProductRepository.GetFooterCategoryByParentID(Convert.ToInt64(configuration["config_value:default_b2c_news_root_group"]));
                        if (group_product.Count > 0)
                        {
                            try
                            {
                                _redisService.Set(cache_name, JsonConvert.SerializeObject(group_product), Convert.ToInt32(configuration["Redis:Database:db_common"]));
                            }
                            catch (Exception ex)
                            {
                                LogHelper.InsertLogTelegram("NewsController - GetFooterAllCategory: " + ex + "\n Token: " + token);

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
                        status = (int)ResponseType.ERROR,
                        msg = "Key ko hop le"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("NewsController - GetFooterAllCategory: " + ex + "\n Token: " + token);
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Error: " + ex.ToString(),
                });
            }
        }

        /// <summary>
        /// Lấy ra tất cả bài viết theo 1 chuyên mục footer, skip+take, sắp xếp theo ngày gần nhất
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("get-footer-article-by-category.json")]
        public async Task<ActionResult> getListFooterArticleByCategory(string token)
        {

            try
            {
                //string j_param = "{'category_id':40,'page':1, 'size': 10}";
                //token = CommonHelper.Encode(j_param, configuration["DataBaseConfig:key_api:b2c"]);

                JArray objParr = null;
                string msg = "";
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    string db_type = string.Empty;
                    int category_id = Convert.ToInt32(objParr[0]["category_id"]);
                    int page = Convert.ToInt32(objParr[0]["page"]);
                    int size = Convert.ToInt32(objParr[0]["size"]);
                    int take = (size <= 0) ? 10 : size;
                    int skip = ((page - 1) <= 0) ? 0 : (page - 1) * take;
                    string cache_key = CacheType.CATEGORY_NEWS + category_id;
                    string j_data = null;
                    try
                    {
                        j_data = await _redisService.GetAsync(cache_key, Convert.ToInt32(configuration["Redis:Database:db_common"]));
                    }
                    catch (Exception ex)
                    {
                        LogHelper.InsertLogTelegram("NewsController - getListFooterArticleByCategory: " + ex + "\n Token: " + token);

                    }
                    List<ArticleFeModel> data_list;
                    int total_count = -1;
                    int total_page = 1;
                    if (j_data == null || j_data == "")
                    {
                        var group_product = await groupProductRepository.GetGroupProductNameAsync(category_id);
                        var data_100 = await articleRepository.getFooterArticleListByCategory(category_id, 0, 100, group_product);
                        if (skip + take > 100)
                        {
                            var data = await articleRepository.getFooterArticleListByCategory(category_id, skip, take, group_product);
                            data_list = data.list_article_fe;
                            total_count = data.total_item_count;
                            total_page = Convert.ToInt32(total_count / take);
                            if (total_page < ((float)total_count / take))
                            {
                                total_page++;
                            }
                        }
                        else
                        {
                            data_list = data_100.list_article_fe.Skip(skip).Take(take).ToList();
                            total_count = data_100.total_item_count;
                            total_page = Convert.ToInt32(total_count / take);
                            if (total_page < ((float)total_count / take))
                            {
                                total_page++;
                            }
                        }

                        try
                        {
                            _redisService.Set(cache_key, JsonConvert.SerializeObject(data_100), DateTime.Now.AddMinutes(15), Convert.ToInt32(configuration["Redis:Database:db_common"]));
                        }
                        catch (Exception ex)
                        {
                            LogHelper.InsertLogTelegram("NewsController - getListFooterArticleByCategory: " + ex + "\n Token: " + token);

                        }

                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            data = data_list,
                            total_item = total_count,
                            total_page = total_page

                        });

                    }
                    else
                    {
                        var group_product = await groupProductRepository.GetGroupProductNameAsync(category_id);

                        if (skip + take > 100)
                        {
                            var data = await articleRepository.getFooterArticleListByCategory(category_id, skip, take, group_product);
                            data_list = data.list_article_fe;
                            total_count = data.total_item_count;
                            total_page = Convert.ToInt32(total_count / take);
                            if (total_page < ((float)total_count / take))
                            {
                                total_page++;
                            }
                        }
                        else
                        {
                            var data_100 = JsonConvert.DeserializeObject<ArticleFEModelPagnition>(j_data);
                            data_list = data_100.list_article_fe.Skip(skip).Take(take).ToList();
                            total_count = data_100.total_item_count;
                            total_page = Convert.ToInt32(total_count / take);
                            if (total_page < ((float)total_count / take))
                            {
                                total_page++;
                            }
                        }

                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            data = data_list,
                            total_item = total_count,
                            total_page = total_page
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
        /// <summary>
        /// Lấy ra bài viết theo 1 chuyên mục
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("get-list-detail-by-categoryid.json")]
        public async Task<ActionResult> getListArticleByCategoryId(string token)
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
                    int page = Convert.ToInt32(objParr[0]["page"]);
                    int size = Convert.ToInt32(objParr[0]["size"]);
                    int take = (size <= 0) ? 10 : size;
                    int skip = ((page - 1) <= 0) ? 0 : (page - 1) * take;
                    string cache_name = CacheType.ARTICLE_CATEGORY_ID + _category_id;
                    string j_data = null;
                    try
                    {
                        j_data = await _redisService.GetAsync(cache_name, Convert.ToInt32(configuration["Redis:Database:db_common"]));
                    }
                    catch (Exception ex)
                    {
                        LogHelper.InsertLogTelegram("NewsController - getListArticleByCategoryId: " + ex + "\n Token: " + token);

                    }
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
                            try
                            {
                                _redisService.Set(cache_name, JsonConvert.SerializeObject(list_article), Convert.ToInt32(configuration["Redis:Database:db_common"]));
                            }
                            catch (Exception ex)
                            {
                                LogHelper.InsertLogTelegram("NewsController - getListArticleByCategoryId: " + ex + "\n Token: " + token);

                            }
                        }
                        db_type = "database";
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        data = list_article.Skip(skip).Take(take),
                        pinned = new List<ArticleFeModel>(),
                        total_item = list_article.Count,
                        total_page = 1
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

        /// <summary>
        /// Lấy ra các chuyên mục hình ảnh
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("get-product-category-by-parent-id.json")]
        public async Task<ActionResult> GetCategoryByParentId(string token)
        {
            try
            {
                //string j_param = "{'category_id':57}";
               // token = CommonHelper.Encode(j_param, configuration["DataBaseConfig:key_api:b2c"]);
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    int _category_id = Convert.ToInt32(objParr[0]["category_id"]);
                    string cache_name = CacheType.B2C_PRODUCT_CATEGORY+_category_id;
                    string j_data = null;
                    try
                    {
                        j_data = await _redisService.GetAsync(cache_name, Convert.ToInt32(configuration["Redis:Database:db_common"]));
                    }
                    catch (Exception ex)
                    {
                        LogHelper.InsertLogTelegram("NewsController - GetCategoryByParentId: " + ex + "\n Token: " + token);

                    }
                    List<ProductGroupViewModel> group_product = null;

                    if (j_data != null)
                    {
                        group_product = JsonConvert.DeserializeObject<List<ProductGroupViewModel>>(j_data);
                    }
                    else
                    {
                        group_product = await groupProductRepository.GetProductGroupByParentID(_category_id, configuration["config_value:ImageStatic"]);
                        if (group_product.Count > 0)
                        {
                            try
                            {
                                _redisService.Set(cache_name, JsonConvert.SerializeObject(group_product), Convert.ToInt32(configuration["Redis:Database:db_common"]));
                            }
                            catch (Exception ex)
                            {
                                LogHelper.InsertLogTelegram("NewsController - GetCategoryByParentId: " + ex + "\n Token: " + token);

                            }
                        }
                    }

                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Success",
                        data = group_product
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
                LogHelper.InsertLogTelegram("NewsController - GetCategoryByParentId: " + ex + "\n Token: " + token);
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Error: " + ex.ToString(),
                });
            }
        }
    }
}
