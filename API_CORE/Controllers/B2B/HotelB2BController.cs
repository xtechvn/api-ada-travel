using API_CORE.Service.Log;
using API_CORE.Service.Price;
using API_CORE.Service.Vin;
using Caching.Elasticsearch;
using Caching.RedisWorker;
using ENTITIES.Models;
using ENTITIES.ViewModels;
using ENTITIES.ViewModels.Hotel;
using ENTITIES.ViewModels.Hotels;
using ENTITIES.ViewModels.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repositories.IRepositories;
using REPOSITORIES.IRepositories;
using REPOSITORIES.IRepositories.Elasticsearch;
using REPOSITORIES.IRepositories.Hotel;
using REPOSITORIES.Repositories;
using REPOSITORIES.Repositories.Hotel;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using static iTextSharp.text.pdf.PdfDiv;
using ClientType = Utilities.Contants.ClientType;

namespace API_CORE.Controllers.B2B
{
    [Route("api/b2b/hotel")]
    [ApiController]
    public class HotelB2BController : Controller
    {
        private IConfiguration configuration;
        private IContractPayRepository contractPayRepository;
        private IHotelBookingMongoRepository hotelBookingMongoRepository;
        private IElasticsearchDataRepository elasticsearchDataRepository;
        private IESRepository<HotelESViewModel> _ESRepository;
        private HotelESRepository _hotelESRepository;
        private IHotelDetailRepository _hotelDetailRepository;
        private readonly RedisConn redisService;
        private IUserRepository _userRepository;
        private IAccountRepository _accountRepository;
        private IHotelBookingRepositories _hotelBookingRepositories;
        private IHotelBookingRoomExtraPackageRepository _hotelBookingRoomExtraPackage;
        private IRequestRepository _requestRepository;
        private IIdentifierServiceRepository _identifierServiceRepository;
        private IVoucherRepository _voucherRepository;
        private IClientRepository _clientRepository;

