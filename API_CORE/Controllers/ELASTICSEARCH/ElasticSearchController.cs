using API_CORE.Controllers.ELASTICSEARCH.Base;
using Caching.Elasticsearch;
using Caching.RedisWorker;
using ENTITIES.ViewModels.ElasticSearch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repositories.IRepositories;
using REPOSITORIES.IRepositories.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.Controllers.ELASTICSEARCH
{
    [Route("api")]
    [ApiController]
    public class ElasticSearchController : ControllerBase
    {
        private IConfiguration configuration;
        private IElasticsearchDataRepository elasticsearchDataRepository;
        private ITourRepository _TourRepository;
        private readonly RedisConn redisService;
        private HotelESRepository _hotelESRepository;

        public ElasticSearchController(IConfiguration _configuration, IElasticsearchDataRepository _elasticsearchDataRepository, ITourRepository TourRepository, RedisConn _redisService)
        {
            configuration = _configuration;
            _TourRepository = TourRepository;
            redisService = _redisService;
            elasticsearchDataRepository = _elasticsearchDataRepository;
            _hotelESRepository = new HotelESRepository(_configuration["DataBaseConfig:Elastic:Host"]);

        }

        [HttpPost("hotel/get-list.json")]
        public async Task<ActionResult> GetDataProduct(string token)
        {
            try
            {
                #region Test
                //var j_param = new Dictionary<string, string>
                //{
                //    {"txtsearch", "may"},
                //    {"search_type", "0"} // 1:vin | 0: manual
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
                //OmM1OTZAKxsTIFlbWRAt4buiKGExLMKzPyZgPA ==
                //OmM1OTZAKxsTIFlbWRAtIM2IKGE0KT7NgSwmYzw=
                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    string txtsearch = objParr[0]["txtsearch"].ToString();
                    string search_type = objParr[0]["search_type"].ToString();

                    if (string.IsNullOrEmpty(txtsearch))
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.EMPTY
                        });
                    }
                    else
                    {
                        //bool isUnicode = Encodging.ASCII.GetByteCount(txtsearch) != Encoding.UTF8.GetByteCount(txtsearch);
                        byte[] utfBytes = Encoding.UTF8.GetBytes(txtsearch.Trim());
                        txtsearch = Encoding.UTF8.GetString(utfBytes);
                    }
                    //var es_service = new esService(configuration);
                    //// Tìm kiếm khách sạn dạng multi
                    //var data_hotel = await es_service.search(txtsearch, search_type, "searchHotel.json");

                    //if (data_hotel != "{}")
                    //{
                    //    JObject jsonObject = JObject.Parse(data_hotel);
                    //    var hits = (JArray)jsonObject["hits"]["hits"];
                    //    var hotel_result = new List<JObject>();
                    //    foreach (var hit in hits)
                    //    {
                    //        JObject source = (JObject)hit["_source"];
                    //        source["keyword"] = source["name"];
                    //        hotel_result.Add(source);
                    //    }

                    //    return Ok(new
                    //    {
                    //        status = (int)ResponseType.SUCCESS,
                    //        data = hotel_result,
                    //    });
                    //}
                    //else
                    //{
                    //    return Ok(new
                    //    {
                    //        status = (int)ResponseType.EMPTY,
                    //        msg = "Không có dữ liệu nào thỏa mãn từ khóa " + txtsearch
                    //    });
                    //}
                    txtsearch = txtsearch.Trim();
                    var hotel_list = await _hotelESRepository.GetListProduct(txtsearch, search_type=="1"?true:false);
                    var city_list = await _hotelESRepository.GetListCity(txtsearch, search_type=="1"?true:false);

                    var data = city_list.Select(x => new
                    {
                        x.city,
                        imagethumb = "",
                        name = @$"Tất cả các khách sạn có tại <span class=""t-hotel"">{x.city}</span>",
                        typeofroom = "",
                        x.isvinhotel,
                        x.hoteltype,
                        hotelid = "",
                        x.id,
                        x.state,
                        x.index_search,
                        groupname = "city",
                        keyword = x.city,
                        product_type = 2

                    }).ToList();
                 
                    data.AddRange(hotel_list.Select(x => new
                    {
                        x.city,
                        x.imagethumb,
                        x.name,
                        x.typeofroom,
                        x.isvinhotel,
                        x.hoteltype,
                        x.hotelid,
                        x.id,
                        x.state,
                        x.index_search,
                        x.groupname,
                        x.keyword,
                        product_type = 0
                    }).ToList()) ;
                   
                    data.Add(new
                    {
                        city = "Từ khóa liên quan",
                        imagethumb = "",
                        name = @$"Tất cả các khách sạn có chứa từ khóa <span class=""t-hotel"">{txtsearch}</span>",
                        typeofroom = "",
                        isvinhotel = (bool?)false,
                        hoteltype = "",
                        hotelid = String.Join(",", hotel_list.Select(m => m.id)),
                        id = (long)0,
                        state = "",
                        index_search = "",
                        groupname = "city",
                        keyword = txtsearch,
                        product_type = 1
                    });
                    if (data != null && data.Count>0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            data = data
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.EMPTY,
                            msg = "Không có dữ liệu nào thỏa mãn từ khóa " + txtsearch
                        });
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
                LogHelper.InsertLogTelegram("InsertData - ElasticSearchController: " + ex);
                return Ok(new { status = ResponseTypeString.Fail, msg = "error: " + ex.ToString() });
            }
        }

        /// <summary>
        /// Load điểm đi điểm đến của Tour phục vụ cho Search Tour
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("tour/location.json")]
        public async Task<ActionResult> getLocation(string token)
        {
            try
            {
                #region Test
                //var j_param = new Dictionary<string, string>
                //{
                //    { "tour_type", "1"}, // 1,2 Province: Nội địa | 3: National: quốc tế
                //      { "start_point", "1"}, // cho phép có tất cả
                //       { "is_page_load", "1"} // khi page load.Khởi tạo giá trị mặc định
                //  };
                //var j_param = new Dictionary<string, string>
                //{
                //      {"tour_type", "2"}, 
                //      {"start_point", "-1"},
                //      //{"end_point", "5"} 
                //};

                // var data_product = JsonConvert.SerializeObject(j_param);
                // token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);

                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    var tour_type = objParr[0]["tour_type"].ToString();
                    var start_point = Convert.ToInt32(objParr[0]["start_point"].ToString());
                    var is_page_load = Convert.ToInt32(objParr[0]["is_page_load"]);

                    // string cache_key = CacheType.B2C_TOUR_SEARCH + tour_type + "_" + start_point;
                    //var location_info = await redisService.GetAsync(cache_key, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_core"]));
                    //if (location_info != null && location_info != "")
                    //{
                    //    return Ok(new
                    //    {
                    //        status = (int)ResponseType.SUCCESS,
                    //        data = JObject.Parse(location_info)
                    //    });
                    //}

                    var es_service = new esService(configuration);
                    // Tìm kiếm  dạng multi
                    var data = await es_service.search(tour_type, false, "getLocationByTour.json");

                    if (data != "{}")
                    {
                        JObject jsonObject = JObject.Parse(data);
                        var hits = (JArray)jsonObject["hits"]["hits"];
                        var lst_location = new List<string>();
                        foreach (var hit in hits)
                        {
                            JObject source = (JObject)hit["_source"];
                            string s_location = source["location_key"].ToString();
                            if (s_location.IndexOf(" ") > -1)
                            {
                                //1_3 1_4
                                //1_3
                                var arr_location = source["location_key"].ToString().Split(" ");
                                foreach (var item in arr_location)
                                {
                                    lst_location.Add(item);
                                }
                            }
                            else
                            {
                                lst_location.Add(s_location);
                            }
                        }

                        // Lấy ra id danh sách điểm đi
                        var lst_start_point_id = lst_location.Select(item => item.Split('_').First()).Distinct().ToList();
                        var lst_end_point_id = new List<string>();
                        // Chọn điểm đi và chọn điểm đến: Thì sẽ list các điểm đến theo điểm đi được chọn
                        if (is_page_load == 1 && lst_start_point_id.Count > 0) //Khởi tạo giá trị mặc định khi chưa biết điểm đến để truyền
                        {
                            start_point = Convert.ToInt32(lst_start_point_id[0]);
                        }
                        if (start_point > 0)
                        {
                            // Lọc ra nhóm điểm đến theo điểm đi                        
                            var lst_group_end_point = lst_location.Where(item => item.StartsWith(start_point + "_")).ToList();
                            // Tách ra danh sách id điểm đến sau khi nhóm                            
                            lst_end_point_id = lst_group_end_point.Select(item => item.Split('_').Last()).ToList();//.ToArray();
                        }
                        else // Không chọn điểm đi nhưng chọn điểm đến: Sẽ lấy All điểm đi và all điểm đến
                        {
                            // Lấy ra all danh sách các điểm đến
                            lst_end_point_id = lst_location.Select(item => item.Split('_').Last()).Distinct().ToList();
                        }

                        var s_start_point = string.Join(",", lst_start_point_id);
                        var s_end_point = string.Join(",", lst_end_point_id);

                        var obj_location_info = await _TourRepository.GetLocationById(Convert.ToInt16(tour_type), s_start_point, s_end_point);

                        var _start_point = obj_location_info.Where(x => x.location_type == 0).ToList();
                        var _end_point = obj_location_info.Where(x => x.location_type == 1).ToList();
                        var data_location = new Dictionary<string, object>
                        {
                            {"start_point", _start_point},
                            {"end_point", _end_point},
                            {"start_point_default", start_point}
                        };
                        if (_start_point.Count > 0 && _end_point.Count > 0)
                        {
                            //redisService.Set(cache_key, JsonConvert.SerializeObject(data_location), Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_core"]));
                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                data = data_location

                            });
                        }
                        else
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.FAILED,
                                msg = "Điểm đi hoặc điểm đến bị thiếu data. Token = " + token + "- tour_type " + tour_type + "-- start_point" + start_point
                            });
                        }
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.EMPTY,
                            msg = "Không có dữ liệu nào thỏa mãn từ khóa " + tour_type
                        });
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
                LogHelper.InsertLogTelegram("tour/location.json - ElasticSearchController: " + ex);
                return Ok(new { status = ResponseTypeString.Fail, msg = "error: " + ex.ToString() });
            }
        }


        [HttpPost("insert/data.json")]
        public async Task<ActionResult> insertDataProduct(string token)
        {
            try
            {
                #region Test
                //var j_param = new OrderElasticsearchViewModel()
                //{
                //    id=0,
                //    OrderId = 1325,
                //    OrderNo = "CVB23C170603",
                //    AccountClientId = 180,
                //    ClientId= 203,
                //    Amount = 1383000,
                //    deposit_type = 0,
                //    CreateTime = DateTime.Now,
                //};

                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:api_manual"]);
                #endregion
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
                {

                    var data = JsonConvert.DeserializeObject<OrderElasticsearchViewModel>(objParr[0].ToString());

                    string Type = "order";
                    string index_name = configuration["DataBaseConfig:Elastic:index_product_order"];
                    IESRepository<OrderElasticsearchViewModel> _ESRepository = new ESRepository<OrderElasticsearchViewModel>(configuration["DataBaseConfig:Elastic:Host"]);

                    var result = _ESRepository.DeleteOrderID(data.id.ToString(), index_name, Type);
                    if (data.id > 0)
                    {
                        var id = _ESRepository.UpSert(data, index_name, Type);
                        if (id == 1)
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                msg = "Thành công"
                            });
                        }
                        else
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.ERROR,
                                msg = "thất bại"
                            });
                        }
                    }
                    else
                    {
                        LogHelper.InsertLogTelegram("insertDataProduct - ElasticSearchController:insert Datakhông thành công OrderNo= " + data.orderno);
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "OrderId phải lớn hơn 0!"
                        });
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
                LogHelper.InsertLogTelegram("insertDataProduct - ElasticSearchController: " + ex);
                return null;
            }
        }

        [HttpPost("order/get-list.json")]
        public async Task<ActionResult> GetOrderDataProduct(string token)
        {
            try
            {
                #region Test
                //var j_param = new Dictionary<string, string>
                //{
                //    {"txtsearch", "1268"},

                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:api_manual"]);
                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:api_manual"]))
                {
                    string txtsearch = objParr[0]["txtsearch"].ToString();

                    string Type = "order";
                    string index_name = configuration["DataBaseConfig:Elastic:index_product_order"];

                    var result = await elasticsearchDataRepository.GetElasticsearchOrder(index_name, txtsearch, Type);
                    var data = result.GroupBy(x => x.id).Select(y => y.First()).ToList();
                    if (result != null && result.Count > 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Thành công",
                            data = data,
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "thất bại"
                        });
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
                LogHelper.InsertLogTelegram("GetOrderDataProduct - ElasticSearchController: " + ex);
                return Ok(new { status = ResponseTypeString.Fail, msg = "error: " + ex.ToString() });
            }
        }

    }
}
