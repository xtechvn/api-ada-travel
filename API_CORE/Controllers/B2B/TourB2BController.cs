using Caching.RedisWorker;
using CACHING.Elasticsearch;
using ENTITIES.ViewModels.Tour;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repositories.IRepositories;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.Controllers.B2B
{
    [Route("api/b2b/tour")]
    [ApiController]
    public class TourB2BController : ControllerBase
    {
        private IConfiguration configuration;
        private ITourRepository _TourRepository;
        private IAttachFileRepository _attachFileRepository;
        private ILocationRepository _locationRepository;
        private TourESRepository _tourIESRepository;
        private readonly RedisConn redisService;
        #region V1:
        public TourB2BController(IConfiguration _configuration, ITourRepository TourRepository, IAttachFileRepository attachFileRepository, RedisConn _redisService,
            ILocationRepository locationRepository)
        {
            configuration = _configuration;
            _TourRepository = TourRepository;
            _tourIESRepository = new TourESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
            _attachFileRepository = attachFileRepository;
            redisService = _redisService;
            _locationRepository = locationRepository;
        }
        [HttpPost("tour-detail-by-id.json")]
        public async Task<ActionResult> GetTourDetailByID(string token)
        {
            #region Test
            //var j_param = new Dictionary<string, object>
            //    {
            //        {"tour_id", "55"},
            //    };
            //var data_product = JsonConvert.SerializeObject(j_param);
            //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
            #endregion
            try
            {
                JArray objParr = null;

                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    int id = Convert.ToInt32(objParr[0]["tour_id"]);
                    var cache_name = CacheName.B2B_TOUR_TourID + id;
                    var db_index = Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_core"]);
                    var str = redisService.Get(cache_name, db_index);
                    if (str != null && str.Trim() != "")
                    {
                        TourProductDetailModel model = JsonConvert.DeserializeObject<TourProductDetailModel>(str);
                        //-- Trả kết quả
                        return Ok(new { status = (int)ResponseType.SUCCESS, data = model });
                    }

                    TourProductDetailModel tourProduct = null;
                    //tourProduct = await _TourRepository.GetTourProductById(id);
                    var tourProduct2 = await _tourIESRepository.GetTourDetaiId(id);
                    if (tourProduct2 != null && tourProduct2.Count > 0)
                        tourProduct = tourProduct2[0];
                    if (tourProduct == null)
                    {
                        LogHelper.InsertLogTelegram("GetTourDetailByID - TourB2BController - không lấy được thông tin TourProduct id=: " + id);
                        return Ok(new { status = (int)ResponseType.ERROR, msg = "Không lấy được thông tin TourProduct" });
                    }
                    if (!string.IsNullOrEmpty(tourProduct.Schedule))
                    {
                        tourProduct.TourSchedule = JsonConvert.DeserializeObject<IEnumerable<TourProductScheduleModel>>(tourProduct.Schedule);
                    }
                    else
                    {
                        if (tourProduct.Days.HasValue && tourProduct.Days.Value > 0)
                        {
                            var ListShedule = new List<TourProductScheduleModel>();
                            for (int i = 1; i <= tourProduct.Days; i++)
                            {
                                ListShedule.Add(new TourProductScheduleModel
                                {
                                    day_num = i,
                                    day_title = string.Empty,
                                    day_description = string.Empty
                                });
                            }
                            tourProduct.TourSchedule = ListShedule;
                        }
                    }


                    if (tourProduct.listimage != null && tourProduct.listimage != "")
                    {

                        var attachments = tourProduct.listimage.Split(",");

                        tourProduct.OtherImages = attachments;
                    }
                    //redisService.Set(cache_name, JsonConvert.SerializeObject(tourProduct), db_index);

                    return Ok(new { status = (int)ResponseType.SUCCESS, data = tourProduct });

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
                LogHelper.InsertLogTelegram("GetTourDetailByID - TourB2BController - : " + ex);
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }

        }

        [HttpPost("list-tour.json")]
        public async Task<ActionResult> GetListTourDetail(string token)
        {
            #region Test
            //var j_param = new Dictionary<string, object>
            //    {
            //        {"pageindex", "1"},
            //        {"pagesize", "20"},
            //        {"tourtype", "3"},

            //    };
            //var data_product = JsonConvert.SerializeObject(j_param);
            //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
            #endregion
            try
            {
                JArray objParr = null;

                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    int pageindex = Convert.ToInt32(objParr[0]["pageindex"]);
                    int pagesize = Convert.ToInt32(objParr[0]["pagesize"]);
                    int tourtype = Convert.ToInt32(objParr[0]["tourtype"].ToString());

                    int db_index = Convert.ToInt32(configuration["Redis:Database:db_core"]);
                    var cache_name = CacheName.B2B_TOUR_ListTour + tourtype + pageindex + pagesize;
                    var str = redisService.Get(cache_name, db_index);
                    if (str != null && str.Trim() != "")
                    {
                        List<ListTourProductViewModel> model = JsonConvert.DeserializeObject<List<ListTourProductViewModel>>(str);
                        //-- Trả kết quả
                        return Ok(new { status = (int)ResponseType.SUCCESS, data = model, total = model[0].TotalRow });
                    }


                    var tourProduct = await _TourRepository.GetListTourProduct(tourtype.ToString(), pagesize, pageindex, null, null);
                    if (tourProduct == null)
                    {
                        LogHelper.InsertLogTelegram("GetTourDetailByID - TourB2BController - TourProduct=null");
                        return Ok(new { status = (int)ResponseType.ERROR, msg = "ListTourProduct=null" });
                    }

                    if (tourProduct != null && tourProduct.Count > 0)
                    {
                        redisService.Set(cache_name, JsonConvert.SerializeObject(tourProduct), db_index);
                        return Ok(new { status = (int)ResponseType.SUCCESS, data = tourProduct, total = tourProduct[0].TotalRow });

                    }
                    else
                    {
                        return Ok(new { status = (int)ResponseType.ERROR, msg = "null" });
                    }
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
                LogHelper.InsertLogTelegram("GetTourDetailByID - TourB2BController - : " + ex);
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }

        }
        [HttpPost("search-tour.json")]
        public async Task<ActionResult> GetListTourSearch(string token)
        {
            #region Test
            //var j_param = new Dictionary<string, object>
            //    {
            //        {"startpoint", "-1"},
            //        {"endpoint", "2"},
            //        {"tourtype", "1"},
            //        {"pageindex", "1"},
            //        {"pagesize", "20"},

            //    };
            //var data_product = JsonConvert.SerializeObject(j_param);
            //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
            #endregion
            try
            {
                JArray objParr = null;

                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    int pageindex = Convert.ToInt32(objParr[0]["pageindex"]);
                    int pagesize = Convert.ToInt32(objParr[0]["pagesize"]);
                    string endpoint = objParr[0]["endpoint"].ToString();
                    string startpoint = objParr[0]["startpoint"].ToString();
                    int tourtype = Convert.ToInt32(objParr[0]["tourtype"].ToString());

                    int db_index = Convert.ToInt32(configuration["Redis:Database:db_core"]);
                    var cache_name = CacheName.B2B_TOUR_Search + startpoint + endpoint + tourtype + pagesize + pageindex;
                    var str = redisService.Get(cache_name, db_index);
                    if (str != null && str.Trim() != "")
                    {
                        TourProductSearchCacheModel model = JsonConvert.DeserializeObject<TourProductSearchCacheModel>(str);
                        //-- Trả kết quả
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            data = model.data,
                            listimages = model.listimages,
                            total = model.data.Count
                        }); ;
                    }

                    var listnational = await _tourIESRepository.GetListTour(startpoint, endpoint, tourtype,"", pageindex, pagesize, true, false);
                    if (listnational != null)
                    {
                        List<string> OtherImages = new List<string>();
                        if (listnational != null && listnational.Count > 0)
                        {
                            var listimg = listnational.Where(x => x.listimage != null && x.listimage.Trim() != "").Select(s => s.listimage.Split(",")).ToList();
                            foreach (var item in listimg)
                            {
                                OtherImages.AddRange(item);
                            }
                        }
                        TourProductSearchCacheModel model = new TourProductSearchCacheModel()
                        {
                            listimages = OtherImages,
                            data = listnational
                        };
                        //redisService.Set(cache_name, JsonConvert.SerializeObject(model), db_index);

                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            data = listnational,
                            listimages = OtherImages,
                            total = listnational.Count
                        }); ;
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "error: "
                        });
                    }
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
                LogHelper.InsertLogTelegram("GetListTourSearch - TourB2BController - : " + ex);
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }

        }


        [HttpPost("get-list-tourid.json")]
        public async Task<ActionResult> GetListbyTourId(string token)
        {
            #region Test
            //var j_param = new Dictionary<string, object>
            //    {
            //        {"listtourid", "1,2,3"},
            //    };
            //var data_product = JsonConvert.SerializeObject(j_param);
            //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
            #endregion
            try
            {
                JArray objParr = null;

                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {

                    string tourid = objParr[0]["listtourid"].ToString();

                    int db_index = Convert.ToInt32(configuration["Redis:Database:db_core"]);
                    var cache_name = CacheName.B2B_TOUR_ListTour + CommonHelper.MD5Hash(tourid);
                    var str = redisService.Get(cache_name, db_index);
                    if (str != null && str.Trim() != "")
                    {
                        List<ListTourProductViewModel> model = JsonConvert.DeserializeObject<List<ListTourProductViewModel>>(str);
                        //-- Trả kết quả
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            data = model
                        }); ;
                    }

                    var tourProduct = new List<ListTourProductViewModel>();

                    var listtourid = tourid.Split(",");
                    foreach (var item in listtourid)
                    {
                        var tourdetail = await _tourIESRepository.GetListTourId(item.ToString());
                        tourProduct.AddRange(tourdetail);
                    }

                    if (tourProduct == null)
                    {
                        LogHelper.InsertLogTelegram("GetListbyTourId - TourB2BController - TourProduct=null");
                        return Ok(new { status = (int)ResponseType.ERROR, msg = "ListTourProduct=null" });
                    }
                    //redisService.Set(cache_name, JsonConvert.SerializeObject(tourProduct), db_index);

                    return Ok(new { status = (int)ResponseType.SUCCESS, data = tourProduct });
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
                LogHelper.InsertLogTelegram("GetListbyTourId - TourB2BController - : " + ex);
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }

        }

        [HttpPost("get-list-favorite-tour.json")]
        public async Task<ActionResult> GetListFavoriteTourProduct(string token)
        {
            #region Test
            //var j_param = new Dictionary<string, object>
            //    {
            //        {"pageindex", "1"},
            //        {"pagesize", "10"},
            //    };
            //var data_product = JsonConvert.SerializeObject(j_param);
            //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
            #endregion
            try
            {
                JArray objParr = null;

                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {

                    int pageindex = Convert.ToInt32(objParr[0]["pageindex"]);
                    int pagesize = Convert.ToInt32(objParr[0]["pagesize"]);

                    int db_index = Convert.ToInt32(configuration["Redis:Database:db_core"]);
                    var cache_name = CacheName.B2B_TOUR_Favourites + pageindex + pagesize;
                    var str = redisService.Get(cache_name, db_index);
                    if (str != null && str.Trim() != "")
                    {
                        List<ListTourProductViewModel> model = JsonConvert.DeserializeObject<List<ListTourProductViewModel>>(str);
                        //-- Trả kết quả
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            data = model
                        }); ;
                    }

                    var ListFavoriteTourProduct = await _TourRepository.GetListFavoriteTourProduct(pageindex, pagesize);
                    if (ListFavoriteTourProduct == null && ListFavoriteTourProduct.Count > 0)
                    {
                        redisService.Set(cache_name, JsonConvert.SerializeObject(ListFavoriteTourProduct), db_index);
                        LogHelper.InsertLogTelegram("GetListFavoriteTourProduct - TourB2BController - null");
                        return Ok(new { status = (int)ResponseType.ERROR, msg = "ERROR" });
                    }

                    return Ok(new { status = (int)ResponseType.SUCCESS, data = ListFavoriteTourProduct });
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
                LogHelper.InsertLogTelegram("GetListFavoriteTourProduct - TourB2BController - : " + ex);
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error" + ex.ToString() });
            }

        }

        [HttpPost("order-tour-detail-by-id.json")]
        public async Task<ActionResult> GetOrderTourDetailByID(string token)
        {
            #region Test
            //var j_param = new Dictionary<string, object>
            //    {
            //        {"tour_id", "49"},
            //    };
            //var data_product = JsonConvert.SerializeObject(j_param);
            //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
            #endregion
            try
            {
                JArray objParr = null;

                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    long id = Convert.ToInt64(objParr[0]["tour_id"]);

                    int db_index = Convert.ToInt32(configuration["Redis:Database:db_core"]);
                    string cache_name = CacheName.B2B_ORDER_TOUR_DETAIL_ID + id;
                    var obj_lst_order = new TourDtailFeViewModel();
                    var strDataCache = redisService.Get(cache_name, db_index);
                    // Kiểm tra có data trong cache ko
                    if (!string.IsNullOrEmpty(strDataCache))
                        // nếu có trả ra luôn object 
                        obj_lst_order = JsonConvert.DeserializeObject<TourDtailFeViewModel>(strDataCache);
                    else
                    {
                        obj_lst_order = await _TourRepository.GetDetailTourFeByID(id);
                        if (obj_lst_order != null)
                        {
                            redisService.Set(cache_name, JsonConvert.SerializeObject(obj_lst_order), db_index);
                            return Ok(new { status = (int)ResponseType.SUCCESS, data = obj_lst_order });
                        }
                        else
                        {
                            return Ok(new { status = (int)ResponseType.ERROR, msg = "null" });
                        }
                    }
                    return Ok(new
                    {

                        status = (int)ResponseType.SUCCESS,
                        data = obj_lst_order
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
                LogHelper.InsertLogTelegram("GetTourDetailFeByID - TourB2BController - : " + ex);
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }

        }
        [HttpPost("order-list-tour-by-accountid.json")]
        public async Task<ActionResult> GetOrderListTourByAccountId(string token)
        {
            #region Test
            //var j_param = new Dictionary<string, object>
            //    {
            //        {"account_id", "157"},
            //        {"pageindex", "1"},
            //        {"pagesize", "10"},
            //        {"textsearch", "CTR23K191324"},
            //    };
            //var data_product = JsonConvert.SerializeObject(j_param);
            //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
            #endregion
            try
            {
                JArray objParr = null;

                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    int id = Convert.ToInt32(objParr[0]["account_id"]);
                    int pageindex = Convert.ToInt32(objParr[0]["pageindex"]);
                    int pagesize = Convert.ToInt32(objParr[0]["pagesize"]);
                    string textseach = objParr[0]["textsearch"].ToString();

                    int db_index = Convert.ToInt32(configuration["Redis:Database:db_core"]);
                    string cache_name = CacheName.B2B_ORDER_TOUR_ACCOUNTID + id;
                    var obj_lst_tour = new List<OrderListTourViewModel>();
                    var strDataCache = redisService.Get(cache_name, db_index);
                    // Kiểm tra có data trong cache ko
                    if (!string.IsNullOrEmpty(strDataCache))
                        // nếu có trả ra luôn object 
                        obj_lst_tour = JsonConvert.DeserializeObject<List<OrderListTourViewModel>>(strDataCache);

                    else
                    {
                        // nếu chưa thì vào db lấy
                        obj_lst_tour = await _TourRepository.GetListTourByAccountId(id);
                        if (obj_lst_tour != null && obj_lst_tour.Count > 0)
                        {
                            redisService.Set(cache_name, JsonConvert.SerializeObject(obj_lst_tour), db_index);

                        }
                        else
                        {
                            //LogHelper.InsertLogTelegram("Không lấy được danh sách tour theo mã. AccountId: " + id);
                            return Ok(new { status = (int)ResponseType.ERROR, msg = "Danh sách tour theo mã. AccountId: " + id + " = NULL" });
                        }
                        // Kiem tra db co data khong

                    }

                    if (!string.IsNullOrEmpty(textseach) && textseach.Trim() != "")
                    {
                        obj_lst_tour = obj_lst_tour.Where(s => s.OrderNo.ToLower().Contains(textseach.ToLower())).ToList();
                    }
                    pageindex = pageindex == 1 ? 0 : pageindex - 1;
                    obj_lst_tour = obj_lst_tour != null ? obj_lst_tour.Skip(pageindex * pagesize).Take(pagesize).ToList() : null;

                    var total = obj_lst_tour != null ? obj_lst_tour.Count : 0;
                    return Ok(new { status = (int)ResponseType.SUCCESS, data = obj_lst_tour, total = total });
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
                LogHelper.InsertLogTelegram("GetTourDetailFeByID - TourB2BController - : " + ex);
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }

        }
        #endregion

        #region V2:

        [HttpPost("location-start")]
        public async Task<ActionResult> GetStartLocation(string token)
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
                    int db_index = Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_core"]);
                    int tourtype = Convert.ToInt32(objParr[0]["tour_type"].ToString());
                    var cache_name = CacheName.B2B_TOUR_LocationStart + tourtype;
                    //-- Đọc từ cache, nếu có trả kết quả:
                    var str = redisService.Get(cache_name, db_index);
                    if (str != null && str.Trim() != "")
                    {
                        List<TourLocationB2CModel> model = JsonConvert.DeserializeObject<List<TourLocationB2CModel>>(str);
                        //-- Trả kết quả
                        return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), msg = "", data = model });
                    }

                    switch (tourtype)
                    {
                        case (int)TourType.Noi_Dia:
                            {
                                var list_tour = await _TourRepository.GetListTourProduct(tourtype.ToString(), 10000, 1, null, null);
                                if (list_tour != null && list_tour.Count > 0)
                                {
                                    var data = list_tour.Where(x => x.startpoint1 != null).Select(x => new TourLocationB2CModel { id = x.startpoint, name = x.startpoint1, type = tourtype }).ToList();
                                    data = data.GroupBy(x => x.id).Select(x => x.First()).ToList();

                                    redisService.Set(cache_name, JsonConvert.SerializeObject(data), db_index);
                                    return Ok(new
                                    {
                                        status = (int)ResponseType.SUCCESS,
                                        msg = "",
                                        data = data
                                    });
                                }
                            }
                            break;
                        case (int)TourType.In_bound:
                            {
                                var list_tour = await _TourRepository.GetListTourProduct(tourtype.ToString(), 10000, 1, null, null);
                                if (list_tour != null && list_tour.Count > 0)
                                {
                                    var data = list_tour.Where(x => x.startpoint2 != null).Select(x => new TourLocationB2CModel { id = x.startpoint, name = x.startpoint2, type = tourtype }).ToList();
                                    data = data.GroupBy(x => x.id).Select(x => x.First()).ToList();
                                    redisService.Set(cache_name, JsonConvert.SerializeObject(data), db_index);
                                    return Ok(new
                                    {
                                        status = (int)ResponseType.SUCCESS,
                                        msg = "",
                                        data = data
                                    });
                                }
                            }
                            break;
                        case (int)TourType.Out_bound:
                            {
                                var list_tour = await _TourRepository.GetListTourProduct(tourtype.ToString(), 10000, 1, null, null);
                                if (list_tour != null && list_tour.Count > 0)
                                {
                                    var data = list_tour.Where(x => x.startpoint3 != null).Select(x => new TourLocationB2CModel { id = x.startpoint, name = x.startpoint3, type = tourtype }).ToList();
                                    data = data.GroupBy(x => x.id).Select(x => x.First()).ToList();

                                    redisService.Set(cache_name, JsonConvert.SerializeObject(data), db_index);
                                    return Ok(new
                                    {
                                        status = (int)ResponseType.SUCCESS,
                                        msg = "",
                                        data = data
                                    });
                                }
                            }
                            break;
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

        [HttpPost("location-end")]
        public async Task<ActionResult> LocationEnd(string token)
        {
            #region Test
            //var j_param = new Dictionary<string, object>
            //    {
            //      
            //        {"tourtype", "1"},
            //        {"start_point", "1"},

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
                    int db_index = Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_core"]);
                    int tourtype = Convert.ToInt32(objParr[0]["tour_type"].ToString());
                    int start_point = Convert.ToInt32(objParr[0]["start_point"].ToString());
                    var cache_name = CacheName.B2B_TOUR_LocationEnd + tourtype + start_point;
                    //-- Đọc từ cache, nếu có trả kết quả:
                    var str = redisService.Get(cache_name, db_index);
                    if (str != null && str.Trim() != "")
                    {
                        List<TourLocationB2CModel> model = JsonConvert.DeserializeObject<List<TourLocationB2CModel>>(str);
                        //-- Trả kết quả
                        return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), msg = "", data = model });
                    }

                    switch (tourtype)
                    {
                        case (int)TourType.In_bound:
                        case (int)TourType.Noi_Dia:
                            {
                                var list_tour = await _TourRepository.GetListTourProduct(tourtype.ToString(), 10000, 1, start_point <= 0 ? null : start_point.ToString(), null);
                                if (list_tour != null && list_tour.Count > 0)
                                {
                                    var data = string.Join(",", list_tour.Where(x => x.groupendpoint != null).Select(x => x.groupendpoint.Trim()));
                                    var provinces = await _locationRepository.GetProvinceByListID(data);
                                    List<TourLocationB2CModel> model = new List<TourLocationB2CModel>();
                                    if (provinces != null && provinces.Count > 0)
                                    {
                                        model = provinces.Select(x => new TourLocationB2CModel() { id = x.Id, name = x.Name, type = tourtype }).ToList();
                                        redisService.Set(cache_name, JsonConvert.SerializeObject(model), db_index);
                                    }
                                    return Ok(new
                                    {
                                        status = (int)ResponseType.SUCCESS,
                                        msg = "",
                                        data = model
                                    });
                                }
                            }
                            break;

                        case (int)TourType.Out_bound:
                            {
                                var list_tour = await _TourRepository.GetListTourProduct(tourtype.ToString(), 10000, 1, start_point <= 0 ? null : start_point.ToString(), null);
                                if (list_tour != null && list_tour.Count > 0)
                                {
                                    var data = string.Join(",", list_tour.Where(x => x.groupendpoint != null).Select(x => x.groupendpoint.Trim()));
                                    var nationals = await _locationRepository.GetNationalByListID(data);
                                    List<TourLocationB2CModel> model = new List<TourLocationB2CModel>();
                                    if (nationals != null && nationals.Count > 0)
                                    {
                                        model = nationals.Select(x => new TourLocationB2CModel() { id = x.Id, name = x.Name, type = tourtype }).ToList();
                                        redisService.Set(cache_name, JsonConvert.SerializeObject(model), db_index);

                                    }
                                    return Ok(new
                                    {
                                        status = (int)ResponseType.SUCCESS,
                                        msg = "",
                                        data = model
                                    });
                                }
                            }
                            break;
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
                LogHelper.InsertLogTelegram("LocationEnd - TourB2CController - [" + token + "] : " + ex.ToString());
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }

        }
        [HttpPost("search-tour")]
        public async Task<ActionResult> SearchTour(string token)
        {
            #region Test
            //var j_param = new Dictionary<string, object>
            //    {
            //        {"startpoint", "-1"},
            //        {"endpoint", "2"},
            //        {"tourtype", "1"},
            //        {"pageindex", "1"},
            //        {"pagesize", "20"},
            //        {"clienttype", "2"},
            //    };
            //var data_product = JsonConvert.SerializeObject(j_param);
            //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
            #endregion
            try
            {
                JArray objParr = null;

                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {

                    int pageindex = Convert.ToInt32(objParr[0]["pageindex"]);
                    int pagesize = Convert.ToInt32(objParr[0]["pagesize"]);
                    string endpoint = objParr[0]["endpoint"].ToString();
                    string startpoint = objParr[0]["startpoint"].ToString();
                    int tourtype = Convert.ToInt32(objParr[0]["tourtype"].ToString());
                    string clienttype =objParr[0]["clienttype"].ToString();
                    int db_index = Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_core"]);
                    //var cache_name = CacheName.B2B_TOUR_Search + startpoint + endpoint+ pageindex+pagesize;
                    var cache_name = CacheName.B2B_TOUR_Search + "_" + tourtype + "_" + startpoint + endpoint + pageindex + pagesize;
                    //-- Đọc từ cache, nếu có trả kết quả:
                    var str = redisService.Get(cache_name, db_index);
                    if (str != null && str.Trim() != "")
                    {
                        List<ListTourProductViewModel> model = JsonConvert.DeserializeObject<List<ListTourProductViewModel>>(str);
                        //-- Trả kết quả
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "",
                            data = model
                        });
                    }
                    if (startpoint.Trim() == "-1") startpoint = "";
                    if (endpoint.Trim() == "-1") endpoint = "";
                    var list_tour = await _TourRepository.GetListTourProduct(tourtype.ToString(), pagesize, pageindex, startpoint, endpoint);
                    foreach(var item in list_tour)
                    {
                        var packages = await _TourRepository.GetListTourProgramPackagesByTourProductId(item.Id, clienttype);
                        if (packages != null && packages.Count > 0)
                        {
                            item.packages = 1;
                        }
                        else
                        {
                            item.packages = 0;
                        }
                    }
                    
                    if (list_tour != null && list_tour.Count > 0)
                    {
                        redisService.Set(cache_name, JsonConvert.SerializeObject(list_tour), db_index);

                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "",
                            data = list_tour
                        });
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "No Data"
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
                LogHelper.InsertLogTelegram("SearchTour - TourB2CController - [" + token + "] : " + ex.ToString());
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }

        }
        [HttpPost("tour-detail")]
        public async Task<ActionResult> GetTourDetail(string token)
        {
            #region Test
            //var j_param = new Dictionary<string, object>
            //    {
            //        {"tour_id", "257"},
            //        {"type", "2"},
            //    };
            //var data_product = JsonConvert.SerializeObject(j_param);
            //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
            #endregion
            try
            {
                JArray objParr = null;

                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    int id = Convert.ToInt32(objParr[0]["tour_id"]);
                    string type = objParr[0]["type"].ToString();
                    if (id <= 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Data invalid!"

                        });
                    }
                    int db_index = Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_core"]);
                    var cache_name = CacheName.B2B_TOUR_Detail + id + type;
                    //-- Đọc từ cache, nếu có trả kết quả:
                    var str = redisService.Get(cache_name, db_index);
                    if (str != null && str.Trim() != "")
                    {
                        TourProductDetailModel model = JsonConvert.DeserializeObject<TourProductDetailModel>(str);
                        //-- Trả kết quả
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "",
                            data = model
                        });
                    }

                    TourProductDetailModel tourProduct = await _TourRepository.GetTourProductById(id);

                    if (tourProduct == null)
                    {
                        LogHelper.InsertLogTelegram("GetTourDetailByID - TourB2BController - không lấy được thông tin TourProduct id=: " + id);
                        return Ok(new { status = (int)ResponseType.ERROR, msg = "Không lấy được thông tin TourProduct" });
                    }

                    if (!string.IsNullOrEmpty(tourProduct.Schedule))
                    {
                        tourProduct.TourSchedule = JsonConvert.DeserializeObject<IEnumerable<TourProductScheduleModel>>(tourProduct.Schedule);
                    }
                    else
                    {
                        if (tourProduct.Days.HasValue && tourProduct.Days.Value > 0)
                        {
                            var ListShedule = new List<TourProductScheduleModel>();
                            for (int i = 1; i <= tourProduct.Days; i++)
                            {
                                ListShedule.Add(new TourProductScheduleModel
                                {
                                    day_num = i,
                                    day_title = string.Empty,
                                    day_description = string.Empty
                                });
                            }
                            tourProduct.TourSchedule = ListShedule;
                        }
                    }


                    if (tourProduct.listimage != null && tourProduct.listimage != "")
                    {

                        var attachments = tourProduct.listimage.Split(",");

                        tourProduct.OtherImages = attachments;
                    }
                    var packages = await _TourRepository.GetListTourProgramPackagesByTourProductId(id, type);
                    if (packages != null && packages.Count > 0)
                    {
                        var selected_packages = packages.OrderBy(x => x.AdultPrice).ToList();
                        tourProduct.min_adultprice = selected_packages.First().AdultPrice;
                        tourProduct.min_childprice = selected_packages.First().ChildPrice;
                        tourProduct.packages = selected_packages;
                        var selected_daily_packages = packages.Where(x => x.IsDaily == true).OrderBy(x => x.AdultPrice).ToList();
                        if (selected_daily_packages != null && selected_daily_packages.Count > 0)
                        {
                            tourProduct.daily_adultprice = selected_packages.First().AdultPrice;
                            tourProduct.daily_childprice = selected_packages.First().ChildPrice;
                            tourProduct.daily_package_id = selected_daily_packages.First().Id;
                        }
                        else
                        {
                            tourProduct.daily_adultprice = tourProduct.Price;
                            tourProduct.daily_childprice = tourProduct.Price;
                            tourProduct.daily_package_id = -1;
                        }
                    }
                    else
                    {
                        tourProduct.min_adultprice = 0;
                        tourProduct.min_childprice = 0;
                        tourProduct.packages = new List<ENTITIES.Models.TourProgramPackages>();
                    }
                    if (tourProduct.packages.Count > 0)
                    {
                        tourProduct.packages = tourProduct.packages.Where(x => x.FromDate != null && (DateTime)x.FromDate > DateTime.Now).ToList();
                    }
                    redisService.Set(cache_name, JsonConvert.SerializeObject(tourProduct), DateTime.Now.AddMinutes(5), db_index);

                    return Ok(new { status = (int)ResponseType.SUCCESS, data = tourProduct });

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
                LogHelper.InsertLogTelegram("GetTourDetail - TourB2CController - [" + token + "] : " + ex.ToString());
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }

        }
        [HttpPost("save-booking")]
        public async Task<ActionResult> SaveBooking(string token)
        {
            #region Test
            //var j_param = new Dictionary<string, object>
            //    {
            //        {"tour_id", "55"},
            //    };
            //var data_product = JsonConvert.SerializeObject(j_param);
            //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
            #endregion
            try
            {
                JArray objParr = null;

                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    var id = Convert.ToInt64(objParr[0]["tour_product_id"]);
                    if (id <= 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Data invalid!"

                        });
                    }
                    BookingTourMongoViewModel request = JsonConvert.DeserializeObject<BookingTourMongoViewModel>(objParr[0].ToString());
                    string cache_name = CacheName.B2B_ORDER_TOUR_ACCOUNTID + request.account_client_id;
                    int db_index = Convert.ToInt32(configuration["Redis:Database:db_core"]);
                    redisService.clear(cache_name, db_index);
                    request.tour_product = await _TourRepository.GetDetailTourProductById(id);
                    var packages = await _TourRepository.GetListTourProgramPackagesByTourProductId(id, request.client_type);
                    if (packages != null && packages.Count > 0)
                    {
                        if (!request.is_daily && request.tour_product_package_id > 0)
                        {
                            request.packages = packages.FirstOrDefault(x => x.Id == request.tour_product_package_id);
                        }
                        else if (request.is_daily)
                        {
                            request.packages = packages.FirstOrDefault(x => x.IsDaily == true && x.FromDate <= request.start_date && x.ToDate >= request.start_date);
                        }
                        else
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.FAILED,
                                msg = "Data invalid!"
                            });
                        }
                    }

                    string booking_id = await _TourRepository.saveBooking(request, "");
                    if (booking_id != null && booking_id.Trim() != "")
                    {
                        return Ok(new { status = (int)ResponseType.SUCCESS, data = booking_id });

                    }
                    return Ok(new { status = (int)ResponseType.FAILED, data = "" });

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
                LogHelper.InsertLogTelegram("SaveBooking - TourB2CController - [" + token + "] : " + ex.ToString());
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }

        }

        #endregion

        [HttpPost("list-tour-position.json")]
        public async Task<ActionResult> GetListTourPosition(string token)
        {
            #region Test
            //var j_param = new Dictionary<string, object>
            //    {
            //        {"pageindex", "1"},
            //        {"pagesize", "20"},
            //        {"tourtype", "1"},
            //        {"positiontype", "1"},

            //    };
            //var data_product = JsonConvert.SerializeObject(j_param);
            //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
            #endregion
            try
            {
                JArray objParr = null;

                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    int pageindex = Convert.ToInt32(objParr[0]["pageindex"]);
                    int pagesize = Convert.ToInt32(objParr[0]["pagesize"]);
                    int tourtype = Convert.ToInt32(objParr[0]["tourtype"].ToString());
                    int PositionType = Convert.ToInt32(objParr[0]["positiontype"].ToString());

                    int db_index = Convert.ToInt32(configuration["Redis:Database:db_core"]);
                    var cache_name = CacheName.B2B_TOUR_ListTour_POSITION + tourtype + pageindex + pagesize;
                    var str = redisService.Get(cache_name, db_index);
                    if (str != null && str.Trim() != "")
                    {
                        List<ListTourProductViewModel> model = JsonConvert.DeserializeObject<List<ListTourProductViewModel>>(str);
                        //-- Trả kết quả
                        return Ok(new { status = (int)ResponseType.SUCCESS, data = model, total = model[0].TotalRow });
                    }


                    var tourProduct = await _TourRepository.GetListTourProductPosition(tourtype.ToString(), pagesize, pageindex, null, null, PositionType);
                    if (tourProduct == null)
                    {
                        LogHelper.InsertLogTelegram("GetTourDetailByID - TourB2BController - TourProduct=null");
                        return Ok(new { status = (int)ResponseType.ERROR, msg = "ListTourProduct=null" });
                    }

                    if (tourProduct != null && tourProduct.Count > 0)
                    {
                        redisService.Set(cache_name, JsonConvert.SerializeObject(tourProduct), db_index);
                        return Ok(new { status = (int)ResponseType.SUCCESS, data = tourProduct, total = tourProduct[0].TotalRow });

                    }
                    else
                    {
                        return Ok(new { status = (int)ResponseType.ERROR, msg = "null" });
                    }
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
                LogHelper.InsertLogTelegram("GetListTourPosition - TourB2CController - [" + token + "] : " + ex.ToString());
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }

        }
        [HttpPost("get-booking-payment")]
        public async Task<ActionResult> GetBookingPaymentById(string token)
        {
            #region Test
            //var j_param = new Dictionary<string, object>
            //    {
            //        {"booking_id", "55"},
            //    };
            //var data_product = JsonConvert.SerializeObject(j_param);
            //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
            #endregion
            try
            {
                JArray objParr = null;

                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    var id = objParr[0]["booking_id"].ToString();
                    if (id ==null || id.Trim()=="")
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Data invalid!"

                        });
                    }
                    var booking = await _TourRepository.getBookingByID(new string[] { id });
                    if(booking!=null && booking.Count > 0)
                    {
                        var exists = booking[0];
                        TourPaymentModel model = new TourPaymentModel()
                        {
                            address = "",
                            bookingId = exists._id,
                            country = "",
                            email = exists.contact.email,
                            firstName = exists.contact.firstName,
                            numberOfAdult = exists.guest.adult,
                            note = "",
                            numberOfChild = exists.guest.child,
                            packageId = exists.packages.Id,
                            startDate = ((DateTime)exists.packages.FromDate).ToString("N0"),
                            totalAmount = (exists.guest.adult * (exists.packages.AdultPrice == null ? 0 : (double)exists.packages.AdultPrice)) * (exists.guest.child * (exists.packages.ChildPrice == null ? 0 : (double)exists.packages.ChildPrice)),
                            totalNights = 1,
                            tourId = exists.tour_product.Id,
                            phoneNumber = exists.contact.phoneNumber,
                            tourName = exists.tour_product.TourName,
                            voucherName = ""
                        };
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Success",
                            data = model
                        });
                    }

                }
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Data invalid!"
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SaveBooking - TourB2CController - [" + token + "] : " + ex.ToString());
                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }

        }
    }
}
