using API_CORE.Service.Log;
using API_CORE.Service.Vin;
using Caching.Elasticsearch;
using Caching.RedisWorker;
using ENTITIES.ViewModels.Hotel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repositories.IRepositories;
using REPOSITORIES.IRepositories;
using REPOSITORIES.IRepositories.Elasticsearch;
using REPOSITORIES.IRepositories.Hotel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using API_CORE.Service.Price;
using System.Globalization;
using StackExchange.Redis;
using Nest;
using ENTITIES.Models;

namespace API_CORE.Controllers.B2C
{
    [Route("api/b2c/hotel")]
    [ApiController]
    public class HotelB2CController : ControllerBase
    {
        private IConfiguration configuration;
        private IElasticsearchDataRepository elasticsearchDataRepository;
        private ITourRepository _TourRepository;
        private readonly RedisConn redisService;
        private HotelESRepository _hotelESRepository;
        private IServicePiceRepository price_repository;
        private LogService LogService;
        private IHotelDetailRepository hotelDetailRepository;
        private IHotelBookingMongoRepository hotelBookingMongoRepository;
        private IIdentifierServiceRepository identifierServiceRepository;

        public HotelB2CController(IConfiguration _configuration, IElasticsearchDataRepository _elasticsearchDataRepository, ITourRepository TourRepository, RedisConn _redisService,
            IServicePiceRepository _price_repository, IHotelDetailRepository _hotelDetailRepository, IHotelBookingMongoRepository _hotelBookingMongoRepository, IIdentifierServiceRepository _identifierServiceRepository)
        {
            configuration = _configuration;
            _TourRepository = TourRepository;
            redisService = _redisService;
            elasticsearchDataRepository = _elasticsearchDataRepository;
            _hotelESRepository = new HotelESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
            price_repository = _price_repository;
            LogService = new LogService(_configuration);
            hotelDetailRepository = _hotelDetailRepository;
            hotelBookingMongoRepository = _hotelBookingMongoRepository;
            identifierServiceRepository = _identifierServiceRepository;

        }

        [HttpPost("suggestion")]
        public async Task<ActionResult> GetListProductAll(string token)
        {
            try
            {
                #region Test
                //var j_param = new Dictionary<string, string>
                //{
                //    {"txtsearch", "coco"},
                //    {"search_type", "1"} // 1:vin | 0: manual
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                //OmM1OTZAKxsTIFlbWRAt4buiKGExLMKzPyZgPA ==
                //OmM1OTZAKxsTIFlbWRAtIM2IKGE0KT7NgSwmYzw=
                #endregion
                //token = "OmM1OTZAKxsTIFlbWRAtIM2IKGE0KT7NgSwmYzw=";

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    string txtsearch = objParr[0]["txtsearch"].ToString();

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

                    var hotel_list = await _hotelESRepository.GetListProductAll(txtsearch);
                    if (hotel_list != null && hotel_list.Count > 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            data = hotel_list.Select(x => new {
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
                            }),
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
                LogHelper.InsertLogTelegram("GetList - HotelB2CController: " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "error: " + ex.ToString()
                });
            }
        }
        [HttpPost("search")]
        public async Task<ActionResult> SearchHotel(string token)
        {
            try
            {
                #region Test

                //var j_param = new Dictionary<string, string>
                //{
                //    {"arrivalDate", "2024-02-10"},
                //    {"departureDate","2024-02-11" },
                //    {"numberOfRoom", "1"},
                //    {"hotelID","24386cea-907e-93d5-0755-b4b1d8f5858a" },
                //    {"numberOfChild","0" },
                //    {"numberOfAdult","2" },
                //    {"numberOfInfant","0" },
                //    {"clientType","2" },
                //    {"client_id","182" },
                //    {"product_type","0" },
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                //token = "OmMgMzBaOBsNB1ANBhB/Y3F1bnVxfnNyc3VjbmM1JltXCCIMQBExKDIxZm1NCQNqBANMWlR1BXNLWDwBBSAUcTBrTm4YWQdUAghzSF5kfkYZYVFodnlRdhxCZgZ/flZjS0pKWV5UdlVdZEBba1oGXEx0HwlRWDJVCBc1W1dBfnJEBixhZWkdMDkPFSt8MwccXQRsdElodBgFJhg1NgpSFzJBAy56TUZeEwUNGBAmGzZNezEqXj4SVWhIbCoZJxoBRWReeQZeVgsXRmNjH00cBFoMABk9GxUICDJHf0k+";
                #endregion


                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    List<HotelSearchEntities> result = new List<HotelSearchEntities>();
                    string status_vin = string.Empty;
                    string arrivalDate = objParr[0]["arrivalDate"].ToString();
                    string departureDate = objParr[0]["departureDate"].ToString();
                    int numberOfRoom = Convert.ToInt32(objParr[0]["numberOfRoom"]);
                    int numberOfChild = Convert.ToInt32(objParr[0]["numberOfChild"]);
                    int numberOfAdult = Convert.ToInt32(objParr[0]["numberOfAdult"]);
                    int numberOfInfant = Convert.ToInt32(objParr[0]["numberOfInfant"]);
                    string hotelName = objParr[0]["hotelName"].ToString();
                    int page = Convert.ToInt32(objParr[0]["page"]);
                    int size = Convert.ToInt32(objParr[0]["size"]);

                    string hotelID = objParr[0]["hotelID"].ToString();
                    //search_result_hotelID_*
                    //int clientType = Convert.ToInt16(objParr[0]["clientType"]);
                    string distributionChannelId = configuration["config_api_vinpearl:Distribution_ID"].ToString();
                    //-- Đọc từ cache, nếu có trả kết quả:
                    string cache_name_id = arrivalDate + departureDate + numberOfRoom + numberOfChild + numberOfAdult + numberOfInfant + EncodeHelpers.MD5Hash(hotelID) + page + size + "5";
                    string hotelIdsVin = "";
                    string hotelIdsManual = "";

                    var hotelids_vin = new List<string>();
                    var hotel_code_manual = new List<string>();
                    var hotel_ids_manual = new List<int>();
                   
                    var from_date = DateTime.ParseExact(arrivalDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    var to_date = DateTime.ParseExact(departureDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    int total_nights = Convert.ToInt32((to_date - from_date).TotalDays);
                    //-- Đọc từ cache, nếu có trả kết quả:
                    int db_index = Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]);
                    var str = redisService.Get(CacheName.HotelSearchB2C + cache_name_id, db_index);
                    if (str != null && str.Trim() != "")
                    {
                        HotelB2CCacheModel model = JsonConvert.DeserializeObject<HotelB2CCacheModel>(str);
                        //-- Trả kết quả
                        return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), msg = status_vin, data = model.hotels, cache_id = CacheName.HotelSearchB2C + cache_name_id });
                    }
                    //LogHelper.InsertLogTelegram("SearchHotel - HotelB2CController: Cannot get from Redis [" + CacheName.HotelSearchB2C + cache_name_id + "] - token: " + token);

