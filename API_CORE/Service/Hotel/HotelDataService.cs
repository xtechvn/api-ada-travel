using API_CORE.Service.Log;
using API_CORE.Service.Price;
using ENTITIES.Models;
using ENTITIES.ViewModels.Hotel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories.Hotel;
using REPOSITORIES.Repositories.Hotel;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Contants;

namespace API_CORE.Service.Hotel
{
    public class HotelDataService
    {
        private IConfiguration configuration;
        private LogService LogService;
        private IHotelDetailRepository hotelDetailRepository;

        public HotelDataService(IConfiguration _configuration, IHotelDetailRepository _hotelDetailRepository)
        {
            configuration = _configuration;
            LogService = new LogService(_configuration);
            hotelDetailRepository = _hotelDetailRepository;

        }
        public HotelRoomDetailModel GetVinHotelRooms(string hotel_id,string response,DateTime arrivalDate,DateTime departureDate)
        {
            HotelRoomDetailModel result = new HotelRoomDetailModel();
            string cancel_template = "Phí thay đổi / Hoàn hủy từ ngày {day} tháng {month} năm {year} là {value}";
            string cancel_percent_template = " {value_2} % giá phòng";
            string zero_percent = "Miễn phí Thay đổi / Hoàn hủy trước ngày {day} tháng {month} năm {year}";
            string cancel_vnd_template = " {value_2} VND";
           

            try
            {
               
                // result.input_api_vin = input_api_vin_all;
                var data_hotel = JObject.Parse(response);
                // Đọc Json ra các Field để map với những trường cần lấy

                #region Check Data Invalid
                if (data_hotel["isSuccess"].ToString().ToLower() == "false")
                {
                    return result;
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
                        DateTime arrivalDate_date_time = arrivalDate;
                        DateTime day_before_arrival_before = arrivalDate;
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
                    if(result!=null && result.rooms!=null && result.rooms.Count > 0)
                    {
                        //-- Tính giá về tay thông qua chính sách giá
                        var profit_list = hotelDetailRepository.GetHotelRoomPricePolicy(hotel_id, "5");

                        foreach (var r in result.rooms)
                        {
                            var r_id = r.id;
                            foreach (var rate in r.rates)
                            {
                                var profit = profit_list.Where(x => x.HotelCode == result.hotel_id && x.RoomTypeCode == r_id && x.PackageName == rate.id).ToList();
                                if (profit != null && profit.Count > 0)
                                {
                                    rate.total_profit = PricePolicyService.CalucateMinProfit(profit, rate.amount, arrivalDate, departureDate);
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
                    }
                    return result;

                }
                
            }
            catch (Exception ex)
            {
                LogService.InsertLog("HotelDataService - GetVinHotelDataFromHMS: " + ex.ToString());
            }
            return result;
        }
        public List<HotelSearchEntities> GetVinHotelAvailability(string response,DateTime arrivalDate,DateTime departureDate)
        {
            List<HotelSearchEntities> result = new List<HotelSearchEntities>();
            string cancel_template = "Phí thay đổi / Hoàn hủy từ ngày {day} tháng {month} năm {year} là {value}";
            string cancel_percent_template = " {value_2} % giá phòng";
            string zero_percent = "Miễn phí Thay đổi / Hoàn hủy trước ngày {day} tháng {month} năm {year}";
            string cancel_vnd_template = " {value_2} VND";
            try
            {

                var data_hotel = JObject.Parse(response);
                // Đọc Json ra các Field để map với những trường cần lấy
                if (data_hotel["isSuccess"] == null || data_hotel["isSuccess"].ToString().ToLower() == "false" || (data_hotel["message"] != null && data_hotel["message"].ToString().ToLower() == "unauthorized"))
                {
                    return result;
                }
                var j_hotel_list = data_hotel["data"]["rates"];
                if (j_hotel_list != null && j_hotel_list.Count() > 0)
                {
                    var room_list = new List<RoomSearchModel>();
                    //-- Get PricePolicy:
                    var from_date = arrivalDate;
                    var to_date = departureDate;
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
                    return result;
                }

            
                
            }
            catch (Exception ex)
            {
                LogService.InsertLog("HotelDataService - GetVinHotelDataFromHMS: " + ex.ToString());
            }
            return result;
        }
        public async Task<List<HotelSearchEntities>> getHotelManualAvailability(string client_type_string, int client_type, string hotelID,string hotelName, DateTime arrivalDate, DateTime departureDate)
        {
            List<HotelSearchEntities> result = new List<HotelSearchEntities>();

            try
            {
                int nights = Convert.ToInt32((departureDate - arrivalDate).TotalDays < 1 ? 1 : (departureDate - arrivalDate).TotalDays);

                var hotel_datas = hotelDetailRepository.GetFEHotelList(new HotelFESearchModel
                {
                    FromDate = arrivalDate,
                    ToDate = departureDate,
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
                            var hotel_rooms = hotelDetailRepository.GetFEHotelRoomList(hotel.Id);
                            //-- Tính giá về tay thông qua chính sách giá
                            var profit_list = hotelDetailRepository.GetHotelRoomPricePolicy(hotel.HotelId, client_type_string);
                            //LogService.InsertLog("VinController - getHotelManualAvailability HotelManual: Profit List [" + hotel.HotelId + client_type_string + "] count= " + profit_list.Count);

                            List<RoomDetail> rooms_list = new List<RoomDetail>();
                            foreach (var r in hotel_rooms)
                            {
                                var room_packages = hotelDetailRepository.GetFERoomPackageListByRoomId(r.Id, arrivalDate, departureDate);
                                var room_packages_daily = hotelDetailRepository.GetFERoomPackageDaiLyListByRoomId(r.Id, arrivalDate, departureDate);
                                rooms_list.Add(PricePolicyService.GetRoomDetail(r.Id.ToString(), arrivalDate, departureDate, nights, room_packages_daily, room_packages, profit_list, hotel_detail, null, (int)client_type));
                            }
                            var min_price = rooms_list.Where(x => x.min_price > 0).OrderBy(x => x.min_price).FirstOrDefault();
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
                    

                }
            }
            catch (Exception ex)
            {
                LogService.InsertLog("HotelDataService - GetVinHotelDataFromHMS: " + ex.ToString());
            }
            return result;
        }
        public async Task<HotelRoomDetailModel> GetManualHotelRooms(int hotelId, string client_type_string, int client_type, DateTime arrivalDate, DateTime departureDate)
        {
            HotelRoomDetailModel result = new HotelRoomDetailModel();


            try
            {
                List<RoomDetail> rooms_list_V2 = new List<RoomDetail>();


                var hotel_detail = await hotelDetailRepository.GetByHotelId(hotelId.ToString());
                var hotel_rooms = hotelDetailRepository.GetFEHotelRoomList(hotelId);
                //-- Tính giá về tay thông qua chính sách giá
                var profit_list = hotelDetailRepository.GetHotelRoomPricePolicy(hotelId.ToString(), ((int)Utilities.Contants.ClientType.CUSTOMER).ToString());
                foreach (var r in hotel_rooms)
                {
                    var room_packages = hotelDetailRepository.GetFERoomPackageListByRoomId(r.Id, arrivalDate, departureDate);
                    var room_packages_daily = hotelDetailRepository.GetFERoomPackageDaiLyListByRoomId(r.Id, arrivalDate, departureDate);
                    rooms_list_V2.Add(PricePolicyService.GetRoomDetail(r.Id.ToString(), arrivalDate, departureDate, (int)((departureDate - arrivalDate).TotalDays), room_packages_daily, room_packages, profit_list, hotel_detail, null));
                }
                var min_price = rooms_list_V2.Where(x => x.min_price > 0).OrderBy(x => x.min_price).FirstOrDefault();
                var min_price_value = min_price == null ? 0 : min_price.min_price;


                //var hotel_rooms = hotelDetailRepository.GetFEHotelRoomList(hotelId);
                var hotel_detail_sql = await hotelDetailRepository.GetById(hotelId);

                if (hotel_rooms != null && hotel_rooms.Any())
                {
                    // Thông tin khách sạn
                    result = new HotelRoomDetailModel()
                    {
                        hotel_id = hotelId.ToString(),
                        rooms = new List<RoomDetail>(),
                    };

                    //-- Tính giá về tay thông qua chính sách giá
                    int nights = Convert.ToInt32((departureDate - arrivalDate).TotalDays < 1 ? 1 : (departureDate - arrivalDate).TotalDays);
                    //var profit_list = hotelDetailRepository.GetHotelRoomPricePolicy(hotelId.ToString(), client_type_string);
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

                        var room_packages = hotelDetailRepository.GetFERoomPackageListByRoomId(room.Id, arrivalDate, departureDate);
                        var room_packages_daily = hotelDetailRepository.GetFERoomPackageDaiLyListByRoomId(room.Id, arrivalDate, departureDate);
                        var item = PricePolicyService.GetRoomDetail(room.Id.ToString(), arrivalDate, departureDate, nights, room_packages_daily, room_packages, profit_list, hotel_detail_sql,
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
                                 min_price = min_price_value,
                                 remainming_room = room.NumberOfRoom ?? 0,
                                 rates = new List<RoomDetailRate>()

                             }, (int)client_type);

                        result.rooms.Add(item);

                        #endregion

                    }
                }

            }
            catch (Exception ex)
            {
                LogService.InsertLog("HotelDataService - GetVinHotelDataFromHMS: " + ex.ToString());
            }
            return result;
        }


    }
}