        public HotelB2BController(IConfiguration _configuration, IHotelBookingMongoRepository _hotelBookingMongoRepository,
            IElasticsearchDataRepository _elasticsearchDataRepository, IHotelDetailRepository hotelDetailRepository,
            RedisConn _redisService, IContractPayRepository _contractPayRepository, IUserRepository userRepository, IAccountRepository accountRepository,
            IHotelBookingRepositories hotelBookingRepositories, IRequestRepository requestRepository, IIdentifierServiceRepository identifierServiceRepository,
            IVoucherRepository voucherRepository, IClientRepository clientRepository, IHotelBookingRoomExtraPackageRepository hotelBookingRoomExtraPackage)
        {
            configuration = _configuration;
            hotelBookingMongoRepository = _hotelBookingMongoRepository;
            elasticsearchDataRepository = _elasticsearchDataRepository;
            _hotelDetailRepository = hotelDetailRepository;
            _ESRepository = new ESRepository<HotelESViewModel>(configuration["DataBaseConfig:Elastic:Host"]);
            _hotelESRepository = new HotelESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
            redisService = _redisService;
            contractPayRepository = _contractPayRepository;
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _hotelBookingRepositories = hotelBookingRepositories;
            _requestRepository = requestRepository;
            _identifierServiceRepository = identifierServiceRepository;
            _voucherRepository = voucherRepository;
            _clientRepository = clientRepository;
            _hotelBookingRoomExtraPackage = hotelBookingRoomExtraPackage;
        }
        [HttpPost("save-booking.json")]
        public async Task<ActionResult> PushBookingToMongo(string token)
        {
            #region Test

            //var object_input = new BookingHotelB2BViewModel()
            //{
            //    booking_id= "65a9d5700d01d2a5175e128c",
            //    account_client_id = 159,
            //    search = new BookingHotelB2BViewModelSearch()
            //    {
            //        arrivalDate = "2024-01-19",
            //        departureDate = "2024-01-23",
            //        hotelID = "1101",
            //        numberOfAdult = 2,
            //        numberOfChild = 1,
            //        numberOfInfant = 1,
            //        numberOfRoom = 2
            //    },
            //    detail = new BookingHotelB2BViewModelDetail()
            //    {
            //        email = "EMAIL đã sửa",
            //        telephone = "PHone đã sửa"
            //    },
            //    contact = new BookingHotelB2BViewModelContact
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

            //    pickup = new BookingHotelB2BViewModelPickUp()
            //    {
            //        arrive = new BookingHotelB2BViewModelPickUpForm
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
            //        departure = new BookingHotelB2BViewModelPickUpForm
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
            //    rooms = new List<BookingHotelB2BViewModelRooms>()
            //    {
            //        new BookingHotelB2BViewModelRooms()
            //        {
            //            room_type_id="e3b015d5-09a4-9cd5-27d2-aaab5ba4432e",
            //            room_type_code="BKSTN",
            //            room_type_name="Standard King",
            //            special_request="Phong view bien",
            //            price=3450000,
            //            profit=50000,
            //            total_amount=3500000,
            //            rates=new List<BookingHotelB2BViewModelRates>()
            //            {
            //                new BookingHotelB2BViewModelRates()
            //                {
            //                    allotment_id="a924ace9-7cc3-4552-b80d-34e725f442ef",
            //                    arrivalDate="2022-09-01",
            //                    departureDate="2022-09-02",
            //                    rate_plan_code="ADBBD20BR1",
            //                    rate_plan_id="cd737197-61b3-443b-9476-fb043140ca40",
            //                    packages=new List<BookingHotelB2BViewModelPackage>(),
            //                    price=1450000,
            //                    profit=50000,
            //                    total_amount=1500000
            //                },
            //                new BookingHotelB2BViewModelRates()
            //                {
            //                    allotment_id="accbbcc-7cc3-4552-b80d-34e725f442ac",
            //                    arrivalDate="2022-09-02",
            //                    departureDate="2022-09-03",
            //                    rate_plan_code="CODE2",
            //                    rate_plan_id="cd737197-61b3-443b-9476-fb043140ca40",
            //                    packages=new List<BookingHotelB2BViewModelPackage>(),
            //                    price=1950000,
            //                    profit=50000,
            //                    total_amount=2000000
            //                }
            //            },
            //            guests = new List<BookingHotelB2BViewModelGuest>()
            //            {
            //               new  BookingHotelB2BViewModelGuest {
            //                   profile_type=2,
            //                   room=1,
            //                   birthday= "2022-09-01",
            //                   firstName="Nguyen",
            //                   lastName="Van A",
            //               },
            //               new  BookingHotelB2BViewModelGuest {
            //                   profile_type=2,
            //                   room=1,
            //                   birthday= "2022-09-01",
            //                   firstName="Tran",
            //                   lastName="Thi A",
            //               },
            //            },
            //        },
            //        new BookingHotelB2BViewModelRooms()
            //        {
            //            room_type_id="e3b015d5-09a4-9cd5-27d2-aaab5ba4432e",
            //            room_type_code="AKSTN",
            //            room_type_name="Standard TWIN",
            //            special_request="Yeu cau su rieng tu",
            //            price=2400000,
            //            profit=50000,
            //            total_amount=2450000,
            //            rates=new List<BookingHotelB2BViewModelRates>()
            //            {
            //                new BookingHotelB2BViewModelRates()
            //                {
            //                    allotment_id="a924ace9-7cc3-4552-b80d-34e725f442ef",
            //                    arrivalDate="2022-09-01",
            //                    departureDate="2022-09-02",
            //                    rate_plan_code="ADBBD20BR1",
            //                    rate_plan_id="cd737197-61b3-443b-9476-fb043140ca40",
            //                    packages=new List<BookingHotelB2BViewModelPackage>(),
            //                    price=1400000,
            //                    profit=50000,
            //                    total_amount=1450000
            //                },
            //                new BookingHotelB2BViewModelRates()
            //                {
            //                    allotment_id="accbbcc-7cc3-4552-b80d-34e725f442ac",
            //                    arrivalDate="2022-09-02",
            //                    departureDate="2022-09-03",
            //                    rate_plan_code="CODE2",
            //                    rate_plan_id="cd737197-61b3-443b-9476-fb043140ca40",
            //                    packages=new List<BookingHotelB2BViewModelPackage>(),
            //                    price=950000,
            //                    profit=50000,
            //                    total_amount=1000000
            //                }
            //            },
            //            guests = new List<BookingHotelB2BViewModelGuest>()
            //            {

            //               new  BookingHotelB2BViewModelGuest {
            //                   profile_type=2,
            //                   room=2,
            //                   birthday= "2022-09-01",
            //                   firstName="Nguyen",
            //                   lastName="Van B",
            //               },
            //               new  BookingHotelB2BViewModelGuest {
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
            //token = CommonHelper.Encode(input_json, configuration["DataBaseConfig:key_api:b2b"]);
            // string input_json = "{ \"contact\": { \"firstName\": \"Cường\", \"lastName\": \"\", \"email\": \"\", \"phoneNumber\": \"0942066299\", \"country\": \"Việt Nam\", \"birthday\": \"\", \"province\": \"\", \"district\": \"\", \"ward\": \"\", \"address\": \"\", \"note\": \"\" }, \"pickup\": { \"arrive\": { \"required\": 1, \"id_request\": null, \"stop_point_code\": \"\", \"vehicle\": \"car\", \"fly_code\": \"\", \"amount_of_people\": 3, \"datetime\": \"2023-04-11T16:04:00Z\", \"note\": \"left note\" }, \"departure\": { \"required\": 1, \"id_request\": null, \"stop_point_code\": \"\", \"vehicle\": \"bus\", \"fly_code\": \"\", \"amount_of_people\": \"2\", \"datetime\": \"2023-04-14T16:04:00Z\", \"note\": \"right note\" } }, \"search\": { \"arrivalDate\": \"2023-04-07\", \"departureDate\": \"2023-04-08\", \"hotelID\": \"340e8b59-4b88-9b69-5283-9922b91c6236\", \"numberOfRoom\": 2, \"numberOfAdult\": 3, \"numberOfChild\": 2, \"numberOfInfant\": 0 }, \"detail\": { \"email\": \"res.VPDSSLNT@vinpearl.com\", \"telephone\": \"(+84-258) 359 8900\" }, \"rooms\": [ { \"room_number\": \"1\", \"room_type_id\": \"b03de1cb-8e75-696a-a5cf-e2d6f8f92865\", \"room_type_code\": \"KDLN\", \"room_type_name\": \"Deluxe King\", \"numberOfAdult\": 2, \"numberOfChild\": 0, \"numberOfInfant\": 0, \"package_includes\": [ \"Internal Breakdown - Daily Breakfast - Child\", \"Internal Breakdown - VinWonders - Child\", \"Internal Breakdown - Daily Breakfast - Adult\", \"Internal Breakdown - Daily Dinner - Adult\", \"Internal Breakdown - Daily Lunch - Child\", \"Internal Breakdown - VinWonders - Adult\", \"Internal Breakdown - Daily Lunch - Adult\", \"Internal Breakdown - Daily Dinner - Child\" ], \"price\": 2975000.0, \"profit\": 50000.0, \"total_amount\": 3025000.0, \"special_request\": \"\", \"rates\": [ { \"arrivalDate\": \"2023-04-07\", \"departureDate\": \"2023-04-08\", \"rate_plan_code\": \"PR12108BBBR1\", \"rate_plan_id\": \"f3233c15-e6a0-4c2b-ad3b-2b6290d9ad2c\", \"allotment_id\": \"c588354a-67cc-4d85-b001-e7e9469bb317\", \"price\": 2975000.0, \"profit\": 50000.0, \"total_amount\": 3025000.0 } ], \"guests\": [ { \"profile_type\": 2, \"room\": 1, \"firstName\": \"hải\", \"lastName\": \"\", \"birthday\": \"1998-04-06\" }, { \"profile_type\": 2, \"room\": 1, \"firstName\": \"long\", \"lastName\": \"\", \"birthday\": \"1998-04-06\" } ] }, { \"room_number\": \"2\", \"room_type_id\": \"b03de1cb-8e75-696a-a5cf-e2d6f8f92865\", \"room_type_code\": \"KDLN\", \"room_type_name\": \"Deluxe King\", \"numberOfAdult\": 1, \"numberOfChild\": 2, \"numberOfInfant\": 0, \"package_includes\": [ \"Internal Breakdown - Daily Breakfast - Child\", \"Internal Breakdown - VinWonders - Child\", \"Internal Breakdown - Daily Breakfast - Adult\", \"Internal Breakdown - Daily Dinner - Adult\", \"Internal Breakdown - Daily Lunch - Child\", \"Internal Breakdown - VinWonders - Adult\", \"Internal Breakdown - Daily Lunch - Adult\", \"Internal Breakdown - Daily Dinner - Child\" ], \"price\": 2975000.0, \"profit\": 50001.0, \"total_amount\": 3025001.0, \"special_request\": \"\", \"rates\": [ { \"arrivalDate\": \"2023-04-07\", \"departureDate\": \"2023-04-08\", \"rate_plan_code\": \"BABBBR1\", \"rate_plan_id\": \"1c960ee3-0b3b-416d-b1dc-7bcd680a1b95\", \"allotment_id\": \"edd184e5-9116-4cef-a299-7fb9a940f9ab\", \"price\": 2975000.0, \"profit\": 50001.0, \"total_amount\": 3025001.0 } ], \"guests\": [ { \"profile_type\": 2, \"room\": 2, \"firstName\": \"quang\", \"lastName\": \"\", \"birthday\": \"1998-04-06\" }, { \"profile_type\": 2, \"room\": 2, \"firstName\": \"vinh\", \"lastName\": \"\", \"birthday\": \"2015-04-06\" }, { \"profile_type\": 2, \"room\": 2, \"firstName\": \"giang\", \"lastName\": \"\", \"birthday\": \"2015-04-06\" } ] } ] }";
            //  token = CommonHelper.Encode(input_json, configuration["DataBaseConfig:key_api:b2b"]);

            #endregion
            try
            {
                // LogHelper.InsertLogTelegram("HotelBookingController - PushBookingToMongo: " + token);
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {

                    BookingHotelB2BViewModel detail = JsonConvert.DeserializeObject<BookingHotelB2BViewModel>(objParr[0].ToString());
                    var hotel = _hotelESRepository.FindByHotelId(detail.search.hotelID);
                    if (hotel == null || hotel.id <= 0)
                    {
                        var hotel_sql = await _hotelDetailRepository.GetByHotelId(detail.search.hotelID);
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
                    detail.detail.check_in_time = hotel == null || hotel.checkintime == null ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 14, 00, 00) : Convert.ToDateTime(hotel.checkintime.ToString());
                    detail.detail.check_out_time = hotel == null || hotel.checkouttime == null ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 00, 00) : Convert.ToDateTime(hotel.checkouttime.ToString());

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
                        clientType = ((int)(ClientType.TIER_1_AGENT)).ToString(),
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
                    if (detail.extrapackages != null && detail.extrapackages.Count > 0)
                    {
                        total_amount_booking += detail.extrapackages.Sum(s => (double)s.Amount);
                    }
                    model.booking_b2b_data = detail;
                    model.total_amount = total_amount_booking;
                    //--Voucher:
                    model.voucher_code = detail.voucher_code;
                    model.extrapackages = detail.extrapackages;
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
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    int? id = Convert.ToInt32(objParr[0]["id"].ToString());
                    int? type = Convert.ToInt32(objParr[0]["type"].ToString());
                    int? client_type = Convert.ToInt32(objParr[0]["client_type"].ToString());
                    DateTime fromdate = Convert.ToDateTime(objParr[0]["fromdate"].ToString());
                    DateTime todate = Convert.ToDateTime(objParr[0]["todate"].ToString());
                    bool isVinHotel = Convert.ToBoolean(objParr[0]["isVinHotel"] == null ? "false" : objParr[0]["isVinHotel"].ToString());
                    string name = objParr[0]["name"].ToString();
                    int total_nights = (todate - fromdate).Days;
                    if (id == null || type == null || id <= 0 || type < 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED
                        });
                    }
                    var hotel_list = await _hotelESRepository.GetListProduct(name, isVinHotel);

