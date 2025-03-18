using Caching.Elasticsearch;
using Caching.RedisWorker;
using ENTITIES.ViewModels.Hotel;
using Microsoft.Extensions.Configuration;
using REPOSITORIES.IRepositories.Elasticsearch;
using REPOSITORIES.IRepositories.Hotel;
using Repositories.IRepositories;
using REPOSITORIES.IRepositories;
using API_CORE.Service.Price;
using API_CORE.Service.Vin;
using ENTITIES.ViewModels.MongoDb;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Utilities.Contants;
using Utilities;
using System.Linq;

namespace API_CORE.Service.Hotel
{
    public class HotelPriceSyncService
    {
        private IConfiguration configuration;
        private HotelESRepository _hotelESRepository;
        private IHotelDetailRepository _hotelDetailRepository;
        private readonly RedisConn redisService;

        public HotelPriceSyncService(IConfiguration _configuration,  IHotelDetailRepository hotelDetailRepository, RedisConn _redisService)
        {
            configuration = _configuration;
            _hotelDetailRepository = hotelDetailRepository;
            _hotelESRepository = new HotelESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
            redisService = _redisService;
        }
        public async Task<bool> Sync()
        {
            try
            {
                DateTime fromdate = DateTime.Now.AddDays(1);
                DateTime todate = DateTime.Now.AddDays(2);
                List<int> client_type_sync = new List<int>() { 1, 2, 3, 4, 5, 9 };
                foreach (var client_type in client_type_sync)
                {
                    var client_types_string = client_type.ToString();
                    if (client_type == (int)ClientType.AGENT || client_type == (int)ClientType.TIER_1_AGENT) client_types_string = "1,2";
                    var vin_lib = new VinpearlLib(configuration);
                    int total_nights = (todate - fromdate).Days;
                    var hotels = await _hotelESRepository.GetAllHotels();
                    var hotel_position = await _hotelDetailRepository.GetListHotelActivePosition();
                    if (hotels != null && hotels.Count > 0)
                    {
                        // hotels = hotels.Where(x => x.isvinhotel == true).ToList();
                        foreach (var h in hotels)
                        {
                            var b2b = hotel_position.FirstOrDefault(x => x.PositionType == 1 && x.HotelId == h.id);
                            var b2c = hotel_position.FirstOrDefault(x => x.PositionType == 2 && x.HotelId == h.id);
                            HotelPriceMongoDbModel model = new HotelPriceMongoDbModel()
                            {
                                arrival_date = fromdate,
                                departure_date = todate,
                                client_type = (int)client_type,
                                hotel_id = h.hotelid,
                                min_price = 0,
                                hotel_name=h.name,
                                city = h.city==null?"": h.city.Trim(),
                                state = h.state == null ? "" : h.state.Trim(),
                                star = h.star == null ? 0 : Convert.ToInt32(h.star),
                                position_b2b=(b2b==null || b2b.Id<=0? 0:(int)b2b.Position),
                                position_b2c=(b2c == null || b2c.Id<=0? 0:(int)b2c.Position),
                                is_commit= h.iscommitfund,
                                is_vinhotel=h.isvinhotel
                            };

                            switch (h.isvinhotel)
                            {
                                case true:
                                    {
                                        string input_api_vin_phase = "{ \"distributionChannelId\": \"" + configuration["config_api_vinpearl:Distribution_ID"].ToString()
                                            + "\", \"propertyID\": \"" + h.hotelid + "\", \"numberOfRoom\":1, \"arrivalDate\":\"" + fromdate.ToString("yyyy-MM-dd")
                                            + "\", \"departureDate\":\"" + todate.ToString("yyyy-MM-dd")
                                            + "\", \"roomOccupancy\":{\"numberOfAdult\":2,\"otherOccupancies\":[{\"otherOccupancyRefCode\":\"child\",\"quantity\":0},{\"otherOccupancyRefCode\":\"infant\",\"quantity\":0}]}}";
                                        var response = vin_lib.getRoomAvailability(input_api_vin_phase).Result;
                                        var data = JObject.Parse(response);
                                        if (data == null || data["isSuccess"] == null || data["isSuccess"].ToString().ToLower() == "false") continue;
                                        var j_rate_list = data["data"]["roomAvailabilityRates"];
                                        List<RoomDetailRate> rates = new List<RoomDetailRate>();
                                        if (j_rate_list == null || j_rate_list.Count() <= 0) continue;
                                        //-- Giá gốc + cancel policy
                                        foreach (var r in j_rate_list)
                                        {
                                            rates.Add(new RoomDetailRate()
                                            {
                                                id = r["ratePlan"]["id"].ToString(),
                                                amount = Convert.ToDouble(r["totalAmount"]["amount"]["amount"]),
                                                code = r["ratePlan"]["rateCode"].ToString(),
                                                description = r["ratePlan"]["description"].ToString(),
                                                name = r["ratePlan"]["name"].ToString(),
                                                guarantee_policy = r["ratePlan"]["guaranteePolicy"]["description"] != null ? r["ratePlan"]["guaranteePolicy"]["description"].ToString() : "",
                                                allotment_id = r["allotments"] != null && r["allotments"].Count() > 0 ? r["allotments"][0]["id"].ToString() : "",
                                                room_code = r["roomType"]["roomTypeID"].ToString().Trim(),

                                            });
                                        }
                                        if (rates == null || rates.Count() <= 0) continue;
                                        var profit_list = _hotelDetailRepository.GetHotelRoomPricePolicy(h.hotelid, client_types_string, true);
                                        List<double> prices = new List<double>();
                                        foreach (var rate in rates)
                                        {
                                            if (client_type == (int)ClientType.STAFF)
                                            {
                                                rate.profit = 0;
                                                rate.total_profit = 0;
                                                rate.total_price = rate.amount;
                                            }
                                            else
                                            {
                                                //var profit = profit_list.Where(x => x.HotelCode == result.hotel_id && x.RoomTypeCode == r_id && x.PackageName == rate.id).ToList();
                                                var profit = profit_list.Where(x =>
                                                x.RoomTypeCode.ToLower().Trim() == rate.room_code.ToLower().Trim()
                                                && x.PackageCode.ToLower().Trim() == rate.code.ToLower().Trim()

                                                ).ToList();

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
                                            prices.Add(rate.total_price);
                                        }
                                        var min_price_value = prices.Where(x => x > 0).OrderBy(x => x).FirstOrDefault();
                                        if (min_price_value > 0)
                                        {
                                            model.min_price = min_price_value;
                                        }
                                    }
                                    break;
                                default:
                                    {
                                        List<RoomDetail> rooms_list = new List<RoomDetail>();
                                        var hotel_detail = await _hotelDetailRepository.GetByHotelId(h.hotelid);
                                        if (hotel_detail == null || hotel_detail.Id <= 0) continue;
                                        var hotel_rooms = _hotelDetailRepository.GetFEHotelRoomList(Convert.ToInt32(h.hotelid));
                                        if (hotel_rooms == null || hotel_rooms.Count <= 0) continue;
                                        //-- Tính giá về tay thông qua chính sách giá
                                        var profit_list = _hotelDetailRepository.GetHotelRoomPricePolicy(h.hotelid, client_types_string);
                                        foreach (var r in hotel_rooms)
                                        {
                                            var room_packages = _hotelDetailRepository.GetFERoomPackageListByRoomId(r.Id, fromdate, todate);
                                            var room_packages_daily = _hotelDetailRepository.GetFERoomPackageDaiLyListByRoomId(r.Id, fromdate, todate);
                                            rooms_list.Add(PricePolicyService.GetRoomDetail(r.Id.ToString(), fromdate, todate, total_nights, room_packages_daily, room_packages, profit_list, hotel_detail, null, (int)client_type));
                                        }
                                        var min_price_value = rooms_list.Where(x => x.min_price > 0).OrderBy(x => x.min_price).FirstOrDefault();
                                        if (min_price_value != null && min_price_value.min_price > 0)
                                        {
                                            model.min_price = min_price_value.min_price;
                                        }
                                    }
                                    break;
                            }
                          await  _hotelDetailRepository.UpSertHotelPrice(model);

                        }
                    }
                }
                try
                {
                    redisService.DeleteCacheByKeyword(CacheName.ALLHotelByLocation, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                    redisService.DeleteCacheByKeyword(CacheName.HotelByLocation, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                }
                catch
                {

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Sync - HotelPriceSyncService: " + ex);
                return false;
            }
            return true;
        }
    }
}