                    try
                    {
                        var hotelids = hotelID.Split(",").ToList();

                        if (hotelids != null && hotelids.Count() > 0)
                        {
                            foreach (var id1 in hotelids)
                            {
                                try
                                {
                                    int id = 0;
                                    if (!int.TryParse(id1,out id))
                                    {
                                        hotelids_vin.Add(id1);
                                    }
                                    else
                                    {
                                        var hotel = await hotelDetailRepository.GetById(Convert.ToInt32(id1));
                                        if (hotel != null && hotel.Id > 0)
                                        {
                                            switch (hotel.IsVinHotel)
                                            {
                                                case true:
                                                    {
                                                        hotelids_vin.Add(hotel.HotelId);
                                                    }
                                                    break;
                                                case null:
                                                case false:
                                                    {
                                                        hotel_code_manual.Add(hotel.HotelId);
                                                        hotel_ids_manual.Add(hotel.Id);

                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                    
                                }
                                catch
                                {

                                }

                            }
                        }

                        hotelIdsVin = JsonConvert.SerializeObject(hotelids_vin);
                        hotelIdsManual = string.Join(",", hotel_code_manual);
                    }
                    catch
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "Data không hợp lệ"
                        });
                    }
                    HotelB2CSearchInputModel input = new HotelB2CSearchInputModel()
                    {
                        arrival_date = from_date,
                        departure_date = to_date,
                        hotelID = hotelID,
                        hotelName = hotelName,
                        ids_manual = hotel_ids_manual,
                        ids_vin = hotelIdsVin,
                        numberOfAdult = numberOfAdult,
                        numberOfChild = numberOfChild,
                        numberOfInfant = numberOfInfant,
                        numberOfRoom = numberOfRoom,
                        total_nights = total_nights,
                        page = page,
                        size = size
                    };
                    List<RoomSearchCacheModel> rooms_list = new List<RoomSearchCacheModel>();
                    //-- Vin Hotel:
                    if (hotelids_vin.Count > 0)
                    {
                        string input_api_vin_all = "{\"arrivalDate\":\"" + arrivalDate + "\",\"departureDate\":\"" + departureDate + "\",\"numberOfRoom\":" + numberOfRoom + ",\"propertyIds\":" + JsonConvert.SerializeObject(hotelids_vin) + ",\"distributionChannelId\":\"" + distributionChannelId + "\",\"roomOccupancy\":{\"numberOfAdult\":" + numberOfAdult + ",\"otherOccupancies\":[{\"otherOccupancyRefCode\":\"child\",\"quantity\":" + numberOfChild + "},{\"otherOccupancyRefCode\":\"infant\",\"quantity\":" + numberOfInfant + "}]}}";
                        int number_room_each = 1;
                        int number_adult_each_room = (numberOfAdult / (float)numberOfRoom) > (int)(numberOfAdult / numberOfRoom) ? (int)(numberOfAdult / numberOfRoom) + 1 : (int)(numberOfAdult / numberOfRoom);
                        int number_child_each_room = numberOfChild == 1 || (((int)numberOfChild / numberOfRoom) <= 1 && numberOfChild > 0) ? 1 : numberOfChild / numberOfRoom;
                        int number_infant_each_room = numberOfInfant == 1 || (((int)numberOfInfant / numberOfRoom) <= 1 && numberOfInfant > 0) ? 1 : numberOfInfant / numberOfRoom;
                        string input_api_vin_phase = "{\"arrivalDate\":\"" + arrivalDate + "\",\"departureDate\":\"" + departureDate + "\",\"numberOfRoom\":" + number_room_each + ",\"propertyIds\":" + JsonConvert.SerializeObject(hotelids_vin) + ",\"distributionChannelId\":\"" + distributionChannelId + "\",\"roomOccupancy\":{\"numberOfAdult\":" + number_adult_each_room + ",\"otherOccupancies\":[{\"otherOccupancyRefCode\":\"child\",\"quantity\":" + number_child_each_room + "},{\"otherOccupancyRefCode\":\"infant\",\"quantity\":" + number_infant_each_room + "}]}}";
                        
                        var vin_lib = new VinpearlLib(configuration);
                        var response = vin_lib.getHotelAvailability(input_api_vin_phase).Result;
                        var data_hotel = JObject.Parse(response);
                        // Đọc Json ra các Field để map với những trường cần lấy

                        if (!(data_hotel["isSuccess"].ToString().ToLower() == "false"))
                        {
                            var j_hotel_list = data_hotel["data"]["rates"];
                            if (j_hotel_list != null && j_hotel_list.Count() > 0)
                            {
                                var rooms = new List<RoomSearchModel>();
                               
                                int nights = (to_date - from_date).Days;
                                foreach (var h in j_hotel_list)
                                {
                                    // Thông tin khách sạn
                                    var hotel_item = new HotelSearchEntities()
                                    {
                                        hotel_id = h["property"]["id"].ToString(),
                                        name = h["property"]["name"].ToString(),
                                        star = Convert.ToDouble(h["property"]["star"]),
                                        country = h["property"]["country"] != null ? h["property"]["country"]["name"].ToString() : "Vietnam",
                                        state = h["property"]["state"]["name"].ToString(),
                                        street = h["property"]["street"].ToString(),
                                        hotel_type = h["property"]["hotelType"] != null ? h["property"]["hotelType"]["name"].ToString() : "Hotel",
                                        review_point = 10,
                                        review_count = 0,
                                        review_rate = "Tuyệt vời",
                                        is_refundable = true,
                                        is_instantly_confirmed = true,
                                        confirmed_time = 0,
                                        email = h["property"]["email"].ToString(),
                                        telephone = h["property"]["telephone"].ToString(),
                                        hotel_group_type = (int)HotelGroupType.VIN

                                    };
                                    // tiện nghi khách sạn
                                    hotel_item.amenities = JsonConvert.DeserializeObject<List<amenitie>>(JsonConvert.SerializeObject(h["property"]["amenities"])).Select(x => new FilterGroupAmenities() { key = x.code, description = x.name, icon = x.icon }).ToList();

                                    // Hình ảnh khách sạn
                                    hotel_item.img_thumb = JsonConvert.DeserializeObject<List<thumbnails>>(JsonConvert.SerializeObject(h["property"]["thumbnails"])).Select(x => x.url).ToList();

                                    // Danh sách các loại phòng của khách sạn
                                    var j_room = h["property"]["roomTypes"];
                                    foreach (var item_r in j_room)
                                    {
                                        #region Chi tiết phòng
                                        var item = new RoomSearchModel
                                        {
                                            id = item_r["id"].ToString(),
                                            code = item_r["code"].ToString(),
                                            name = item_r["name"].ToString(),
                                            type_of_room = item_r["typeOfRoom"].ToString(),
                                            hotel_id = h["property"]["id"].ToString(),
                                            rates = new List<RoomRate>(),
                                        };
                                        #endregion
                                        rooms.Add(item);
                                    }
                                    hotel_item.type_of_room = rooms.Select(x => x.type_of_room).Distinct().ToList();

                                    //-- Giá gốc
                                    var j_rate_data = h["rates"];
                                    if (j_rate_data.Count() > 0)
                                    {
                                        foreach (var r in j_rate_data)
                                        {
                                            double price = Convert.ToDouble(r["totalAmount"]["amount"]["amount"]);

                                            rooms.Where(x => x.id.Trim() == r["roomTypeID"].ToString().Trim()).First().rates.Add(
                                                new RoomRate()
                                                {
                                                    rate_plan_id = r["ratePlanID"].ToString(),
                                                    amount = price,
                                                    profit = 0,
                                                    total_profit = 0,
                                                    total_price = price,
                                                    rate_plan_code = r["rateAvailablity"]["ratePlanCode"].ToString(),
                                                }
                                            );


                                        }
                                    }

                                    hotel_item.room_name = rooms.Where(x => x.rates.Count > 0).Select(x => x.name).Distinct().ToList();
                                    rooms_list.Add(new RoomSearchCacheModel()
                                    {
                                        hotel_id= hotel_item.hotel_id,
                                        rooms=rooms
                                    });
                                    //-- Add vào kết quả
                                    result.Add(hotel_item);
                                }

                            }
                        }
                    }
                    if (hotel_ids_manual.Count > 0)
                    {
                        var hotel_datas = hotelDetailRepository.GetFEHotelList(new HotelFESearchModel
                        {
                            FromDate = from_date,
                            ToDate = to_date,
                            HotelId = hotelID,
                            HotelType = hotelName,
                            PageIndex = 1,
                            PageSize = 50
                        });
                        string hotel_ids = "";
                        if (hotel_datas != null && hotel_datas.Any())
                        {
                            var data_results = hotel_datas.GroupBy(x => x.Id).Select(x => x.First()).ToList();

                            if (data_results != null && data_results.Any())
                            {
                                hotel_ids = string.Join(",", data_results.Select(x => x.HotelId));
                                foreach (var hotel in data_results)
                                {
                                    var hotel_detail = await hotelDetailRepository.GetByHotelId(hotel.Id.ToString());
                                    //-- Tính giá về tay thông qua chính sách giá
                                    result.Add(new HotelSearchEntities
                                    {
                                        hotel_id = hotel.Id.ToString(),
                                        name = hotel.Name,
                                        star = hotel.Star,
                                        country = hotel.Country,
                                        state = hotel.State,
                                        street = hotel.Street,
                                        hotel_type = hotel.HotelType,
                                        review_point = 10,
                                        review_count = hotel.ReviewCount ?? 0,
                                        review_rate = "Tuyệt vời",
                                        is_refundable = hotel.IsRefundable,
                                        is_instantly_confirmed = hotel.IsInstantlyConfirmed,
                                        confirmed_time = hotel.VerifyDate ?? 0,
                                        min_price = 0,
                                        email = hotel.Email,
                                        telephone = hotel.Telephone,
                                        img_thumb = new List<string> { hotel.ImageThumb },
                                        hotel_group_type= (int)HotelGroupType.ManualHotel
                                    });

                                }
                            }


                        }
                    }
                    if (result.Count > 0)
                    {
                        //-- Cache kết quả:
                        HotelB2CCacheModel cache_data = new HotelB2CCacheModel();
                        cache_data.hotels = result;
                        cache_data.search = input;
                        cache_data.ids_vin = hotelids_vin;
                        cache_data.ids_manual = hotel_ids_manual;
                        cache_data.rooms = rooms_list;
                        redisService.Set(CacheName.HotelSearchB2C + cache_name_id, JsonConvert.SerializeObject(cache_data), DateTime.Now.AddDays(1), db_index);

                    }
                    return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), msg = status_vin, data = result, cache_id = CacheName.HotelSearchB2C + cache_name_id });
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
                LogService.InsertLog("SearchHotel - HotelB2CController: " + ex.ToString());

                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "error: " + ex.ToString() });
            }

        }

        [HttpPost("hotel-min-price")]
        public async Task<ActionResult> GetHotelMinPrice(string token)
        {
            #region Test
            //var j_param = new Dictionary<string, string>
            //    {
            //          { "cache_id", "HS_B2C_04/10/202404/11/20241110d6ef5f7fa914c19931a55bb262ec879c005" }
            //   };

            //var data_product = JsonConvert.SerializeObject(j_param);
            //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
            #endregion
            try
            {
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    //-- Đọc từ cache, nếu có trả kết quả:
                    string cache_name = objParr[0]["cache_id"].ToString();
                    string cache_name_price = cache_name.Replace(CacheName.HotelSearchB2C, CacheName.HotelSearchB2CPrice);
                    int db_index = Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"].ToString());

                    var json = redisService.Get(cache_name_price, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                    if (json != null && json.Trim() != "")
                    {
                        List<HotelB2CMinPriceModel> cache_data = JsonConvert.DeserializeObject<List<HotelB2CMinPriceModel>>(json);
                        return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), msg = "Get Data From Cache Success", data = cache_data, cache_id = cache_name });
                    }
                    else
                    {
                        //LogHelper.InsertLogTelegram("GetHotelMinPrice - HotelB2CController: Cannot get from Redis [" + cache_name_price + "] - token: " + token);

                        json = redisService.Get(cache_name, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                        if (json == null || json.Trim() == "")
                        {
                            var status_vin = "Không tìm thấy khách sạn nào thỏa mãn điều kiện này";
                            return Ok(new { status = ((int)ResponseType.EMPTY).ToString(), msg = status_vin, cache_id = cache_name });
                        }
                        HotelB2CCacheModel cache_data = JsonConvert.DeserializeObject<HotelB2CCacheModel>(json);
                        string cancel_template = "Phí thay đổi / Hoàn hủy từ ngày {day} tháng {month} năm {year} là {value}";
                        string cancel_percent_template = " {value_2} % giá phòng";
                        string zero_percent = "Miễn phí Thay đổi / Hoàn hủy trước ngày {day} tháng {month} năm {year}";
                        string cancel_vnd_template = " {value_2} VND";

                        List<HotelB2CMinPriceModel> data = new List<HotelB2CMinPriceModel>();
                        foreach (var h in cache_data.hotels)
                        {
                            switch (h.hotel_group_type)
                            {
                                case (int)HotelGroupType.VIN:
                                    {
                                        string arrivalDate = cache_data.search.arrival_date.ToString("yyyy-MM-dd");
                                        string departureDate = cache_data.search.departure_date.ToString("yyyy-MM-dd");
                                        int numberOfRoom = cache_data.search.numberOfRoom;
                                        int numberOfChild = cache_data.search.numberOfChild;
                                        int numberOfAdult = cache_data.search.numberOfAdult;
                                        int numberOfInfant = cache_data.search.numberOfInfant;

                                        string hotelID = h.hotel_id;
                                        string distributionChannelId = configuration["config_api_vinpearl:Distribution_ID"].ToString();
                                        string input_api_vin_all = "{ \"distributionChannelId\": \"" + distributionChannelId + "\", \"propertyID\": \"" + hotelID + "\", \"numberOfRoom\":" + numberOfRoom + ", \"arrivalDate\":\"" + arrivalDate + "\", \"departureDate\":\"" + departureDate + "\", \"roomOccupancy\":{\"numberOfAdult\":" + numberOfAdult + ",\"otherOccupancies\":[{\"otherOccupancyRefCode\":\"child\",\"quantity\":" + numberOfChild + "},{\"otherOccupancyRefCode\":\"infant\",\"quantity\":" + numberOfInfant + "}]}}";
                                        string input_api_vin_phase = "{ \"distributionChannelId\": \"" + distributionChannelId + "\", \"propertyID\": \"" + hotelID + "\", \"numberOfRoom\":" + numberOfRoom + ", \"arrivalDate\":\"" + arrivalDate + "\", \"departureDate\":\"" + departureDate + "\", \"roomOccupancy\":{\"numberOfAdult\":" + numberOfAdult + ",\"otherOccupancies\":[{\"otherOccupancyRefCode\":\"child\",\"quantity\":" + numberOfChild + "},{\"otherOccupancyRefCode\":\"infant\",\"quantity\":" + numberOfInfant + "}]}}";
                                        //-- Đọc từ cache, nếu có trả kết quả:
                                        string cache_name_id = arrivalDate + departureDate + numberOfRoom + numberOfChild + numberOfAdult + numberOfInfant + h.hotel_id + (int)ClientTypes.CUSTOMER;
                                        var str = redisService.Get(CacheName.HotelRoomDetail + cache_name_id, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                                        string response = "";
                                        if (str != null && str.Trim() != "")
                                        {
                                            HotelRoomDetailModel model = JsonConvert.DeserializeObject<HotelRoomDetailModel>(str);
                                            //-- Trả kết quả
                                            var min_price = model.rooms == null || model.rooms.Count <= 0 ? new RoomDetail() { min_price = 0 } : model.rooms.OrderBy(x => x.min_price).First();
                                            data.Add(new HotelB2CMinPriceModel()
                                            {
                                                amount = min_price != null ? min_price.min_price : 0,
                                                hotel_id = h.hotel_id,
                                                price = 0,
                                                profit = 0
                                            });
                                            break;
                                        }
                                        else
                                        {
                                            var vin_lib = new VinpearlLib(configuration);
                                            response = vin_lib.getRoomAvailability(input_api_vin_phase).Result;
                                        }

                                        HotelRoomDetailModel result = new HotelRoomDetailModel();
                                        result.input_api_vin = input_api_vin_all;
                                        var data_hotel = JObject.Parse(response);
                                        // Đọc Json ra các Field để map với những trường cần lấy

                                        #region Check Data Invalid
                                        if (data_hotel["isSuccess"].ToString().ToLower() == "false")
                                        {
                                            return Ok(new
                                            {
                                                status = (int)ResponseType.EMPTY,
                                                msg = "Tham số không hợp lệ",
                                                data = data_hotel
                                            });
                                        }
                                        #endregion


                                        var j_room_list = data_hotel["data"]["propertyInfo"]["roomTypes"];
                                        var j_rate_list = data_hotel["data"]["roomAvailabilityRates"];
                                        if (j_room_list.Count() > 0 && j_rate_list.Count() > 0)
                                        {
                                            // Thông tin khách sạn
                                            result = new HotelRoomDetailModel()
                                            {
                                                hotel_id = data_hotel["data"]["propertyInfo"]["id"].ToString(),
                                                rooms = new List<RoomDetail>(),
                                            };

                                            foreach (var room in j_room_list)
                                            {
                                                #region Hình ảnh phòng

                                                var img_thumb_room = new List<thumbnails>();
                                                var j_thumb_room_img = room["thumbnails"];
                                                foreach (var item_thumb in j_thumb_room_img)
                                                {
                                                    var item_img_room = new thumbnails
                                                    {
                                                        id = item_thumb["id"].ToString(),
                                                        url = item_thumb["url"].ToString()
                                                    };
                                                    img_thumb_room.Add(item_img_room);
                                                }
                                                #endregion

                                                #region Chi tiết phòng
                                                var item = new RoomDetail
                                                {
                                                    id = room["id"].ToString(),
                                                    code = room["code"].ToString(),
                                                    name = room["name"].ToString(),
                                                    description = room["description"].ToString(),
                                                    max_adult = Convert.ToInt32(room["maxAdult"]),
                                                    max_child = Convert.ToInt32(room["maxChild"]),
                                                    img_thumb = img_thumb_room,
                                                    remainming_room = Convert.ToInt32(room["numberOfRoom"]),
                                                    rates = new List<RoomDetailRate>(),
                                                    number_bed_room = Convert.ToInt32(room["numberOfBedRoom"]),
                                                    number_extra_bed = Convert.ToInt32(room["maxExtraBeds"]),
                                                    bed_room_type_name = "",


                                                };
                                                #endregion
                                                result.rooms.Add(item);
                                            }

                                            //-- Giá gốc + cancel policy
                                            foreach (var r in j_rate_list)
                                            {
                                                var list = JsonConvert.DeserializeObject<List<RoomDetailPackage>>(r["packages"].ToString());
                                                if (result.rooms.Where(x => x.id.Trim() == r["roomType"]["roomTypeID"].ToString().Trim()).First().package_includes == null || result.rooms.Where(x => x.id.Trim() == r["roomType"]["roomTypeID"].ToString().Trim()).First().package_includes.Count < 1)
                                                {
                                                    result.rooms.Where(x => x.id.Trim() == r["roomType"]["roomTypeID"].ToString().Trim()).First().package_includes = list.Where(x => x.packageType.ToUpper().Contains("INCLUDE") && x.description != null).Select(x => x.description.ToString().Trim()).ToList();
                                                }

                                                var cancel_policy = JsonConvert.DeserializeObject<RoomDetailCancelPolicy>(r["ratePlan"]["cancelPolicy"].ToString());
                                                cancel_policy.detail = cancel_policy.detail.OrderBy(x => x.amount).ToList();
                                                var cancel_policy_output = new List<string>();
                                                DateTime arrivalDate_date_time = DateTime.ParseExact(arrivalDate, "yyyy-MM-dd", null);
                                                DateTime day_before_arrival_before = DateTime.ParseExact(arrivalDate, "yyyy-MM-dd", null);

                                                foreach (var c in cancel_policy.detail)
                                                {
                                                    if (c.amount <= 0)
                                                    {
                                                        day_before_arrival_before = arrivalDate_date_time - new TimeSpan(c.daysBeforeArrival, 0, 0, 0, 0);
                                                        string str_cp = zero_percent.Replace("{day}", day_before_arrival_before.Day.ToString()).Replace("{month}", day_before_arrival_before.Month.ToString()).Replace("{year}", day_before_arrival_before.Year.ToString());
                                                        cancel_policy_output.Add(str_cp);

                                                    }
                                                    else
                                                    {

                                                        try
                                                        {
                                                            string str_cp = cancel_template.Replace("{day}", day_before_arrival_before.Day.ToString()).Replace("{month}", day_before_arrival_before.Month.ToString()).Replace("{year}", day_before_arrival_before.Year.ToString());
                                                            str_cp = c.type.ToLower() == "percent" ? str_cp.Replace("{value}", cancel_percent_template.Replace("{value_2}", c.amount.ToString())) : str_cp.Replace("{value}", cancel_vnd_template.Replace("{value_2}", c.amount.ToString()));
                                                            cancel_policy_output.Add(str_cp);
                                                            day_before_arrival_before = arrivalDate_date_time - new TimeSpan(c.daysBeforeArrival, 0, 0, 0, 0);
                                                        }
                                                        catch { }

                                                    }
                                                }
                                                result.rooms.Where(x => x.id.Trim() == r["roomType"]["roomTypeID"].ToString().Trim()).First().rates.Add(
                                                    new RoomDetailRate()
                                                    {
                                                        id = r["ratePlan"]["id"].ToString(),
                                                        amount = Convert.ToDouble(r["totalAmount"]["amount"]["amount"]),
                                                        room_code = r["roomType"]["roomTypeID"].ToString(),
                                                        code = r["ratePlan"]["rateCode"].ToString(),
                                                        description = r["ratePlan"]["description"].ToString(),
                                                        name = r["ratePlan"]["name"].ToString(),
                                                        cancel_policy = cancel_policy_output,
                                                        guarantee_policy = r["ratePlan"]["guaranteePolicy"]["description"].ToString(),
                                                        allotment_id = r["allotments"] != null && r["allotments"].Count() > 0 ? r["allotments"][0]["id"].ToString() : "",
                                                        package_includes = list
                                                    }
                                                );
                                            }
                                            DateTime fromdate = DateTime.ParseExact(arrivalDate, "yyyy-M-d", null);
                                            DateTime todate = DateTime.ParseExact(departureDate, "yyyy-M-d", null);
                                            //-- Tính giá về tay thông qua chính sách giá
                                            var profit_list = hotelDetailRepository.GetHotelRoomPricePolicy(hotelID, "5");

                                            foreach (var r in result.rooms)
                                            {
                                                var r_id = r.id;
                                                foreach (var rate in r.rates)
                                                {
                                                    var profit = profit_list.Where(x => x.HotelCode == result.hotel_id && x.RoomTypeCode == r_id && x.PackageName == rate.id).ToList();
                                                    if (profit != null && profit.Count > 0)
                                                    {
                                                        rate.total_profit = PricePolicyService.CalucateMinProfit(profit, rate.amount, fromdate, todate);
                                                        rate.total_price = rate.amount + rate.total_profit;

                                                    }
                                                    else
                                                    {
                                                        rate.profit = 0;
                                                        rate.total_profit = 0;
                                                        rate.total_price = 0;
                                                    }
                                                    r.min_price = r.min_price <= 0 ? rate.total_price : ((rate.total_price > 0 && r.min_price > rate.total_price) ? rate.total_price : r.min_price);
                                                }
                                            }

                                            //-- Cache kết quả:
                                            redisService.Set(CacheName.HotelRoomDetail + cache_name_id, JsonConvert.SerializeObject(result), DateTime.Now.AddMinutes(30), db_index);
                                            var min_price = result.rooms==null || result.rooms.Count<=0?new RoomDetail() { min_price=0} : result.rooms.OrderBy(x => x.min_price).First();
                                            data.Add(new HotelB2CMinPriceModel()
                                            {
                                                amount = min_price != null ? min_price.min_price : 0,
                                                hotel_id = h.hotel_id,
                                                price = 0,
                                                profit = 0
                                            });
                                        }
                                    }
                                    break;
                                case (int)HotelGroupType.ManualHotel:
                                    {
                                        

                                        string arrivalDate = cache_data.search.arrival_date.ToString("yyyy-MM-dd");
                                        string departureDate = cache_data.search.departure_date.ToString("yyyy-MM-dd");
                                        int numberOfRoom = cache_data.search.numberOfRoom;
                                        int numberOfChild = cache_data.search.numberOfChild;
                                        int numberOfAdult = cache_data.search.numberOfAdult;
                                        int numberOfInfant = cache_data.search.numberOfInfant;

                                        string hotelID = h.hotel_id;
                                        string distributionChannelId = configuration["config_api_vinpearl:Distribution_ID"].ToString();
                                        string input_api_vin_all = "{ \"distributionChannelId\": \"" + distributionChannelId + "\", \"propertyID\": \"" + hotelID + "\", \"numberOfRoom\":" + numberOfRoom + ", \"arrivalDate\":\"" + arrivalDate + "\", \"departureDate\":\"" + departureDate + "\", \"roomOccupancy\":{\"numberOfAdult\":" + numberOfAdult + ",\"otherOccupancies\":[{\"otherOccupancyRefCode\":\"child\",\"quantity\":" + numberOfChild + "},{\"otherOccupancyRefCode\":\"infant\",\"quantity\":" + numberOfInfant + "}]}}";
                                        string input_api_vin_phase = "{ \"distributionChannelId\": \"" + distributionChannelId + "\", \"propertyID\": \"" + hotelID + "\", \"numberOfRoom\":" + numberOfRoom + ", \"arrivalDate\":\"" + arrivalDate + "\", \"departureDate\":\"" + departureDate + "\", \"roomOccupancy\":{\"numberOfAdult\":" + numberOfAdult + ",\"otherOccupancies\":[{\"otherOccupancyRefCode\":\"child\",\"quantity\":" + numberOfChild + "},{\"otherOccupancyRefCode\":\"infant\",\"quantity\":" + numberOfInfant + "}]}}";
                                        string cache_name_id = arrivalDate + departureDate + numberOfRoom + numberOfChild + numberOfAdult + numberOfInfant + hotelID + (int)ClientTypes.CUSTOMER;
                                        
                                        var str = redisService.Get(CacheName.HotelRoomDetail + cache_name_id, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                                        if (str != null && str.Trim() != "")
                                        {
                                            HotelRoomDetailModel model = JsonConvert.DeserializeObject<HotelRoomDetailModel>(str);
                                            //-- Trả kết quả
                                            var min_price_all = model.rooms == null || model.rooms.Count <= 0 ? new RoomDetail() { min_price = 0 } : model.rooms.OrderBy(x => x.min_price).First();
                                            data.Add(new HotelB2CMinPriceModel()
                                            {
                                                amount = min_price_all != null ? min_price_all.min_price : 0,
                                                hotel_id = h.hotel_id,
                                                price = 0,
                                                profit = 0
                                            });
                                        }

                                        HotelRoomDetailModel result = new HotelRoomDetailModel();
                                        result.input_api_vin = input_api_vin_all;
                                        // Đọc Json ra các Field để map với những trường cần lấy
                                        var hotel = await hotelDetailRepository.GetByHotelId(hotelID);
                                   
                                        var hotel_rooms = hotelDetailRepository.GetFEHotelRoomList(hotel.Id);
                                        if (hotel_rooms != null && hotel_rooms.Any())
                                        {
                                            // Thông tin khách sạn
                                            result = new HotelRoomDetailModel()
                                            {
                                                hotel_id = hotelID,
                                                rooms = new List<RoomDetail>(),
                                            };

                                            var profit_list = hotelDetailRepository.GetHotelRoomPricePolicy(hotelID, "5");

                                            foreach (var room in hotel_rooms)
                                            {
                                                var room_packages = hotelDetailRepository.GetFERoomPackageListByRoomId(Convert.ToInt32(room.Id), cache_data.search.arrival_date, cache_data.search.departure_date);
                                                var room_packages_daily = hotelDetailRepository.GetFERoomPackageDaiLyListByRoomId(room.Id, cache_data.search.arrival_date, cache_data.search.departure_date);

                                                var img_thumb_room = new List<thumbnails>();
                                                if (!string.IsNullOrEmpty(room.RoomAvatar))
                                                {
                                                    var j_thumb_room_img = room.RoomAvatar.Split(",").ToArray();

                                                    img_thumb_room = j_thumb_room_img.Select(s => new thumbnails
                                                    {
                                                        id = string.Empty,
                                                        url = !s.Contains("http") && configuration["config_value:ImageStatic"] != null && configuration["config_value:ImageStatic"].Trim() != "" && !s.Contains(configuration["config_value:ImageStatic"]) ? configuration["config_value:ImageStatic"] + s : s
                                                    }).ToList();
                                                }
                                                else
                                                {
                                                    img_thumb_room = new List<thumbnails> { new thumbnails
                                                        {
                                                           id = string.Empty,
                                                            url = room.Avatar
                                                        } 
                                                    };
                                                }
                                                double min_price = 0;
                                                var package_prices = room_packages.Where(s => s.RoomTypeId == room.Id);
                                                if (package_prices != null && package_prices.Any())
                                                {
                                                    var min_package_item = package_prices.Aggregate((c, d) => c.Amount < d.Amount ? c : d);
                                                    min_price = package_prices.Where(s => s.PackageCode == min_package_item.PackageCode).Select(s => s.Amount).Average();
                                                }
                                                var item = PricePolicyService.GetRoomDetail(room.Id.ToString(), cache_data.search.arrival_date, cache_data.search.departure_date, cache_data.search.total_nights, room_packages_daily, room_packages, profit_list, hotel,
                                                    new RoomDetail
                                                    {
                                                        id = room.Id.ToString(),
                                                        //code = roo,
                                                        name = room.Name,
                                                        description = room.Description,
                                                        max_adult = room.NumberOfAdult ?? 0,
                                                        max_child = room.NumberOfChild ?? 0,
                                                        img_thumb = img_thumb_room,
                                                        min_price = min_price * cache_data.search.total_nights,
                                                        remainming_room = room.NumberOfRoom ?? 0,
                                                        rates = room_packages.Where(s => s.RoomTypeId == room.Id).Select(s => new RoomDetailRate
                                                        {
                                                            id = s.Id.ToString(),
                                                            amount = s.Amount,
                                                            code = s.PackageCode,
                                                            program_id = s.ProgramId,
                                                            apply_date = s.ApplyDate
                                                        }).Distinct().ToList(),
                                                        bed_room_type_name = room.BedRoomTypeName
                                                    }
                                                    );
                                                result.rooms.Add(item);
                                            }
                                            redisService.Set(CacheName.HotelRoomDetail + cache_name_id, JsonConvert.SerializeObject(result), DateTime.Now.AddMinutes(30), db_index);
                                            //-- Trả kết quả
                                            var min_price_all = result.rooms == null || result.rooms.Count <= 0 ? new RoomDetail() { min_price = 0 } : result.rooms.OrderBy(x => x.min_price).First();
                                            data.Add(new HotelB2CMinPriceModel()
                                            {
                                                amount = min_price_all != null ? min_price_all.min_price : 0,
                                                hotel_id = h.hotel_id,
                                                price = 0,
                                                profit = 0
                                            });
                                        }
                                    
                                    }
                                    break;
                            }
                        }
                        if (data.Count > 0)
                        {
                            redisService.Set(cache_name_price, JsonConvert.SerializeObject(data), DateTime.Now.AddDays(1), db_index);
                        }
                        return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), msg = "Get Data Success", data = data, cache_id = cache_name });
                    }
                   
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
                LogService.InsertLog("GetHotelMinPrice - HotelB2CController: " + ex.ToString());

                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "error: " + ex.ToString() });
            }
        }
        [HttpPost("search-filter")]
        public async Task<ActionResult> GetHotelFilterFields(string token)
        {
            #region Test
            //var j_param = new Dictionary<string, string>
            //    {
            //          { "cache_id", "HS_B2C_04/10/202404/11/20241110d6ef5f7fa914c19931a55bb262ec879c005" }
            //    };

            //var data_product = JsonConvert.SerializeObject(j_param);
            //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
            #endregion
            try
            {
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    string cache_id = objParr[0]["cache_id"].ToString();

                    var str = redisService.Get(cache_id, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                    if (str != null && str.Trim() != "")
                    {
                        HotelB2CCacheModel obj = JsonConvert.DeserializeObject<HotelB2CCacheModel>(str);
                        if(obj.filter!=null && obj.filter.star != null)
                        {

                        }
                        else
                        {
                            List<HotelSearchEntities> source = obj.hotels;
                            HotelFilters filters = new HotelFilters();
                            // Hotel Filter:
                            //---- Hạng sao :
                            filters.star = source.Select(x => new FilterGroup() { key = Math.Round(x.star, 0).ToString(), description = Math.Round(x.star, 0).ToString() + " sao" }).ToList();
                            filters.star = filters.star.GroupBy(x => x.key).Select(g => g.First()).ToList();
                            //----- refunable
                            filters.refundable = source.Select(x => new FilterGroup() { key = x.is_refundable.ToString(), description = x.is_refundable == true ? "Cho phép hủy đặt phòng" : "Không cho phép hủy đặt phòng" }).Distinct().ToList();
                            filters.refundable = filters.refundable.GroupBy(x => x.key).Select(g => g.First()).ToList();
                            //---- Khoảng giá
                            var price = source.Select(x => x.min_price).ToList();
                            filters.price_range = new Dictionary<string, double> {
                               {"max", price.OrderByDescending(x => x).FirstOrDefault()},
                               {"min", price.OrderBy(x => x).FirstOrDefault()},
                            };
                            //---- Tiện ích:
                            if (source.Any(x=>x.amenities!=null))
                            {
                                var a = source.SelectMany(x => x.amenities);

                                filters.amenities = a.Select(x => new FilterGroup() { key = x.key, description = x.description }).ToList();
                                filters.amenities = filters.amenities.GroupBy(x => x.key).Select(g => g.First()).ToList();
                            }

                            //---- Loại phòng:
                            if (source.Any(x => x.type_of_room != null))
                            {
                                var b = source.SelectMany(x => x.type_of_room);

                                filters.type_of_room = b.Select(z => new FilterGroup() { key = z, description = z }).ToList();
                                filters.type_of_room = filters.type_of_room.GroupBy(x => x.key).Select(g => g.First()).ToList();
                            }
                                                        //---- Loại khách sạn:
                            filters.hotel_type = source.Select(z => new FilterGroup() { key = z.hotel_type, description = z.hotel_type }).ToList();
                            filters.hotel_type = filters.hotel_type.GroupBy(x => x.key).Select(g => g.First()).ToList();
                            //-- Add to cache obj:
                            obj.filter = filters;
                            //-- Cache kết quả:
                            int db_index = Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"].ToString());
                            redisService.Set(cache_id, JsonConvert.SerializeObject(obj), DateTime.Now.AddDays(1), db_index);
                        }
                        //-- Trả kết quả
                        return Ok(new
                        {
                            data = obj.filter,
                            status = (int)ResponseType.SUCCESS,
                            msg = " Success",
                            cache_id= cache_id
                        });

                    }
                    else
                    {

                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Cannot Get Data From Cache",
                            cache_id = cache_id

                        });
                    }

                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key không hợp lệ",
                    });
                }

            }
            catch (Exception ex)
            {
                LogService.InsertLog("VinController - GetHotelFilterFields ["+token+"]: " + ex.ToString());

                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "error: " + ex.ToString() });
            }
        }

        [HttpPost("get-hotel-room")]
        public async Task<ActionResult> GetHotelRoom(string token)
        {
            #region Test
            //var j_param = new Dictionary<string, string>
            //{
            //    {"arrivalDate", "2023-05-10"},
            //    {"departureDate","2023-05-12" },
            //    {"numberOfRoom", "1"},
            //    {"hotelID","1114" },
            //    {"numberOfChild","0" },
            //    {"numberOfAdult","2" },
            //    {"numberOfInfant","0" },
            //    {"hotelType","0" },
            //};
            //var data_product = JsonConvert.SerializeObject(j_param);
            //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
            #endregion
            try
            {
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    int hotel_type = Convert.ToInt32(objParr[0]["hotelType"].ToString());
                    switch (hotel_type)
                    {
                        case (int)HotelGroupType.VIN:
                            {
                                return await GetVinHotelRoom(token);
                            }
                        case (int)HotelGroupType.ManualHotel:
                            {
                                return await GetManualHotelRoom(token);
                            }
                        default:
                            {
                                return Ok(new
                                {
                                    status = (int)ResponseType.FAILED,
                                    msg = "Key không hợp lệ",
                                });
                            }
                    }

                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Key không hợp lệ",
                    });
                }

            }
            catch (Exception ex)
            {
                LogService.InsertLog("VinController - GetHotelFilterFields: " + ex.ToString());

                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "error: " + ex.ToString() });
            }
        }
        [HttpPost("get-hotel-room-packages")]
        public async Task<ActionResult> GetHotelRoomPackages(string token)
        {
            #region Test
            //var j_param = new Dictionary<string, string>
            //{
            //    {"arrivalDate", "2023-05-10"},
            //    {"departureDate","2023-05-12" },
            //    {"numberOfRoom", "1"},
            //    {"hotelID","1114" },
            //    {"numberOfChild","0" },
            //    {"numberOfAdult","2" },
            //    {"numberOfInfant","0" },
            //    {"hotelType","0" },
            //};
            //var data_product = JsonConvert.SerializeObject(j_param);
            //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
            #endregion
            try
            {
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    int hotel_type = Convert.ToInt32(objParr[0]["hotelType"].ToString());
                    switch (hotel_type)
                    {
                        case (int)HotelGroupType.VIN:
                            {
                                return await GetVinHotelRoomPackage(token);
                            }
                        case (int)HotelGroupType.ManualHotel:
                            {
                                return await GetManualHotelRoomPackage(token);
                            }
                        default:
                            {
                                return Ok(new
                                {
                                    status = (int)ResponseType.FAILED,
                                    msg = "Key không hợp lệ",
                                });
                            }
                    }

                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Key không hợp lệ",
                    });
                }

            }
            catch (Exception ex)
            {
                LogService.InsertLog("VinController - GetHotelFilterFields: " + ex.ToString());

                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "error: " + ex.ToString() });
            }
        }

        /// <summary>
        /// Tracking  thông tin 1 phòng khách sạn và các loại phòng trong đó theo 1 khoảng thời gian
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("vin/get-hotel-rooms.json")]
        public async Task<ActionResult> GetVinHotelRoom(string token)
        {
            try
            {
                #region Test

                //var j_param = new Dictionary<string, string>
                //{
                //    {"arrivalDate", "2023-05-10"},
                //    {"departureDate","2023-05-12" },
                //    {"numberOfRoom", "1"},
                //    {"hotelID","1114" },
                //    {"numberOfChild","0" },
                //    {"numberOfAdult","2" },
                //    {"numberOfInfant","0" },
                //    {"hotelType","2" },
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion.


                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {

                    string cancel_template = "Phí thay đổi / Hoàn hủy từ ngày {day} tháng {month} năm {year} là {value}";
                    string cancel_percent_template = " {value_2} % giá phòng";
                    string zero_percent = "Miễn phí Thay đổi / Hoàn hủy trước ngày {day} tháng {month} năm {year}";
                    string cancel_vnd_template = " {value_2} VND";

                    string status_vin = string.Empty;
                    string arrivalDate = objParr[0]["arrivalDate"].ToString();
                    string departureDate = objParr[0]["departureDate"].ToString();
                    DateTime arrivalDate_date = DateTime.ParseExact(objParr[0]["arrivalDate"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    DateTime departureDate_date = DateTime.ParseExact(objParr[0]["departureDate"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    int numberOfRoom = Convert.ToInt16(objParr[0]["numberOfRoom"]);
                    int numberOfChild = Convert.ToInt16(objParr[0]["numberOfChild"]);
                    int numberOfAdult = Convert.ToInt16(objParr[0]["numberOfAdult"]);
                    int numberOfInfant = Convert.ToInt16(objParr[0]["numberOfInfant"]);

                    string hotelID = objParr[0]["hotelID"].ToString();
                    string distributionChannelId = configuration["config_api_vinpearl:Distribution_ID"].ToString();
                    string input_api_vin_all = "{ \"distributionChannelId\": \"" + distributionChannelId + "\", \"propertyID\": \"" + hotelID + "\", \"numberOfRoom\":" + numberOfRoom + ", \"arrivalDate\":\"" + arrivalDate + "\", \"departureDate\":\"" + departureDate + "\", \"roomOccupancy\":{\"numberOfAdult\":" + numberOfAdult + ",\"otherOccupancies\":[{\"otherOccupancyRefCode\":\"child\",\"quantity\":" + numberOfChild + "},{\"otherOccupancyRefCode\":\"infant\",\"quantity\":" + numberOfInfant + "}]}}";
                    int number_room_each = 1;
                    int number_adult_each_room = (numberOfAdult / (float)numberOfRoom) > (int)(numberOfAdult / numberOfRoom) ? (int)(numberOfAdult / numberOfRoom) + 1 : (int)(numberOfAdult / numberOfRoom);
                    int number_child_each_room = numberOfChild == 1 || (((int)numberOfChild / numberOfRoom) <= 1 && numberOfChild > 0) ? 1 : numberOfChild / numberOfRoom;
                    int number_infant_each_room = numberOfInfant == 1 || (((int)numberOfInfant / numberOfRoom) <= 1 && numberOfInfant > 0) ? 1 : numberOfInfant / numberOfRoom;
                    string input_api_vin_phase = "{ \"distributionChannelId\": \"" + distributionChannelId + "\", \"propertyID\": \"" + hotelID + "\", \"numberOfRoom\":" + number_room_each + ", \"arrivalDate\":\"" + arrivalDate + "\", \"departureDate\":\"" + departureDate + "\", \"roomOccupancy\":{\"numberOfAdult\":" + number_adult_each_room + ",\"otherOccupancies\":[{\"otherOccupancyRefCode\":\"child\",\"quantity\":" + number_child_each_room + "},{\"otherOccupancyRefCode\":\"infant\",\"quantity\":" + number_infant_each_room + "}]}}";

                    //-- Đọc từ cache, nếu có trả kết quả:
                    string cache_name_id = arrivalDate + departureDate + numberOfRoom + numberOfChild + numberOfAdult + numberOfInfant+ EncodeHelpers.MD5Hash(objParr[0]["hotelID"].ToString())   + (int)ClientTypes.CUSTOMER;
                    var str = redisService.Get(CacheName.HotelRoomDetail + cache_name_id, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                    string response = "";
                    if (str != null && str.Trim() != "")
                    {
                        HotelRoomDetailModel model = JsonConvert.DeserializeObject<HotelRoomDetailModel>(str);
                        var view_model = JsonConvert.DeserializeObject<List<RoomDetailViewModel>>(JsonConvert.SerializeObject(model.rooms));
                        //-- Trả kết quả
                        return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), msg = "Get Data From Cache Success", data = view_model, cache_id = CacheName.HotelRoomDetail + cache_name_id });
                    }
                    else
                    {
                        var vin_lib = new VinpearlLib(configuration);
                        response = vin_lib.getRoomAvailability(input_api_vin_phase).Result;
                    }
                    //LogHelper.InsertLogTelegram("GetVinHotelRoom - HotelB2CController: Cannot get from Redis [" + CacheName.HotelRoomDetail + cache_name_id + "] - token: " + token);

                    HotelRoomDetailModel result = new HotelRoomDetailModel();
                    result.input_api_vin = input_api_vin_all;
                    var data_hotel = JObject.Parse(response);
                    // Đọc Json ra các Field để map với những trường cần lấy

                    #region Check Data Invalid
                    if (data_hotel["isSuccess"].ToString().ToLower() == "false")
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.EMPTY,
                            msg = "Tham số không hợp lệ",
                            data = data_hotel
                        });
                    }
                    #endregion


                    var j_room_list = data_hotel["data"]["propertyInfo"]["roomTypes"];
                    var j_rate_list = data_hotel["data"]["roomAvailabilityRates"];
                    if (j_room_list.Count() > 0 && j_rate_list.Count() > 0)
                    {
                        // Thông tin khách sạn
                        result = new HotelRoomDetailModel()
                        {
                            hotel_id = data_hotel["data"]["propertyInfo"]["id"].ToString(),
                            rooms = new List<RoomDetail>(),
                        };

                        foreach (var room in j_room_list)
                        {
                            #region Hình ảnh phòng

                            var img_thumb_room = new List<thumbnails>();
                            var j_thumb_room_img = room["thumbnails"];
                            foreach (var item_thumb in j_thumb_room_img)
                            {
                                var item_img_room = new thumbnails
                                {
                                    id = item_thumb["id"].ToString(),
                                    url = item_thumb["url"].ToString()
                                };
                                img_thumb_room.Add(item_img_room);
                            }
                            #endregion

                            #region Chi tiết phòng
                            var item = new RoomDetail
                            {
                                id = room["id"].ToString(),
                                code = room["code"].ToString(),
                                name = room["name"].ToString(),
                                description = room["description"].ToString(),
                                max_adult = Convert.ToInt32(room["maxAdult"]),
                                max_child = Convert.ToInt32(room["maxChild"]),
                                img_thumb = img_thumb_room,
                                remainming_room = Convert.ToInt32(room["numberOfRoom"]),
                                rates = new List<RoomDetailRate>(),
                                number_bed_room = Convert.ToInt32(room["numberOfBedRoom"]),
                                number_extra_bed = Convert.ToInt32(room["maxExtraBeds"]),
                                bed_room_type_name ="",
                                
                                
                            };
                            #endregion
                            result.rooms.Add(item);
                        }

                        //-- Giá gốc + cancel policy
                        foreach (var r in j_rate_list)
                        {
                            var list = JsonConvert.DeserializeObject<List<RoomDetailPackage>>(r["packages"].ToString());
                            if (result.rooms.Where(x => x.id.Trim() == r["roomType"]["roomTypeID"].ToString().Trim()).First().package_includes == null || result.rooms.Where(x => x.id.Trim() == r["roomType"]["roomTypeID"].ToString().Trim()).First().package_includes.Count < 1)
                            {
                                result.rooms.Where(x => x.id.Trim() == r["roomType"]["roomTypeID"].ToString().Trim()).First().package_includes = list.Where(x => x.packageType.ToUpper().Contains("INCLUDE") && x.description != null).Select(x => x.description.ToString().Trim()).ToList();
                            }

                            var cancel_policy = JsonConvert.DeserializeObject<RoomDetailCancelPolicy>(r["ratePlan"]["cancelPolicy"].ToString());
                            cancel_policy.detail = cancel_policy.detail.OrderBy(x => x.amount).ToList();
                            var cancel_policy_output = new List<string>();
                            DateTime arrivalDate_date_time = DateTime.ParseExact(arrivalDate, "yyyy-MM-dd", null);
                            DateTime day_before_arrival_before = DateTime.ParseExact(arrivalDate, "yyyy-MM-dd", null);

                            foreach (var c in cancel_policy.detail)
                            {
                                if (c.amount <= 0)
                                {
                                    day_before_arrival_before = arrivalDate_date_time - new TimeSpan(c.daysBeforeArrival, 0, 0, 0, 0);
                                    string str_cp = zero_percent.Replace("{day}", day_before_arrival_before.Day.ToString()).Replace("{month}", day_before_arrival_before.Month.ToString()).Replace("{year}", day_before_arrival_before.Year.ToString());
                                    cancel_policy_output.Add(str_cp);

                                }
                                else
                                {

                                    string str_cp = cancel_template.Replace("{day}", day_before_arrival_before.Day.ToString()).Replace("{month}", day_before_arrival_before.Month.ToString()).Replace("{year}", day_before_arrival_before.Year.ToString());
                                    str_cp = c.type.ToLower() == "percent" ? str_cp.Replace("{value}", cancel_percent_template.Replace("{value_2}", c.amount.ToString())) : str_cp.Replace("{value}", cancel_vnd_template.Replace("{value_2}", c.amount.ToString()));
                                    cancel_policy_output.Add(str_cp);
                                    day_before_arrival_before = arrivalDate_date_time - new TimeSpan(c.daysBeforeArrival, 0, 0, 0, 0);

                                }
                            }
                            result.rooms.Where(x => x.id.Trim() == r["roomType"]["roomTypeID"].ToString().Trim()).First().rates.Add(
                                new RoomDetailRate()
                                {
                                    id = r["ratePlan"]["id"].ToString(),
                                    amount = Convert.ToDouble(r["totalAmount"]["amount"]["amount"]),
                                    room_code = r["roomType"]["roomTypeID"].ToString(),
                                    code = r["ratePlan"]["rateCode"].ToString(),
                                    description = r["ratePlan"]["description"].ToString(),
                                    name = r["ratePlan"]["name"].ToString(),
                                    cancel_policy = cancel_policy_output,
                                    guarantee_policy = r["ratePlan"]["guaranteePolicy"]["description"].ToString(),
                                    allotment_id = r["allotments"] != null && r["allotments"].Count() > 0 ? r["allotments"][0]["id"].ToString() : "",
                                    package_includes = list
                                }
                            );
                        }
                        DateTime fromdate = DateTime.ParseExact(arrivalDate, "yyyy-M-d", null);
                        DateTime todate = DateTime.ParseExact(departureDate, "yyyy-M-d", null);
                        //-- Tính giá về tay thông qua chính sách giá
                        var profit_list = hotelDetailRepository.GetHotelRoomPricePolicy(hotelID, "5");

                        foreach (var r in result.rooms)
                        {
                            var r_id = r.id;
                            foreach (var rate in r.rates)
                            {
                                var profit = profit_list.Where(x => x.HotelCode == result.hotel_id && x.RoomTypeCode == r_id && x.PackageName == rate.id).ToList();
                                if (profit != null && profit.Count > 0)
                                {
                                    rate.total_profit = PricePolicyService.CalucateMinProfit(profit, rate.amount, fromdate, todate);
                                    rate.total_price = rate.amount + rate.total_profit;

                                }
                                else
                                {
                                    rate.profit = 0;
                                    rate.total_profit = 0;
                                    rate.total_price = 0;
                                }
                                r.min_price = r.min_price <= 0 ? rate.total_price : ((rate.total_price > 0 && r.min_price > rate.total_price) ? rate.total_price : r.min_price);
                            }
                        }


                        //-- Cache kết quả:
                        if (result!=null && result.rooms!=null && result.rooms.Count>0)
                        {

                            int db_index = Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"].ToString());
                            redisService.Set(CacheName.HotelRoomDetail + cache_name_id, JsonConvert.SerializeObject(result), DateTime.Now.AddDays(1), db_index);
                        }
                        var view_model = JsonConvert.DeserializeObject<List<RoomDetailViewModel>>(JsonConvert.SerializeObject(result.rooms));
                        //-- Trả kết quả
                        return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), msg = status_vin, data = view_model, cache_id = CacheName.HotelRoomDetail + cache_name_id });

                    }
                    else
                    {
                        status_vin = "Không tìm thấy danh sách phòng thỏa mãn điều kiện";
                        return Ok(new { status = ((int)ResponseType.EMPTY).ToString(), msg = status_vin });
                    }
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
                LogService.InsertLog("VinController - getHotelRoomsAvailability: " + ex.ToString());
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "error: " + ex.ToString() });
            }

        }

        /// <summary>
        /// Tracking  thông tin 1 phòng khách sạn và các loại phòng trong đó theo 1 khoảng thời gian
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("vin/get-room-packages.json")]
        public async Task<ActionResult> GetVinHotelRoomPackage(string token)
        {
            try
            {
                #region Test

                //var j_param = new Dictionary<string, string>
                //    {
                //          { "cache_id", "hotel_detail_2023-05-102023-05-121020d0c06e7b-28fe-896e-1915-cbe8540f14d82"},
                //          { "roomID", "3efb8c7b-65c4-0ddb-69e7-034b14348056"},
                //    };

                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion


                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    if (objParr[0]["roomID"] == null || objParr[0]["roomID"].ToString().Trim() == "")
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Dữ liệu không hợp lệ"
                        });
                    }
                    string arrivalDate = objParr[0]["arrivalDate"].ToString();
                    string departureDate = objParr[0]["departureDate"].ToString();
                    DateTime arrival_date = DateTime.Parse(arrivalDate);
                    DateTime departure_date = DateTime.Parse(departureDate);

                    string roomID = objParr[0]["roomID"].ToString();

                    string cache_name_id = objParr[0]["cache_id"].ToString();
                    var str = redisService.Get(cache_name_id, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                    string response = "";
                    if (str != null && str.Trim() != "")
                    {
                        HotelRoomDetailModel model = JsonConvert.DeserializeObject<HotelRoomDetailModel>(str);
                        var view_model = model.rooms.Where(x => x.id == roomID).FirstOrDefault();
                        //-- Trả kết quả
                        return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), msg = "Get Data From Cache Success", data = view_model, cache_id = cache_name_id });
                    }

                   

                    else
                    {
                        //LogHelper.InsertLogTelegram("GetVinHotelRoomPackage - HotelB2CController: Cannot get from Redis [" + cache_name_id + "] - token: " + token);

                        var status_vin = "Không tìm thấy danh sách phòng thỏa mãn điều kiện";
                        return Ok(new { status = ((int)ResponseType.EMPTY).ToString(), msg = status_vin, cache_id = cache_name_id });
                    }
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
                LogService.InsertLog("VinController - getHotelRoomsPackages: " + ex.ToString());
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "error: " + ex.ToString() });
            }

        }

        [HttpPost("hotel-manual/get-hotel-rooms.json")]
        public async Task<ActionResult> GetManualHotelRoom(string token)
        {
            try
            {
                #region Test

                //var j_param = new Dictionary<string, string>
                //{
                //    {"arrivalDate", "2023-03-07"},
                //    {"departureDate","2023-03-09" },
                //    {"numberOfRoom", "1"},
                //    {"hotelID","1114" },
                //    {"numberOfChild","0" },
                //    {"numberOfAdult","2" },
                //    {"numberOfInfant","0" },
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion.


                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {

                   
                    string arrivalDate = objParr[0]["arrivalDate"].ToString();
                    string departureDate = objParr[0]["departureDate"].ToString();
                    int numberOfRoom = Convert.ToInt16(objParr[0]["numberOfRoom"]);
                    int numberOfChild = Convert.ToInt16(objParr[0]["numberOfChild"]);
                    int numberOfAdult = Convert.ToInt16(objParr[0]["numberOfAdult"]);
                    int numberOfInfant = Convert.ToInt16(objParr[0]["numberOfInfant"]);
                    string hotelID = objParr[0]["hotelID"].ToString();

                    DateTime arrival_date = DateTime.ParseExact(arrivalDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    DateTime departure_date = DateTime.ParseExact(departureDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    int total_nights = Convert.ToInt32((departure_date - arrival_date).TotalDays);

                    //-- Đọc từ cache, nếu có trả kết quả:
                    string cache_name_id = arrivalDate + departureDate + numberOfRoom + numberOfChild + numberOfAdult + numberOfInfant + objParr[0]["hotelID"].ToString() + (int)ClientTypes.CUSTOMER;
                    var str = redisService.Get(CacheName.HotelRoomDetail + cache_name_id, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                    string response = "";
                    if (str != null && str.Trim() != "")
                    {
                        HotelRoomDetailModel model = JsonConvert.DeserializeObject<HotelRoomDetailModel>(str);
                        var view_model = JsonConvert.DeserializeObject<List<RoomDetailViewModel>>(JsonConvert.SerializeObject(model.rooms));
                        //--Trả kết quả
                        return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), msg = "Get Data From Cache Success", data = view_model, cache_id = CacheName.HotelRoomDetail + cache_name_id });
                    }
                    //LogHelper.InsertLogTelegram("GetManualHotelRoom - HotelB2CController: Cannot get from Redis [" + CacheName.HotelRoomDetail + cache_name_id + "] - token: " + token);

                    HotelRoomDetailModel result = new HotelRoomDetailModel();
                    string input_api_vin_all = "{ \"distributionChannelId\": \"\", \"propertyID\": \"" + hotelID + "\", \"numberOfRoom\":" + numberOfRoom + ", \"arrivalDate\":\"" + arrivalDate + "\", \"departureDate\":\"" + departureDate + "\", \"roomOccupancy\":{\"numberOfAdult\":" + numberOfAdult + ",\"otherOccupancies\":[{\"otherOccupancyRefCode\":\"child\",\"quantity\":" + numberOfChild + "},{\"otherOccupancyRefCode\":\"infant\",\"quantity\":" + numberOfInfant + "}]}}";

                    result.input_api_vin = input_api_vin_all;
                    // Đọc Json ra các Field để map với những trường cần lấy
                    var hotel = await hotelDetailRepository.GetByHotelId(hotelID);
                    if (hotel == null || hotel.Id <= 0)
                    {
                        return Ok(new { status = ((int)ResponseType.FAILED).ToString(), msg = "Không tìm thấy danh sách phòng thỏa mãn điều kiện" });
                    }
                    var hotel_rooms = hotelDetailRepository.GetFEHotelRoomList(hotel.Id);
                    if (hotel_rooms != null && hotel_rooms.Any())
                    {
                        // Thông tin khách sạn
                        result = new HotelRoomDetailModel()
                        {
                            hotel_id = hotelID,
                            rooms = new List<RoomDetail>(),
                        };

                        var profit_list = hotelDetailRepository.GetHotelRoomPricePolicy(hotelID, "5");
                       
                        foreach (var room in hotel_rooms)
                        {
                            var room_packages = hotelDetailRepository.GetFERoomPackageListByRoomId(Convert.ToInt32(room.Id), arrival_date, departure_date);
                            var room_packages_daily = hotelDetailRepository.GetFERoomPackageDaiLyListByRoomId(room.Id, arrival_date, departure_date);

                            var img_thumb_room = new List<thumbnails>();
                            if (!string.IsNullOrEmpty(room.RoomAvatar))
                            {
                                var j_thumb_room_img = room.RoomAvatar.Split(",").ToArray();

                                img_thumb_room = j_thumb_room_img.Select(s => new thumbnails
                                {
                                    id = string.Empty,
                                    url = !s.Contains("http") && configuration["config_value:ImageStatic"]!=null && configuration["config_value:ImageStatic"].Trim()!=""&& !s.Contains(configuration["config_value:ImageStatic"])? configuration["config_value:ImageStatic"]+s:s
                                }).ToList();
                            }
                            else
                            {
                                img_thumb_room = new List<thumbnails> { new thumbnails
                                {
                                   id = string.Empty,
                                    url = room.Avatar
                                } };
                            }
                            double min_price = 0;
                            var package_prices = room_packages.Where(s => s.RoomTypeId == room.Id);
                            if (package_prices != null && package_prices.Any())
                            {
                                var min_package_item = package_prices.Aggregate((c, d) => c.Amount < d.Amount ? c : d);
                                min_price = package_prices.Where(s => s.PackageCode == min_package_item.PackageCode).Select(s => s.Amount).Average();
                            }
                            var item = PricePolicyService.GetRoomDetail(room.Id.ToString(), arrival_date, departure_date, total_nights, room_packages_daily, room_packages, profit_list, hotel,
                                new RoomDetail
                                {
                                    id = room.Id.ToString(),
                                    //code = roo,
                                    name = room.Name,
                                    description = room.Description,
                                    max_adult = room.NumberOfAdult ?? 0,
                                    max_child = room.NumberOfChild ?? 0,
                                    img_thumb = img_thumb_room,
                                    min_price = min_price * total_nights,
                                    remainming_room = room.NumberOfRoom ?? 0,
                                    rates = room_packages.Where(s => s.RoomTypeId == room.Id).Select(s => new RoomDetailRate
                                    {
                                        id = s.Id.ToString(),
                                        amount = s.Amount,
                                        code = s.PackageCode,
                                        program_id = s.ProgramId,
                                        apply_date = s.ApplyDate
                                    }).Distinct().ToList(),
                                    bed_room_type_name = room.BedRoomTypeName
                                }
                                );
                            result.rooms.Add(item);
                        }

                       
                      

                        int db_index = Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"].ToString());
                        redisService.Set(CacheName.HotelRoomDetail + cache_name_id, JsonConvert.SerializeObject(result), DateTime.Now.AddDays(1), db_index);
                        var view_model = JsonConvert.DeserializeObject<List<RoomDetailViewModel>>(JsonConvert.SerializeObject(result.rooms));
                        //-- Trả kết quả
                        return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), msg = "Success", data = view_model, cache_id = CacheName.HotelRoomDetail + cache_name_id });
                    }
                    else
                    {
                        return Ok(new { status = ((int)ResponseType.EMPTY).ToString(), msg = "Không tìm thấy danh sách phòng thỏa mãn điều kiện" });
                    }
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
                LogService.InsertLog("VinController - getHotelRoomsAvailability: " + ex.ToString());
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "error: " + ex.ToString() });
            }

        }

        [HttpPost("hotel-manual/get-room-packages.json")]
        public async Task<ActionResult> GetManualHotelRoomPackage(string token)
        {
            try
            {
                #region Test

                //var j_param = new Dictionary<string, string>
                //    {
                //          { "cache_id", "hotel_detail_2023-05-102023-05-121020d0c06e7b-28fe-896e-1915-cbe8540f14d82"},
                //          { "roomID", "3efb8c7b-65c4-0ddb-69e7-034b14348056"},
                //    };

                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    if (objParr[0]["roomID"] == null || objParr[0]["roomID"].ToString().Trim() == "")
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Dữ liệu không hợp lệ"
                        });
                    }

                    string roomID = objParr[0]["roomID"].ToString();
                    string arrivalDate = objParr[0]["arrivalDate"].ToString();
                    string departureDate = objParr[0]["departureDate"].ToString();
                    var arrival_date = DateTime.ParseExact(arrivalDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    var departure_date = DateTime.ParseExact(departureDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    int nights = Convert.ToInt32((departure_date - arrival_date).TotalDays < 1 ? 1 : (departure_date - arrival_date).TotalDays);
                    int db_index = Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]);
                    //-- Get HotelID:
                    int room_id = -1;
                    bool success = int.TryParse(roomID, out room_id);
                    if (!success)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "Data không hợp lệ"
                        });
                    }
                    var hotel = await hotelDetailRepository.GetHotelContainRoomid(int.Parse(roomID));
                    //-- Đọc từ cache, nếu có trả kết quả:
                    string cache_name_id = arrivalDate + departureDate + roomID + (int)ClientTypes.CUSTOMER;
                    var str = redisService.Get(CacheName.B2C_HotelPackage + EncodeHelpers.MD5Hash(hotel.HotelId) + "_" + cache_name_id, db_index);
                    if (str != null && str.Trim() != "")
                    {
                        RoomDetail model = JsonConvert.DeserializeObject<RoomDetail>(str);
                        //-- Trả kết quả
                        return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), msg = "Get Data Success", data = model, cache_id = string.Empty });
                    }
                    //LogHelper.InsertLogTelegram("GetManualHotelRoomPackage - HotelB2CController: Cannot get from Redis [" + CacheName.B2C_HotelPackage + cache_name_id + "] - token: " + token);

                    var room_packages = hotelDetailRepository.GetFERoomPackageListByRoomId(int.Parse(roomID), arrival_date, departure_date);
                    var room_packages_daily = hotelDetailRepository.GetFERoomPackageDaiLyListByRoomId(int.Parse(roomID), arrival_date, departure_date);

                    var profit_list = hotelDetailRepository.GetHotelRoomPricePolicy(hotel.HotelId, "5");

                    var view_model = PricePolicyService.GetRoomDetail(roomID, arrival_date, departure_date, nights, room_packages_daily, room_packages, profit_list, hotel, null);

                    if (view_model == null)
                    {
                        return Ok(new { status = ((int)ResponseType.EMPTY).ToString(), msg = "Không tìm thấy danh sách gói thỏa mãn điều kiện", cache_id = string.Empty });
                    }
                    else
                    {
                        redisService.Set(CacheName.B2C_HotelPackage + cache_name_id, JsonConvert.SerializeObject(view_model), DateTime.Now.AddDays(1), db_index);
                        return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), msg = "Get Data Success", data = view_model, cache_id = string.Empty });
                    }
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
                LogService.InsertLog("VinController - getHotelRoomsPackages: " + ex.ToString());
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "error: " + ex.ToString() });
            }

        }

        [HttpPost("save-booking.json")]
        public async Task<ActionResult> PushBookingToMongo(string token)
        {
            #region Test

            //var object_input = new BookingHotelB2CViewModel()
            //{
            //    booking_id= "65a9d5700d01d2a5175e128c",
            //    account_client_id = 159,
            //    search = new BookingHotelB2CViewModelSearch()
            //    {
            //        arrivalDate = "2024-01-19",
            //        departureDate = "2024-01-23",
            //        hotelID = "1101",
            //        numberOfAdult = 2,
            //        numberOfChild = 1,
            //        numberOfInfant = 1,
            //        numberOfRoom = 2
            //    },
            //    detail = new BookingHotelB2CViewModelDetail()
            //    {
            //        email = "EMAIL đã sửa",
            //        telephone = "PHone đã sửa"
            //    },
            //    contact = new BookingHotelB2CViewModelContact
            //    {
            //        birthday = "2022-09-01",
            //        country = "VN",
            //        province = "Hanoi",
            //        district = "Cau Giay",
            //        ward = "Dich vong hau",
            //        email = "abc@gmail.com",
            //        firstName = "Minh",
            //        lastName = "Nguyen",
            //        note = "Field Note DA SUA DOI",
            //        phoneNumber = "0123456789",
            //    },

            //    pickup = new BookingHotelB2CViewModelPickUp()
            //    {
            //        arrive = new BookingHotelB2CViewModelPickUpForm
            //        {
            //            required = 1,
            //            amount_of_people = 4,
            //            date = "2022-09-01",
            //            fly_code = "ABCXYZ",
            //            id_request = "PICKUP_ARRIVE_DEFAULT",
            //            note = "Yeu cau viet tai field nay",
            //            stop_point_code = "",
            //            vehicle = "Xe o to",
            //            time = "11:30:00.000",

            //        },
            //        departure = new BookingHotelB2CViewModelPickUpForm
            //        {
            //            required = 0,
            //            amount_of_people = 4,
            //            date = "2022-09-03",
            //            fly_code = "",
            //            id_request = "PICKUP_ARRIVE_DEFAULT",
            //            note = "Yeu cau viet tai field nay",
            //            stop_point_code = "San bay X",
            //            vehicle = "Xe o to",
            //            time = "11:30:00.000",
            //        }
            //    },
            //    rooms = new List<BookingHotelB2CViewModelRooms>()
            //    {
            //        new BookingHotelB2CViewModelRooms()
            //        {
            //            room_type_id="e3b015d5-09a4-9cd5-27d2-aaab5ba4432e",
            //            room_type_code="BKSTN",
            //            room_type_name="Standard King",
            //            special_request="Phong view bien",
            //            price=3450000,
            //            profit=50000,
            //            total_amount=3500000,
            //            rates=new List<BookingHotelB2CViewModelRates>()
            //            {
            //                new BookingHotelB2CViewModelRates()
            //                {
            //                    allotment_id="a924ace9-7cc3-4552-b80d-34e725f442ef",
            //                    arrivalDate="2022-09-01",
            //                    departureDate="2022-09-02",
            //                    rate_plan_code="ADBBD20BR1",
            //                    rate_plan_id="cd737197-61b3-443b-9476-fb043140ca40",
            //                    packages=new List<BookingHotelB2CViewModelPackage>(),
            //                    price=1450000,
            //                    profit=50000,
            //                    total_amount=1500000
            //                },
            //                new BookingHotelB2CViewModelRates()
            //                {
            //                    allotment_id="accbbcc-7cc3-4552-b80d-34e725f442ac",
            //                    arrivalDate="2022-09-02",
            //                    departureDate="2022-09-03",
            //                    rate_plan_code="CODE2",
            //                    rate_plan_id="cd737197-61b3-443b-9476-fb043140ca40",
            //                    packages=new List<BookingHotelB2CViewModelPackage>(),
            //                    price=1950000,
            //                    profit=50000,
            //                    total_amount=2000000
            //                }
            //            },
            //            guests = new List<BookingHotelB2CViewModelGuest>()
            //            {
            //               new  BookingHotelB2CViewModelGuest {
            //                   profile_type=2,
            //                   room=1,
            //                   birthday= "2022-09-01",
            //                   firstName="Nguyen",
            //                   lastName="Van A",
            //               },
            //               new  BookingHotelB2CViewModelGuest {
            //                   profile_type=2,
            //                   room=1,
            //                   birthday= "2022-09-01",
            //                   firstName="Tran",
            //                   lastName="Thi A",
            //               },
            //            },
            //        },
            //        new BookingHotelB2CViewModelRooms()
            //        {
            //            room_type_id="e3b015d5-09a4-9cd5-27d2-aaab5ba4432e",
            //            room_type_code="AKSTN",
            //            room_type_name="Standard TWIN",
            //            special_request="Yeu cau su rieng tu",
            //            price=2400000,
            //            profit=50000,
            //            total_amount=2450000,
            //            rates=new List<BookingHotelB2CViewModelRates>()
            //            {
            //                new BookingHotelB2CViewModelRates()
            //                {
            //                    allotment_id="a924ace9-7cc3-4552-b80d-34e725f442ef",
            //                    arrivalDate="2022-09-01",
            //                    departureDate="2022-09-02",
            //                    rate_plan_code="ADBBD20BR1",
            //                    rate_plan_id="cd737197-61b3-443b-9476-fb043140ca40",
            //                    packages=new List<BookingHotelB2CViewModelPackage>(),
            //                    price=1400000,
            //                    profit=50000,
            //                    total_amount=1450000
            //                },
            //                new BookingHotelB2CViewModelRates()
            //                {
            //                    allotment_id="accbbcc-7cc3-4552-b80d-34e725f442ac",
            //                    arrivalDate="2022-09-02",
            //                    departureDate="2022-09-03",
            //                    rate_plan_code="CODE2",
            //                    rate_plan_id="cd737197-61b3-443b-9476-fb043140ca40",
            //                    packages=new List<BookingHotelB2CViewModelPackage>(),
            //                    price=950000,
            //                    profit=50000,
            //                    total_amount=1000000
            //                }
            //            },
            //            guests = new List<BookingHotelB2CViewModelGuest>()
            //            {

            //               new  BookingHotelB2CViewModelGuest {
            //                   profile_type=2,
            //                   room=2,
            //                   birthday= "2022-09-01",
            //                   firstName="Nguyen",
            //                   lastName="Van B",
            //               },
            //               new  BookingHotelB2CViewModelGuest {
            //                   profile_type=2,
            //                   room=2,
            //                   birthday= "2022-09-01",
            //                   firstName="Nguyen",
            //                   lastName="Van C",
            //               },
            //            },
            //        }
            //    },

            //};
            //var input_json = JsonConvert.SerializeObject(object_input);
            //token = CommonHelper.Encode(input_json, configuration["DataBaseConfig:key_api:b2c"]);
            // string input_json = "{ \"contact\": { \"firstName\": \"Cường\", \"lastName\": \"\", \"email\": \"\", \"phoneNumber\": \"0942066299\", \"country\": \"Việt Nam\", \"birthday\": \"\", \"province\": \"\", \"district\": \"\", \"ward\": \"\", \"address\": \"\", \"note\": \"\" }, \"pickup\": { \"arrive\": { \"required\": 1, \"id_request\": null, \"stop_point_code\": \"\", \"vehicle\": \"car\", \"fly_code\": \"\", \"amount_of_people\": 3, \"datetime\": \"2023-04-11T16:04:00Z\", \"note\": \"left note\" }, \"departure\": { \"required\": 1, \"id_request\": null, \"stop_point_code\": \"\", \"vehicle\": \"bus\", \"fly_code\": \"\", \"amount_of_people\": \"2\", \"datetime\": \"2023-04-14T16:04:00Z\", \"note\": \"right note\" } }, \"search\": { \"arrivalDate\": \"2023-04-07\", \"departureDate\": \"2023-04-08\", \"hotelID\": \"340e8b59-4b88-9b69-5283-9922b91c6236\", \"numberOfRoom\": 2, \"numberOfAdult\": 3, \"numberOfChild\": 2, \"numberOfInfant\": 0 }, \"detail\": { \"email\": \"res.VPDSSLNT@vinpearl.com\", \"telephone\": \"(+84-258) 359 8900\" }, \"rooms\": [ { \"room_number\": \"1\", \"room_type_id\": \"b03de1cb-8e75-696a-a5cf-e2d6f8f92865\", \"room_type_code\": \"KDLN\", \"room_type_name\": \"Deluxe King\", \"numberOfAdult\": 2, \"numberOfChild\": 0, \"numberOfInfant\": 0, \"package_includes\": [ \"Internal Breakdown - Daily Breakfast - Child\", \"Internal Breakdown - VinWonders - Child\", \"Internal Breakdown - Daily Breakfast - Adult\", \"Internal Breakdown - Daily Dinner - Adult\", \"Internal Breakdown - Daily Lunch - Child\", \"Internal Breakdown - VinWonders - Adult\", \"Internal Breakdown - Daily Lunch - Adult\", \"Internal Breakdown - Daily Dinner - Child\" ], \"price\": 2975000.0, \"profit\": 50000.0, \"total_amount\": 3025000.0, \"special_request\": \"\", \"rates\": [ { \"arrivalDate\": \"2023-04-07\", \"departureDate\": \"2023-04-08\", \"rate_plan_code\": \"PR12108BBBR1\", \"rate_plan_id\": \"f3233c15-e6a0-4c2b-ad3b-2b6290d9ad2c\", \"allotment_id\": \"c588354a-67cc-4d85-b001-e7e9469bb317\", \"price\": 2975000.0, \"profit\": 50000.0, \"total_amount\": 3025000.0 } ], \"guests\": [ { \"profile_type\": 2, \"room\": 1, \"firstName\": \"hải\", \"lastName\": \"\", \"birthday\": \"1998-04-06\" }, { \"profile_type\": 2, \"room\": 1, \"firstName\": \"long\", \"lastName\": \"\", \"birthday\": \"1998-04-06\" } ] }, { \"room_number\": \"2\", \"room_type_id\": \"b03de1cb-8e75-696a-a5cf-e2d6f8f92865\", \"room_type_code\": \"KDLN\", \"room_type_name\": \"Deluxe King\", \"numberOfAdult\": 1, \"numberOfChild\": 2, \"numberOfInfant\": 0, \"package_includes\": [ \"Internal Breakdown - Daily Breakfast - Child\", \"Internal Breakdown - VinWonders - Child\", \"Internal Breakdown - Daily Breakfast - Adult\", \"Internal Breakdown - Daily Dinner - Adult\", \"Internal Breakdown - Daily Lunch - Child\", \"Internal Breakdown - VinWonders - Adult\", \"Internal Breakdown - Daily Lunch - Adult\", \"Internal Breakdown - Daily Dinner - Child\" ], \"price\": 2975000.0, \"profit\": 50001.0, \"total_amount\": 3025001.0, \"special_request\": \"\", \"rates\": [ { \"arrivalDate\": \"2023-04-07\", \"departureDate\": \"2023-04-08\", \"rate_plan_code\": \"BABBBR1\", \"rate_plan_id\": \"1c960ee3-0b3b-416d-b1dc-7bcd680a1b95\", \"allotment_id\": \"edd184e5-9116-4cef-a299-7fb9a940f9ab\", \"price\": 2975000.0, \"profit\": 50001.0, \"total_amount\": 3025001.0 } ], \"guests\": [ { \"profile_type\": 2, \"room\": 2, \"firstName\": \"quang\", \"lastName\": \"\", \"birthday\": \"1998-04-06\" }, { \"profile_type\": 2, \"room\": 2, \"firstName\": \"vinh\", \"lastName\": \"\", \"birthday\": \"2015-04-06\" }, { \"profile_type\": 2, \"room\": 2, \"firstName\": \"giang\", \"lastName\": \"\", \"birthday\": \"2015-04-06\" } ] } ] }";
            //  token = CommonHelper.Encode(input_json, configuration["DataBaseConfig:key_api:b2c"]);

            #endregion
            try
            {
                // LogHelper.InsertLogTelegram("HotelBookingController - PushBookingToMongo: " + token);
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {

                    BookingHotelB2BViewModel detail = JsonConvert.DeserializeObject<BookingHotelB2BViewModel>(objParr[0].ToString());
                    var hotel = _hotelESRepository.FindByHotelId(detail.search.hotelID);
                    if (hotel == null || hotel.id <= 0)
                    {
                        var hotel_sql = await hotelDetailRepository.GetByHotelId(detail.search.hotelID);
                        if (hotel_sql != null && hotel_sql.Id > 0)
                        {
                            hotel = new HotelESViewModel()
                            {
                                checkintime = hotel_sql.CheckinTime == null ? DateTime.Now : (DateTime)hotel_sql.CheckinTime,
                                checkouttime = hotel_sql.CheckoutTime == null ? DateTime.Now : (DateTime)hotel_sql.CheckoutTime,
                                city = hotel_sql.City == null ? "" : hotel_sql.City,
                                country = hotel_sql.Country == null ? "" : hotel_sql.Country,
                                email = hotel_sql.Email == null ? "" : hotel_sql.Email,
                                groupname = hotel_sql.GroupName == null ? "" : hotel_sql.GroupName,
                                hotelid = hotel_sql.HotelId == null ? "" : hotel_sql.HotelId,
                                hoteltype = hotel_sql.HotelType ?? "",
                                id = hotel_sql.Id,
                                imagethumb = hotel_sql.ImageThumb == null ? "" : hotel_sql.ImageThumb,
                                isinstantlyconfirmed = hotel_sql.IsInstantlyConfirmed ?? false,
                                isrefundable = hotel_sql.IsRefundable ?? false,
                                name = hotel_sql.Name,
                                numberofroooms = hotel_sql.NumberOfRoooms ?? 0,
                                reviewcount = hotel_sql.ReviewCount ?? 0,
                                reviewrate = hotel_sql.ReviewRate == null ? 0 : (double)hotel_sql.ReviewRate,
                                _id = hotel_sql.Id.ToString(),
                                star = hotel_sql.Star == null ? 0 : (double)hotel_sql.Star,
                                state = hotel_sql.State ?? "",
                                street = hotel_sql.Street ?? "",
                                telephone = hotel_sql.Telephone,
                                typeofroom = hotel_sql.TypeOfRoom
                            };
                        }
                        else
                        {
                            LogHelper.InsertLogTelegram("HotelBookingController- Token: " + token + " - PushBookingToMongo: Cannot Find HotelId=" + detail.search.hotelID);

                            return Ok(new
                            {
                                status = (int)ResponseType.FAILED,
                                msg = "Khong thể lưu booking"
                            });
                        }
                    }
                    BookingHotelMongoViewModel model = new BookingHotelMongoViewModel();
                    model.account_client_id = detail.account_client_id;
                    detail.detail = new BookingHotelB2BViewModelDetail()
                    {
                        address = hotel != null ? hotel.street : "",
                        check_in_time = hotel == null || hotel.checkintime == null ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 14, 00, 00) : Convert.ToDateTime(hotel.checkintime.ToString()),
                        check_out_time = hotel == null || hotel.checkouttime == null ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 00, 00) : Convert.ToDateTime(hotel.checkouttime.ToString()),
                        email = hotel != null ? hotel.email : "",
                        image_thumb = hotel != null ? hotel.imagethumb : "",
                        telephone = hotel != null ? hotel.telephone : "",
                    };
                    //-- Xử lý input thành model để lưu mongo:
                    model.account_client_id = detail.account_client_id;
                    model.booking_data = new MongoBookingData()
                    {
                        arrivalDate = detail.search.arrivalDate,
                        departureDate = detail.search.departureDate,
                        distributionChannel = configuration["config_api_vinpearl:Distribution_ID"].ToString(),
                        propertyId = detail.search.hotelID,
                        reservations = new List<HotelMongoReservation>(),
                        sourceCode = "",
                        propertyName = hotel != null ? hotel.name : "",

                    };
                    model.booking_order = new HotelMongoBookingOrder()
                    {
                        arrivalDate = detail.search.arrivalDate,
                        departureDate = detail.search.departureDate,
                        clientType = ((int)ClientTypes.TIER_1_AGENT).ToString(),
                        hotelID = detail.search.hotelID,
                        numberOfAdult = detail.search.numberOfAdult,
                        numberOfChild = detail.search.numberOfChild,
                        numberOfInfant = detail.search.numberOfInfant,
                        numberOfRoom = detail.search.numberOfRoom
                    };
                    model.create_booking = DateTime.Now;

                    var guest_profile = new List<HotelMongoProfile>();
                    guest_profile.Add(new HotelMongoProfile()
                    {
                        birthday = detail.contact.birthday,
                        email = detail.contact.email,
                        firstName = detail.contact.firstName,
                        lastName = detail.contact.lastName,
                        phoneNumber = detail.contact.phoneNumber,
                        profileType = "Booker"


                    });
                    /*
                    foreach (var g in detail.pasenger)
                    {
                        guest_profile.Add(new HotelMongoProfile()
                        {
                            birthday = g.birthday,
                            firstName = g.firstName,
                            lastName = g.lastName,
                            email = "",
                            phoneNumber = "",
                            profileType = "guest"
                        });
                    }*/
                    double total_amount_booking = 0;
                    foreach (var room in detail.rooms)
                    {
                        var list_rate = new List<HotelMongoRoomRate>();
                        double total_amount = 0;

                        total_amount += room.price;
                        total_amount_booking += room.total_amount;
                        foreach (var rate in room.rates)
                        {
                            var arrive_date_datetime = DateTime.ParseExact(rate.arrivalDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            var departure_date_datetime = DateTime.ParseExact(rate.departureDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            int stay_date_by_rate = (departure_date_datetime - arrive_date_datetime).Days;
                            if (stay_date_by_rate > 1)
                            {
                                for (int i = 1; i <= stay_date_by_rate; i++)
                                {
                                    list_rate.Add(new HotelMongoRoomRate()
                                    {
                                        allotmentId = rate.allotment_id,
                                        ratePlanCode = rate.rate_plan_code,
                                        ratePlanRefID = rate.rate_plan_id,
                                        roomTypeCode = room.room_type_code,
                                        roomTypeRefID = room.room_type_id,
                                        stayDate = arrive_date_datetime.AddDays(i - 1).ToString("yyyy-MM-dd"),
                                    });
                                }
                            }
                            else
                            {
                                list_rate.Add(new HotelMongoRoomRate()
                                {
                                    allotmentId = rate.allotment_id,
                                    ratePlanCode = rate.rate_plan_code,
                                    ratePlanRefID = rate.rate_plan_id,
                                    roomTypeCode = room.room_type_code,
                                    roomTypeRefID = room.room_type_id,
                                    stayDate = arrive_date_datetime.ToString("yyyy-MM-dd"),
                                });
                            }

                        }
                        model.booking_data.reservations.Add(new HotelMongoReservation()
                        {
                            isPackagesSpecified = false,
                            isProfilesSpecified = false,
                            isRoomRatesSpecified = true,
                            isSpecialRequestSpecified = false,
                            packages = new List<HotelMongoPackage>()
                            {

                            },
                            profiles = guest_profile,
                            roomRates = list_rate,
                            roomOccupancy = new HotelMongoRoomOccupancy()
                            {
                                numberOfAdult = detail.search.numberOfAdult,
                                otherOccupancies = new List<HotelMongoOtherOccupancy>()
                                {
                                     new HotelMongoOtherOccupancy(){otherOccupancyRefCode="child",otherOccupancyRefID="child",quantity=detail.search.numberOfChild},
                                     new HotelMongoOtherOccupancy(){otherOccupancyRefCode="infant",otherOccupancyRefID="infant",quantity=detail.search.numberOfInfant}
                                }
                            },
                            totalAmount = new HotelMongoTotalAmount()
                            {
                                amount = (int)total_amount,
                                currencyCode = "VND"
                            },
                            specialRequests = new List<HotelMongoSpecialRequest>()

                        });
                    }

                    model.booking_b2b_data = detail;
                    model.total_amount = total_amount_booking;
                    string id = await hotelBookingMongoRepository.saveBooking(model, detail.booking_id);
                    if (id != null)
                    {
                        return Ok(new
                        {
                            data = id,
                            status = (int)ResponseType.SUCCESS,
                            msg = "Thành công"

                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Khong thể lưu booking"
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
                LogHelper.InsertLogTelegram("HotelBookingController- Token: " + token + " - PushBookingToMongo: " + ex.ToString());

                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }

        }

        [HttpPost("vin/get-hotels")]
        public async Task<ActionResult> GetVinHotelAvailability(string token)
        {
            try
            {
                #region Test

                //var j_param = new Dictionary<string, string>
                //{
                //    {"arrivalDate", "2024-02-10"},
                //    {"departureDate","2024-02-11" },
                //    {"numberOfRoom", "1"},
                //    {"hotelID","24386cea-907e-93d5-0755-b4b1d8f5858a" },
                //    {"numberOfChild","0" },
                //    {"numberOfAdult","2" },
                //    {"numberOfInfant","0" },
                //    {"clientType","2" },
                //    {"client_id","182" },
                //    {"product_type","0" },
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion


                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    List<HotelSearchEntities> result = new List<HotelSearchEntities>();
                    string status_vin = string.Empty;
                    string arrivalDate = objParr[0]["arrivalDate"].ToString();
                    string departureDate = objParr[0]["departureDate"].ToString();
                    int numberOfRoom = Convert.ToInt16(objParr[0]["numberOfRoom"]);
                    int numberOfChild = Convert.ToInt16(objParr[0]["numberOfChild"]);
                    int numberOfAdult = Convert.ToInt16(objParr[0]["numberOfAdult"]);
                    int numberOfInfant = Convert.ToInt16(objParr[0]["numberOfInfant"]);
                    string hotelID = objParr[0]["hotelID"].ToString();
                    int product_type = Convert.ToInt32(objParr[0]["product_type"].ToString());
                    string hotelName = objParr[0]["hotelName"].ToString();
                    long client_id = Convert.ToInt64(objParr[0]["client_id"]);
                    int client_type = 5;

                    string distributionChannelId = configuration["config_api_vinpearl:Distribution_ID"].ToString();
                    string ids_list = hotelID;
                    //-- Đọc từ cache, nếu có trả kết quả:
                    string cache_name_id = arrivalDate + "_" + departureDate + "_" + numberOfRoom + numberOfChild + numberOfAdult + numberOfInfant + "_" + hotelID + "_" + client_type;

                    int id = 0;
                    try
                    {
                        var list_vin_hotel_id = hotelID.Split(",").ToList();
                        var list_vin_hotel_hotel_id = new List<string>();
                        switch (product_type)
                        {
                            case 0:
                                {
                                    if (hotelID.Trim() != "" && list_vin_hotel_id != null && list_vin_hotel_id.Count() > 0)
                                    {
                                        foreach (var id1 in list_vin_hotel_id)
                                        {
                                            try
                                            {
                                                var hotel = await hotelDetailRepository.GetById(Convert.ToInt32(id1));
                                                list_vin_hotel_hotel_id.Add(hotel.HotelId);
                                            }
                                            catch
                                            {
                                                list_vin_hotel_hotel_id.Add(id1);

                                            }

                                        }
                                    }
                                    else
                                    {
                                        var hotel = await hotelDetailRepository.GetByType(true);
                                        if (hotel != null && hotel.Count > 0)
                                        {
                                            hotel = hotel.Where(x => CommonHelper.RemoveUnicode(x.Name).ToLower().Contains(CommonHelper.RemoveUnicode(hotelName).ToLower())).ToList();
                                            list_vin_hotel_hotel_id = hotel.Select(x => x.HotelId).ToList();
                                            list_vin_hotel_id = hotel.Select(x => x.Id.ToString()).ToList();

                                        }
                                    }
                                }
                                break;
                            case 1:
                                {
                                    var hotel = await hotelDetailRepository.GetByType(true);
                                    if (hotel != null && hotel.Count > 0)
                                    {
                                        hotel = hotel.Where(x => CommonHelper.RemoveUnicode(x.Name).ToLower().Contains(CommonHelper.RemoveUnicode(hotelName).ToLower())).ToList();
                                        list_vin_hotel_hotel_id = hotel.Select(x => x.HotelId).ToList();
                                        list_vin_hotel_id = hotel.Select(x => x.Id.ToString()).ToList();

                                    }
                                }
                                break;
                            case 2:
                                {
                                    var hotel = await hotelDetailRepository.GetByType(true);
                                    if (hotel != null && hotel.Count > 0)
                                    {
                                        hotel = hotel.Where(x => x.City != null && x.City.Trim() != "" && CommonHelper.RemoveUnicode(x.City).ToLower().Contains(CommonHelper.RemoveUnicode(hotelName).ToLower())).ToList();
                                        list_vin_hotel_hotel_id = hotel.Select(x => x.HotelId).ToList();
                                        list_vin_hotel_id = hotel.Select(x => x.Id.ToString()).ToList();

                                    }
                                }
                                break;
                        }

                        hotelID = JsonConvert.SerializeObject(list_vin_hotel_hotel_id);
                        ids_list = JsonConvert.SerializeObject(list_vin_hotel_id);
                    }
                    catch
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "Data không hợp lệ"
                        });
                    }
                    string input_api_vin_all = "{\"arrivalDate\":\"" + arrivalDate + "\",\"departureDate\":\"" + departureDate + "\",\"numberOfRoom\":" + numberOfRoom + ",\"propertyIds\":" + hotelID + ",\"distributionChannelId\":\"" + distributionChannelId + "\",\"roomOccupancy\":{\"numberOfAdult\":" + numberOfAdult + ",\"otherOccupancies\":[{\"otherOccupancyRefCode\":\"child\",\"quantity\":" + numberOfChild + "},{\"otherOccupancyRefCode\":\"infant\",\"quantity\":" + numberOfInfant + "}]}}";
                    int number_room_each = 1;
                    int number_adult_each_room = (numberOfAdult / (float)numberOfRoom) > (int)(numberOfAdult / numberOfRoom) ? (int)(numberOfAdult / numberOfRoom) + 1 : (int)(numberOfAdult / numberOfRoom);
                    int number_child_each_room = numberOfChild == 1 || (((int)numberOfChild / numberOfRoom) <= 1 && numberOfChild > 0) ? 1 : numberOfChild / numberOfRoom;
                    int number_infant_each_room = numberOfInfant == 1 || (((int)numberOfInfant / numberOfRoom) <= 1 && numberOfInfant > 0) ? 1 : numberOfInfant / numberOfRoom;
                    string input_api_vin_phase = "{\"arrivalDate\":\"" + arrivalDate + "\",\"departureDate\":\"" + departureDate + "\",\"numberOfRoom\":" + number_room_each + ",\"propertyIds\":" + hotelID + ",\"distributionChannelId\":\"" + distributionChannelId + "\",\"roomOccupancy\":{\"numberOfAdult\":" + number_adult_each_room + ",\"otherOccupancies\":[{\"otherOccupancyRefCode\":\"child\",\"quantity\":" + number_child_each_room + "},{\"otherOccupancyRefCode\":\"infant\",\"quantity\":" + number_infant_each_room + "}]}}";

                    var str = redisService.Get(CacheName.ClientHotelSearchResult + cache_name_id, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                    string response = "";
                    if (str != null && str.Trim() != "")
                    {
                        HotelSearchModel model = JsonConvert.DeserializeObject<HotelSearchModel>(str);
                        var view_model = model.hotels;
                        //-- Trả kết quả
                        return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), msg = "Get Data From Cache Success", data = view_model, cache_id = CacheName.ClientHotelSearchResult + cache_name_id });
                    }
                    else
                    {

                        var vin_lib = new VinpearlLib(configuration);
                        response = vin_lib.getHotelAvailability(input_api_vin_phase).Result;

                    }
                    //LogHelper.InsertLogTelegram("GetVinHotelAvailability - HotelB2CController: Cannot get from Redis [" + CacheName.ClientHotelSearchResult + cache_name_id + "] - token: " + token);

                    var data_hotel = JObject.Parse(response);
                    // Đọc Json ra các Field để map với những trường cần lấy

                    #region Check Data Invalid
                    if (data_hotel["isSuccess"].ToString().ToLower() == "false")
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.EMPTY,
                            msg = "Tham số không hợp lệ",
                            data = data_hotel
                        });
                    }
                    #endregion


                    var j_hotel_list = data_hotel["data"]["rates"];
                    if (j_hotel_list != null && j_hotel_list.Count() > 0)
                    {
                        var room_list = new List<RoomSearchModel>();
                        //-- Get PricePolicy:
                        var from_date = DateTime.ParseExact(arrivalDate, "yyyy-MM-dd", null);
                        var to_date = DateTime.ParseExact(departureDate, "yyyy-MM-dd", null);
                        int nights = (to_date - from_date).Days;

                        foreach (var h in j_hotel_list)
                        {
                            // Thông tin khách sạn
                            var hotel_item = new HotelSearchEntities()
                            {
                                hotel_id = h["property"]["id"].ToString(),
                                name = h["property"]["name"].ToString(),
                                star = Convert.ToDouble(h["property"]["star"]),
                                country = h["property"]["country"] != null ? h["property"]["country"]["name"].ToString() : "Vietnam",
                                state = h["property"]["state"]["name"].ToString(),
                                street = h["property"]["street"].ToString(),
                                hotel_type = h["property"]["hotelType"] != null ? h["property"]["hotelType"]["name"].ToString() : "Hotel",
                                review_point = 10,
                                review_count = 0,
                                review_rate = "Tuyệt vời",
                                is_refundable = true,
                                is_instantly_confirmed = true,
                                confirmed_time = 0,
                                email = h["property"]["email"].ToString(),
                                telephone = h["property"]["telephone"].ToString(),
                            };
                            // tiện nghi khách sạn
                            hotel_item.amenities = JsonConvert.DeserializeObject<List<amenitie>>(JsonConvert.SerializeObject(h["property"]["amenities"])).Select(x => new FilterGroupAmenities() { key = x.code, description = x.name, icon = x.icon }).ToList();

                            // Hình ảnh khách sạn
                            hotel_item.img_thumb = JsonConvert.DeserializeObject<List<thumbnails>>(JsonConvert.SerializeObject(h["property"]["thumbnails"])).Select(x => x.url).ToList();

                            // Danh sách các loại phòng của khách sạn
                            var j_room = h["property"]["roomTypes"];
                            var rooms = new List<RoomSearchModel>();
                            foreach (var item_r in j_room)
                            {
                                #region Hình ảnh phòng
                                /*
                                var img_thumb_room = new List<thumbnails>();
                                var j_thumb_room_img = item_r["thumbnails"];
                                foreach (var item_thumb in j_thumb_room_img)
                                {
                                    var item_img_room = new thumbnails
                                    {
                                        id = item_thumb["id"].ToString(),
                                        url = item_thumb["url"].ToString()
                                    };
                                    img_thumb_room.Add(item_img_room);
                                }*/
                                #endregion

                                #region Chi tiết phòng
                                var item = new RoomSearchModel
                                {
                                    id = item_r["id"].ToString(),
                                    code = item_r["code"].ToString(),
                                    name = item_r["name"].ToString(),
                                    type_of_room = item_r["typeOfRoom"].ToString(),
                                    hotel_id = h["property"]["id"].ToString(),
                                    rates = new List<RoomRate>(),
                                };
                                #endregion
                                rooms.Add(item);
                            }
                            hotel_item.type_of_room = rooms.Select(x => x.type_of_room).Distinct().ToList();

                            //-- Giá gốc
                            var j_rate_data = h["rates"];
                            if (j_rate_data.Count() > 0)
                            {
                                foreach (var r in j_rate_data)
                                {
                                    double price = Convert.ToDouble(r["totalAmount"]["amount"]["amount"]);

                                    rooms.Where(x => x.id.Trim() == r["roomTypeID"].ToString().Trim()).First().rates.Add(
                                        new RoomRate()
                                        {
                                            rate_plan_id = r["ratePlanID"].ToString(),
                                            amount = price,
                                            profit = 0,
                                            total_profit = 0,
                                            total_price = price,
                                            rate_plan_code = r["rateAvailablity"]["ratePlanCode"].ToString(),
                                        }
                                    );


                                }
                            }

                            hotel_item.room_name = rooms.Where(x => x.rates.Count > 0).Select(x => x.name).Distinct().ToList();
                            room_list.AddRange(rooms);
                            //-- Add vào kết quả
                            result.Add(hotel_item);
                        }

                        //-- Cache kết quả:
                        HotelSearchModel cache_data = new HotelSearchModel();
                        cache_data.hotels = result;
                        cache_data.input_api_vin = input_api_vin_phase;
                        cache_data.rooms = room_list;
                        cache_data.client_type = client_type;
                        cache_data.hotel_ids = ids_list;
                        int db_index = Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"].ToString());
                        redisService.Set(CacheName.ClientHotelSearchResult + cache_name_id, JsonConvert.SerializeObject(cache_data), DateTime.Now.AddDays(1), db_index);

                        return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), msg = status_vin, data = result, cache_id = CacheName.ClientHotelSearchResult + cache_name_id });
                    }
                    else
                    {
                        status_vin = "Không tìm thấy khách sạn nào thỏa mãn điều kiện này";
                        return Ok(new { status = ((int)ResponseType.EMPTY).ToString(), msg = status_vin, cache_id = CacheName.ClientHotelSearchResult + cache_name_id });
                    }
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
                LogService.InsertLog("VinController - getHotelAvailability: " + ex.ToString());

                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "error: " + ex.ToString() });
            }

        }

        [HttpPost("search-by-location")]
        public async Task<ActionResult> ListByLocation(string token)
        {
            try
            {
                #region Test
                //var j_param = new Dictionary<string, string>
                //{
                //    {"id", "1"},
                //    {"type", "0"},
                //    {"name", "Hà Nội"},
                //    {"fromdate", DateTime.Now.ToString()},
                //    {"todate", DateTime.Now.AddDays(1).ToString()},
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    int? id = Convert.ToInt32(objParr[0]["id"].ToString());
                    int? type = Convert.ToInt32(objParr[0]["type"].ToString());
                    int? client_type = Convert.ToInt32(objParr[0]["client_type"].ToString());
                    DateTime fromdate = Convert.ToDateTime(objParr[0]["fromdate"].ToString());
                    DateTime todate = Convert.ToDateTime(objParr[0]["todate"].ToString());
                    string name = objParr[0]["name"].ToString();
                    int total_nights = (todate - fromdate).Days;
                    if (id == null || type == null || id <= 0 || type < 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED
                        });
                    }
                    var hotel_list = await _hotelESRepository.GetListByLocation(name, (int)id, (int)type);
                    if (hotel_list == null || hotel_list.Count <= 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED
                        });
                    }

                    string cache_name = CacheName.HotelExclusiveListB2B + id + type + fromdate.ToString("yyyyMMdd") + todate.ToString("yyyyMMdd") + "_" + name + "_" + total_nights + client_type;
                    var str = redisService.Get(cache_name, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                    if (str != null && str.Trim() != "")
                    {
                        List<HotelSearchEntities> model = JsonConvert.DeserializeObject<List<HotelSearchEntities>>(str);
                        //-- Trả kết quả
                        return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), data = model, cache_id = string.Empty });
                    }
                    //LogHelper.InsertLogTelegram("ListByLocation - HotelB2CController: Cannot get from Redis [" + cache_name + "] - token: " + token);

                    List<HotelSearchEntities> result = new List<HotelSearchEntities>();
                    var hotel_datas = hotelDetailRepository.GetFEHotelList(new HotelFESearchModel
                    {
                        FromDate = fromdate,
                        ToDate = todate,
                        HotelId = string.Join(",", hotel_list.Select(x => x.hotelid)),
                        HotelType = "",
                        PageIndex = 1,
                        PageSize = 1000
                    });

                    if (hotel_datas != null && hotel_datas.Any())
                    {
                        var data_results = hotel_datas.GroupBy(x => x.Id).Select(x => x.First()).ToList();

                        if (data_results != null && data_results.Any())
                        {
                            foreach (var hotel in data_results)
                            {
                                //var hotel_detail = await hotelDetailRepository.GetByHotelId(hotel.Id.ToString());
                                //var hotel_rooms = hotelDetailRepository.GetFEHotelRoomList(hotel.Id);
                                ////-- Tính giá về tay thông qua chính sách giá
                                //var profit_list = hotelDetailRepository.GetHotelRoomPricePolicy(hotel.HotelId, "", client_type.ToString());
                                //List<RoomDetail> rooms_list = new List<RoomDetail>();
                                //foreach (var r in hotel_rooms)
                                //{
                                //    var room_packages = hotelDetailRepository.GetFERoomPackageListByRoomId(r.Id, fromdate, todate);
                                //    var room_packages_daily = hotelDetailRepository.GetFERoomPackageDaiLyListByRoomId(r.Id, fromdate, todate);
                                //    rooms_list.Add(PricePolicyService.GetRoomDetail(r.Id.ToString(), fromdate, todate, total_nights, room_packages_daily, room_packages, profit_list, hotel_detail, null));
                                //}
                                //var min_price = rooms_list.Where(x => x.min_price > 0).OrderBy(x => x.min_price).FirstOrDefault();
                                result.Add(new HotelSearchEntities
                                {
                                    hotel_id = hotel.Id.ToString(),
                                    name = hotel.Name,
                                    star = hotel.Star,
                                    country = hotel.Country,
                                    state = hotel.State,
                                    street = hotel.Street,
                                    hotel_type = hotel.HotelType,
                                    review_point = 10,
                                    review_count = hotel.ReviewCount ?? 0,
                                    review_rate = "Tuyệt vời",
                                    is_refundable = hotel.IsRefundable,
                                    is_instantly_confirmed = hotel.IsInstantlyConfirmed,
                                    confirmed_time = hotel.VerifyDate ?? 0,
                                    is_vin_hotel = hotel.IsVinHotel,
                                    //min_price = min_price == null ? 0 : min_price.min_price,
                                    min_price = 0,
                                    email = hotel.Email,
                                    telephone = hotel.Telephone,
                                    img_thumb = new List<string> { hotel.ImageThumb },
                                    is_commit = hotel.IsCommitFund,
                                });
                            }
                        }
                      
                        if (result.Count > 0)
                        {
                            redisService.Set(cache_name, JsonConvert.SerializeObject(result), DateTime.Now.AddDays(1), Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));

                            return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), data = result, cache_id = string.Empty });
                        }

                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED
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
                LogHelper.InsertLogTelegram("ListByLocation - HotelB2CController: " + ex);
                return Ok(new { status = ResponseTypeString.Fail, msg = "error: " + ex.ToString() });
            }
        }
        [HttpPost("search-by-location-detail")]
        public async Task<ActionResult> ListByLocationDetail(string token)
        {
            try
            {
                #region Test

                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    int? id = Convert.ToInt32(objParr[0]["id"].ToString());
                    int? type = Convert.ToInt32(objParr[0]["type"].ToString());
                    int? client_type = 5;
                    DateTime fromdate = Convert.ToDateTime(objParr[0]["fromdate"].ToString());
                    DateTime todate = Convert.ToDateTime(objParr[0]["todate"].ToString());
                    string name = objParr[0]["name"].ToString();
                    string hotelid = objParr[0]["hotelid"].ToString();
                    bool? is_vin_hotel = Convert.ToBoolean(objParr[0]["is_vin_hotel"] == null || objParr[0]["is_vin_hotel"].ToString().Trim() == "" ? "false" : objParr[0]["is_vin_hotel"].ToString());
                    int total_nights = (todate - fromdate).Days;
                    List<HotelSearchEntities> result = new List<HotelSearchEntities>();
                    bool is_cached = false;
                    string cache_name_detail = CacheName.ClientHotelSearchResult +"_" + fromdate.ToString("yyyyMMdd") + "_" + todate.ToString("yyyyMMdd") + "_" + "1" + "0" + "1" + "0" + "_"+ hotelid + client_type;
                    var str = redisService.Get(cache_name_detail, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                    if (str != null && str.Trim() != "")
                    {
                        List<HotelSearchEntities> model = JsonConvert.DeserializeObject<List<HotelSearchEntities>>(str);
                        if (model != null && model != null && model.Count > 0)
                        {
                            return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), data = model, cache_id = string.Empty });

                        }
                    }
                    //LogHelper.InsertLogTelegram("ListByLocationDetail - HotelB2CController: Cannot get from Redis [" + cache_name_detail + "] - token: " + token);

                    if (!is_cached && is_vin_hotel != null && is_vin_hotel == true)
                    {
                        string distributionChannelId = configuration["config_api_vinpearl:Distribution_ID"].ToString();

                        var hotel_Detail = await hotelDetailRepository.GetById(Convert.ToInt32(hotelid));
                        var hotel_id_vin = hotel_Detail.HotelId;
                        int number_room_each = 1;
                        int number_adult_each_room = 1;
                        int number_child_each_room = 0;
                        int number_infant_each_room = 0;
                        string input_api_vin_phase = "{\"arrivalDate\":\"" + fromdate.ToString("yyyy-MM-dd") + "\",\"departureDate\":\"" + todate.ToString("yyyy-MM-dd") + "\",\"numberOfRoom\":" + number_room_each + ",\"propertyIds\":[\"" + hotel_id_vin + "\"],\"distributionChannelId\":\"" + distributionChannelId + "\",\"roomOccupancy\":{\"numberOfAdult\":" + number_adult_each_room + ",\"otherOccupancies\":[{\"otherOccupancyRefCode\":\"child\",\"quantity\":" + number_child_each_room + "},{\"otherOccupancyRefCode\":\"infant\",\"quantity\":" + number_infant_each_room + "}]}}";

                        var vin_lib = new VinpearlLib(configuration);
                        var response = vin_lib.getHotelAvailability(input_api_vin_phase).Result;


                        var data_hotel = JObject.Parse(response);
                        // Đọc Json ra các Field để map với những trường cần lấy

                        #region Check Data Invalid
                        if (data_hotel["isSuccess"].ToString().ToLower() == "false")
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.EMPTY,
                                msg = "Tham số không hợp lệ",
                                data = data_hotel
                            });
                        }
                        #endregion


                        var j_hotel_list = data_hotel["data"]["rates"];
                        if (j_hotel_list != null && j_hotel_list.Count() > 0)
                        {
                            var room_list = new List<RoomSearchModel>();
                            int nights = (todate - fromdate).Days;

                            var h = j_hotel_list[0];
                            // Thông tin khách sạn
                            var hotel_item = new HotelSearchEntities()
                            {
                                hotel_id = h["property"]["id"].ToString(),
                                name = h["property"]["name"].ToString(),
                                star = Convert.ToDouble(h["property"]["star"]),
                                country = h["property"]["country"] != null ? h["property"]["country"]["name"].ToString() : "Vietnam",
                                state = h["property"]["state"]["name"].ToString(),
                                street = h["property"]["street"].ToString(),
                                hotel_type = h["property"]["hotelType"] != null ? h["property"]["hotelType"]["name"].ToString() : "Hotel",
                                review_point = 10,
                                review_count = 0,
                                review_rate = "Tuyệt vời",
                                is_refundable = true,
                                is_instantly_confirmed = true,
                                confirmed_time = 0,
                                email = h["property"]["email"].ToString(),
                                telephone = h["property"]["telephone"].ToString(),
                            };
                            // tiện nghi khách sạn
                            hotel_item.amenities = JsonConvert.DeserializeObject<List<amenitie>>(JsonConvert.SerializeObject(h["property"]["amenities"])).Select(x => new FilterGroupAmenities() { key = x.code, description = x.name, icon = x.icon }).ToList();

                            // Hình ảnh khách sạn
                            hotel_item.img_thumb = JsonConvert.DeserializeObject<List<thumbnails>>(JsonConvert.SerializeObject(h["property"]["thumbnails"])).Select(x => x.url).ToList();

                            // Danh sách các loại phòng của khách sạn
                            var j_room = h["property"]["roomTypes"];
                            var rooms = new List<RoomSearchModel>();
                            foreach (var item_r in j_room)
                            {
                                #region Chi tiết phòng
                                var item = new RoomSearchModel
                                {
                                    id = item_r["id"].ToString(),
                                    code = item_r["code"].ToString(),
                                    name = item_r["name"].ToString(),
                                    type_of_room = item_r["typeOfRoom"].ToString(),
                                    hotel_id = h["property"]["id"].ToString(),
                                    rates = new List<RoomRate>(),
                                };
                                #endregion
                                rooms.Add(item);
                            }
                            hotel_item.type_of_room = rooms.Select(x => x.type_of_room).Distinct().ToList();

                            //-- Giá gốc
                            var j_rate_data = h["rates"];
                            if (j_rate_data.Count() > 0)
                            {
                                foreach (var r in j_rate_data)
                                {
                                    double price = Convert.ToDouble(r["totalAmount"]["amount"]["amount"]);

                                    rooms.Where(x => x.id.Trim() == r["roomTypeID"].ToString().Trim()).First().rates.Add(
                                        new RoomRate()
                                        {
                                            rate_plan_id = r["ratePlanID"].ToString(),
                                            amount = price,
                                            profit = 0,
                                            total_profit = 0,
                                            total_price = price,
                                            rate_plan_code = r["rateAvailablity"]["ratePlanCode"].ToString(),
                                        }
                                    );


                                }
                            }
                            //-- Tinh gia ve tay:
                            var profit_vin = hotelDetailRepository.GetHotelRoomPricePolicy(hotel_id_vin,  client_type.ToString());
                            var list_min_price = new List<HotelMinPriceViewModel>();
                            foreach (var r in rooms)
                            {
                                foreach (var rate in r.rates)
                                {
                                    //var profit = profit_list.Where(x => x.HotelCode == r.hotel_id && x.RoomCode == r.id && x.ProgramCode == rate.rate_plan_id).FirstOrDefault();
                                    var profit = profit_vin.Where(x => x.HotelCode == r.hotel_id && x.RoomTypeCode == r.id && x.PackageName == rate.rate_plan_id).ToList();


                                    if (profit != null && profit.Count > 0)
                                    {
                                        rate.total_profit = PricePolicyService.CalucateMinProfit(profit, rate.amount, fromdate, todate);
                                        rate.total_price = rate.amount + rate.total_profit;
                                    }
                                    else
                                    {
                                        rate.total_profit = 0;
                                        rate.total_price = 0;
                                    }
                                    list_min_price.Add(new HotelMinPriceViewModel() { hotel_id = r.hotel_id, min_price = rate.total_price, vin_price = rate.amount, profit = rate.total_profit });
                                }
                            }
                            var min_price = list_min_price.Count > 0 ? list_min_price.OrderBy(x => x.min_price).First() : null;
                            hotel_item.min_price = min_price == null ? 0 : min_price.min_price;
                            result.Add(hotel_item);
                        }
                    }
                    else if (!is_cached)
                    {
                        var hotel_datas = hotelDetailRepository.GetFEHotelList(new HotelFESearchModel
                        {
                            FromDate = fromdate,
                            ToDate = todate,
                            HotelId = hotelid,
                            HotelType = "",
                            PageIndex = 1,
                            PageSize = 10
                        });
                        if (hotel_datas != null && hotel_datas.Any())
                        {
                            var data_results = hotel_datas.GroupBy(x => x.Id).Select(x => x.First()).ToList();

                            if (data_results != null && data_results.Any())
                            {
                                List<RoomDetail> rooms_list = new List<RoomDetail>();

                                var hotel = data_results[0];
                                var hotel_detail = await hotelDetailRepository.GetByHotelId(hotel.HotelId);
                                var hotel_rooms = hotelDetailRepository.GetFEHotelRoomList(Convert.ToInt32(hotel.HotelId));
                                //-- Tính giá về tay thông qua chính sách giá
                                var profit_list = hotelDetailRepository.GetHotelRoomPricePolicy(hotel.HotelId,  client_type.ToString());
                                foreach (var r in hotel_rooms)
                                {
                                    var room_packages = hotelDetailRepository.GetFERoomPackageListByRoomId(r.Id, fromdate, todate);
                                    var room_packages_daily = hotelDetailRepository.GetFERoomPackageDaiLyListByRoomId(r.Id, fromdate, todate);
                                    rooms_list.Add(PricePolicyService.GetRoomDetail(r.Id.ToString(), fromdate, todate, total_nights, room_packages_daily, room_packages, profit_list, hotel_detail, null));
                                }
                                var min_price = rooms_list.Where(x => x.min_price > 0).OrderBy(x => x.min_price).FirstOrDefault();
                                var min_price_value = min_price == null ? 0 : min_price.min_price;

                                result.Add(new HotelSearchEntities
                                {
                                    hotel_id = hotel.Id.ToString(),
                                    name = hotel.Name,
                                    star = hotel.Star,
                                    country = hotel.Country,
                                    state = hotel.State,
                                    street = hotel.Street,
                                    hotel_type = hotel.HotelType,
                                    review_point = 10,
                                    review_count = hotel.ReviewCount ?? 0,
                                    review_rate = "Tuyệt vời",
                                    is_refundable = hotel.IsRefundable,
                                    is_instantly_confirmed = hotel.IsInstantlyConfirmed,
                                    confirmed_time = hotel.VerifyDate ?? 0,
                                    min_price = min_price_value,
                                    email = hotel.Email,
                                    telephone = hotel.Telephone,
                                    img_thumb = new List<string> { hotel.ImageThumb },
                                    is_commit = hotel.IsCommitFund
                                });
                            }

                        }
                    }

                    //-- Cache kết quả:
                    if(result!=null && result.Count > 0)
                    {
  
                        int db_index = Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"].ToString());
                        redisService.Set(cache_name_detail, JsonConvert.SerializeObject(result), DateTime.Now.AddDays(1), db_index);
                    }
                    
                    return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), data = result, cache_id = string.Empty });

                }
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "Key /Data invalid!"
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListByLocation - HotelB2CController: " + ex);
                return Ok(new { status = ResponseTypeString.Fail, msg = "error: " + ex.ToString() });
            }
        }
        [HttpPost("get-hotel-location")]
        public async Task<ActionResult> GetHotelLocations(string token)
        {
            try
            {
                #region Test

                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    var hotels = await _hotelESRepository.GetAllHotels();
                    int i = 0;
                    hotels = hotels.Where(
                           x => x.isdisplaywebsite == true
                           && (x.isvinhotel == null || x.isvinhotel == false)
                           && x.city != null && x.city.Trim() != ""
                           && !(new List<string>() { "Hà Nội", "Đà Nẵng", "TP. Hồ Chí Minh" }.Contains(x.city.Trim()))
                       ).GroupBy(x => x.city).Select(x => x.First()).ToList();
                    var result = hotels.Count > 0 ? hotels.Select(x => new HotelESLocationViewModel()
                    {
                        id = ++i,
                        type = 1,
                        name = x.city
                    }).ToList() : new List<HotelESLocationViewModel>();

                    result.Add(new HotelESLocationViewModel()
                    {
                        id = ++i,
                        type = 0,
                        name = "Hà Nội"
                    });
                    result.Add(new HotelESLocationViewModel()
                    {
                        id = ++i,
                        type = 0,
                        name = "Đà Nẵng"
                    });
                    result.Add(new HotelESLocationViewModel()
                    {
                        id = ++i,
                        type = 0,
                        name = "TP. Hồ Chí Minh"
                    });
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Success",
                        data = result
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
                LogHelper.InsertLogTelegram("GetHotelLocations - HotelB2CController: " + ex);
                return Ok(new { status = ResponseType.FAILED, msg = "error: " + ex.ToString() });
            }
        }

        [HttpPost("search-by-location-position")]
        public async Task<ActionResult> ListByLocationPosition(string token)
        {
            try
            {
                #region Test
                //var j_param = new Dictionary<string, string>
                //{
                //    {"id", "1"},
                //    {"type", "0"},
                //    {"name", "Nha Trang"},
                //    {"fromdate", DateTime.Now.ToString()},
                //    {"todate", DateTime.Now.AddDays(1).ToString()},
                //    {"position_type", "2"},
                //    {"client_type", "2"},
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    int? id = Convert.ToInt32(objParr[0]["id"].ToString());
                    int? type = Convert.ToInt32(objParr[0]["type"].ToString());
                    int? client_type = Convert.ToInt32(objParr[0]["client_type"].ToString());
                    int PositionType = Convert.ToInt32(objParr[0]["position_type"].ToString());
                    DateTime fromdate = Convert.ToDateTime(objParr[0]["fromdate"].ToString());
                    DateTime todate = Convert.ToDateTime(objParr[0]["todate"].ToString());
                    string name = objParr[0]["name"].ToString();
                    int total_nights = (todate - fromdate).Days;
                    if (id == null || type == null || id <= 0 || type < 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED
                        });
                    }
                    var hotel_list = await _hotelESRepository.GetListByLocation(name, (int)id, (int)type);
                    if (hotel_list == null || hotel_list.Count <= 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED
                        });
                    }
                    
                    string cache_name = CacheName.HotelExclusiveListB2C_POSITION + name + "_"  + client_type;
                    var str = redisService.Get(cache_name, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                    if (str != null && str.Trim() != "")
                    {
                        List<HotelSearchEntities> model = JsonConvert.DeserializeObject<List<HotelSearchEntities>>(str);
                        //-- Trả kết quả
                        return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), data = model, cache_id = string.Empty });
                    }
                    //LogHelper.InsertLogTelegram("ListByLocation - HotelB2CController: Cannot get from Redis [" + cache_name + "] - token: " + token);

                    List<HotelSearchEntities> result = new List<HotelSearchEntities>();
                    var hotel_datas = hotelDetailRepository.GetFEHotelListPosition(new HotelFESearchModel
                    {
                        FromDate = fromdate,
                        ToDate = todate,
                        HotelId = string.Join(",", hotel_list.Select(x => x.hotelid)),
                        HotelType = "",
                        PositionType = PositionType,
                        PageIndex = 1,
                        PageSize = 1000
                    });

                    if (hotel_datas != null && hotel_datas.Any())
                    {
                        var data_results = hotel_datas.GroupBy(x => x.Id).Select(x => x.First()).ToList();

                        if (data_results != null && data_results.Any())
                        {
                            foreach (var hotel in data_results)
                            {
                                //var hotel_detail = await hotelDetailRepository.GetByHotelId(hotel.Id.ToString());
                                //var hotel_rooms = hotelDetailRepository.GetFEHotelRoomList(hotel.Id);
                                ////-- Tính giá về tay thông qua chính sách giá
                                //var profit_list = hotelDetailRepository.GetHotelRoomPricePolicy(hotel.HotelId, "", client_type.ToString());
                                //List<RoomDetail> rooms_list = new List<RoomDetail>();
                                //foreach (var r in hotel_rooms)
                                //{
                                //    var room_packages = hotelDetailRepository.GetFERoomPackageListByRoomId(r.Id, fromdate, todate);
                                //    var room_packages_daily = hotelDetailRepository.GetFERoomPackageDaiLyListByRoomId(r.Id, fromdate, todate);
                                //    rooms_list.Add(PricePolicyService.GetRoomDetail(r.Id.ToString(), fromdate, todate, total_nights, room_packages_daily, room_packages, profit_list, hotel_detail, null));
                                //}
                                //var min_price = rooms_list.Where(x => x.min_price > 0).OrderBy(x => x.min_price).FirstOrDefault();
                                result.Add(new HotelSearchEntities
                                {
                                    hotel_id = hotel.Id.ToString(),
                                    name = hotel.Name,
                                    star = hotel.Star,
                                    country = hotel.Country,
                                    state = hotel.State,
                                    street = hotel.Street,
                                    hotel_type = hotel.HotelType,
                                    review_point = 10,
                                    review_count = hotel.ReviewCount ?? 0,
                                    review_rate = "Tuyệt vời",
                                    is_refundable = hotel.IsRefundable,
                                    is_instantly_confirmed = hotel.IsInstantlyConfirmed,
                                    confirmed_time = hotel.VerifyDate ?? 0,
                                    is_vin_hotel = hotel.IsVinHotel,
                                    //min_price = min_price == null ? 0 : min_price.min_price,
                                    min_price = 0,
                                    email = hotel.Email,
                                    telephone = hotel.Telephone,
                                    img_thumb = new List<string> { hotel.ImageThumb },
                                    is_commit = hotel.IsCommitFund,
                                    position=hotel.Hotelposition.ToString(),
                                });
                            }
                        }
                       
                        if (result.Count > 0)
                        {
                            redisService.Set(cache_name, JsonConvert.SerializeObject(result), DateTime.Now.AddDays(1), Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));

                            return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), data = result, cache_id = string.Empty });
                        }

                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED
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
                LogHelper.InsertLogTelegram("ListByLocation - HotelB2CController: " + ex);
                return Ok(new { status = ResponseTypeString.Fail, msg = "error: " + ex.ToString() });
            }
        }
    }
}