                    // hotel_list = await _hotelESRepository.GetListByLocation(name, (int)id, (int)type);
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
                    // LogHelper.InsertLogTelegram("ListByLocation - HotelB2BController: Cannot get from Redis [" + cache_name + "] - token: " + token);

                    List<HotelSearchEntities> result = new List<HotelSearchEntities>();
                    var hotel_datas = _hotelDetailRepository.GetFEHotelList(new HotelFESearchModel
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
                            redisService.Set(cache_name, JsonConvert.SerializeObject(result), Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
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
                LogHelper.InsertLogTelegram("ListByLocation - HotelB2BController: " + ex);
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
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                   // int? id = Convert.ToInt32(objParr[0]["id"].ToString());
                   // int? type = Convert.ToInt32(objParr[0]["type"].ToString());
                    int? client_type = Convert.ToInt32(objParr[0]["client_type"].ToString());
                    DateTime fromdate = Convert.ToDateTime(objParr[0]["fromdate"].ToString());
                    DateTime todate = Convert.ToDateTime(objParr[0]["todate"].ToString());
                   // string name = objParr[0]["name"].ToString();
                    string hotelid = objParr[0]["hotelid"].ToString();
                    bool? is_vin_hotel = Convert.ToBoolean(objParr[0]["is_vin_hotel"] == null || objParr[0]["is_vin_hotel"].ToString().Trim() == "" ? "false" : objParr[0]["is_vin_hotel"].ToString());
                    int total_nights = (todate - fromdate).Days;
                    List<HotelSearchEntities> result = new List<HotelSearchEntities>();
                    bool is_cached = false;
                    string cache_name_detail = CacheName.ClientHotelSearchResult + 0 + "_" + fromdate.ToString("yyyyMMdd") + "_" + todate.ToString("yyyyMMdd") + "_" + "1" + "0" + "1" + "0" + "_" + hotelid + "_" + client_type;
                    var str = redisService.Get(cache_name_detail, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                    if (str != null && str.Trim() != "")
                    {
                        HotelSearchModel model = JsonConvert.DeserializeObject<HotelSearchModel>(str);
                        if (model != null && model.hotels != null && model.hotels.Count > 0)
                        {
                            var selected = model.hotels.FirstOrDefault(x => x.hotel_id == hotelid);
                            if (selected != null)
                            {
                                result.Add(selected);
                                is_cached = true;
                            }

                        }
                    }
                    // LogHelper.InsertLogTelegram("ListByLocationDetail - HotelB2BController: Cannot get from Redis [" + cache_name_detail + "] - token: " + token);

                    if (!is_cached && is_vin_hotel != null && is_vin_hotel == true)
                    {
                        string distributionChannelId = configuration["config_api_vinpearl:Distribution_ID"].ToString();

                        var hotel_Detail = await _hotelDetailRepository.GetById(Convert.ToInt32(hotelid));
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
                            var client_types_string = client_type.ToString();
                            if (client_type == (int)ClientType.AGENT || client_type == (int)ClientType.TIER_1_AGENT) client_types_string = "1,2";
                            var profit_vin = _hotelDetailRepository.GetHotelRoomPricePolicy(hotel_id_vin, client_types_string);
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
                        var hotel_datas = _hotelDetailRepository.GetFEHotelList(new HotelFESearchModel
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
                                var hotel_detail = await _hotelDetailRepository.GetByHotelId(hotel.HotelId);
                                var hotel_rooms = _hotelDetailRepository.GetFEHotelRoomList(Convert.ToInt32(hotel.HotelId));
                                //-- Tính giá về tay thông qua chính sách giá
                                var client_types_string = client_type.ToString();
                                if (client_type == (int)ClientType.AGENT || client_type == (int)ClientType.TIER_1_AGENT) client_types_string = "1,2";
                                var profit_list = _hotelDetailRepository.GetHotelRoomPricePolicy(hotel.HotelId, client_types_string);
                                foreach (var r in hotel_rooms)
                                {
                                    var room_packages = _hotelDetailRepository.GetFERoomPackageListByRoomId(r.Id, fromdate, todate);
                                    var room_packages_daily = _hotelDetailRepository.GetFERoomPackageDaiLyListByRoomId(r.Id, fromdate, todate);
                                    rooms_list.Add(PricePolicyService.GetRoomDetail(r.Id.ToString(), fromdate, todate, total_nights, room_packages_daily, room_packages, profit_list, hotel_detail, null, (int)client_type));
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
                    if (result.Count > 0)
                    {
                        //-- Cache kết quả:
                        HotelSearchModel cache_data = new HotelSearchModel();
                        cache_data.hotels = result;
                        cache_data.input_api_vin = "";
                        cache_data.rooms = new List<RoomSearchModel>();
                        cache_data.client_type = (int)client_type;
                        cache_data.hotel_ids = hotelid;
                        int db_index = Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"].ToString());
                        redisService.Set(cache_name_detail, JsonConvert.SerializeObject(cache_data),DateTime.Now.AddDays(1), db_index);
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
                LogHelper.InsertLogTelegram("ListByLocation - HotelB2BController: " + ex);
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
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    string cache_name = CacheName.HotelLocationB2B;
                    int db_index = Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"].ToString());
                    var str = redisService.Get(cache_name, db_index);
                    if (str != null && str.Trim() != "")
                    {
                        List<HotelESLocationViewModel> model = JsonConvert.DeserializeObject<List<HotelESLocationViewModel>>(str);
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Success",
                            data = model
                        });
                    }
                    //LogHelper.InsertLogTelegram("GetHotelLocations - HotelB2BController: Cannot get from Redis [" + cache_name + "] - token: " + token);

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
                    redisService.Set(cache_name, JsonConvert.SerializeObject(result), db_index);

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
                LogHelper.InsertLogTelegram("GetHotelLocations - HotelB2BController: " + ex);
                return Ok(new { status = ResponseType.FAILED, msg = "error: " + ex.ToString() });
            }
        }
        [HttpPost("get-bank-account")]
        public async Task<ActionResult> GetAdavigoBankAccount(string token)
        {

            try
            {
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    int suplier_adavigo = 604;
                    var success = int.TryParse(configuration["config_value:SUPPLIERID_ADAVIGO"], out suplier_adavigo);
                    var data = await contractPayRepository.GetBankAccountDataTableBySupplierId(suplier_adavigo);
                    if (data != null && data.Count > 0)
                    {
                        return Ok(new
                        {
                            data = data,
                            status = (int)ResponseType.SUCCESS,
                            msg = "Thành công"

                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Khong tim thay du lieu"
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
                LogHelper.InsertLogTelegram("GetAdavigoBankAccount- Token: " + token + " - " + ex.ToString());

                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }

        }
        [HttpPost("hotel-commit-location")]
        public async Task<ActionResult> HotelCommitLocation(string token)
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
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {


                    string cache_name = CacheName.HotelLocationB2BList;
                    var str = redisService.Get(cache_name, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                    if (str != null && str.Trim() != "")
                    {
                        List<HotelCommitLocationResponse> model = JsonConvert.DeserializeObject<List<HotelCommitLocationResponse>>(str);
                        //--Trả kết quả
                        return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), data = model, cache_id = string.Empty });
                    }
                    // LogHelper.InsertLogTelegram("HotelCommitLocation - HotelB2BController: Cannot get from Redis [" + cache_name + "] - token: " + token);

                    var hotel_datas = _hotelDetailRepository.GetFEHotelDetails(null, true, null, 1, 1000);

                    if (hotel_datas != null && hotel_datas.Any())
                    {
                        int id = 0;

                        var model = hotel_datas.Where(x => x.City != null && x.City.Trim() != "").GroupBy(x => x.City).Select(x => x.First()).ToList();
                        var data_results = new List<HotelCommitLocationResponse>();
                        foreach (var data in model)
                        {
                            data_results.Add(new HotelCommitLocationResponse() { id = ++id, name = data.City });
                        }

                        if (data_results.Count > 0)
                        {
                            redisService.Set(cache_name, JsonConvert.SerializeObject(data_results), Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                            return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), data = data_results, cache_id = string.Empty });
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
                LogHelper.InsertLogTelegram("HotelCommit - HotelB2BController: " + ex);
                return Ok(new { status = ResponseTypeString.Fail, msg = "error: " + ex.ToString() });
            }
        }
        [HttpPost("commit")]
        public async Task<ActionResult> HotelCommit(string token)
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
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
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
                    //var hotel_list = await _hotelESRepository.GetListByLocation(name, (int)id, (int)type);
                    //if (hotel_list == null || hotel_list.Count <= 0)
                    //{
                    //    return Ok(new
                    //    {
                    //        status = (int)ResponseType.FAILED
                    //    });
                    //}
                    string keyword = CommonHelper.RemoveUnicode(name).ToLower();

                    string cache_name = CacheName.HotelCommitB2B + "_" + id + type + "_" + fromdate.ToString("yyyyMMdd") + "_" + todate.ToString("yyyyMMdd") + "_" + id + "_" + total_nights + client_type;
                    var str = redisService.Get(cache_name, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                    if (str != null && str.Trim() != "")
                    {
                        List<HotelSearchEntities> model = JsonConvert.DeserializeObject<List<HotelSearchEntities>>(str);
                        //-- Trả kết quả
                        return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), data = model, cache_id = string.Empty });
                    }
                    // LogHelper.InsertLogTelegram("HotelCommit - HotelB2BController: Cannot get from Redis [" + cache_name + "] - token: " + token);

                    List<HotelSearchEntities> result = new List<HotelSearchEntities>();
                    var hotel_datas = _hotelDetailRepository.GetFEHotelDetails(null, true, null, 1, 1000);

                    if (hotel_datas != null && hotel_datas.Any())
                    {
                        var data_results = hotel_datas.Where(x =>
                        (x.ProvinceName != null && CommonHelper.RemoveUnicode(x.ProvinceName.ToLower()).Contains(keyword))
                        || (x.City != null && CommonHelper.RemoveUnicode(x.City.ToLower()).Contains(keyword))
                        || (x.Country != null && CommonHelper.RemoveUnicode(x.Country.ToLower()).Contains(keyword))
                        || (x.Street != null && x.Street.ToLower().Contains(name.ToLower()))
                        ).GroupBy(x => x.Id).Select(x => x.First()).ToList();

                        if (data_results != null && data_results.Any())
                        {
                            foreach (var hotel in data_results)
                            {
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
                            redisService.Set(cache_name, JsonConvert.SerializeObject(result), Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));

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
                LogHelper.InsertLogTelegram("HotelCommit - HotelB2BController: " + ex);
                return Ok(new { status = ResponseTypeString.Fail, msg = "error: " + ex.ToString() });
            }
        }
        [HttpPost("commit-detail")]
        public async Task<ActionResult> HotelCommitDetail(string token)
        {
            try
            {
                #region Test

                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    int? id = Convert.ToInt32(objParr[0]["id"].ToString());
                    int? type = Convert.ToInt32(objParr[0]["type"].ToString());
                    int? client_type = Convert.ToInt32(objParr[0]["client_type"].ToString());
                    DateTime fromdate = Convert.ToDateTime(objParr[0]["fromdate"].ToString());
                    DateTime todate = Convert.ToDateTime(objParr[0]["todate"].ToString());
                    string name = objParr[0]["name"].ToString();
                    string hotelid = objParr[0]["hotelid"].ToString();
                    bool? is_vin_hotel = Convert.ToBoolean(objParr[0]["is_vin_hotel"] == null || objParr[0]["is_vin_hotel"].ToString().Trim() == "" ? "false" : objParr[0]["is_vin_hotel"].ToString());
                    int total_nights = (todate - fromdate).Days;
                    List<HotelSearchEntities> result = new List<HotelSearchEntities>();
                    bool is_cached = false;
                    string cache_name_detail = CacheName.ClientHotelSearchResult + "_" + fromdate.ToString("yyyyMMdd") + "_" + todate.ToString("yyyyMMdd") + "_" + "1" + "0" + "1" + "0" + "_" + hotelid + "_" + client_type;
                    var str = redisService.Get(cache_name_detail, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                    if (str != null && str.Trim() != "")
                    {
                        HotelSearchModel model = JsonConvert.DeserializeObject<HotelSearchModel>(str);
                        if (model != null && model.hotels != null && model.hotels.Count > 0)
                        {
                            var selected = model.hotels.FirstOrDefault(x => x.hotel_id == hotelid);
                            if (selected != null)
                            {
                                result.Add(selected);
                                is_cached = true;
                            }

                        }
                    }
                    // LogHelper.InsertLogTelegram("HotelCommitDetail - HotelB2BController: Cannot get from Redis [" + cache_name_detail + "] - token: " + token);

                    if (!is_cached && is_vin_hotel != null && is_vin_hotel == true)
                    {
                        string distributionChannelId = configuration["config_api_vinpearl:Distribution_ID"].ToString();

                        var hotel_Detail = await _hotelDetailRepository.GetById(Convert.ToInt32(hotelid));
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
                            var client_types_string = client_type.ToString();
                            if (client_type == (int)ClientType.AGENT || client_type == (int)ClientType.TIER_1_AGENT) client_types_string = "1,2";
                            var profit_vin = _hotelDetailRepository.GetHotelRoomPricePolicy(hotel_id_vin, client_types_string);
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
                        var hotel_datas = _hotelDetailRepository.GetFEHotelList(new HotelFESearchModel
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
                                var hotel_detail = await _hotelDetailRepository.GetByHotelId(hotel.HotelId);
                                var hotel_rooms = _hotelDetailRepository.GetFEHotelRoomList(Convert.ToInt32(hotel.HotelId));
                                //-- Tính giá về tay thông qua chính sách giá
                                var client_types_string = client_type.ToString();
                                if (client_type == (int)ClientType.AGENT || client_type == (int)ClientType.TIER_1_AGENT) client_types_string = "1,2";
                                var profit_list = _hotelDetailRepository.GetHotelRoomPricePolicy(hotel.HotelId, client_types_string);
                                foreach (var r in hotel_rooms)
                                {
                                    var room_packages = _hotelDetailRepository.GetFERoomPackageListByRoomId(r.Id, fromdate, todate);
                                    var room_packages_daily = _hotelDetailRepository.GetFERoomPackageDaiLyListByRoomId(r.Id, fromdate, todate);
                                    rooms_list.Add(PricePolicyService.GetRoomDetail(r.Id.ToString(), fromdate, todate, total_nights, room_packages_daily, room_packages, profit_list, hotel_detail, null, (int)client_type));
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
                    if (result != null && result.Count > 0)
                    {
                        //-- Cache kết quả:
                        HotelSearchModel cache_data = new HotelSearchModel();
                        cache_data.hotels = result;
                        cache_data.input_api_vin = "";
                        cache_data.rooms = new List<RoomSearchModel>();
                        cache_data.client_type = (int)client_type;
                        cache_data.hotel_ids = hotelid;
                        int db_index = Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"].ToString());
                        redisService.Set(cache_name_detail, JsonConvert.SerializeObject(cache_data), db_index);
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
                LogHelper.InsertLogTelegram("HotelCommitDetail - HotelB2BController: " + ex);
                return Ok(new { status = ResponseTypeString.Fail, msg = "error: " + ex.ToString() });
            }
        }
        [HttpPost("commit-position")]
        public async Task<ActionResult> HotelCommitPosition(string token)
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
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
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
                    //var hotel_list = await _hotelESRepository.GetListByLocation(name, (int)id, (int)type);
                    //if (hotel_list == null || hotel_list.Count <= 0)
                    //{
                    //    return Ok(new
                    //    {
                    //        status = (int)ResponseType.FAILED
                    //    });
                    //}
                    string keyword = CommonHelper.RemoveUnicode(name).ToLower();

                    string cache_name = CacheName.HotelCommitB2B + "_" + id + type + "_" + fromdate.ToString("yyyyMMdd") + "_" + todate.ToString("yyyyMMdd") + "_" + name + "_" + total_nights + client_type;
                    var str = redisService.Get(cache_name, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                    if (str != null && str.Trim() != "")
                    {
                        List<HotelSearchEntities> model = JsonConvert.DeserializeObject<List<HotelSearchEntities>>(str);
                        //-- Trả kết quả
                        return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), data = model, cache_id = string.Empty });
                    }
                    //LogHelper.InsertLogTelegram("HotelCommit - HotelB2BController: Cannot get from Redis [" + cache_name + "] - token: " + token);

                    List<HotelSearchEntities> result = new List<HotelSearchEntities>();
                    var hotel_datas = _hotelDetailRepository.GetFEHotelDetailPosition(null, true, null, 1, 1000);

                    if (hotel_datas != null && hotel_datas.Any())
                    {
                        var data_results = hotel_datas.Where(x =>
                        (x.ProvinceName != null && CommonHelper.RemoveUnicode(x.ProvinceName.ToLower()).Contains(keyword))
                        || (x.City != null && CommonHelper.RemoveUnicode(x.City.ToLower()).Contains(keyword))
                        || (x.Country != null && CommonHelper.RemoveUnicode(x.Country.ToLower()).Contains(keyword))
                        || (x.Street != null && x.Street.ToLower().Contains(name.ToLower()))
                        ).GroupBy(x => x.Id).Select(x => x.First()).ToList();

                        if (data_results != null && data_results.Any())
                        {
                            foreach (var hotel in data_results)
                            {
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
                            redisService.Set(cache_name, JsonConvert.SerializeObject(result), Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
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
                LogHelper.InsertLogTelegram("HotelCommitPosition - HotelB2BController: " + ex);
                return Ok(new { status = ResponseTypeString.Fail, msg = "error: " + ex.ToString() });
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
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
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

                    string cache_name = CacheName.HotelExclusiveListB2C_POSITION + name + "_" + client_type;
                    var str = redisService.Get(cache_name, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                    if (str != null && str.Trim() != "")
                    {
                        List<HotelSearchEntities> model = JsonConvert.DeserializeObject<List<HotelSearchEntities>>(str);
                        //-- Trả kết quả
                        return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), data = model, cache_id = string.Empty });
                    }
                    // LogHelper.InsertLogTelegram("ListByLocation - HotelB2CController: Cannot get from Redis [" + cache_name + "] - token: " + token);

                    List<HotelSearchEntities> result = new List<HotelSearchEntities>();
                    var hotel_datas = _hotelDetailRepository.GetFEHotelListPosition(new HotelFESearchModel
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
                                    position = hotel.Hotelposition.ToString(),
                                });
                            }
                        }
                        if (result.Count > 0)
                        {
                            redisService.Set(cache_name, JsonConvert.SerializeObject(result), Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
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
                LogHelper.InsertLogTelegram("ListByLocationPosition - HotelB2BController: " + ex);
                return Ok(new { status = ResponseTypeString.Fail, msg = "error: " + ex.ToString() });
            }
        }
        [HttpPost("request-hotel-booking.json")]
        public async Task<ActionResult> PushRequestHotelBooking(string token)
        {
            try
            {
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    BookingHotelB2BViewModel detail = JsonConvert.DeserializeObject<BookingHotelB2BViewModel>(objParr[0].ToString());
                    var Hotel_Booking = new HotelBooking();
                    var account_client = await _accountRepository.GetAccountClient(detail.account_client_id);
                    var UserId = _userRepository.GetUserAgentByClientId((long)account_client.ClientId);
                    var sale = _userRepository.GetHeadOfDepartmentByRoleID((int)RoleType.TPDHKS);
                    Hotel_Booking.CreatedBy = (int?)UserId;
                    Hotel_Booking.SalerId = sale.Id;
                    Hotel_Booking.Status = 0;
                    Hotel_Booking.ExtraPackageAmount = 0;
                    Hotel_Booking.TotalDiscount = 0;
                    Hotel_Booking.TotalOthersAmount = 0;
                    Hotel_Booking.BookingId = await _identifierServiceRepository.buildServiceNo(1);

                    if (detail.search != null)
                    {
                        var hotel_sql = await _hotelDetailRepository.GetByHotelId(detail.search.hotelID);
                        Hotel_Booking.SupplierId = hotel_sql != null ? hotel_sql.SupplierId : null;
                        Hotel_Booking.HotelName = hotel_sql != null ? hotel_sql.Name : null;
                        Hotel_Booking.CreatedDate = DateTime.Now;
                        Hotel_Booking.PropertyId = detail.search.hotelID;
                        Hotel_Booking.ArrivalDate = (DateTime)DateUtil.Parse(detail.search.arrivalDate.Replace('-', '/'));
                        Hotel_Booking.DepartureDate = (DateTime)DateUtil.Parse(detail.search.departureDate.Replace('-', '/'));
                        Hotel_Booking.NumberOfAdult = detail.search.numberOfAdult;
                        Hotel_Booking.NumberOfChild = detail.search.numberOfChild;
                        Hotel_Booking.NumberOfInfant = detail.search.numberOfInfant;
                    }
                    if (detail.detail != null)
                    {
                        Hotel_Booking.Address = detail.detail.address;
                        Hotel_Booking.ImageThumb = detail.detail.image_thumb;
                        Hotel_Booking.Telephone = detail.detail.telephone;
                        Hotel_Booking.Email = detail.detail.email;
                        Hotel_Booking.CheckinTime = detail.detail.check_in_time;
                        Hotel_Booking.CheckoutTime = detail.detail.check_out_time;
                        Hotel_Booking.Note = detail.detail.note;

                    }
                    if (detail.rooms != null)
                    {
                        Hotel_Booking.NumberOfRoom = detail.rooms.Count();
                        Hotel_Booking.TotalAmount = detail.rooms.Sum(s => s.total_amount);
                        Hotel_Booking.TotalProfit = detail.rooms.Sum(s => s.profit);
                        Hotel_Booking.TotalPrice = detail.rooms.Sum(s => s.price);
                        Hotel_Booking.ArrivalDate = detail.rooms.Max(s => s.rates.Min(x => (DateTime)DateUtil.Parse(x.arrivalDate.Replace('-', '/'))));
                        Hotel_Booking.DepartureDate = detail.rooms.Max(s => s.rates.Max(x => (DateTime)DateUtil.Parse(x.departureDate.Replace('-', '/'))));
                    }
                    if (detail.extrapackages != null && detail.extrapackages.Count > 0)
                    {
                        Hotel_Booking.TotalAmount += detail.extrapackages.Sum(s => (double)s.Amount);
                    }
                    var HotelBookingId = _hotelBookingRepositories.CreateHotelBooking(Hotel_Booking);
                    if (HotelBookingId < 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Không thành công!"
                        });
                    }

                    foreach (var room in detail.rooms)
                    {
                        if (room.numberOfAdult == 0 && room.numberOfChild == 0 && room.numberOfInfant == 0) { room.numberOfAdult = 1; }
                        var HotelBookingRooms = new HotelBookingRooms();
                        HotelBookingRooms.HotelBookingId = HotelBookingId;
                        HotelBookingRooms.RoomTypeCode = room.room_type_code == null ? "" : room.room_type_code;
                        HotelBookingRooms.RoomTypeId = room.room_type_id;
                        HotelBookingRooms.RoomTypeName = room.room_type_name;
                        HotelBookingRooms.NumberOfAdult = room.numberOfAdult;
                        HotelBookingRooms.NumberOfChild = room.numberOfChild;
                        HotelBookingRooms.NumberOfInfant = room.numberOfInfant;
                        HotelBookingRooms.NumberOfRooms = 1;
                        HotelBookingRooms.ExtraPackageAmount = 0;
                        HotelBookingRooms.Status = 0;
                        HotelBookingRooms.TotalUnitPrice = 0;
                        HotelBookingRooms.CreatedBy = sale.Id;
                        HotelBookingRooms.PackageIncludes = room.package_includes != null ? string.Join(',', room.package_includes) : "";

                        HotelBookingRooms.TotalAmount = room.total_amount;
                        HotelBookingRooms.Profit = room.profit;
                        HotelBookingRooms.Price = room.price;
                        HotelBookingRooms.IsRoomAvailable = 0;

                        var HotelBookingRoomsId = _hotelBookingRepositories.CreateHotelBookingRooms(HotelBookingRooms);

                        //---- Create Hotel Booking Room Rates
                        foreach (var rate in room.rates)
                        {
                            var HotelBookingRoomRates = new HotelBookingRoomRates();
                            HotelBookingRoomRates.HotelBookingRoomId = HotelBookingRoomsId;
                            HotelBookingRoomRates.Nights = (short?)((DateTime)DateUtil.Parse(rate.departureDate.Replace('-', '/')) - (DateTime)DateUtil.Parse(rate.arrivalDate.Replace('-', '/'))).TotalDays;
                            HotelBookingRoomRates.RatePlanId = rate.rate_plan_id;
                            HotelBookingRoomRates.RatePlanCode = rate.rate_plan_code;
                            HotelBookingRoomRates.AllotmentId = rate.allotment_id;
                            HotelBookingRoomRates.TotalAmount = rate.total_amount;
                            HotelBookingRoomRates.Profit = rate.profit;
                            HotelBookingRoomRates.Price = rate.price;
                            HotelBookingRoomRates.PackagesInclude = rate.package_includes != null ? string.Join(',', rate.package_includes) : "";
                            HotelBookingRoomRates.StartDate = (DateTime)DateUtil.Parse(rate.arrivalDate.Replace('-', '/'));
                            HotelBookingRoomRates.EndDate = (DateTime)DateUtil.Parse(rate.departureDate.Replace('-', '/'));
                            HotelBookingRoomRates.StayDate = (DateTime)DateUtil.Parse(rate.arrivalDate.Replace('-', '/'));
                            HotelBookingRoomRates.SalePrice = rate.total_amount / HotelBookingRoomRates.Nights;
                            HotelBookingRoomRates.CreatedBy = (int?)UserId;
                            HotelBookingRoomRates.CreatedDate = DateTime.Now;
                            HotelBookingRoomRates.OperatorPrice = rate.price / HotelBookingRoomRates.Nights;

                            var Create_HotelBookingRooms = _hotelBookingRepositories.CreateHotelBookingRoomRates(HotelBookingRoomRates);

                        }

                    }
                    var hotel = await _hotelDetailRepository.GetByHotelId(Hotel_Booking.PropertyId);
                    var mode = new Request();
                    mode.BookingId = HotelBookingId;
                    mode.FromDate = Hotel_Booking.ArrivalDate;
                    mode.ToDate = Hotel_Booking.DepartureDate;
                    mode.SalerId = Hotel_Booking.CreatedBy;
                    mode.CreatedBy = Hotel_Booking.CreatedBy;
                    mode.Status = (int?)RequestStatus.TAO_MOI;
                    mode.HotelId = hotel == null ? 0 : hotel.Id;
                    mode.ClientId = (int?)account_client.ClientId;
                    mode.RequestNo = await _identifierServiceRepository.buildRequest();
                    mode.Price = Hotel_Booking.TotalAmount;
                    mode.Amount = Hotel_Booking.TotalAmount;
                    if (detail.voucher != null && detail.voucher.id > 0)
                    {
                        mode.Discount = detail.voucher.discount;
                        mode.VoucherId = detail.voucher.id;
                        mode.VoucherName = detail.voucher.code;
                        mode.Amount = Hotel_Booking.TotalAmount - detail.voucher.discount;
                    }

                    if(detail.extrapackages!=null && detail.extrapackages.Count > 0)
                    {
                        foreach(var extra in detail.extrapackages)
                        {
                            extra.HotelBookingId = HotelBookingId;
                            extra.CreatedBy = 0;
                            extra.CreatedDate = DateTime.Now;
                            extra.UpdatedBy = 0;
                            extra.UpdatedDate = DateTime.Now;
                            extra.Profit =0;
                            extra.UnitPrice = extra.Amount;
                            extra.PackageCompanyId = 0;
                            var id =  _hotelBookingRoomExtraPackage.CreateHotelBookingRoomExtraPackages(extra);
                        }
                    }

                    var request = await _requestRepository.InsertRequest(mode);
                    if (request > 0)
                    {
                        var Request_token = configuration["BotSetting:Request_token"];
                        var Request_group_id = configuration["BotSetting:Request_group_id"];
                        var user = _userRepository.GetDetail((long)UserId);
                        var client = _clientRepository.GetDetail((long)account_client.ClientId);
                        var log2 = user.NickName != null && user.NickName != "" ? user.NickName : "";
                        string log = "Request " + mode.RequestNo + " đã tạo mới thành công " +
                            "\n Khách hàng : " + client.Email + " - " + client.ClientName + "" +
                            "\n Sale phụ trách : " + log2 + "  " + user.Email + " - " + user.FullName + "" +
                            "\n Số tiền : " + ((double)mode.Price).ToString("N0") + "" +
                      "\n Vào lúc : " + ((DateTime.Now).ToString("dd/MM/yyyy HH:mm"));
                        LogHelper.InsertLogTelegramRequest(log, Request_token, Request_group_id);
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Thành công!",
                            data = HotelBookingId
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "Thất bại!",

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
                LogHelper.InsertLogTelegram("HotelBookingController- Token: " + token + " - PushRequestHotelBooking: " + ex.ToString());

                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }
        }
        [HttpPost("get-list-request-hotel-booking-clientid.json")]
        public async Task<ActionResult> GetListRequestHotelBookingByClientId(string token)
        {
            try
            {
                #region Test
                //var model = new
                //{
                //    ClientId = "4081",
                //    PageIndex = "1",
                //    PageSize = "20",
                //};
                //token = CommonHelper.Encode(JsonConvert.SerializeObject(model), configuration["DataBaseConfig:key_api:b2b"]);
                #endregion
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    RequestSearchModel searchModel = JsonConvert.DeserializeObject<RequestSearchModel>(objParr[0].ToString());
                    var account_client = await _accountRepository.GetAccountClient(searchModel.ClientId);
                    if (account_client != null)
                    {
                        searchModel.ClientId = (long)account_client.ClientId;
                    }

                    var data = await _requestRepository.GetPagingList(searchModel);
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Thành công!",
                        data = data,
                        total_row = data[0].TotalRow,
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
                LogHelper.InsertLogTelegram("HotelBookingController- Token: " + token + " - GetListRequestHotelBookingByClientId: " + ex.ToString());

                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }
        }
        [HttpPost("get-detail-request-hotel-booking-id.json")]
        public async Task<ActionResult> GetDetailRequestHotelBookingById(string token)
        {
            try
            {
                #region Test
                //var model = new
                //{
                //    id = "25124",
                //};
                //token = CommonHelper.Encode(JsonConvert.SerializeObject(model), configuration["DataBaseConfig:key_api:b2b"]);
                #endregion
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    var id = Convert.ToInt32(objParr[0]["id"].ToString());
                    var detail = new DetailRequestModel();

                    var booking = await _requestRepository.GetDetailByBookingId(id);
                    var Hotelbooking = await _hotelBookingRepositories.GetDetailHotelBookingByID(id);

                    if (booking != null && booking.BookingId > 0)
                    {
                        var requestDetail = await _requestRepository.GetDetailRequestByRequestId(booking.RequestId);
                        detail.RequestNo = booking.RequestNo;
                        detail.RequestId = booking.RequestId;
                        detail.HotelName = Hotelbooking != null ? Hotelbooking[0].HotelName : "";
                        detail.HotelId = Hotelbooking != null ? Hotelbooking[0].PropertyId : "";
                        detail.telephone = Hotelbooking != null ? Hotelbooking[0].Telephone : "";
                        detail.email = Hotelbooking != null ? Hotelbooking[0].Email : "";
                        detail.Status = (int)booking.Status;
                        detail.StatusName = requestDetail.StatusName;
                        detail.arrivalDate = Hotelbooking != null ? Hotelbooking[0].ArrivalDate.ToString("yyyy-MM-dd") : "";
                        detail.departureDate = Hotelbooking != null ? Hotelbooking[0].DepartureDate.ToString("yyyy-MM-dd") : "";
                        var rooms = await _hotelBookingRepositories.GetHotelBookingRoomsByHotelBookingID(id);
                        var packages = await _hotelBookingRepositories.GetHotelBookingRoomRatesByBookingRoomsRateByHotelBookingID(id);
                        var ExtraPackages = await _hotelBookingRepositories.GetListHotelBookingRoomsExtraPackageByBookingId(id);
                        detail.voucher = new HotelOrderDataVoucher();
                        if (booking.VoucherId != null && booking.VoucherId > 0)
                        {
                            detail.voucher = new HotelOrderDataVoucher()
                            {
                                id = (int)booking.VoucherId,
                                code = booking.VoucherName,
                                discount = (double)booking.Discount
                            };
                        }
                        List<HotelBookingRoomRates> package_daterange = new List<HotelBookingRoomRates>();

                        if (packages != null && packages.Count > 0)
                        {
                            foreach (var p in packages)
                            {
                                if (p.StartDate == null && p.EndDate == null)
                                {
                                    if (packages.Count < 1 || !package_daterange.Any(x => x.HotelBookingRoomId == p.HotelBookingRoomId && x.RatePlanId == p.RatePlanId))
                                    {
                                        var add_value = p;
                                        add_value.StartDate = add_value.StayDate;
                                        add_value.EndDate = add_value.StayDate.AddDays(1);
                                        p.StayDate = (DateTime)add_value.StartDate;
                                        p.SalePrice = p.TotalAmount;
                                        p.OperatorPrice = p.Price;
                                        package_daterange.Add(add_value);
                                    }
                                    else
                                    {
                                        var p_d = package_daterange.FirstOrDefault(x => x.HotelBookingRoomId == p.HotelBookingRoomId && x.RatePlanId == p.RatePlanId && ((DateTime)x.EndDate).Date == p.StayDate.Date);
                                        if (p_d != null)
                                        {

                                            if (p_d.StartDate == null || p_d.StartDate > p.StayDate)
                                                p_d.StartDate = p.StayDate;
                                            p_d.EndDate = p.StayDate.AddDays(1);
                                        }
                                        else
                                        {
                                            var add_value = p;
                                            add_value.StartDate = add_value.StayDate;
                                            add_value.EndDate = add_value.StayDate.AddDays(1);
                                            p.StayDate = (DateTime)add_value.StartDate;
                                            p.SalePrice = p.TotalAmount;
                                            p.OperatorPrice = p.Price;
                                            package_daterange.Add(add_value);
                                        }
                                    }
                                }
                                else
                                {
                                    package_daterange.Add(p);
                                }
                            }
                        }
                        detail.Rooms = rooms == null || rooms.Count < 1 ? new List<HotelBookingRooms>() : rooms; ;
                        detail.Rates = package_daterange == null || package_daterange.Count < 1 ? new List<HotelBookingRoomRates>() : package_daterange; ;
                        detail.ExtraPackages = ExtraPackages == null || ExtraPackages.Count < 1 ? new List<HotelBookingRoomExtraPackages>() : ExtraPackages; ;
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Thành công!",
                        data = detail,

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
                LogHelper.InsertLogTelegram("HotelBookingController- Token: " + token + " - GetDetailRequestHotelBookingById: " + ex.ToString());

                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }
        }

        [HttpPost("update-request-hotel-booking-id.json")]
        public async Task<ActionResult> UpdateRequestHotelBookingById(string token)
        {
            try
            {
                #region Test
                //var model = new
                //{
                //    id = "25039",
                //};
                //token = CommonHelper.Encode(JsonConvert.SerializeObject(model), configuration["DataBaseConfig:key_api:b2b"]);
                #endregion
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    var id = Convert.ToInt32(objParr[0]["id"].ToString());
                    var booking = await _requestRepository.GetDetailByBookingId(id);
                    var model = new Request();
                    model.RequestId = booking.RequestId;
                    model.Status = (int?)RequestStatus.HOAN_THANH;
                    var update = await _requestRepository.UpdateRequest(model);
                    if (update > 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Thành công!"

                        });

                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "không Thành công!"

                        });
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Thành công!"

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

                LogHelper.InsertLogTelegram("HotelBookingController- Token: " + token + " - UpdateRequestHotelBookingById: " + ex.ToString());

                return Ok(new { status = (int)ResponseType.ERROR, msg = "error: " + ex.ToString() });
            }
        }
        [HttpPost("get-surcharge.json")]
        public async Task<ActionResult> GetHotelSurcharge(string token)
        {
            try
            {
                #region Test

                //var j_param = new Dictionary<string, string>
                //{

                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
                #endregion.


                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {

                    string hotelID = objParr[0]["hotelID"].ToString();
                    if (hotelID == null || hotelID.Trim() == "")
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Data invalid!"
                        });
                    }

