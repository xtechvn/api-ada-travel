using APP.PUSH_LOG.Functions;
using Entities.ViewModels;
using ENTITIES.ViewModels.Booking;
using ENTITIES.ViewModels.BookingFly;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REPOSITORIES.IRepositories;
using REPOSITORIES.IRepositories.Fly;
using REPOSITORIES.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using WEB.API.Service.Queue;

namespace API_CORE.Controllers.BOOKING.Fly
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : Controller
    {
        private IConfiguration configuration;
        private IFlyBookingMongoRepository bookingRepository;
        private IOrderRepository orderRepository;
        private IAccountRepository accountRepository;
        private ISaveBookingRepository saveBookingRepository;
        public BookingController(IConfiguration _configuration, IFlyBookingMongoRepository _bookingRepository, IOrderRepository _ordersRepository, IAccountRepository _accountRepository, ISaveBookingRepository _saveBookingRepository)
        {
            configuration = _configuration;
            bookingRepository = _bookingRepository;
            orderRepository = _ordersRepository;
            accountRepository=_accountRepository;
            saveBookingRepository = _saveBookingRepository;
    }

        /// <summary>
        /// Dùng để lưu booking ngoài frontend B2C
        /// Phụ trách project: Sang
        /// Data sẽ được lưu vào Mongo DB
        /// Input: là chuỗi Json do bên Project frontend truyền về. Lấy từ API Datacom
        /// </summary>
        /// <returns></returns>
        [HttpPost("flight/save-booking.json")]
        public async Task<ActionResult> saveBooking(string token)
        {

            try
            {
                LogHelper.InsertLogTelegram("save-booking,token:" + token);
                JArray objParr = null;

                #region Test
                /*
                var object_input = new
                {
                    utmmedium  =23154851,
                    account_client_id=158,
                    voucher_name="CHAOHE",
                    session_id= "DC12508SGNHAN190423100114209001",
                    booking_id= 7486284,
                    booking_data= JsonConvert.DeserializeObject<object>("{		\"BookingId\" : 7486284,		\"OrderCode\" : \"DC125087486284\",		\"ListBooking\" : [			{				\"Status\" : \"FAIL\",				\"AutoIssue\" : false,				\"Airline\" : \"VJ\",				\"BookingCode\" : \"\",				\"GdsCode\" : \"\",				\"Flight\" : \"0VJ160H1_ECOSGNHAN202304192220\",				\"Route\" : \"SGNHAN\",				\"ErrorCode\" : \"0006\",				\"ErrorMessage\" : \"Giờ khởi hành sát ngày đặt vé, không thể giữ chỗ, cần xuất vé ngay\",				\"BookingImage\" : null,				\"ExpiryDate\" : \"2023-04-19T11:42:22.4191533+07:00\",				\"ExpiryDt\" : \"19042023 11:42:22\",				\"ExpiryTime\" : 0,				\"ResponseTime\" : 0.8188573,				\"System\" : \"VJ\",				\"Price\" : 0,				\"Difference\" : 0,				\"Session\" : \"DC12508SGNHAN190423100114209\",				\"FareData\" : {					\"FareDataId\" : 0,					\"Airline\" : \"VJ\",					\"AirlineObj\" : {						\"code\" : null,						\"nameVi\" : null,						\"nameEn\" : null,						\"airLineGroup\" : null,						\"logo\" : null					},					\"Itinerary\" : 1,					\"Leg\" : 0,					\"Promo\" : false,					\"Currency\" : \"VND\",					\"System\" : \"VJ\",					\"FareType\" : \"agent\",					\"CacheAge\" : 0,					\"Availability\" : 1,					\"Adt\" : 1,					\"Chd\" : 0,					\"Inf\" : 0,					\"FareAdt\" : 1349000,					\"FareChd\" : 0,					\"FareInf\" : 0,					\"TaxAdt\" : 727900,					\"TaxChd\" : 0,					\"TaxInf\" : 0,					\"FeeAdt\" : 0,					\"FeeChd\" : 0,					\"FeeInf\" : 0,					\"ServiceFeeAdt\" : 0,					\"ServiceFeeChd\" : 0,					\"ServiceFeeInf\" : 0,					\"TotalNetPrice\" : 2076900,					\"TotalServiceFee\" : 0,					\"TotalDiscount\" : 0,					\"TotalCommission\" : 0,					\"TotalPrice\" : 2076900,					\"AdavigoPrice\" : {						\"price\" : 0,						\"amount\" : 0,						\"price_id\" : 0,						\"profit\" : 0					},					\"AdavigoPriceAdt\" : null,					\"ListFlight\" : [						{							\"FlightId\" : 0,							\"Airline\" : \"VJ\",							\"Operating\" : \"VJ\",							\"Leg\" : 0,							\"StartPoint\" : \"SGN\",							\"EndPoint\" : \"HAN\",							\"StartDate\" : \"2023-04-19T22:20:00\",							\"EndDate\" : \"2023-04-20T00:30:00\",							\"StartDt\" : \"19042023 22:20:00\",							\"EndDt\" : \"20042023 00:30:00\",							\"FlightNumber\" : \"VJ160\",							\"StopNum\" : 0,							\"HasDownStop\" : false,							\"Duration\" : 130,							\"NoRefund\" : true,							\"GroupClass\" : \"Economy\",							\"GroupClassObj\" : {								\"id\" : 0,								\"airline\" : null,								\"classCode\" : null,								\"fareType\" : null,								\"detailVi\" : null,								\"detailEn\" : null,								\"description\" : null							},							\"FareClass\" : \"H1_ECO\",							\"FareBasis\" : \"\",							\"SeatRemain\" : 1,							\"Promo\" : false,							\"FlightValue\" : \"0VJ160H1_ECOSGNHAN202304192220\",							\"ListSegment\" : [								{									\"Id\" : 0,									\"Airline\" : \"VJ\",									\"MarketingAirline\" : \"VJ\",									\"OperatingAirline\" : \"VJ\",									\"StartPoint\" : \"SGN\",									\"EndPoint\" : \"HAN\",									\"StartTime\" : \"2023-04-19T22:20:00\",									\"StartTimeZoneOffset\" : \"+07:00\",									\"EndTime\" : \"2023-04-20T00:30:00\",									\"EndTimeZoneOffset\" : \"+07:00\",									\"StartTm\" : \"19042023 22:20:00\",									\"EndTm\" : \"20042023 00:30:00\",									\"FlightNumber\" : \"VJ160\",									\"Duration\" : 130,									\"Class\" : \"H1_ECO\",									\"Cabin\" : \"Economy\",									\"FareBasis\" : null,									\"Seat\" : 1,									\"Plane\" : \"321\",									\"StartTerminal\" : null,									\"EndTerminal\" : null,									\"HasStop\" : false,									\"StopPoint\" : null,									\"StopTime\" : 0,									\"DayChange\" : true,									\"StopOvernight\" : false,									\"ChangeStation\" : false,									\"ChangeAirport\" : false,									\"LastItem\" : true,									\"HandBaggage\" : \"7kg\",									\"AllowanceBaggage\" : \"\"								}							]						}					]				},				\"ListTicket\" : null,				\"Warnings\" : null,				\"ListPassenger\" : [					{						\"Index\" : 0,						\"ParentId\" : 0,						\"FirstName\" : \"NGUYEN\",						\"LastName\" : \"VAN ANH\",						\"Type\" : \"ADT\",						\"Gender\" : true,						\"Birthday\" : \"\",						\"Nationality\" : \"\",						\"PassportNumber\" : \"\",						\"PassportExpirationDate\" : null,						\"Membership\" : \"\",						\"Wheelchair\" : false,						\"Vegetarian\" : false,						\"ListBaggage\" : [ ],						\"ListSeat\" : [ ]					}				],				\"PriceId\" : 2560,				\"Amount\" : 2131900,				\"Profit\" : 55000			}		],		\"Status\" : false,		\"ErrorCode\" : \"0006\",		\"ErrorValue\" : \"\",		\"ErrorField\" : \"\",		\"Message\" : \"VJ: Giờ khởi hành sát ngày đặt vé, không thể giữ chỗ, cần xuất vé ngay\",		\"Language\" : \"vi\"	}"),
                    booking_session= JsonConvert.DeserializeObject<object>("{		\"search\" : \"{\\\"search\\\":{\\\"StartPoint\\\":\\\"SGN\\\",\\\"EndPoint\\\":\\\"HAN\\\",\\\"StartDate\\\":\\\"19/04/2023\\\",\\\"EndDate\\\":\\\"\\\",\\\"Adt\\\":1,\\\"Child\\\":0,\\\"Baby\\\":0},\\\"go\\\":{\\\"FareDataId\\\":0,\\\"Airline\\\":\\\"VJ\\\",\\\"AirlineObj\\\":{\\\"code\\\":\\\"VJ\\\",\\\"nameVi\\\":\\\"Vietjet Air\\\",\\\"nameEn\\\":\\\"Vietjet Air\\\",\\\"airLineGroup\\\":\\\"LCC\\\",\\\"logo\\\":\\\"https://static-image.adavigo.com/uploads/images/vj.gif\\\"},\\\"Itinerary\\\":1,\\\"Leg\\\":0,\\\"Promo\\\":false,\\\"Currency\\\":\\\"VND\\\",\\\"System\\\":\\\"VJ\\\",\\\"FareType\\\":\\\"agent\\\",\\\"CacheAge\\\":0,\\\"Availability\\\":1,\\\"Adt\\\":1,\\\"Chd\\\":0,\\\"Inf\\\":0,\\\"FareAdt\\\":1349000,\\\"FareChd\\\":0,\\\"FareInf\\\":0,\\\"TaxAdt\\\":727900,\\\"TaxChd\\\":0,\\\"TaxInf\\\":0,\\\"FeeAdt\\\":0,\\\"FeeChd\\\":0,\\\"FeeInf\\\":0,\\\"ServiceFeeAdt\\\":0,\\\"ServiceFeeChd\\\":0,\\\"ServiceFeeInf\\\":0,\\\"TotalNetPrice\\\":2076900,\\\"TotalServiceFee\\\":0,\\\"TotalDiscount\\\":0,\\\"TotalCommission\\\":0,\\\"TotalPrice\\\":2076900,\\\"AdavigoPrice\\\":{\\\"price\\\":2131900,\\\"amount\\\":2131900,\\\"price_id\\\":2560,\\\"profit\\\":55000},\\\"AdavigoPriceAdt\\\":{\\\"price\\\":2131900,\\\"amount\\\":2131900,\\\"price_id\\\":2560,\\\"profit\\\":55000},\\\"ListFlight\\\":[{\\\"FlightId\\\":0,\\\"Airline\\\":\\\"VJ\\\",\\\"Operating\\\":\\\"VJ\\\",\\\"Leg\\\":0,\\\"StartPoint\\\":\\\"SGN\\\",\\\"EndPoint\\\":\\\"HAN\\\",\\\"StartDate\\\":\\\"2023-04-19T22:20:00\\\",\\\"EndDate\\\":\\\"2023-04-20T00:30:00\\\",\\\"StartDt\\\":\\\"19042023 22:20:00\\\",\\\"EndDt\\\":\\\"20042023 00:30:00\\\",\\\"FlightNumber\\\":\\\"VJ160\\\",\\\"StopNum\\\":0,\\\"HasDownStop\\\":false,\\\"Duration\\\":130,\\\"NoRefund\\\":true,\\\"GroupClass\\\":\\\"Economy\\\",\\\"GroupClassObj\\\":{\\\"id\\\":2750,\\\"airline\\\":\\\"VJ\\\",\\\"classCode\\\":\\\"*_ECO\\\",\\\"fareType\\\":\\\"Economy\\\",\\\"detailVi\\\":\\\"ECO\\\",\\\"detailEn\\\":\\\"ECO\\\",\\\"description\\\":\\\"<li  style=\\\\\\\"padding-left: 0px !important;\\\\\\\"><img src=\\\\\\\"https://static-image.adavigo.com/uploads/images/webicons/ticket.svg\\\\\\\"><span style=\\\\\\\"margin-left: 10px;\\\\\\\">Thay đổi vé mất phí + chênh lệch nếu có</span></li><li  style=\\\\\\\"padding-left: 0px !important;\\\\\\\"><img src=\\\\\\\"https://static-image.adavigo.com/uploads/images/webicons/ticket.svg\\\\\\\"><span style=\\\\\\\"margin-left: 10px;\\\\\\\">Hoàn bảo lưu định danh tên khách trước 24h so với giờ khởi hành</span></li><li  style=\\\\\\\"padding-left: 0px !important;\\\\\\\"><img src=\\\\\\\"https://static-image.adavigo.com/uploads/images/webicons/ticket2.svg\\\\\\\"><span style=\\\\\\\"margin-left: 10px; \\\\\\\">Không đổi tên</span></li>\\\"},\\\"FareClass\\\":\\\"H1_ECO\\\",\\\"FareBasis\\\":\\\"\\\",\\\"SeatRemain\\\":1,\\\"Promo\\\":false,\\\"FlightValue\\\":\\\"0VJ160H1_ECOSGNHAN202304192220\\\",\\\"ListSegment\\\":[{\\\"Id\\\":0,\\\"Airline\\\":\\\"VJ\\\",\\\"MarketingAirline\\\":\\\"VJ\\\",\\\"OperatingAirline\\\":\\\"VJ\\\",\\\"StartPoint\\\":\\\"SGN\\\",\\\"EndPoint\\\":\\\"HAN\\\",\\\"StartTime\\\":\\\"2023-04-19T22:20:00\\\",\\\"StartTimeZoneOffset\\\":\\\"+07:00\\\",\\\"EndTime\\\":\\\"2023-04-20T00:30:00\\\",\\\"EndTimeZoneOffset\\\":\\\"+07:00\\\",\\\"StartTm\\\":\\\"19042023 22:20:00\\\",\\\"EndTm\\\":\\\"20042023 00:30:00\\\",\\\"FlightNumber\\\":\\\"VJ160\\\",\\\"Duration\\\":130,\\\"Class\\\":\\\"H1_ECO\\\",\\\"Cabin\\\":\\\"Economy\\\",\\\"FareBasis\\\":null,\\\"Seat\\\":1,\\\"Plane\\\":\\\"321\\\",\\\"StartTerminal\\\":null,\\\"EndTerminal\\\":null,\\\"HasStop\\\":false,\\\"StopPoint\\\":null,\\\"StopTime\\\":0,\\\"DayChange\\\":true,\\\"StopOvernight\\\":false,\\\"ChangeStation\\\":false,\\\"ChangeAirport\\\":false,\\\"LastItem\\\":true,\\\"HandBaggage\\\":\\\"7kg\\\",\\\"AllowanceBaggage\\\":\\\"\\\"}]}],\\\"Profit\\\":55000,\\\"Amount\\\":2131900},\\\"Session\\\":\\\"DC12508SGNHAN190423100114209\\\",\\\"isTwoWayFare\\\":false,\\\"extraInfo\\\":{\\\"EndPointName\\\":\\\"Hà Nội\\\",\\\"StartPointName\\\":\\\"Hồ Chí Minh\\\",\\\"StringDateGoChoosen\\\":\\\"Thứ 4, 19 tháng 4\\\",\\\"StringDateBackChoosen\\\":\\\"Thứ 4, 19 tháng 4\\\",\\\"DurationFlyGo\\\":\\\"2h10m\\\",\\\"FirstTimeGo\\\":\\\"\\\\n                                 22\\\\n                        :\\\\n                        20\\\\n                            \\\",\\\"LastTimeGo\\\":\\\"\\\\n                                 00\\\\n                        :\\\\n                        30\\\\n                            \\\",\\\"FirstDateGo\\\":\\\"\\\\n                               19/04/2023\\\\n                            \\\",\\\"LastDateGo\\\":\\\"20/04/2023\\\"},\\\"filterFare\\\":{\\\"takeOffTime\\\":[],\\\"airlineCompany\\\":[],\\\"ticketClass\\\":[],\\\"stopNum\\\":[],\\\"minValue\\\":0,\\\"maxValue\\\":10643000}}\",		\"info\" : \"{\\\"infoContact\\\":{\\\"firstName\\\":\\\"NGUYEN\\\",\\\"lastName\\\":\\\"MINH\\\",\\\"phone\\\":\\\"0123456789\\\",\\\"email\\\":\\\"mn13795@gmail.com\\\",\\\"area\\\":\\\"+84\\\"},\\\"Passengers\\\":[{\\\"fullName\\\":\\\"nguyen van anh\\\",\\\"gender\\\":\\\"male\\\"}],\\\"Adt\\\":[{\\\"fullName\\\":\\\"nguyen van anh\\\",\\\"gender\\\":\\\"male\\\"}],\\\"Child\\\":[],\\\"Baby\\\":[]}\"	}"),
                    booking_order = JsonConvert.DeserializeObject<object>("{		\"BookType\" : \"book-normal\",		\"UseAgentContact\" : true,		\"Contact\" : {			\"Gender\" : true,			\"FirstName\" : \"NGUYEN\",			\"LastName\" : \"MINH\",			\"Area\" : \"+84\",			\"Phone\" : \"123456789\",			\"Email\" : \"mn13795@gmail.com\",			\"Address\" : \"\"		},		\"ListPassenger\" : [			{				\"Index\" : 0,				\"ParentId\" : 0,				\"FirstName\" : \"NGUYEN\",				\"LastName\" : \"VAN ANH\",				\"Type\" : \"ADT\",				\"Gender\" : true,				\"Birthday\" : \"\",				\"Nationality\" : \"\",				\"PassportNumber\" : \"\",				\"PassportExpirationDate\" : \"\",				\"Membership\" : \"\",				\"Wheelchair\" : false,				\"Vegetarian\" : false,				\"ListBaggage\" : [ ]			}		],		\"ListFareData\" : [			{				\"Session\" : \"DC12508SGNHAN190423100114209\",				\"FareDataId\" : 0,				\"ListFlight\" : [					{						\"FlightValue\" : \"0VJ160H1_ECOSGNHAN202304192220\",						\"Leg\" : 0					}				]			}		],		\"AccountCode\" : \"\",		\"Remark\" : \"\",		\"BookingDate\" : \"2023-04-19T04:42:21.961Z\",		\"HeaderUser\" : \"datacom\",		\"HeaderPass\" : \"dtc@19860312\",		\"AgentAccount\" : \"DC12508\",		\"AgentPassword\" : \"3fdhyswe\",		\"ProductKey\" : \"jl8rhr7dlqjn8et\",		\"Currency\" : \"VND\",		\"Language\" : \"vi\",		\"IpRequest\" : \"\"	}"),
                };
                string text = JsonConvert.SerializeObject(object_input);
                token = CommonHelper.Encode(text, configuration["DataBaseConfig:key_api:b2c"]);*/
                #endregion

                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    var booking_data = JObject.Parse(objParr[0]["booking_data"].ToString());

                    var data = new BookingFlyMongoDbModel()
                    {
                        //account_client_id = (int)objParr[0]["client_id"],
                        //utmmedium = objParr[0]["utmmedium"].ToString(),
                        //utm_source = objParr[0]["utm_source"].ToString(),
                        //booking_data = JsonConvert.DeserializeObject<BookingFlyData>(objParr[0]["booking_data"].ToString()),
                        //booking_order = JsonConvert.DeserializeObject<BookingFlyOrder>(objParr[0]["booking_order"].ToString()),
                        //booking_session= JsonConvert.DeserializeObject<BookingFlySessionString>(objParr[0]["booking_session"].ToString()),
                        //booking_id = Convert.ToInt32(booking_data["BookingId"]),
                        //create_date = DateTime.Now,
                        //session_id = (string)booking_data.SelectToken("ListBooking[0].Session"),
                        //voucher_name= objParr[0]["voucher_name"]!=null? objParr[0]["voucher_name"].ToString(): ""
                    };
                    data.account_client_id = (int)objParr[0]["client_id"];
                    data.utmmedium = objParr[0]["utmmedium"].ToString();
                    data.utm_source = objParr[0]["utm_source"].ToString();
                    data.booking_data = JsonConvert.DeserializeObject<BookingFlyData>(objParr[0]["booking_data"].ToString());
                    data.booking_order = JsonConvert.DeserializeObject<BookingFlyOrder>(objParr[0]["booking_order"].ToString());
                    data.booking_session = JsonConvert.DeserializeObject<BookingFlySessionString>(objParr[0]["booking_session"].ToString());
                    data.booking_id = Convert.ToInt32(booking_data["BookingId"]);
                    data.create_date = DateTime.Now;
                    data.session_id = (string)booking_data.SelectToken("ListBooking[0].Session");
                    data.voucher_name = objParr[0]["voucher_name"] != null ? objParr[0]["voucher_name"].ToString() : "";

                    string session = (string)booking_data.SelectToken("ListBooking[0].Session");
                    var list_session_id = Array.ConvertAll(session.Split(','), s => (s).ToString());
                    var datalist =await bookingRepository.getBookingBySessionId(list_session_id,data.account_client_id);
                    if (datalist.Count != 0) 
                    {
                        var delete = bookingRepository.DeleteBookingBySessionId(data);
                        var create = bookingRepository.saveBooking(data);
                        if (create != null)
                        {

                            MongoDBSMSAccess.InsertLogMongoDb(configuration, JsonConvert.SerializeObject(data), data.session_id);
                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                msg = "Thêm mới thành công",
                                data = data
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
                    var result = bookingRepository.saveBooking(data);
                    if (result != null)
                    {

                        MongoDBSMSAccess.InsertLogMongoDb(configuration, JsonConvert.SerializeObject(data), data.session_id);
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Thêm mới thành công",
                            data = data
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
               
                LogHelper.InsertLogTelegram("saveBooking - BookingControl: " + ex + ";Token: "+token);
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "ERROR!",
                    
                });
            }
        }

        [HttpPost("flight/get-booking-by-booking-id.json")]
        public async Task<ActionResult> getBookingByBookingId(string token)
        {
           
            try
            {
                JArray objParr = null;
                #region Test
                //var booking_id = new List<int> { 5905999, -1 };
                //var j_param = new Dictionary<string, List<int>>
                //{
                //    {"booking_id", booking_id},
                //    {"client_id", 123}

                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion

                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    List<int> data_booking_id = JsonConvert.DeserializeObject<List<int>>(objParr[0]["booking_id"].ToString());
                    int client_id = Convert.ToInt32(objParr[0]["client_id"]);
                    //int client_id = Convert.ToInt32(13333);
                    var result = await bookingRepository.getBookingByBookingIdAsync(data_booking_id, client_id);

                    if (result != null)
                    {

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
                LogHelper.InsertLogTelegram("getBookingByBookingId - BookingControl: token "+token+"\n " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "ERROR!",
                });
            }
        }

        [HttpPost("flight/get-booking-by-session-id.json")]
        public async Task<ActionResult> getBookingBySessionId(string token, int source_booking_type )
        {
       
            try
            {
                JArray objParr = null;
                #region Test
                //var j_param = new Dictionary<string, string>
                //{
                //    {"session_id", "DC12508SGNHAN190423100114209001"},
                //    {"client_id", "158"},
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion

                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    string session_id = objParr[0]["session_id"].ToString();
                    var list_session_id = Array.ConvertAll(session_id.Split(','), s => (s).ToString());
                   
                    int account_client_id = Convert.ToInt32(objParr[0]["client_id"]);
                   // var account_client = await accountRepository.GetByClientId(account_client_id);
                   // var AccountclientId = account_client != null ? account_client.Id : account_client_id;
                    if (source_booking_type !=(int) SourceBoongkingType.Sql)
                    {
                        var result = await bookingRepository.getBookingBySessionId(list_session_id, account_client_id);
                        if (result.Count > 0 && result != null)
                        {

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
                        var result = await orderRepository.getOrderDetailBySessionId(session_id);
                        if (result.Count > 0 && result != null)
                        {

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
                LogHelper.InsertLogTelegram("getBookingBySessionId - BookingControl: token+" + token + "\n " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "ERROR!",
                });
            }
        }
        [HttpPost("flight/save-booking-ada.json")]
        public async Task<ActionResult> saveBookingAda(string token)
        {
            try
            {
                JArray objParr = null;
                #region Test

                //string text = "{\"order\":{\"orderCode\":\"ADV557452505300036\",\"transactionTime\":\"30/05-13:03\",\"totalPrice\":\"6.456.500\",\"success\":true,\"message\":\"success\",\"remark\":\"\",\"numberOfAdult\":1,\"numberOfChild\":1,\"numberOfInfant\":0,\"companyName\":\"\",\"companyAddress\":\",HuyệnVũThư,TháiBình\",\"taxCode\":\"\",\"invoiceEmail\":\"\",\"paymentType\":\"Thanhtoántạivănphòng\",\"customerName\":\"NguyễnThanhTuyền\",\"customerEmail\":\"thanhtuyen0171@gmail.com\",\"customerPhone\":\"0933428416\"},\"passengers\":[{\"paxName\":\"NGUYEN/VANA/MRS\",\"birthday\":\"01/01/1972\",\"loyalty\":\"\",\"type\":\"ADT\",\"baggages\":[{\"segment\":\"VCSSGN\",\"weight\":\"20\",\"price\":\"216000\"}]}],\"bookings\":[{\"pnr\":\"6VIRBK\",\"recordStatus\":\"HOLD\",\"timeLimit\":\"31/05/202501:18\",\"airline\":\"VN\",\"purchaseTimeLimit\":\"31/05/202513:18\",\"routeInfos\":[{\"routeID\":\"1\",\"from\":\"VCS\",\"to\":\"SGN\",\"departDate\":\"07-06-2025\",\"timeFrom\":\"07:30\",\"timeTo\":\"08:40\",\"airCraft\":\"VN\",\"flightNo\":\"1854\",\"class\":\"HPXVNF(Economy)\",\"aircraftType\":\"ATR\",\"cabinClass\":\"Economy\",\"flightTime\":\"01:10\",\"freeBaggage\":{\"pieces\":1,\"description\":\"23KG\"},\"handBaggage\":{\"pieces\":1,\"description\":\"10KG\"}},{\"routeID\":\"1\",\"from\":\"SGN\",\"to\":\"VCS\",\"departDate\":\"08-06-2025\",\"timeFrom\":\"07:30\",\"timeTo\":\"08:40\",\"airCraft\":\"VN\",\"flightNo\":\"1854\",\"class\":\"HPXVNF(Economy)\",\"aircraftType\":\"ATR\",\"cabinClass\":\"Economy\",\"flightTime\":\"01:10\",\"freeBaggage\":{\"pieces\":1,\"description\":\"23KG\"},\"handBaggage\":{\"pieces\":1,\"description\":\"10KG\"}}],\"fareData\":{\"fareADT\":1039000,\"taxADT\":529000,\"vatADT\":84000,\"fareCHD\":936000,\"taxCHD\":489500,\"vatCHD\":75000,\"fareINF\":0,\"taxINF\":0,\"vatINF\":0,\"otherFee\":0,\"totalPrice\":6456500},\"system\":\"VN\"}]}";
                //var data_product = JsonConvert.SerializeObject(text);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2c"]);
                #endregion

                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2c"]))
                {
                    LogHelper.InsertLogTelegram("token:" + token );
                    var model = JsonConvert.DeserializeObject<BookingFlyMua_Di>(objParr[0].ToString());
                    var data = await saveBookingRepository.saveBookingAda(model);
                        if(data > 0)
                        return Ok(new
                          {
                              status = (int)ResponseType.SUCCESS,
                              msg = "thêm mới thành công!",
                            data = data
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
                LogHelper.InsertLogTelegram("saveBookingAda - BookingControl: token+" + token + "\n " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "ERROR!",
                });
            }
            return Ok(new
            {
                status = (int)ResponseType.ERROR,
                msg = "thêm mới không thành công!"
            });
        }
    }
}
