using API_CORE.Service.Vin;
using APP.PUSH_LOG.Functions;
using Caching.RedisWorker;
using ENTITIES.ViewModels.Booking;
using ENTITIES.ViewModels.MongoDb;
using ENTITIES.ViewModels.Order;
using ENTITIES.ViewModels.VinWonder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Repositories.IRepositories;
using REPOSITORIES.IRepositories;
using REPOSITORIES.IRepositories.VinWonder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace API_CORE.Controllers.B2C
{
    [Route("api/b2c/vinwonder")]
    [ApiController]
    public class VinWonderB2CController : ControllerBase
    {
        private readonly IPlayGroundDetailRepository _playGroundDetailRepository;
        private readonly IVinWonderBookingRepository _vinWonderBookingRepository;
        public IConfiguration configuration;
        private IAccountRepository _accountRepository;
        private IOrderRepository _ordersRepository;
        private IOtherBookingRepository _otherBookingRepository;
        private readonly RedisConn _redisService;
        public VinWonderB2CController(IConfiguration config, IPlayGroundDetailRepository playGroundDetailRepository, RedisConn redisService, IVinWonderBookingRepository vinWonderBookingRepository, IAccountRepository accountRepository, IOrderRepository ordersRepository,
            IOtherBookingRepository otherBookingRepository)
        {
            configuration = config;
            _playGroundDetailRepository = playGroundDetailRepository;
            _redisService = redisService;
            _vinWonderBookingRepository = vinWonderBookingRepository;
            _accountRepository = accountRepository;
            _ordersRepository = ordersRepository;
            _otherBookingRepository = otherBookingRepository;
        }
        /// <summary>
        /// Lấy ra bài viết vin wwonder theo mã địa điểm
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("get-detail-by-location-code.json")]
        public async Task<ActionResult> GetDetailByLocationCode(string token)
        {
            try
            {
                // string j_param = "{'code':'5'}";
                // token = CommonHelper.Encode(j_param, configuration["DataBaseConfig:key_api:b2c"]);
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    string db_type = string.Empty;
                    string location_code = objParr[0]["code"].ToString();
                    string cache_name = CacheType.PLAYGROUND_DETAIL + location_code;
                    var j_data = await _redisService.GetAsync(cache_name, Convert.ToInt32(configuration["Redis:Database:db_common"]));
                    var news_detail = new VinWonderPlayGroundViewModel();

                    if (j_data != null)
                    {
                        news_detail = JsonConvert.DeserializeObject<VinWonderPlayGroundViewModel>(j_data);
                        db_type = "cache";
                    }
                    else
                    {
                        news_detail = _playGroundDetailRepository.GetPlayGroundDetailByLocationCode(location_code, (int)ServicesType.VinWonderTicket, configuration["config_value:ImageStatic"]);

                        if (news_detail != null && news_detail.title != null && news_detail.title.Trim() != "")
                        {

                            _redisService.Set(cache_name, JsonConvert.SerializeObject(news_detail), Convert.ToInt32(configuration["Redis:Database:db_common"]));
                        }
                        db_type = "database";
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        data = news_detail,
                        location_code = location_code,
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
                LogHelper.InsertLogTelegram("VinWonderB2CController - GetDetailByLocationCode: " + ex + "\n Token: " + token);
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Error: " + ex.ToString(),
                });
            }
        }

        [HttpPost("get-vinwonder-by-account-client")]
        public async Task<ActionResult> GetVinWonderByAccountClientId(string token)
        {
            try
            {
                #region Test
                //var j_param = new Dictionary<string, object>
                //{
                //    {"account_clientid", "155"},
                //    {"pageindex", "1"},
                //    {"pagesize", "10"},
                //    {"keyword", ""},
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {

                    long AccountClientId = (long)objParr[0]["account_clientid"];
                    long PageIndex = (long)objParr[0]["pageindex"];
                    long PageSize = (long)objParr[0]["pagesize"];
                    string keyword = objParr[0]["keyword"].ToString();


                    int db_index = Convert.ToInt32(configuration["Redis:Database:db_core"]);
                    string cache_name = CacheName.ORDER_VINWONDER_ACCOUNTID + AccountClientId;
                    var obj_lst_vinWonder = new List<ListVinWonderViewModel>();
                    var strDataCache = _redisService.Get(cache_name, db_index);
                    // Kiểm tra có data trong cache ko
                    if (!string.IsNullOrEmpty(strDataCache))
                        // nếu có trả ra luôn object 
                        obj_lst_vinWonder = JsonConvert.DeserializeObject<List<ListVinWonderViewModel>>(strDataCache);
                    else
                    {
                        // nếu chưa thì vào db lấy
                        var data = await _vinWonderBookingRepository.GetVinWonderByAccountClientId(AccountClientId, PageIndex, PageSize, keyword);
                        obj_lst_vinWonder = data;
                        if (data != null)
                        {
                            _redisService.Set(cache_name, JsonConvert.SerializeObject(data), db_index);

                        }
                        else
                        {
                            LogHelper.InsertLogTelegram("Không lấy được danh sách vinWonder theo mã. AccountId: " + AccountClientId);
                            return Ok(new { status = (int)ResponseType.ERROR, msg = "Không lấy được danh sách vinWonder theo mã. AccountId: " + AccountClientId });
                        }
                        // Kiem tra db co data khong

                    }
                    PageIndex = PageIndex == 1 ? 0 : PageIndex - 1;
                    obj_lst_vinWonder = obj_lst_vinWonder != null ? obj_lst_vinWonder.Skip((int)(PageIndex * PageSize)).Take((int)PageSize).ToList() : null;
                    var total = obj_lst_vinWonder != null ? obj_lst_vinWonder.Count : 0;

                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Successfully ",
                        data = obj_lst_vinWonder,
                        total = total
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key ko hợp lệ"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("VinWonderB2CController - GetDetailByLocationCode: " + ex + "\n Token: " + token);
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Error: " + ex.ToString(),
                });
            }
        }

        /// <summary>
        /// Dùng để lưu booking ngoài frontend B2C
        /// Phụ trách project: Sang
        /// Data sẽ được lưu vào Mongo DB
        /// Input: là chuỗi Json do bên Project frontend truyền về.
        /// </summary>
        /// <returns></returns>
        [HttpPost("save-booking.json")]
        public async Task<ActionResult> saveBooking(string token)
        {

            try
            {
                // LogHelper.InsertLogTelegram("save-booking-vinwonder,token:" + token);
                JArray objParr = null;

                #region Test
                // string json = "{ \"data\": { \"cart\": [ [ { \"Key\": \"VKATC-THUYCUNG\", \"Name\": \"Vé vào cửa Thủy Cung\", \"Date\": \"14/07/2023\", \"SiteName\": \"VinKE & Aquarium Times City \", \"SiteCode\": 1, \"TotalPrice\": 120000, \"Number\": 3, \"Items\": [ { \"Code\": 1, \"ServiceKey\": \"VKATC-THUYCUNG-NL\", \"ShortName\": \"Thủy Cung NL\", \"Name\": \"Vé vào cửa Thủy Cung dành cho Dịch vụ người lớn\", \"Description\": null, \"SessionKey\": null, \"GroupCode\": \"VVC\", \"GroupName\": \"Vé vào cửa\", \"TypeCode\": \"NL\", \"TypeName\": \"Người lớn\", \"BasePrice\": 120000, \"Price\": 120000, \"TotalPromotionDiscountPrice\": 0, \"TotalPrice\": 0, \"RateDiscountPercent\": 0, \"RateDiscountPrice\": 0, \"PromotionDiscountPercent\": 0, \"PromotionDiscountPrice\": 0, \"VATPercent\": 10, \"Availability\": -1, \"NumberOfUses\": 1, \"Slots\": -1, \"DateTimeUsed\": { \"DateFrom\": \"14/07/2023\", \"DateTo\": \"14/07/2023\", \"TimeStart\": 0, \"MinuteStart\": 0, \"TimeEnd\": 23, \"MinuteEnd\": 59, \"GateCode\": \"\", \"GateName\": \"\", \"WeekDays\": \"Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday,\", \"DateUsed\": 0, \"NumberOfUses\": null }, \"Promotions\": [], \"PromotionServiceInfo\": null, \"Transaction\": { \"Code\": null, \"Name\": null, \"TypeCode\": null, \"TypeName\": null }, \"TransactionClassification\": null, \"CardCode\": 3, \"IsTicketBonus\": false, \"IsRevenueBonus\": false, \"OriginalPrice\": 0, \"OutletsIds\": null, \"SapServiceCode\": null, \"RetailItemType\": null, \"ServiceSaleForm\": null, \"RevenueGroup\": null, \"RevenueClassificationCode\": null, \"RevenueProperty\": null, \"RateGroup\": null, \"TimeSlot\": null, \"ExpiredDate\": null, \"SessionId\": null, \"ServiceWarehouseId\": null, \"RateForm\": null, \"ComboDetails\": [], \"Outlets\": null } ], \"IsShow\": true, \"RateCode\": \"125\", \"PeopleNumObj\": { \"adt\": 1, \"child\": 0, \"old\": 0 }, \"PeopleString\": \"1 người lớn\" }, { \"Key\": \"VKATC-VINKE\", \"Name\": \"Vé vào cửa VinKE\", \"Date\": \"14/07/2023\", \"SiteName\": \"VinKE & Aquarium Times City \", \"SiteCode\": 1, \"TotalPrice\": 280000, \"Number\": 1, \"Items\": [ { \"Code\": 22, \"ServiceKey\": \"VKATC-VINKE-NL\", \"ShortName\": \"VinKE NL\", \"Name\": \"Vé vào cửa VinKE dành cho Dịch vụ người lớn\", \"Description\": null, \"SessionKey\": null, \"GroupCode\": \"VVC\", \"GroupName\": \"Vé vào cửa\", \"TypeCode\": \"NL\", \"TypeName\": \"Người lớn\", \"BasePrice\": 200000, \"Price\": 140000, \"TotalPromotionDiscountPrice\": 0, \"TotalPrice\": 280000, \"RateDiscountPercent\": 30, \"RateDiscountPrice\": 0, \"PromotionDiscountPercent\": 0, \"PromotionDiscountPrice\": 0, \"VATPercent\": 10, \"Availability\": 2, \"NumberOfUses\": 1, \"Slots\": 0, \"DateTimeUsed\": { \"DateFrom\": \"14/07/2023\", \"DateTo\": \"14/07/2023\", \"TimeStart\": 0, \"MinuteStart\": 0, \"TimeEnd\": 23, \"MinuteEnd\": 59, \"GateCode\": \"\", \"GateName\": \"\", \"WeekDays\": \"Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday,\", \"DateUsed\": 0, \"NumberOfUses\": null }, \"Promotions\": [], \"PromotionServiceInfo\": null, \"Transaction\": { \"Code\": \"1000\", \"Name\": \"Doanh thu bán vé \", \"TypeCode\": \"Revenue\", \"TypeName\": \"Doanh Thu\" }, \"TransactionClassification\": null, \"CardCode\": 3, \"IsTicketBonus\": false, \"IsRevenueBonus\": false, \"OriginalPrice\": 0, \"OutletsIds\": null, \"SapServiceCode\": \"000000000074000547\", \"RetailItemType\": \"VC01\", \"ServiceSaleForm\": \"2\", \"RevenueGroup\": \"1\", \"RevenueClassificationCode\": \"\", \"RevenueProperty\": \"VWTC\", \"RateGroup\": \"\", \"TimeSlot\": null, \"ExpiredDate\": null, \"SessionId\": null, \"ServiceWarehouseId\": null, \"RateForm\": { \"Id\": 9, \"Name\": \"Bảng giá thông thường\", \"Code\": \"THONGTHUONG\t\", \"IsActive\": true }, \"ComboDetails\": [], \"Outlets\": null } ], \"IsShow\": true, \"RateCode\": \"162\", \"PeopleNumObj\": { \"adt\": 2, \"child\": 0, \"old\": 0 }, \"PeopleString\": \"2 người lớn\" }, { \"Key\": \"VKATC-VINKE\", \"Name\": \"Vé vào cửa VinKE\", \"Date\": \"12/07/2023\", \"SiteName\": \"VinKE & Aquarium Times City \", \"SiteCode\": 1, \"TotalPrice\": 280000, \"Number\": 1, \"Items\": [ { \"Code\": 22, \"ServiceKey\": \"VKATC-VINKE-NL\", \"ShortName\": \"VinKE NL\", \"Name\": \"Vé vào cửa VinKE dành cho Dịch vụ người lớn\", \"Description\": null, \"SessionKey\": null, \"GroupCode\": \"VVC\", \"GroupName\": \"Vé vào cửa\", \"TypeCode\": \"NL\", \"TypeName\": \"Người lớn\", \"BasePrice\": 200000, \"Price\": 140000, \"TotalPromotionDiscountPrice\": 0, \"TotalPrice\": 280000, \"RateDiscountPercent\": 30, \"RateDiscountPrice\": 0, \"PromotionDiscountPercent\": 0, \"PromotionDiscountPrice\": 0, \"VATPercent\": 10, \"Availability\": 2, \"NumberOfUses\": 1, \"Slots\": 0, \"DateTimeUsed\": { \"DateFrom\": \"12/07/2023\", \"DateTo\": \"12/07/2023\", \"TimeStart\": 0, \"MinuteStart\": 0, \"TimeEnd\": 23, \"MinuteEnd\": 59, \"GateCode\": \"\", \"GateName\": \"\", \"WeekDays\": \"Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday,\", \"DateUsed\": 0, \"NumberOfUses\": null }, \"Promotions\": [], \"PromotionServiceInfo\": null, \"Transaction\": { \"Code\": \"1000\", \"Name\": \"Doanh thu bán vé \", \"TypeCode\": \"Revenue\", \"TypeName\": \"Doanh Thu\" }, \"TransactionClassification\": null, \"CardCode\": 3, \"IsTicketBonus\": false, \"IsRevenueBonus\": false, \"OriginalPrice\": 0, \"OutletsIds\": null, \"SapServiceCode\": \"000000000074000547\", \"RetailItemType\": \"VC01\", \"ServiceSaleForm\": \"2\", \"RevenueGroup\": \"1\", \"RevenueClassificationCode\": \"\", \"RevenueProperty\": \"VWTC\", \"RateGroup\": \"\", \"TimeSlot\": null, \"ExpiredDate\": null, \"SessionId\": null, \"ServiceWarehouseId\": null, \"RateForm\": { \"Id\": 9, \"Name\": \"Bảng giá thông thường\", \"Code\": \"THONGTHUONG\t\", \"IsActive\": true }, \"ComboDetails\": [], \"Outlets\": null } ], \"IsShow\": true, \"RateCode\": \"162\", \"PeopleNumObj\": { \"adt\": 2, \"child\": 0, \"old\": 0 }, \"PeopleString\": \"2 người lớn\" } ] ], \"infoContact\": { \"firstName\": \"ANH\", \"lastName\": \"HIEU 992\", \"phone\": \"0857290966\", \"email\": \"anhhieuk51@gmail.com\", \"area\": \"+84\", \"isUserBook\": 1, \"userRequest\": \"\" } }, \"requestVin\": [ { \"Channelcode\": \"OTA\", \"Date\": \"14/07/2023\", \"BookingCode\": \"BookingCodeAdavigo\", \"InvoiceCode\": \"InvoiceCodeAdavigo\", \"ReservationCode\": \"\", \"PromotionCode\": \"\", \"SiteCode\": 1, \"Services\": [ { \"ServiceCode\": 1, \"Number\": 3, \"RateCode\": 125 }, { \"ServiceCode\": 22, \"Number\": 2, \"RateCode\": 162 } ], \"Customer\": [ { \"FullName\": \"HIEU 992 ANH\", \"Email\": \"anhhieuk51@gmail.com\", \"Phone\": \"+84857290966\", \"DateOfBirth\": \"\", \"Sex\": null, \"IdentityType\": null, \"IdentityDetail\": null, \"ZonePickupId\": null, \"ZoneReleaseId\": null, \"NationalityID\": null, \"PlaceOfBirth\": null } ] }, { \"Channelcode\": \"OTA\", \"Date\": \"12/07/2023\", \"BookingCode\": \"BookingCodeAdavigo\", \"InvoiceCode\": \"InvoiceCodeAdavigo\", \"ReservationCode\": \"\", \"PromotionCode\": \"\", \"SiteCode\": 1, \"Services\": [ { \"ServiceCode\": 22, \"Number\": 2, \"RateCode\": 162 } ], \"Customer\": [ { \"FullName\": \"HIEU 992 ANH\", \"Email\": \"anhhieuk51@gmail.com\", \"Phone\": \"+84857290966\", \"DateOfBirth\": \"\", \"Sex\": null, \"IdentityType\": null, \"IdentityDetail\": null, \"ZonePickupId\": null, \"ZoneReleaseId\": null, \"NationalityID\": null, \"PlaceOfBirth\": null } ] }, { \"Channelcode\": \"OTA\", \"Date\": \"05/07/2023\", \"BookingCode\": \"BookingCodeAdavigo\", \"InvoiceCode\": \"InvoiceCodeAdavigo\", \"ReservationCode\": \"\", \"PromotionCode\": \"\", \"SiteCode\": 2, \"Services\": [ { \"ServiceCode\": 393, \"Number\": 1, \"RateCode\": 196 }, { \"ServiceCode\": 1278, \"Number\": 1, \"RateCode\": 328 } ], \"Customer\": [ { \"FullName\": \"HIEU 992 ANH\", \"Email\": \"anhhieuk51@gmail.com\", \"Phone\": \"+84857290966\", \"DateOfBirth\": \"\", \"Sex\": null, \"IdentityType\": null, \"IdentityDetail\": null, \"ZonePickupId\": null, \"ZoneReleaseId\": null, \"NationalityID\": null, \"PlaceOfBirth\": null } ] }, { \"Channelcode\": \"OTA\", \"Date\": \"23/07/2023\", \"BookingCode\": \"BookingCodeAdavigo\", \"InvoiceCode\": \"InvoiceCodeAdavigo\", \"ReservationCode\": \"\", \"PromotionCode\": \"\", \"SiteCode\": 2, \"Services\": [ { \"ServiceCode\": 180, \"Number\": 2, \"RateCode\": 161 } ], \"Customer\": [ { \"FullName\": \"HIEU 992 ANH\", \"Email\": \"anhhieuk51@gmail.com\", \"Phone\": \"+84857290966\", \"DateOfBirth\": \"\", \"Sex\": null, \"IdentityType\": null, \"IdentityDetail\": null, \"ZonePickupId\": null, \"ZoneReleaseId\": null, \"NationalityID\": null, \"PlaceOfBirth\": null } ] } ],\"client_id\":\"182\",\"voucher_name\":\"\", }";
                // token = CommonHelper.Encode(json, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion

                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    var booking_data = JObject.Parse(objParr[0].ToString());
                    MongoDBSMSAccess.InsertLogMongoDb(configuration, JsonConvert.SerializeObject(booking_data), "SaveBookingVinWonder");

                    var data = new BookingVinWonderMongoDbModel()
                    {
                        account_client_id = (int)objParr[0]["client_id"],
                        data = JsonConvert.DeserializeObject<VinWonderBookingB2CData>(objParr[0]["data"].ToString()),
                        is_checkout = 0,
                        requestVin = JsonConvert.DeserializeObject<List<VinWonderBookingB2Request>>(objParr[0]["requestVin"].ToString()),
                        create_date = DateTime.Now,
                        voucher_name = objParr[0]["voucher_name"] != null && objParr[0]["voucher_name"].ToString().Trim() != "" ? objParr[0]["voucher_name"].ToString() : "",
                    };
                    var result = _vinWonderBookingRepository.saveBooking(data);
                    MongoDBSMSAccess.InsertLogMongoDb(configuration, JsonConvert.SerializeObject(data), result);
                    if (result != null)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Thêm mới thành công",
                            data = result
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "Thêm mới không thành công",
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

                LogHelper.InsertLogTelegram("saveBooking - VinWonderB2CController: " + ex + ";Token: " + token);
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "ERROR!",

                });
            }
        }
        [HttpPost("get-booking-by-id.json")]
        public async Task<ActionResult> getVinWonderBookingBySessionId(string token)
        {

            try
            {
                JArray objParr = null;
                #region Test
                var j_param = new Dictionary<string, string>
                {
                    {"session_id", "64cc6a9905785d88c3fc7ccb"},
                };
                var data_product = JsonConvert.SerializeObject(j_param);
                // token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion

                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    string session_id = objParr[0]["session_id"].ToString();
                    var session_list = session_id.Split(",");
                    var result = _vinWonderBookingRepository.GetBookingById(session_list);

                    if (result != null && result.Count > 0)
                    {
                        string s = JsonConvert.SerializeObject(result);
                        var settings = new JsonSerializerSettings
                        {
                            ContractResolver = new DefaultContractResolver
                            {
                                NamingStrategy = new DefaultNamingStrategy()
                            }
                        };
                        var jsonObject = JsonConvert.DeserializeObject<JArray>(s, settings);
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Thành công",
                            data = jsonObject
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "không thành công",

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
                LogHelper.InsertLogTelegram("getVinWonderBookingBySessionId - VinWonderB2CController: token+" + token + "\n " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "ERROR!",
                });
            }
        }

        [HttpPost("get-price.json")]
        public async Task<ActionResult> getPrice(string token)
        {
            try
            {
                #region Test                      

                var list = new List<Dictionary<string, string>>();
                Dictionary<string, string> dictionary1 = new Dictionary<string, string>
                {
                     {"rate_code","1" },
                     {"service_id","2,1" }
                };
                list.Add(dictionary1);
                var dictionary2 = new Dictionary<string, string>
                {
                    {"rate_code","159" },
                    {"service_id","536,kk0" }
                };
                list.Add(dictionary2);

                var data_product = JsonConvert.SerializeObject(list);
                // token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion.

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    var price_list = new List<PriceVinWonderViewModels>();
                    JArray outerJArray = JArray.Parse(objParr.ToString()); // JsonConvert.DeserializeObject<List<dynamic>>(objParr.ToString());
                    foreach (JArray innerJArray in outerJArray)
                    {
                        foreach (JObject jObject in innerJArray)
                        {
                            string rate_code = jObject["rate_code"].ToString();
                            string service_ids = jObject["service_id"].ToString();
                            var prices = await _vinWonderBookingRepository.getVinWonderPricePolicyByServiceId(rate_code, service_ids);
                            if (prices == null) continue;
                            price_list = price_list.Union(prices).ToList();
                        }
                    }
                    var group_prices = price_list.GroupBy(p => p.rate_code)
                                    .Select(group => new
                                    {
                                        rate_code = group.Key,
                                        prices = group.Select(p => new { p.service_id, p.profit, p.unit_type,p.id,p.baseprice }).ToList()
                                    });
                    return Ok(new { status = price_list.Count > 0 ? ((int)ResponseType.SUCCESS) : ((int)ResponseType.EMPTY), data = group_prices });
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
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "error:vinwonderB2c - getPrice " + ex.ToString() });
            }
        }
        [HttpPost("get-order-detail.json")]
        public async Task<ActionResult> getOrderDetail(string token)
        {

            try
            {
                #region Test
                var j_param = new Dictionary<string, object>
                {
                    {"order_id", "12211"},
                    {"client_id", "173"},
                };
                var data_product = JsonConvert.SerializeObject(j_param);

                // token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    //-- Convert AccountClientId to ClientId
                    long account_client_id = (long)objParr[0]["client_id"];
                    var account_client = await _accountRepository.GetAccountClient(account_client_id);
                    long client_id = (long)account_client.ClientId;

                    long order_id = (long)objParr[0]["order_id"];


                    int db_index = Convert.ToInt32(configuration["Redis:Database:db_core"]);
                    string cache_name = CacheName.ORDER_VINWONDER_ORDERID + order_id;
                    var obj_vinWonder = new OrderVinWonderDetailViewModel();
                    var strDataCache = _redisService.Get(cache_name, db_index);
                    // Kiểm tra có data trong cache ko
                    if (!string.IsNullOrEmpty(strDataCache))
                        // nếu có trả ra luôn object 
                        obj_vinWonder = JsonConvert.DeserializeObject<OrderVinWonderDetailViewModel>(strDataCache);
                    else
                    {
                        // nếu chưa thì vào db lấy
                        var data = await _ordersRepository.getOrderVinWonderDetail(order_id, client_id);
                        if (data != null && data.Count > 0) obj_vinWonder = data[0];
                        if (obj_vinWonder != null)
                        {
                            _redisService.Set(cache_name, JsonConvert.SerializeObject(obj_vinWonder), db_index);

                        }
                        else
                        {
                            LogHelper.InsertLogTelegram("Không lấy được danh sách vinWonder theo mã. Orderid: " + order_id);
                            return Ok(new { status = (int)ResponseType.ERROR, msg = "Không lấy được danh sách vinWonder theo mã. Orderid: " + order_id });
                        }
                        // Kiem tra db co data khong

                    }

                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Successfully ",
                        data = obj_vinWonder,
                    });

                }
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "ERROR "
                });
            }
            catch (Exception ex)
            {

                LogHelper.InsertLogTelegram("getOrderDetail- OrderController- order/get-order-detail.json: " + ex.ToString());
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "Error On Excution. Vui lòng liên hệ IT" });
            }
        }
        [HttpPost("get-booking-by-booking-id.json")]
        public async Task<ActionResult> getBookingVinWonderByBookingId(string token)
        {

            try
            {
                JArray objParr = null;
                #region Test

                var j_param = new Dictionary<string, string>
                {
                    {"booking_id", "64cbc084a14762d95a8eac98"},


                };
                var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion

                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {

                    var booking_id = objParr[0]["booking_id"].ToString();
                    string cache_name = CacheName.BOOKING_VINWONDER_ + booking_id;
                    int db_index = Convert.ToInt32(configuration["Redis:Database:db_core"]);
                    //int client_id = Convert.ToInt32(13333);
                    var result = await _vinWonderBookingRepository.GetVinWonderByBookingId(booking_id);

                    if (result != null)
                    {
                        _redisService.Set(cache_name, JsonConvert.SerializeObject(result), db_index);

                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Thành công",
                            data = result
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "không thành công",

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
                LogHelper.InsertLogTelegram("getBookingVinWonderByBookingId - VinWonderB2CController: token " + token + "\n " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "ERROR!",
                });
            }
        }


    }
}
