using API_CORE.Service.Log;
using API_CORE.Service.Price;
using API_CORE.Service.Vin;
using Caching.Elasticsearch;
using Caching.RedisWorker;
using ENTITIES.Models;
using ENTITIES.ViewModels.Hotel;
using ENTITIES.ViewModels.Vinpreal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories;
using REPOSITORIES.IRepositories.Clients;
using REPOSITORIES.IRepositories.Hotel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.Controllers
{
    [Route("api/room")]
    [ApiController]
    public class VinController : ControllerBase
    {
        private IConfiguration configuration;
        private IServicePiceRepository price_repository;
        private readonly RedisConn redisService;
        private LogService LogService;
        private IHotelDetailRepository hotelDetailRepository;
        private IAccountClientRepository accountClientRepository;
        private HotelESRepository hotelESRepository;
        private List<int> LIST_CLIENT_SAMETYPE = new List<int>() { 1, 2 };

        public VinController(IConfiguration _configuration, IServicePiceRepository _price_repository, RedisConn _redisService, 
            IHotelDetailRepository _hotelDetailRepository, IAccountClientRepository _accountClientRepository)
        {
            configuration = _configuration;
            price_repository = _price_repository;
            redisService = _redisService;
            LogService = new LogService(_configuration);
            hotelDetailRepository = _hotelDetailRepository;
            accountClientRepository = _accountClientRepository;
            hotelESRepository = new HotelESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
        }

        #region VIN Hotel
        [HttpPost("vin/vinpearl/get-hotel.json")]
        public async Task<ActionResult> getHotel(string token)
        {
            try
            {
                #region Test
                //var j_param = new Dictionary<string, string>
                //{
                //    {"page", "xx"},
                //    {"limit","15" }
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
                #endregion.

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    int page = Convert.ToInt16(objParr[0]["page"]);
                    int limit = Convert.ToInt16(objParr[0]["limit"]);

                    var vin_lib = new VinpearlLib(configuration);
                    var response = vin_lib.getAllRoom(page, limit).Result;

                    return Ok(new { status = response == "" ? ((int)ResponseType.EMPTY).ToString() : ((int)ResponseType.SUCCESS).ToString(), data = response == "" ? null : JObject.Parse(response) });
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
                LogService.InsertLog("VinController - getHotel: " + ex.ToString());
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "error: " + ex.ToString() });
            }

        }
        /// <summary>
        /// Tìm kiếm và trả thông tin tất cả các khách sạn theo ID và thông tin đặt phòng
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("vin/vinpearl/tracking-hotel-availability.json")]
        public async Task<ActionResult> getHotelAvailability(string token)
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
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
                #endregion


                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
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
                    int client_type= Convert.ToInt32(objParr[0]["clientType"]);
                    string client_type_string = LIST_CLIENT_SAMETYPE.Contains(client_type) ? "1,2" : client_type.ToString();
                    string distributionChannelId = configuration["config_api_vinpearl:Distribution_ID"].ToString();
                    string ids_list = hotelID;
                    //-- Đọc từ cache, nếu có trả kết quả:
                    string cache_name_id = arrivalDate + departureDate + numberOfRoom + numberOfChild + numberOfAdult + numberOfInfant + EncodeHelpers.MD5Hash(hotelID) +client_type_string;

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
                            case 3:
                                {
                                    var hotel = await hotelDetailRepository.GetByType(true);
                                    if (hotel != null && hotel.Count > 0)
                                    {
                                        hotel = hotel.Where(x => x.State != null && x.State.Trim() != "" && CommonHelper.RemoveUnicode(x.State).ToLower().Contains(CommonHelper.RemoveUnicode(hotelName).ToLower())).ToList();
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
                    string api_vin_token = "";
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
                        api_vin_token = vin_lib.token;
                        response = vin_lib.getHotelAvailability(input_api_vin_phase).Result;

                    }
                    //LogHelper.InsertLogTelegram("ListByLocationDetail - VinController: Cannot get from Redis [" + CacheName.ClientHotelSearchResult + cache_name_id + "] - token: " + token);

                    var data_hotel = JObject.Parse(response);
                    // Đọc Json ra các Field để map với những trường cần lấy

                    #region Check Data Invalid
                    if (data_hotel["isSuccess"]==null || data_hotel["isSuccess"].ToString().ToLower() == "false" || (data_hotel["message"] != null && data_hotel["message"].ToString().ToLower() == "unauthorized"))
                    {
                        LogHelper.InsertLogTelegram("VinController - getHotelAvailability with ["+ api_vin_token + "] ["+ input_api_vin_phase + "] error: " + response);
                        return Ok(new
                        {
                            status = (int)ResponseType.EMPTY,
                            msg = "Không tìm thấy dữ liệu khách sạn Vinpearl từ hệ thống, quý khách vui lòng thử lại hoặc lựa chọn khách sạn khác",
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
                        //var price_policy = hotelDetailRepository.GetHotelRoomPricePolicy(id.ToString(), "", from_date, to_date, string.Join(",",client_types));
                        int nights = (to_date - from_date).Days;
                        //var policy_list = hotelDetailRepository.GetHotelRoomPricePolicy(hotelID, "", from_date, to_date, "");

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
        [HttpPost("vin/vinpearl/get-searched-min-price.json")]
        public async Task<ActionResult> GetSearchResultMinPrice(string token)
        {
            #region Test
            //var j_param = new Dictionary<string, string>
            //    {
            //          { "cache_id", "search_result_2023-04-202023-04-233255e4b76d17d981b1334e0a5cb27382c7f02"},
            //          { "client_id", "182"},
            //   };

            //var data_product = JsonConvert.SerializeObject(j_param);
            //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
            #endregion
            try
            {
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    //-- Đọc từ cache, nếu có trả kết quả:
                    string cache_name_id = objParr[0]["cache_id"].ToString();
                    long client_id = Convert.ToInt64(objParr[0]["client_id"]);
                    int client_type = Convert.ToInt32(objParr[0]["clientType"]);
                    string client_type_string = LIST_CLIENT_SAMETYPE.Contains(client_type) ? "1,2" : client_type.ToString();

                    var str = redisService.Get(cache_name_id, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                    string response = "";
                    if (str != null && str.Trim() != "")
                    {

                        HotelSearchModel model = JsonConvert.DeserializeObject<HotelSearchModel>(str);
                        List<HotelMinPriceViewModel> result = new List<HotelMinPriceViewModel>();
                        var api_vin_input = JObject.Parse(model.input_api_vin);
                        var hotel_list = model.rooms.Select(x => x.hotel_id).ToList();
                        hotel_list = hotel_list.Distinct().ToList();

                        //-- trường hợp chưa tính:
                        DateTime fromdate = DateTime.ParseExact(api_vin_input["arrivalDate"].ToString(), "yyyy-M-d", null);
                        DateTime todate = DateTime.ParseExact(api_vin_input["departureDate"].ToString(), "yyyy-M-d", null);
                        int number_of_room = Convert.ToInt32(api_vin_input["numberOfRoom"].ToString());
                        var rate_plan_list = model.rooms.SelectMany(x => x.rates).Select(x => x.rate_plan_id).ToList();
                        rate_plan_list = rate_plan_list.Distinct().ToList();
                        int day_spend = Convert.ToInt32((todate - fromdate).TotalDays < 1 ? 1 : (todate - fromdate).TotalDays);

                        var _r_list = model.rooms.Select(x => x.id).ToList();
                        _r_list = _r_list.Distinct().ToList();
                        //-- Lấy chính sách giá
                        //var profit_list = hotelDetailRepository.GetHotelRoomPricePolicy(string.Join(",", hotel_list), "", fromdate, todate, "");
                        var profit_vin = hotelDetailRepository.GetHotelRoomPricePolicy(string.Join(",", hotel_list), client_type_string);
                        //LogService.InsertLog("VinController - GetSearchResultMinPrice HotelVIN: Profit List ["+ string.Join(",", hotel_list) + client_type_string + "] count= " + profit_vin.Count);

                        //-- Tính giá về tay
                        var input_api_vin = JObject.Parse(model.input_api_vin);

                        foreach (var r in model.rooms)
                        {
                            foreach (var rate in r.rates)
                            {
                                if (client_type == (int)ClientType.STAFF)
                                {
                                    rate.total_profit = 0;
                                    rate.total_price = rate.amount;
                                }
                                else
                                {
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
                                }
                                
                                result.Add(new HotelMinPriceViewModel() { hotel_id = r.hotel_id, min_price = rate.total_price, vin_price = rate.amount, profit = rate.total_profit });
                            }
                        }
                        //--- Giá thấp nhất
                        result = result.Where(x => x.min_price > 0).OrderBy(x => x.min_price).GroupBy(x => x.hotel_id).Select(g => g.First()).ToList();
                        foreach (var h in hotel_list)
                        {
                            if (!result.Any(x => x.hotel_id == h))
                            {
                                result.Add(new HotelMinPriceViewModel() { hotel_id = h, min_price = 0 });
                            }
                        }
                        //-- Cache lại kết quả:
                        int db_index = Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"].ToString());
                        redisService.Set(cache_name_id, JsonConvert.SerializeObject(model), DateTime.Now.AddDays(1), db_index);
                        //-- Trả kết quả
                        return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), msg = "Get Data From Cache Success", data = result, cache_id = cache_name_id });
                    }
                    else
                    {
                        //LogHelper.InsertLogTelegram("GetSearchResultMinPrice - VinController: Cannot get from Redis [" + cache_name_id + "] - token: " + token);

                        var status_vin = "Không tìm thấy khách sạn nào thỏa mãn điều kiện này";
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
                LogService.InsertLog("VinController - GetSearchResultMinPrice: " + ex.ToString());

                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "error: " + ex.ToString() });
            }
        }

        [HttpPost("vin/vinpearl/get-room-detail-availability.json")]
        public async Task<ActionResult> getRoomDetailAvailability(string token)
        {
            try
            {

                #region Test
                //var j_param = new Dictionary<string, string>
                //{
                //      { "propertyID", "d0c06e7b-28fe-896e-1915-cbe8540f14d8"},
                //      { "numberOfRoom", "1"},
                //      { "arrivalDate", "2023-10-10"},
                //      { "departureDate","2023-10-12" },
                //      { "roomoccupancy", "{'numberOfAdult': '1','otherOccupancies': [{ 'otherOccupancyRefID': 'child','otherOccupancyRefCode': 'child', 'quantity': '0' },{ 'otherOccupancyRefID': 'infant','otherOccupancyRefCode': 'infant', 'quantity': '0' }]}" },
                //      { "isFilteredByRoomTypeId", "true"},
                //      { "isFilteredByRatePlanId", "true"},
                //      { "ratePlanId", "6ffdb462-b485-48fa-9d2e-b55af50c8acd"},
                //      { "roomTypeId", "3efb8c7b-65c4-0ddb-69e7-034b14348056"},
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    int numberOfChild = Convert.ToInt16(objParr[0]["numberOfChild"]);
                    int numberOfAdult = Convert.ToInt16(objParr[0]["numberOfAdult"]);
                    int numberOfInfant = Convert.ToInt16(objParr[0]["numberOfInfant"]);

                    RoomDetailsPackageViewModel roomDetailsPackage = new RoomDetailsPackageViewModel();
                    roomDetailsPackage.arrivalDate = objParr[0]["arrivalDate"].ToString();
                    roomDetailsPackage.departureDate = objParr[0]["departureDate"].ToString();
                    roomDetailsPackage.distributionChannelId = configuration["config_api_vinpearl:Distribution_ID"].ToString();
                    roomDetailsPackage.propertyID = objParr[0]["propertyID"].ToString();
                    roomDetailsPackage.numberOfRoom = 1;
                    roomDetailsPackage.isFilteredByRoomTypeId = true;
                    roomDetailsPackage.isFilteredByRatePlanId = false;
                    roomDetailsPackage.ratePlanId = "";
                    roomDetailsPackage.roomTypeIds = new List<string>() { objParr[0]["roomTypeId"].ToString() };
                    roomDetailsPackage.roomOccupancy = new RoomOccupancy()
                    {
                        numberOfAdult = numberOfAdult,
                        otherOccupancies = new List<OtherOccupancies>()
                        {
                            new OtherOccupancies(){otherOccupancyRefID="child", otherOccupancyRefCode="child",quantity=numberOfChild},
                            new OtherOccupancies(){otherOccupancyRefID="infant", otherOccupancyRefCode="infant",quantity=numberOfInfant},
                        }

                    };

                    long client_id = Convert.ToInt64(objParr[0]["client_id"]);
                    int client_type = Convert.ToInt32(objParr[0]["clientType"]);


                    string input_api_vin = JsonConvert.SerializeObject(roomDetailsPackage);

                    var vin_lib = new VinpearlLib(configuration);
                    var data = vin_lib.getRoomDetailAvailability(input_api_vin).Result;

                    var data_hotel = JObject.Parse(data);

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
                    string hotel_ids = roomDetailsPackage.propertyID;
                    List<string> rate_plans = new List<string>();
                    List<string> roomids = new List<string>();

                    rate_plans.Add(roomDetailsPackage.ratePlanId);
                    roomids.AddRange(roomDetailsPackage.roomTypeIds);
                    DateTime fromdate = DateTime.ParseExact(roomDetailsPackage.arrivalDate, "yyyy-M-d", null);
                    DateTime todate = DateTime.ParseExact(roomDetailsPackage.departureDate, "yyyy-M-d", null);
                    string client_type_string = LIST_CLIENT_SAMETYPE.Contains(client_type) ? "1,2" : client_type.ToString();

                    var profit_list = hotelDetailRepository.GetHotelRoomPricePolicy(roomDetailsPackage.propertyID,  client_type_string);
                    //LogService.InsertLog("VinController - getRoomDetailAvailability HotelVIN: Profit List [" + roomDetailsPackage.propertyID + client_type_string + "] count= " + profit_list.Count);

                    var Packages_hotel_list = data_hotel["data"]["roomAvailabilityRates"];
                    var hotel_detail_sql = await hotelDetailRepository.GetByHotelId(roomDetailsPackage.propertyID);
                    bool is_cached = false;
                    int db_index = Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"].ToString());

                    string cache_name_detail = CacheName.B2B_VIN_ROOM_DETAIL + 0 + fromdate.ToString("yyyyMMdd") + todate.ToString("yyyyMMdd") + "1" + "0" + "1" + "0" + EncodeHelpers.MD5Hash(roomDetailsPackage.propertyID) + client_type_string;
                    var str = redisService.Get(cache_name_detail, db_index);
                    if (str != null && str.Trim() != "")
                    {
                        try
                        {
                            VinHotelDetailViewModel model = JsonConvert.DeserializeObject<VinHotelDetailViewModel>(str);
                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                msg = "Thành công",
                                data = model.packages,
                                Surcharge = model.surcharges
                            });
                        }
                        catch { }
                    }
                    //LogHelper.InsertLogTelegram("getRoomDetailAvailability - VinController: Cannot get from Redis [" + cache_name_detail + "] - token: " + token);

                    List<ListPackagesHotelViewModel> ListPackagesHotel = new List<ListPackagesHotelViewModel>();
                    if (Packages_hotel_list.Count() > 0)
                    {
                        //lấy giá gốc
                        foreach (var item in Packages_hotel_list)
                        {


                            var list_rates = item["rates"];

                            foreach (var h in list_rates)
                            {
                                ListPackagesHotelViewModel ListPackages = new ListPackagesHotelViewModel();
                                ListPackages.cancelPolicy = item["ratePlan"]["cancelPolicy"];

                                List<PackagesHotelViewModel> PackagesHotel = new List<PackagesHotelViewModel>();
                                var list_Packages = h["packages"];

                                foreach (var p in list_Packages)
                                {
                                    var list_Packages_hotel = new PackagesHotelViewModel
                                    {

                                        packageType = p["packageType"].ToString(),
                                        packageId = p["id"].ToString(),
                                        name = p["name"].ToString(),
                                        price = (double)p["amountPerStayingDate"],
                                        hotelId = roomDetailsPackage.propertyID,
                                        roomID = roomDetailsPackage.roomTypeIds[0],
                                        ratePlanId = roomDetailsPackage.ratePlanId,
                                    };

                                    PackagesHotel.Add(list_Packages_hotel);

                                };

                                ListPackages.List_packagesHotel = PackagesHotel;
                                ListPackages.amount = (double)h["amount"]["amount"]["amount"];

                                if (ListPackagesHotel.Count >= 1)
                                {
                                    foreach (var i in ListPackagesHotel)
                                    {
                                        if (JsonConvert.SerializeObject(ListPackages.List_packagesHotel).Equals(JsonConvert.SerializeObject(i.List_packagesHotel)))
                                        {
                                            ListPackagesHotel.Add(ListPackages);
                                        }
                                    }
                                }
                                else
                                {
                                    ListPackagesHotel.Add(ListPackages);
                                }

                            }

                        }


                        foreach (var hotel in ListPackagesHotel)
                        {

                            List<double> total = new List<double>();
                            RoomRate rate = new RoomRate()
                            {
                                amount = hotel.amount,
                                hotel_id = roomDetailsPackage.propertyID,
                                room_id = roomDetailsPackage.roomTypeIds[0],
                                rate_plan_id = roomDetailsPackage.ratePlanId,
                            };
                            foreach (var package in hotel.List_packagesHotel)
                            {
                                if (client_type == (int)ClientType.STAFF)
                                {
                                    rate.profit = 0;
                                    rate.total_profit = 0;
                                    rate.total_price = rate.amount;
                                }
                                else
                                {
                                    var profit = profit_list.Where(x => x.HotelCode == package.hotelId && x.RoomTypeCode == package.roomID && x.PackageName == rate.rate_plan_id).FirstOrDefault();
                                    if (profit != null)
                                    {
                                        rate.profit = profit.Profit;
                                        rate.total_profit = PricePolicyService.CalucateMinProfit(new List<HotelPricePolicyViewModel>() { profit }, rate.amount, fromdate, todate);
                                        rate.total_price = rate.amount + rate.total_profit;

                                    }
                                    else
                                    {
                                        rate.profit = 0;
                                        rate.total_profit = 0;
                                        //rate.total_price = rate.amount;
                                        rate.total_price = 0;
                                    }
                                }
                               
                            }
                        }
                    }
                    VinHotelDetailViewModel cache_model = new VinHotelDetailViewModel()
                    {
                        packages = ListPackagesHotel,
                        surcharges = hotel_detail_sql.OtherSurcharge == null ? "" : hotel_detail_sql.OtherSurcharge
                    };
                    redisService.Set(cache_name_detail, JsonConvert.SerializeObject(cache_model), DateTime.Now.AddDays(1), db_index);
                    var hotel_detail =  hotelESRepository.FindByHotelId(roomDetailsPackage.propertyID);

                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Thành công",
                        data = ListPackagesHotel,
                        Surcharge = hotel_detail_sql.OtherSurcharge == null ? "" : hotel_detail_sql.OtherSurcharge,
                        hotel_id = hotel_detail != null && hotel_detail.id>0 ? hotel_detail.id : -1
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

                LogService.InsertLog("VinController - getRoomDetailAvailability: " + ex.ToString());
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "Error On Excution. Vui lòng liên hệ IT" });
            }
        }

        //API CREATE BOOKING  VIN
        [HttpPost("vin/vinpearl/booking/create-booking")]
        public async Task<ActionResult> getCreateBookingVin(string token)
        {
            try
            {
                #region Test
                //var j_param = new Dictionary<string, object>
                //    {
                //        {"create_booking","{'distributionChannel':'55221271-b512-4fce-b6b6-98c997c73965','propertyID':'d0c06e7b-28fe-896e-1915-cbe8540f14d8','arrivalDate':'2023-12-01','departureDate':'2023-12-02','reservations':[{'roomOccupancy':{'numberOfAdult':1,'otherOccupancies':[{'otherOccupancyRefID':'child','otherOccupancyRefCode':'child','quantity':0},{'otherOccupancyRefID':'infant','otherOccupancyRefCode':'infant','quantity':0}]},'numberOfRoom':1,'totalAmount':{'amount':1980500.0,'currencyCode':'VND'},'isReferenceIdSpecified':false,'referenceIds':[],'isSpecialRequestSpecified':false,'specialRequests':[],'isProfilesSpecified':true,'profiles':[{'profileRefID':'32f1d908-9269-4818-887a-88ff1dd800cf','firstName':'ADAVIGO','profileType':'TravelAgent'},{'firstName':'Nguyen','lastName':'Minh','email':'mn13795@gmail.com','phoneNumber':'0123456789','profileType':'Guest'},{'firstName':'Nguyen','lastName':'Minh','email':'mn13795@gmail.com','phoneNumber':'0123456789','profileType':'Booker'}],'isRoomRatesSpecified':true,'roomRates':[{'stayDate':'2023-03-01T00:00:00.0000000','roomTypeRefID':'e3b015d5-09a4-9cd5-27d2-aaab5ba4432e','ratePlanRefID':'6ffdb462-b485-48fa-9d2e-b55af50c8acd','allotmentId':'a924ace9-7cc3-4552-b80d-34e725f442ef'}]}]}" }
                //    };

                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2"]);
                #endregion.
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {


                    var create_booking = JObject.Parse(objParr[0]["create_booking"].ToString());

                    string input_api_vin_create_booking = JsonConvert.SerializeObject(create_booking);

                    var vin_lib = new VinpearlLib(configuration);
                    var data_create_booking = vin_lib.getVinpearlCreateBooking(input_api_vin_create_booking).Result;

                    var data_createbooking = JObject.Parse(data_create_booking);

                    if (data_createbooking["isSuccess"].ToString().ToLower() == "true")
                    {
                        LogHelper.InsertLogTelegram("getCreateBookingVin - VinController-data-create-booking: " + data_createbooking.ToString());
                        string input_api_vin_guarantee_methods = "{\"organization\":\" vinpearl\",}";

                        string reservationID = (string)data_createbooking.SelectToken("data.reservations[0].reservationID");
                        var data_guarantee_methods = vin_lib.getGuaranteeMethods(reservationID, input_api_vin_guarantee_methods).Result;
                        var data_guaranteemethods = JObject.Parse(data_guarantee_methods);

                        if (data_guaranteemethods["isSuccess"].ToString().ToLower() == "true")
                        {
                            LogHelper.InsertLogTelegram("getCreateBookingVin - VinController-data-guarantee-methods: " + data_guaranteemethods.ToString());
                            string amount = data_createbooking.SelectToken("data.reservations[0].total.amount.amount").ToString();
                            List<string> list_id = new List<string>();
                            var guaranteeMethods = data_guaranteemethods["data"]["guaranteeMethods"];
                            foreach (var i in guaranteeMethods)
                            {
                                var id = i["id"].ToString();
                                list_id.Add(id);
                            }
                            string ListId = string.Join(",", list_id.ToArray());
                            string input_api_vin_Batch_Commit = "{\"items\":[{\"reservationId\":\"" + reservationID + "\",\"guaranteeInfos\":[{\"guaranteeRefID\":\"00000001-0000-0000-0000-000000000000\",\"guaranteePolicyId\":\"" + ListId + "\",\"guaranteeValue\":\"" + amount + "\"}]}]}";

                            var data_Batch_Commit = vin_lib.getBatchCommit(input_api_vin_Batch_Commit).Result;
                            var data_BatchCommit = JObject.Parse(data_Batch_Commit);

                            if (data_BatchCommit["isSuccess"].ToString().ToLower() == "true")
                            {
                                LogHelper.InsertLogTelegram("getCreateBookingVin - VinController-data-Batch-Commit: " + data_BatchCommit.ToString());
                                return Ok(new
                                {
                                    status = (int)ResponseType.SUCCESS,
                                    msg = "Thành công",
                                    data_create_booking = data_createbooking,
                                    data_guarantee_method = data_guaranteemethods,
                                    data_commit_booking = data_BatchCommit,

                                });
                            }
                            else
                            {
                                return Ok(new
                                {
                                    status = (int)ResponseType.FAILED,
                                    msg = "Tham số Batch Commit  không hợp lệ",
                                    data_create_booking = data_createbooking,
                                    data_guarantee_method = data_guaranteemethods,
                                    data_commit_booking = data_BatchCommit,
                                });
                            }
                        }
                        else
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.FAILED,
                                msg = "Tham guarantee methods số không hợp lệ",
                                data_create_booking = data_createbooking,
                                data_guarantee_method = data_guaranteemethods,
                                data_commit_booking = "",
                            });
                        }
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Tham số booking không hợp lệ",
                            data_create_booking = data_createbooking,
                            data_guarantee_method = "",
                            data_commit_booking = "",
                        });
                    }
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key invalid!",

                    });
                }


            }
            catch (Exception ex)
            {
                LogService.InsertLog("VinController - getCreateBookingVin: " + ex.ToString());
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "Error On Excution. Vui lòng liên hệ IT" });
            }
        }
        /// <summary>
        /// Lấy ra các trường để filter danh sách khách sạn b2b đã tìm kiếm trước đó
        /// </summary>
        /// <param name="token">chứa client_id</param>
        /// <returns></returns>
        [HttpPost("vin/vinpearl/get-filter-hotel.json")]
        public async Task<ActionResult> GetHotelFilterFields(string token)
        {
            #region Test
            //var j_param = new Dictionary<string, string>
            //    {
            //          { "cache_id", "search_result_2023-10-122023-10-131111cfe997c153e7c566c2260af3a54023421"},
            //    };

            //var data_product = JsonConvert.SerializeObject(j_param);
            //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
            #endregion
            try
            {
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    string cache_id = objParr[0]["cache_id"].ToString();
                    var str = redisService.Get(cache_id, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                    if (str != null && str.Trim() != "")
                    {
                        HotelSearchModel obj = JsonConvert.DeserializeObject<HotelSearchModel>(str);
                        List<HotelSearchEntities> source = obj.hotels;
                        HotelFilters filters = new HotelFilters();
                        if (source != null)
                        {
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
                            if (source.Where(x => x.amenities != null).Count() > 0)
                            {
                                var a = source.SelectMany(x => x.amenities);
                                if (a != null && a.Count() > 0)
                                {
                                    filters.amenities = a.Select(x => new FilterGroup() { key = x.key, description = x.description }).ToList();
                                    filters.amenities = filters.amenities.GroupBy(x => x.key).Select(g => g.First()).ToList();
                                }
                            }
                            //---- Loại phòng:
                            if (source.Where(x => x.type_of_room != null).Count() > 0)
                            {
                                filters.type_of_room = source.SelectMany(x => x.type_of_room).Select(z => new FilterGroup() { key = z, description = z }).ToList();
                                filters.type_of_room = filters.type_of_room.GroupBy(x => x.key).Select(g => g.First()).ToList();
                            }
                            //---- Loại khách sạn:
                            if(source.Where(x => x.hotel_type != null && x.hotel_type.Trim() != "").Count() > 0)
                            {
                                filters.hotel_type = source.Where(x => x.hotel_type != null && x.hotel_type.Trim() != "").Select(z => new FilterGroup() { key = z.hotel_type, description = z.hotel_type }).ToList();
                                filters.hotel_type = filters.hotel_type.GroupBy(x => x.key).Select(g => g.First()).ToList();
                            }
                            //-- Add to cache obj:
                            obj.filters = filters;
                            //-- Cache kết quả:
                            int db_index = Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"].ToString());
                            redisService.Set(cache_id, JsonConvert.SerializeObject(obj), DateTime.Now.AddDays(1), db_index);
                            //-- Trả kết quả
                            return Ok(new
                            {
                                data = filters,
                                status = (int)ResponseType.SUCCESS,
                                msg = " Success"
                            });
                        }

                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Cannot Get Data From Cache",
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
                LogService.InsertLog("VinController - GetHotelFilterFields ["+token+"]: " + ex.ToString());

                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "error: " + ex.ToString() });
            }
        }

        /// <summary>
        /// Tracking  thông tin 1 phòng khách sạn và các loại phòng trong đó theo 1 khoảng thời gian
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("vin/vinpearl/get-hotel-rooms.json")]
        public async Task<ActionResult> getHotelRoomsAvailability(string token) 
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
                //    {"clientType","2" },
                //    {"client_id","159" },
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
                #endregion.


                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {

                    string cancel_template = "Phí thay đổi / Hoàn hủy từ ngày {day} tháng {month} năm {year} là {value}";
                    string cancel_percent_template = " {value_2} % giá phòng";
                    string zero_percent = "Miễn phí Thay đổi / Hoàn hủy trước ngày {day} tháng {month} năm {year}";
                    string cancel_vnd_template = " {value_2} VND";

                    string status_vin = string.Empty;
                    string arrivalDate = objParr[0]["arrivalDate"].ToString();
                    string departureDate = objParr[0]["departureDate"].ToString();
                    int numberOfRoom = Convert.ToInt16(objParr[0]["numberOfRoom"]);
                    int numberOfChild = Convert.ToInt16(objParr[0]["numberOfChild"]);
                    int numberOfAdult = Convert.ToInt16(objParr[0]["numberOfAdult"]);
                    int numberOfInfant = Convert.ToInt16(objParr[0]["numberOfInfant"]);

                    string hotelID = objParr[0]["hotelID"].ToString();
                    string distributionChannelId = configuration["config_api_vinpearl:Distribution_ID"].ToString();
                    long client_id = Convert.ToInt64(objParr[0]["client_id"]);
                    int client_type = Convert.ToInt32(objParr[0]["clientType"]);

                    string input_api_vin_all = "{ \"distributionChannelId\": \"" + distributionChannelId + "\", \"propertyID\": \"" + hotelID + "\", \"numberOfRoom\":" + numberOfRoom + ", \"arrivalDate\":\"" + arrivalDate + "\", \"departureDate\":\"" + departureDate + "\", \"roomOccupancy\":{\"numberOfAdult\":" + numberOfAdult + ",\"otherOccupancies\":[{\"otherOccupancyRefCode\":\"child\",\"quantity\":" + numberOfChild + "},{\"otherOccupancyRefCode\":\"infant\",\"quantity\":" + numberOfInfant + "}]}}";
                    int number_room_each = 1;
                    int number_adult_each_room = (numberOfAdult / (float)numberOfRoom) > (int)(numberOfAdult / numberOfRoom) ? (int)(numberOfAdult / numberOfRoom) + 1 : (int)(numberOfAdult / numberOfRoom);
                    int number_child_each_room = numberOfChild == 1 || (((int)numberOfChild / numberOfRoom) <= 1 && numberOfChild > 0) ? 1 : numberOfChild / numberOfRoom;
                    int number_infant_each_room = numberOfInfant == 1 || (((int)numberOfInfant / numberOfRoom) <= 1 && numberOfInfant > 0) ? 1 : numberOfInfant / numberOfRoom;
                    string input_api_vin_phase = "{ \"distributionChannelId\": \"" + distributionChannelId + "\", \"propertyID\": \"" + hotelID + "\", \"numberOfRoom\":" + number_room_each + ", \"arrivalDate\":\"" + arrivalDate + "\", \"departureDate\":\"" + departureDate + "\", \"roomOccupancy\":{\"numberOfAdult\":" + number_adult_each_room + ",\"otherOccupancies\":[{\"otherOccupancyRefCode\":\"child\",\"quantity\":" + number_child_each_room + "},{\"otherOccupancyRefCode\":\"infant\",\"quantity\":" + number_infant_each_room + "}]}}";
                    var hotel_detail_sql = await hotelDetailRepository.GetByHotelId(hotelID);

                    string client_type_string = LIST_CLIENT_SAMETYPE.Contains(client_type) ? "1,2" : client_type.ToString();

                    //-- Đọc từ cache, nếu có trả kết quả:
                    string cache_name_id = arrivalDate + departureDate + numberOfRoom + numberOfChild + numberOfAdult + numberOfInfant + EncodeHelpers.MD5Hash(objParr[0]["hotelID"].ToString())  + client_type_string;
                    var str = redisService.Get(CacheName.HotelRoomDetail + cache_name_id, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                    string response = "";
                    if (str != null && str.Trim() != "")
                    {
                        HotelRoomDetailModel model = JsonConvert.DeserializeObject<HotelRoomDetailModel>(str);
                        var view_model = JsonConvert.DeserializeObject<List<RoomDetailViewModel>>(JsonConvert.SerializeObject(model.rooms));
                        //-- Trả kết quả
                        return Ok(new
                        {
                            status = ((int)ResponseType.SUCCESS).ToString(),
                            msg = "Get Data From Cache Success",
                            data = view_model,
                            surcharge = hotel_detail_sql.OtherSurcharge == null ? "" : hotel_detail_sql.OtherSurcharge,
                            cache_id = CacheName.HotelRoomDetail + cache_name_id
                        });
                    }
                    else
                    {
                        var vin_lib = new VinpearlLib(configuration);
                        response = vin_lib.getRoomAvailability(input_api_vin_phase).Result;
                    }
                    //LogHelper.InsertLogTelegram("getHotelRoomsAvailability - VinController: Cannot get from Redis [" + CacheName.HotelRoomDetail + cache_name_id + "] - token: " + token);

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
                            //-- Rates packages:

                            if (result.rooms != null && result.rooms.Where(x => x.id.Trim() == r["roomType"]["roomTypeID"].ToString().Trim()).FirstOrDefault() != null)
                            {
                                
                                if (result.rooms.Where(x => x.id.Trim() == r["roomType"]["roomTypeID"].ToString().Trim()).First().rates == null) result.rooms.Where(x => x.id.Trim() == r["roomType"]["roomTypeID"].ToString().Trim()).First().rates = new List<RoomDetailRate>();
                                var rate_of_room = result.rooms.Where(x => x.id.Trim() == r["roomType"]["roomTypeID"].ToString().Trim()).First().rates;
                                result.rooms.Where(x => x.id.Trim() == r["roomType"]["roomTypeID"].ToString().Trim()).First().rates.Add(
                                    new RoomDetailRate()
                                    {
                                        id = r["ratePlan"]["id"].ToString(),
                                        amount = Convert.ToDouble(r["totalAmount"]["amount"]["amount"]),
                                        code = r["ratePlan"]["rateCode"].ToString(),
                                        description = r["ratePlan"]["description"].ToString(),
                                        name = r["ratePlan"]["name"].ToString(),
                                        cancel_policy = cancel_policy_output,
                                        guarantee_policy = r["ratePlan"]["guaranteePolicy"]["description"] != null ? r["ratePlan"]["guaranteePolicy"]["description"].ToString() : "",
                                        allotment_id = r["allotments"] != null && r["allotments"].Count() > 0 ? r["allotments"][0]["id"].ToString() : "",
                                        package_includes = list
                                    }
                                );
                            }
                        }
                        DateTime fromdate = DateTime.ParseExact(arrivalDate, "yyyy-M-d", null);
                        DateTime todate = DateTime.ParseExact(departureDate, "yyyy-M-d", null);
                        //-- Tính giá về tay thông qua chính sách giá

                        var profit_list = hotelDetailRepository.GetHotelRoomPricePolicy(hotelID,  client_type_string);
                       // LogService.InsertLog("VinController - getHotelRoomsAvailability HotelVIN: Profit List [" + hotelID+"|" + client_type_string + "] count= " + profit_list.Count);

                        foreach (var r in result.rooms)
                        {
                            var r_id = r.id;
                            foreach (var rate in r.rates)
                            {
                                if (client_type == (int)ClientType.STAFF)
                                {
                                    rate.profit = 0;
                                    rate.total_profit = 0;
                                    rate.total_price = rate.amount;
                                }
                                else
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
                                }
                                r.min_price = r.min_price <= 0 ? rate.total_price : ((rate.total_price > 0 && r.min_price > rate.total_price) ? rate.total_price : r.min_price);

                            }
                        }


                        //-- Cache kết quả:

                        int db_index = Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"].ToString());
                        redisService.Set(CacheName.HotelRoomDetail + cache_name_id, JsonConvert.SerializeObject(result), DateTime.Now.AddDays(1),  db_index);
                        var view_model = JsonConvert.DeserializeObject<List<RoomDetailViewModel>>(JsonConvert.SerializeObject(result.rooms));

                        //-- Trả kết quả
                        return Ok(new
                        {
                            status = ((int)ResponseType.SUCCESS).ToString(),
                            msg = status_vin,
                            data = view_model,
                            surcharge = hotel_detail_sql.OtherSurcharge == null ? "" : hotel_detail_sql.OtherSurcharge,
                            cache_id = CacheName.HotelRoomDetail + cache_name_id,

                        });

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
                LogService.InsertLog("VinController -[" + token + "] - getHotelRoomsAvailability: " + ex.ToString());
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "error: " + ex.ToString() });
            }

        }



        /// <summary>
        /// Tracking  thông tin 1 phòng khách sạn và các loại phòng trong đó theo 1 khoảng thời gian
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("vin/vinpearl/get-room-packages.json")]
        public async Task<ActionResult> getHotelRoomsPackages(string token)
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
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
                #endregion


                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    if (objParr[0]["roomID"] == null || objParr[0]["roomID"].ToString().Trim() == "")
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Dữ liệu không hợp lệ"
                        });
                    }
                    //string status_vin = string.Empty;
                    //string arrivalDate = objParr[0]["arrivalDate"].ToString();
                    string roomID = objParr[0]["roomID"].ToString();
                    //string departureDate = objParr[0]["departureDate"].ToString();
                    //int numberOfRoom = Convert.ToInt16(objParr[0]["numberOfRoom"]);
                    //int numberOfChild = Convert.ToInt16(objParr[0]["numberOfChild"]);
                    //int numberOfAdult = Convert.ToInt16(objParr[0]["numberOfAdult"]);
                    //int numberOfInfant = Convert.ToInt16(objParr[0]["numberOfInfant"]);

                    //string hotelID = objParr[0]["hotelID"].ToString();
                    //int clientType = Convert.ToInt16(objParr[0]["clientType"]);
                    //string distributionChannelId = configuration["config_api_vinpearl:Distribution_ID"].ToString();
                    //long client_id = Convert.ToInt64(objParr[0]["client_id"]);
                    ////-- Đọc từ cache, nếu có trả kết quả:
                    //string cache_name_id = arrivalDate + departureDate + numberOfRoom + numberOfChild + numberOfAdult + numberOfInfant + objParr[0]["hotelID"].ToString() + clientType;
                    //var str = redisService.Get(CacheName.HotelRoomDetail + cache_name_id, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
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
                    //else
                    //{
                    //    //string input_api_vin = "{\"arrivalDate\":\"" + arrivalDate + "\",\"departureDate\":\"" + departureDate + "\",\"numberOfRoom\":" + numberOfRoom + ",\"propertyId\":" + hotelID + ",\"distributionChannelId\":\"" + distributionChannelId + "\",\"roomOccupancy\":{\"numberOfAdult\":" + numberOfAdult + ",\"otherOccupancies\":[{\"otherOccupancyRefCode\":\"child\",\"quantity\":" + numberOfChild + "},{\"otherOccupancyRefCode\":\"infant\",\"quantity\":" + numberOfInfant + "}]}}";
                    //    string input_api_vin = "{ \"distributionChannelId\": \""+ distributionChannelId + "\", \"propertyID\": \""+hotelID+ "\", \"numberOfRoom\":" + numberOfRoom + ", \"arrivalDate\":\"" + arrivalDate + "\", \"departureDate\":\"" + departureDate + "\", \"roomOccupancy\":{\"numberOfAdult\":" + numberOfAdult + ",\"otherOccupancies\":[{\"otherOccupancyRefCode\":\"child\",\"quantity\":" + numberOfChild + "},{\"otherOccupancyRefCode\":\"infant\",\"quantity\":" + numberOfInfant + "}]}}";
                    //    var vin_lib = new VinpearlLib(configuration);
                    //    response = vin_lib.getRoomAvailability(input_api_vin).Result;
                    //}

                    //HotelRoomDetailModel result = new HotelRoomDetailModel();
                    //var data_hotel = JObject.Parse(response);
                    //// Đọc Json ra các Field để map với những trường cần lấy

                    //#region Check Data Invalid
                    //if (data_hotel["isSuccess"].ToString().ToLower() == "false")
                    //{
                    //    return Ok(new
                    //    {
                    //        status = (int)ResponseType.EMPTY,
                    //        msg = "Tham số không hợp lệ",
                    //        data = data_hotel
                    //    });
                    //}
                    //#endregion


                    //var j_room_list = data_hotel["data"]["propertyInfo"]["roomTypes"];
                    //var j_rate_list = data_hotel["data"]["roomAvailabilityRates"];
                    //if (j_room_list.Count() > 0 && j_rate_list.Count() > 0)
                    //{
                    //    // Thông tin khách sạn
                    //    result = new HotelRoomDetailModel()
                    //    {
                    //        hotel_id = data_hotel["data"]["propertyInfo"]["id"].ToString(),
                    //        rooms = new List<RoomDetail>(),
                    //    };

                    //    foreach (var room in j_room_list)
                    //    {
                    //        #region Hình ảnh phòng

                    //        var img_thumb_room = new List<thumbnails>();
                    //        var j_thumb_room_img = room["thumbnails"];
                    //        foreach (var item_thumb in j_thumb_room_img)
                    //        {
                    //            var item_img_room = new thumbnails
                    //            {
                    //                id = item_thumb["id"].ToString(),
                    //                url = item_thumb["url"].ToString()
                    //            };
                    //            img_thumb_room.Add(item_img_room);
                    //        }
                    //        #endregion

                    //        #region Chi tiết phòng
                    //        var item = new RoomDetail
                    //        {
                    //            id = room["id"].ToString(),
                    //            code = room["code"].ToString(),
                    //            name = room["name"].ToString(),
                    //            img_thumb = img_thumb_room,

                    //            rates = new List<RoomDetailRate>(),
                    //        };
                    //        #endregion
                    //        result.rooms.Add(item);
                    //    }
                    //    //-- Giá gốc
                    //    foreach (var r in j_rate_list)
                    //    {
                    //        result.rooms.Where(x => x.id.Trim() == r["roomType"]["roomTypeID"].ToString().Trim()).First().rates.Add(
                    //            new RoomDetailRate()
                    //            {
                    //                id = r["ratePlan"]["id"].ToString(),
                    //                amount = Convert.ToDouble(r["totalAmount"]["amount"]["amount"]),
                    //                code = r["ratePlan"]["rateCode"].ToString(),
                    //                description = r["ratePlan"]["description"].ToString(),
                    //                name = r["ratePlan"]["name"].ToString(),
                    //                cancel_policy = JsonConvert.DeserializeObject<RoomDetailCancelPolicy>(r["ratePlan"]["cancelPolicy"].ToString()),
                    //                guarantee_policy = r["ratePlan"]["guaranteePolicy"]["description"].ToString()
                    //            }
                    //        );
                    //    }
                    //    //-- Tính giá về tay thông qua chính sách giá
                    //    var profit_list = await servicePiceRoomRepository.GetHotelRoomProfitFromSP(new List<string>() { data_hotel["data"]["propertyInfo"]["id"].ToString() }, result.rooms.SelectMany(x => x.rates).Select(x => x.id).ToList(), result.rooms.Select(x => x.id).ToList());
                    //    foreach (var r in result.rooms)
                    //    {
                    //        foreach (var rate in r.rates)
                    //        {
                    //            var profit = profit_list.Where(x => x.hotel_id == result.hotel_id && x.room_id == r.id && x.rate_plan_id == rate.id).FirstOrDefault();
                    //            if (profit != null)
                    //            {

                    //                RateHelper.GetProfit(rate, arrivalDate, departureDate, numberOfRoom, profit.profit, profit.profit_unit_id);
                    //            }
                    //            else
                    //            {
                    //                RateHelper.GetDefaultProfitAdavigo(rate, clientType, numberOfRoom, arrivalDate, departureDate);

                    //            }
                    //            r.min_price = r.min_price <= 0 ? rate.total_price : ((rate.total_price > 0 && r.min_price > rate.total_price) ? rate.total_price : r.min_price);
                    //        }
                    //    }
                    //    //-- Cache kết quả:

                    //    int db_index = Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"].ToString());
                    //    redisService.Set(CacheName.HotelRoomDetail + cache_name_id, JsonConvert.SerializeObject(result), DateTime.Now.AddHours(2), db_index);
                    //    var view_model = result.rooms.Where(x => x.id == roomID).FirstOrDefault();
                    //    //-- Trả kết quả
                    //    return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), msg = status_vin, data = view_model });

                    //}
                    else
                    {
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
        #endregion

        #region Hotel Manual:

        [HttpPost("hotel-manual/tracking-hotel-availability.json")]
        public async Task<ActionResult> getHotelManualAvailability(string token)
        {
            try
            {
                #region Test

                //var j_param = new Dictionary<string, string>
                //{
                //    { "arrivalDate", "2023-03-01"},
                //    { "departureDate","2023-03-03" },
                //    { "numberOfRoom", "1"},
                //    { "hotelID","1104" },
                //    { "numberOfChild","1" },
                //    { "numberOfAdult","2" },
                //    { "numberOfInfant","1" },
                //    { "clientType","2" },
                //    { "client_id","182" },
                //    { "product_type","0" },
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
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
                    int client_type = Convert.ToInt32(objParr[0]["clientType"]);
                    string client_type_string = LIST_CLIENT_SAMETYPE.Contains(client_type) ? "1,2" : client_type.ToString();

                    var arrival_date = DateTime.Parse(arrivalDate);
                    var departure_date = DateTime.Parse(departureDate);
                    int nights = Convert.ToInt32((departure_date - arrival_date).TotalDays < 1 ? 1 : (departure_date - arrival_date).TotalDays);
                    // -- Read from Cache:
                    string cache_name= CacheName.ClientHotelSearchResult + product_type + arrivalDate + departureDate + numberOfRoom + numberOfChild + numberOfAdult + numberOfInfant + EncodeHelpers.MD5Hash(hotelID) + client_type_string;
                    var str = redisService.Get(cache_name, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                    if (str != null && str.Trim() != "")
                    {
                        HotelSearchModel model = JsonConvert.DeserializeObject<HotelSearchModel>(str);
                        var view_model = model.hotels;
                        // --Trả kết quả
                        return Ok(new
                        {
                            status = ((int)ResponseType.SUCCESS).ToString(),
                            msg = "Get Data From Cache Success",
                            data = view_model,
                            cache_id = cache_name
                        });
                    }
                    //LogHelper.InsertLogTelegram("getHotelManualAvailability - VinController: Cannot get from Redis [" + cache_name + "] - token: " + token);

                    var hotel_datas = hotelDetailRepository.GetFEHotelList(new HotelFESearchModel
                    {
                        FromDate = arrival_date,
                        ToDate = departure_date,
                        HotelId = hotelID,
                        HotelType = hotelName,
                        PageIndex = 1,
                        PageSize = 50
                    });
                    string hotel_ids="";
                    if (hotel_datas != null && hotel_datas.Any())
                    {
                        var data_results = hotel_datas.GroupBy(x => x.Id).Select(x => x.First()).ToList();

                        if (data_results != null && data_results.Any())
                        {
                            hotel_ids = string.Join(",", data_results.Select(x => x.HotelId));
                            foreach (var hotel in data_results)
                            {
                                var hotel_detail = await hotelDetailRepository.GetByHotelId(hotel.Id.ToString());
                                var hotel_rooms = hotelDetailRepository.GetFEHotelRoomList(hotel.Id);
                                //-- Tính giá về tay thông qua chính sách giá
                                var profit_list = hotelDetailRepository.GetHotelRoomPricePolicy(hotel.HotelId,client_type_string) ;
                                //LogService.InsertLog("VinController - getHotelManualAvailability HotelManual: Profit List [" + hotel.HotelId + client_type_string + "] count= " + profit_list.Count);

                                List<RoomDetail> rooms_list = new List<RoomDetail>();
                                foreach(var r in hotel_rooms)
                                {
                                    var room_packages = hotelDetailRepository.GetFERoomPackageListByRoomId(r.Id, arrival_date, departure_date);
                                    var room_packages_daily = hotelDetailRepository.GetFERoomPackageDaiLyListByRoomId(r.Id, arrival_date, departure_date);
                                    rooms_list.Add(PricePolicyService.GetRoomDetail(r.Id.ToString(),arrival_date,departure_date, nights, room_packages_daily, room_packages, profit_list, hotel_detail, null, (int)client_type));
                                }
                                var min_price = rooms_list.Where(x=>x.min_price>0).OrderBy(x => x.min_price).FirstOrDefault();
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
                                 
                                    min_price = min_price == null ? 0 : min_price.min_price,
                                    email = hotel.Email,
                                    telephone = hotel.Telephone,
                                    img_thumb = new List<string> { hotel.ImageThumb }
                                });
                              
                            }
                        }
                        //-- Cache kết quả:
                        HotelSearchModel cache_data = new HotelSearchModel();
                        cache_data.hotels = result;
                        cache_data.input_api_vin = "";
                        cache_data.rooms = new List<RoomSearchModel>();
                        cache_data.client_type = client_type;
                        cache_data.hotel_ids = hotel_ids;
                        int db_index = Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"].ToString());
                        redisService.Set(cache_name, JsonConvert.SerializeObject(cache_data), DateTime.Now.AddDays(1),  db_index);

                        if (result.Count > 0)
                        {
                            return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), msg = status_vin, data = result, cache_id = cache_name });
                        }

                    }
                    status_vin = "Không tìm thấy khách sạn nào thỏa mãn điều kiện này";
                    return Ok(new { status = ((int)ResponseType.EMPTY).ToString(), msg = status_vin, cache_id = cache_name });
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

        [HttpPost("hotel-manual/get-hotel-rooms.json")]
        public async Task<ActionResult> getHotelRoomsManualAvailability(string token)
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
                //    {"clientType","2" },
                //    {"client_id","159" },
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
                #endregion.


                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {

                    //string cancel_template = "Phí thay đổi / Hoàn hủy từ ngày {day} tháng {month} năm {year} là {value}";
                    //string cancel_percent_template = " {value_2} % giá phòng";
                    //string zero_percent = "Miễn phí Thay đổi / Hoàn hủy trước ngày {day} tháng {month} năm {year}";
                    //string cancel_vnd_template = " {value_2} VND";

                    string status_vin = string.Empty;
                    string arrivalDate = objParr[0]["arrivalDate"].ToString();
                    string departureDate = objParr[0]["departureDate"].ToString();
                    int numberOfRoom = Convert.ToInt16(objParr[0]["numberOfRoom"]);
                    int numberOfChild = Convert.ToInt16(objParr[0]["numberOfChild"]);
                    int numberOfAdult = Convert.ToInt16(objParr[0]["numberOfAdult"]);
                    int numberOfInfant = Convert.ToInt16(objParr[0]["numberOfInfant"]);

                    DateTime arrival_date = DateTime.Parse(arrivalDate);
                    DateTime departure_date = DateTime.Parse(departureDate);
                    var total_nights = (int)((departure_date - arrival_date).TotalDays);

                    string hotelID = objParr[0]["hotelID"].ToString();
                    string distributionChannelId = configuration["config_api_vinpearl:Distribution_ID"].ToString();
                    long client_id = Convert.ToInt64(objParr[0]["client_id"]);
                    int client_type = Convert.ToInt32(objParr[0]["clientType"]);
                    string client_type_string = LIST_CLIENT_SAMETYPE.Contains(client_type) ? "1,2" : client_type.ToString();

                    string input_api_vin_all = "{ \"distributionChannelId\": \"" + distributionChannelId + "\", \"propertyID\": \"" + hotelID + "\", \"numberOfRoom\":" + numberOfRoom + ", \"arrivalDate\":\"" + arrivalDate + "\", \"departureDate\":\"" + departureDate + "\", \"roomOccupancy\":{\"numberOfAdult\":" + numberOfAdult + ",\"otherOccupancies\":[{\"otherOccupancyRefCode\":\"child\",\"quantity\":" + numberOfChild + "},{\"otherOccupancyRefCode\":\"infant\",\"quantity\":" + numberOfInfant + "}]}}";
                    int number_room_each = 1;
                    int number_adult_each_room = (numberOfAdult / (float)numberOfRoom) > (int)(numberOfAdult / numberOfRoom) ? (int)(numberOfAdult / numberOfRoom) + 1 : (int)(numberOfAdult / numberOfRoom);
                    int number_child_each_room = numberOfChild == 1 || (((int)numberOfChild / numberOfRoom) <= 1 && numberOfChild > 0) ? 1 : numberOfChild / numberOfRoom;
                    int number_infant_each_room = numberOfInfant == 1 || (((int)numberOfInfant / numberOfRoom) <= 1 && numberOfInfant > 0) ? 1 : numberOfInfant / numberOfRoom;
                    string input_api_vin_phase = "{ \"distributionChannelId\": \"" + distributionChannelId + "\", \"propertyID\": \"" + hotelID + "\", \"numberOfRoom\":" + number_room_each + ", \"arrivalDate\":\"" + arrivalDate + "\", \"departureDate\":\"" + departureDate + "\", \"roomOccupancy\":{\"numberOfAdult\":" + number_adult_each_room + ",\"otherOccupancies\":[{\"otherOccupancyRefCode\":\"child\",\"quantity\":" + number_child_each_room + "},{\"otherOccupancyRefCode\":\"infant\",\"quantity\":" + number_infant_each_room + "}]}}";
                    var hotel = await hotelDetailRepository.GetByHotelId(hotelID);

                    //-- Đọc từ cache, nếu có trả kết quả:
                    string cache_name = CacheName.HotelRoomDetail + arrivalDate + departureDate + numberOfRoom + numberOfChild + numberOfAdult + numberOfInfant + EncodeHelpers.MD5Hash(objParr[0]["hotelID"].ToString())  + client_type_string;
                    var str = redisService.Get(cache_name, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                    if (str != null && str.Trim() != "")
                    {
                        HotelRoomDetailModel model = JsonConvert.DeserializeObject<HotelRoomDetailModel>(str);
                        var view_model = JsonConvert.DeserializeObject<List<RoomDetailViewModel>>(JsonConvert.SerializeObject(model.rooms));
                        //-- Trả kết quả
                        return Ok(new
                        {
                            status = ((int)ResponseType.SUCCESS).ToString(),
                            msg = "Get Data From Cache Success",
                            data = view_model,
                            surcharge = hotel.OtherSurcharge == null ? "" : hotel.OtherSurcharge,
                            cache_id = cache_name
                        });
                    }
                    //LogHelper.InsertLogTelegram("getHotelRoomsManualAvailability - VinController: Cannot get from Redis [" + cache_name + "] - token: " + token);

                    HotelRoomDetailModel result = new HotelRoomDetailModel();
                    result.input_api_vin = input_api_vin_all;

                    // Đọc Json ra các Field để map với những trường cần lấy
                    if (hotel == null || hotel.Id <= 0)
                    {
                        status_vin = "Không tìm thấy danh sách phòng thỏa mãn điều kiện";
                        return Ok(new { status = ((int)ResponseType.EMPTY).ToString(), msg = status_vin });
                    }
                    var hotel_rooms = hotelDetailRepository.GetFEHotelRoomList(hotel.Id);
                    var hotel_detail_sql = await hotelDetailRepository.GetById(hotel.Id);

                    if (hotel_rooms != null && hotel_rooms.Any())
                    {
                        // Thông tin khách sạn
                        result = new HotelRoomDetailModel()
                        {
                            hotel_id = hotelID,
                            rooms = new List<RoomDetail>(),
                        };

                        //-- Tính giá về tay thông qua chính sách giá
                        int nights = Convert.ToInt32((departure_date - arrival_date).TotalDays < 1 ? 1 : (departure_date - arrival_date).TotalDays);
                        var profit_list = hotelDetailRepository.GetHotelRoomPricePolicy(hotelID,  client_type_string);
                       // LogService.InsertLog("VinController - getHotelRoomsManualAvailability HotelManual: Profit List [" + hotelID + client_type_string + "] count= " + profit_list.Count);

                        foreach (var room in hotel_rooms)
                        {
                            #region Hình ảnh phòng

                            var img_thumb_room = new List<thumbnails>();
                            if (!string.IsNullOrEmpty(room.RoomAvatar))
                            {
                                var j_thumb_room_img = room.RoomAvatar.Split(",").ToArray();

                                img_thumb_room = j_thumb_room_img.Select(s => new thumbnails
                                {
                                    id = string.Empty,
                                    url = s
                                }).ToList();
                            }

                            #endregion
                            #region Packages đi kèm:



                            #endregion
                            #region Chi tiết phòng

                            var room_packages = hotelDetailRepository.GetFERoomPackageListByRoomId(room.Id, arrival_date, departure_date);
                            var room_packages_daily = hotelDetailRepository.GetFERoomPackageDaiLyListByRoomId(room.Id, arrival_date, departure_date);
                            var item = PricePolicyService.GetRoomDetail(room.Id.ToString(), arrival_date, departure_date, nights, room_packages_daily, room_packages, profit_list, hotel_detail_sql,
                                 new RoomDetail
                                 {
                                     id = room.Id.ToString(),
                                     name = room.Name,
                                     description = room.Description,
                                     max_adult = room.NumberOfAdult ?? 0,
                                     max_child = room.NumberOfChild ?? 0,
                                     img_thumb = new List<thumbnails> {
                                         new thumbnails {
                                                url = room.Avatar
                                         }
                                     },
                                     min_price = 0,
                                     remainming_room = room.NumberOfRoom ?? 0,
                                     rates = new List<RoomDetailRate>()

                                 }, (int)client_type);

                            result.rooms.Add(item);

                            #endregion

                        }
                        var view_model = JsonConvert.DeserializeObject<List<RoomDetailViewModel>>(JsonConvert.SerializeObject(result.rooms));
                        //-- Cache kết quả:

                        int db_index = Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"].ToString());
                        redisService.Set(cache_name, JsonConvert.SerializeObject(result), DateTime.Now.AddDays(1),  db_index);

                        //-- Trả kết quả
                        return Ok(new
                        {
                            status = ((int)ResponseType.SUCCESS).ToString(),
                            msg = status_vin,
                            data = view_model,
                            surcharge = hotel_detail_sql.OtherSurcharge == null ? "" : hotel_detail_sql.OtherSurcharge,
                            cache_id = cache_name,

                        });
                    }
                    else
                    {
                        status_vin = "Không tìm thấy danh sách phòng thỏa mãn điều kiện";
                        return Ok(new { 
                            status = ((int)ResponseType.EMPTY).ToString(), 
                            msg = status_vin,
                            surcharge = "",
                            cache_id = cache_name
                        });
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
        public async Task<ActionResult> getHotelManualRoomsPackages(string token)
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
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    if (objParr[0]["roomID"] == null || objParr[0]["roomID"].ToString().Trim() == "")
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Dữ liệu không hợp lệ"
                        });
                    }
                    string cache_name = objParr[0]["cache_id"].ToString();
                    string roomID = objParr[0]["roomID"].ToString();
                    string arrivalDate = objParr[0]["arrivalDate"].ToString();
                    string departureDate = objParr[0]["departureDate"].ToString();
                    DateTime arrival_date = DateTime.Parse(arrivalDate);
                    DateTime departure_date = DateTime.Parse(departureDate);
                    int nights = Convert.ToInt32((departure_date - arrival_date).TotalDays < 1 ? 1 : (departure_date - arrival_date).TotalDays);
                    long client_id = Convert.ToInt64(objParr[0]["client_id"]);
                    int client_type = Convert.ToInt32(objParr[0]["clientType"]);
                    string client_type_string = LIST_CLIENT_SAMETYPE.Contains(client_type) ? "1,2" : client_type.ToString();

                    int db_index = Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]);
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
                    string cache_name_id = arrivalDate + departureDate + roomID + client_type_string;
                    var str = redisService.Get(CacheName.B2C_HotelPackage+ EncodeHelpers.MD5Hash(hotel.HotelId)+"_" + cache_name_id, db_index);
                    if (str != null && str.Trim() != "")
                    {
                        RoomDetail model = JsonConvert.DeserializeObject<RoomDetail>(str);
                        //-- Trả kết quả
                        return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), msg = "Get Data Success", data = model, cache_id = string.Empty });
                    }
                    //LogHelper.InsertLogTelegram("getHotelManualRoomsPackages - VinController: Cannot get from Redis [" + cache_name + "] - token: " + token);

                    var room_packages = hotelDetailRepository.GetFERoomPackageListByRoomId(int.Parse(roomID), arrival_date, departure_date);
                    var room_packages_daily = hotelDetailRepository.GetFERoomPackageDaiLyListByRoomId(int.Parse(roomID), arrival_date,  departure_date);
                  
                    var profit_list = hotelDetailRepository.GetHotelRoomPricePolicy(hotel.HotelId, client_type_string);
                    //LogService.InsertLog("VinController - getHotelManualRoomsPackages HotelManual: Profit List [" + hotel.HotelId + client_type_string + "] count= " + profit_list.Count);
             
                    var view_model = PricePolicyService.GetRoomDetail(roomID, arrival_date, departure_date, nights, room_packages_daily, room_packages, profit_list, hotel, null, (int)client_type);

                    if (view_model == null )
                    {
                        return Ok(new { status = ((int)ResponseType.EMPTY).ToString(), msg = "Không tìm thấy danh sách phòng thỏa mãn điều kiện", cache_id = string.Empty });
                    }
                    else
                    {
                        redisService.Set(CacheName.B2C_HotelPackage + cache_name_id, JsonConvert.SerializeObject(view_model),DateTime.Now.AddDays(1), db_index);
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
        #endregion
    }

}