                    var hotel = await _hotelDetailRepository.GetByHotelId(hotelID);
                    if (hotel == null || hotel.Id <= 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Data invalid!"
                        });
                    }
                    //-- Đọc từ cache, nếu có trả kết quả:
                    string cache_name = CacheName.B2B_HOTEL_SURCHARGE + hotelID;
                    var str = redisService.Get(cache_name, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                    if (str != null && str.Trim() != "")
                    {

                        IEnumerable<HotelSurchargeGridModel> model = JsonConvert.DeserializeObject<IEnumerable<HotelSurchargeGridModel>>(str);
                        if (model.Any())
                        {
                            //-- Trả kết quả
                            return Ok(new
                            {
                                status = ((int)ResponseType.SUCCESS).ToString(),
                                msg = "Get Data From Cache Success",
                                data = model,
                            });
                        }
                        
                    }
                    var surchage = _hotelDetailRepository.GetHotelSurchargeList(hotel.Id, 1, 200);
                    if (surchage != null && surchage.Count() > 0)
                    {
                        surchage = surchage.Where(x => x.Status == 0);
                        //-- Cache kết quả:
                        int db_index = Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"].ToString());
                        redisService.Set(cache_name, JsonConvert.SerializeObject(surchage), DateTime.Now.AddHours(1), db_index);
                    }
                    return Ok(new
                    {
                        status = ((int)ResponseType.SUCCESS).ToString(),
                        msg = "Get Data Success",
                        data = surchage
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
                LogHelper.InsertLogTelegram("HotelB2BController - GetHotelSurcharge: " + ex.ToString());
                return Ok(new { status = ((int)ResponseType.ERROR).ToString(), msg = "error: " + ex.ToString() });
            }

        }

        #region V2:
        [HttpPost("v2-list-by-location")]
        public async Task<ActionResult> GetListHotelByLocation(string token)
        {
            try
            {
                #region Test
                //var j_param = new Dictionary<string, string>
                //{
                //    {"type", "0"},
                //    {"name", "Hà Nội"},
                //    {"fromdate", DateTime.Now.ToString()},
                //    {"todate", DateTime.Now.AddDays(1).ToString()},
                //};
                //var data_product = JsonConvert.SerializeObject(j_param);
                //token = CommonHelper.Encode(data_product, configuration["DataBaseConfig:key_api:b2b"]);
                #endregion

                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["DataBaseConfig:key_api:b2b"]))
                {
                    int? type = Convert.ToInt32(objParr[0]["type"].ToString());
                    int? client_type = Convert.ToInt32(objParr[0]["client_type"].ToString());
                    int? hotel_position = Convert.ToInt32(objParr[0]["hotel_position"].ToString());
                    DateTime fromdate = Convert.ToDateTime(objParr[0]["fromdate"].ToString());
                    DateTime todate = Convert.ToDateTime(objParr[0]["todate"].ToString());
                    int? index = 1;
                    int? size = 30;
                    if (objParr[0]["index"]!=null)
                        index = Convert.ToInt32(objParr[0]["index"].ToString());
                    if (objParr[0]["size"] != null)
                        size = Convert.ToInt32(objParr[0]["size"].ToString());
                    string name = objParr[0]["name"].ToString();
                    int total_nights = (todate - fromdate).Days;
                    string cache_name = CacheName.HotelByLocation + type + fromdate.ToString("yyyyMMdd") + todate.ToString("yyyyMMdd") + "_" + name + "_" + client_type + "_" + type + "_" + (hotel_position == null ? "" : ((int)hotel_position).ToString()) + "_" + index + size;

                    if (name == null || type == null || hotel_position==null|| hotel_position<0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED
                        });
                    }
                    string hotel_ids = "";
                    if(hotel_position > 0)
                    {
                        var hotel_list = await _hotelDetailRepository.GetByPositionType((int)hotel_position);

                        if ((hotel_list == null || hotel_list.Count <= 0))
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.FAILED
                            });
                        }

                        var hotel_list_es = await _hotelESRepository.GetListByLocationName(name, (int)type);

                        if ((hotel_list_es == null || hotel_list_es.Count <= 0))
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.FAILED
                            });
                        }
                        hotel_ids = string.Join(",", hotel_list_es.Select(x => x.hotelid));
                        hotel_list_es = hotel_list_es.Where(x => hotel_ids.Contains(x.hotelid)).ToList();
                        hotel_ids = string.Join(",", hotel_list_es.Select(x => x.hotelid));

                    }
                    else
                    {
                        var hotel_list_es = await _hotelESRepository.GetListByLocationName(name, (int)type);

                        if ((hotel_list_es == null || hotel_list_es.Count <= 0))
                        {
                            hotel_ids = null;
                        }
                        else
                        {
                            hotel_ids = string.Join(",", hotel_list_es.Select(x => x.hotelid));
                        }
                    }
                    try
                    {
                        var str = redisService.Get(cache_name, Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
                        if (str != null && str.Trim() != "")
                        {
                            List<HotelSearchEntities> model = JsonConvert.DeserializeObject<List<HotelSearchEntities>>(str);
                            //-- Trả kết quả
                            return Ok(new { status = ((int)ResponseType.SUCCESS).ToString(), data = model, cache_id = string.Empty });
                        }
                        // LogHelper.InsertLogTelegram("ListByLocation - HotelB2BController: Cannot get from Redis [" + cache_name + "] - token: " + token);
                    }
                    catch { }

                    List<HotelSearchEntities> result = new List<HotelSearchEntities>();
                    List<HotelFEDataModel> hotel_datas = new List<HotelFEDataModel>();
                    hotel_datas = _hotelDetailRepository.GetFEHotelList(new HotelFESearchModel
                    {
                        FromDate = fromdate,
                        ToDate = todate,
                        HotelId = hotel_ids,
                        HotelType = "",
                        PageIndex = 1,
                        PageSize = 1000,

                    });

                    if (hotel_datas != null && hotel_datas.Any())
                    {
                        var data_results = hotel_datas.GroupBy(x => x.Id).Select(x => x.First()).ToList();
                        if (data_results != null && data_results.Any())
                        {
                            data_results = data_results.Skip((index <= 1 ? 0 : ((int)index-1)) * (int)size).Take((int)size).ToList();
                            
                            foreach (var hotel in data_results)
                            {
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
                            redisService.Set(cache_name, JsonConvert.SerializeObject(result),DateTime.Now.AddDays(1), Convert.ToInt32(configuration["DataBaseConfig:Redis:Database:db_search_result"]));
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
                LogHelper.InsertLogTelegram("ListByLocation - HotelB2BController: " + ex);
                return Ok(new { status = ResponseTypeString.Fail, msg = "error: " + ex.ToString() });
            }
        }
        [HttpPost("v2-list-by-location-detail")]
        public async Task<ActionResult> GetListHotelByLocationDetail(string token)
        {
            return await ListByLocationDetail(token);
        }
        #endregion
    }
}
