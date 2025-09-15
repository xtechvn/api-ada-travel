using Caching.Elasticsearch;
using DAL;
using DAL.Clients;
using DAL.Fly;
using DAL.MongoDB.Flight;
using DAL.Orders;
using Entities.ConfigModels;
using ENTITIES.Models;
using ENTITIES.ViewModels.BookingFly;
using ENTITIES.ViewModels.Order;
using Microsoft.Extensions.Options;
using Nest;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Payments;
using Utilities;
using Utilities.Contants;
using static ENTITIES.ViewModels.B2C.AccountB2CViewModel;
using static Utilities.Contants.UserConstant;

namespace REPOSITORIES.Repositories
{
    public class SaveBookingRepository : ISaveBookingRepository
    {

        private readonly IOptions<DataBaseConfig> dataBaseConfig;
        private readonly OrderDAL OrderDAL;
        private readonly ClientDAL ClientDAL;
        private readonly PassengerDAL PassengerDAL;
        private readonly FlightSegmentDAL FlightSegmentDAL;
        private readonly AirlinesDAL AirlinesDAL;
        private readonly ContactClientDAL ContactClientDAL;
        private readonly AccountClientDAL AccountClientDAL;
        private readonly IdentifierServiceRepository identifierServiceRepository;
        private readonly UserDAL userDAL;
        private readonly BaggageDAL baggageDAL;
        private int id_bot = 2052;
        public SaveBookingRepository(IOptions<DataBaseConfig> _dataBaseConfig)
        {
            dataBaseConfig = _dataBaseConfig;
            OrderDAL = new OrderDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            ClientDAL = new ClientDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            PassengerDAL = new PassengerDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            FlightSegmentDAL = new FlightSegmentDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            AirlinesDAL = new AirlinesDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            ContactClientDAL = new ContactClientDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            AccountClientDAL = new AccountClientDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            userDAL = new UserDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            identifierServiceRepository = new IdentifierServiceRepository(dataBaseConfig);
            baggageDAL = new BaggageDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
        public async Task<long> saveBookingAda(BookingFlyMua_Di data)
        {
            try
            {
                //client
                var clentid = 0;
                var Acclentid = 0;
                var CheckPhone = ClientDAL.CheckPhoneClient(data.order.customerPhone);
                if (CheckPhone == true)
                {
                    var model_client = new ENTITIES.Models.Client();

                    model_client.ClientName = data.order.customerName;
                    model_client.Phone = data.order.customerPhone;
                    model_client.Email = data.order.customerEmail;
                    model_client.UtmSource = (int)ClientUtmSource.B2C;
                    model_client.ClientType = (byte?)ClientType.CUSTOMER;
                    model_client.ClientCode = await identifierServiceRepository.buildClientNo(8, (int)ClientType.CUSTOMER);
                    model_client.JoinDate = DateTime.Now;

                    var id = ClientDAL.Insert(model_client);
                    clentid = (int)id;
                    AccountClient accountClient = new AccountClient();
                    accountClient.UserName = model_client.Email;
                    accountClient.ClientType = (byte?)ClientType.CUSTOMER;
                    accountClient.Password = "e10adc3949ba59abbe56e057f20f883e";
                    accountClient.PasswordBackup = "e10adc3949ba59abbe56e057f20f883e";
                    accountClient.Status = (int)UserStatus.ACTIVE;
                    accountClient.GroupPermission = GroupPermissionType.Admin;
                    accountClient.ClientId = clentid;
                    Acclentid = (int)AccountClientDAL.Insert(accountClient);

                }
                else
                {
                    var detail_client = ClientDAL.GetByPhone(data.order.customerPhone);
                    clentid = (int)detail_client.Id;
                }
                //ContactClient
                var ContactClientViewModel = new ContactClientViewModel
                {
                    ClientId = clentid,
                    Name = data.order.customerName,
                    Mobile = data.order.customerPhone,
                    Email = data.order.customerEmail,
                    CreateDate = DateTime.Now,
                };
                var contactClientId = ContactClientDAL.CreateContactClients(ContactClientViewModel);
                //order
                var model_order = new ENTITIES.Models.Order();
                model_order.OrderNo = await identifierServiceRepository.buildOrderNo((int)ServicesType.FlyingTicket, (int)SourcePaymentType.b2c);
                model_order.CreateTime = DateTime.Now;
                model_order.Amount = Convert.ToInt32(data.order.totalPrice.Replace(".", string.Empty));
                model_order.Profit = data.bookings.Sum(s => s.fareData.issueFee);
                model_order.Price = model_order.Amount - model_order.Profit;
                model_order.OrderStatus = (byte?)OrderStatus.CREATED_ORDER;
                model_order.ProductService = "3";
                model_order.ServiceType = 3;
                model_order.ClientId = clentid;
                model_order.ContactClientId = contactClientId;
                model_order.IsLock = false;
                model_order.Discount = 0;
                model_order.PaymentStatus = (int)PaymentStatus.UNPAID;
                model_order.ExpriryDate = DateTime.Now;
                model_order.AccountClientId = Acclentid;
                model_order.SystemType = 2;
                model_order.CreatedBy = id_bot;
                model_order.SalerId = -1;
                model_order.UserUpdateId = id_bot;
                model_order.PercentDecrease = 0;
                model_order.SalerGroupId = "";
                model_order.Note = "";
                model_order.UtmMedium = "";
                model_order.UtmSource = "";
                model_order.BankCode = "";
                model_order.SmsContent = "";
                model_order.PaymentType = 0;
                model_order.SupplierId = 1;
                model_order.ContractId = contactClientId;
                model_order.StartDate = DateTime.ParseExact(data.bookings[0].routeInfos[0].departDate + " " + data.bookings[0].routeInfos[0].timeFrom, "dd-MM-yyyy HH:mm", null);
                if (data.bookings.Count > 1)
                {
                    model_order.Label = data.order.orderCode + "- Vé khứ hồi " + data.bookings[0].routeInfos[0].from + "-" + data.bookings[0].routeInfos[0].to;

                    model_order.EndDate = DateTime.ParseExact(data.bookings[1].routeInfos[0].departDate + " " + data.bookings[1].routeInfos[0].timeFrom, "dd-MM-yyyy HH:mm", null);
                }
                else
                {
                    if (data.bookings[0].routeInfos.Count > 1)
                    {
                        model_order.EndDate = DateTime.ParseExact(data.bookings[0].routeInfos[1].departDate + " " + data.bookings[0].routeInfos[1].timeFrom, "dd-MM-yyyy HH:mm", null);
                        model_order.Label = data.order.orderCode + "- Vé khứ hồi " + data.bookings[0].routeInfos[0].from + "-" + data.bookings[0].routeInfos[0].to;
                    }
                    else
                    {
                        model_order.Label = data.order.orderCode + "- Vé 1 chiều " + data.bookings[0].routeInfos[0].from + "-" + data.bookings[0].routeInfos[0].to;
                        model_order.EndDate = DateTime.ParseExact(data.bookings[0].routeInfos[0].departDate + " " + data.bookings[0].routeInfos[0].timeFrom, "dd-MM-yyyy HH:mm", null);
                    }
                }


                var order_id = OrderDAL.CreateOrder(model_order);
                if (order_id < 0) return -1;
                List<long> group_fly_id = new List<long>();
                var list_FlyBookingDetail = new List<FlyBookingDetailViewModel>();
                var user = userDAL.GetHeadOfDepartmentByRoleID((int)RoleType.TPDHVe);
                if (data.bookings != null && data.bookings.Count > 0)
                {
                    var dem = 0;

                    foreach (var item in data.bookings)
                    {
                        foreach (var routeInfos in item.routeInfos)
                        {

                            var FlyBookingDetail = new FlyBookingDetailViewModel
                            {
                                OrderId = order_id,
                                Status = 0,
                                BookingCode = item.pnr,
                                StartPoint = routeInfos.from,
                                StartDate = DateTime.ParseExact(routeInfos.departDate + " " + routeInfos.timeFrom, "dd-MM-yyyy HH:mm", null),
                                EndPoint = routeInfos.to,
                                EndDate = DateTime.ParseExact(routeInfos.departDate + " " + routeInfos.timeTo, "dd-MM-yyyy HH:mm", null),
                                ServiceCode = await identifierServiceRepository.buildServiceNo((int)ServicesType.FlyingTicket),
                                Session = data.order.orderCode,
                                Currency = "VND",
                                Leg = dem > 0 ? 1 : 0,
                                Airline = routeInfos.airCraft,
                                ExpiryDate = (item.timeLimit==""|| item.recordStatus== "ERROR")?DateTime.Now.AddHours(6): DateTime.ParseExact(item.timeLimit.Replace("/","-"), "dd-MM-yyyy HH:mm:ss", null),
                                OthersAmount = 0,
                                Adgcommission = 0,
                                SupplierId = 0,
                                Difference = 0,
                                Flight = routeInfos.flightNo,
                                GroupClass = routeInfos.cabinClass,
                                AdultNumber = data.order.numberOfAdult,
                                ChildNumber = data.order.numberOfChild,
                                InfantNumber = data.order.numberOfInfant,
                                FareAdt = item.routeInfos.Count > 1 ? item.fareData.fareADT - (item.fareData.issueFee / ( data.order.numberOfAdult + data.order.numberOfChild)/2) : item.fareData.fareADT - item.fareData.issueFee / (data.order.numberOfAdult + data.order.numberOfChild),
                                TaxAdt = item.fareData.taxADT,
                                FeeAdt = item.fareData.vatADT,
                                FareChd = item.routeInfos.Count > 1 ? item.fareData.fareCHD - (item.fareData.issueFee / ( data.order.numberOfAdult + data.order.numberOfChild)/2) : item.fareData.fareCHD - item.fareData.issueFee / (data.order.numberOfAdult + data.order.numberOfChild),
                                TaxChd = item.fareData.taxCHD,
                                FeeChd = item.fareData.vatCHD,
                                FareInf = item.fareData.fareINF,
                                TaxInf = item.fareData.taxINF,
                                FeeInf = item.fareData.fareINF,
                                ServiceFeeAdt = 0,
                                ServiceFeeChd = 0,
                                ServiceFeeInf = 0,
                                AmountAdt = item.routeInfos.Count > 1 ? (item.fareData.fareADT + item.fareData.taxADT + item.fareData.vatADT) * data.order.numberOfAdult : (item.fareData.fareADT + item.fareData.taxADT + item.fareData.vatADT) * data.order.numberOfAdult,
                                AmountChd = item.routeInfos.Count > 1 ? (item.fareData.fareCHD + item.fareData.taxCHD + item.fareData.vatCHD) * data.order.numberOfChild : (item.fareData.fareCHD + item.fareData.taxCHD + item.fareData.vatCHD) * data.order.numberOfChild,
                                AmountInf = (item.fareData.fareINF + item.fareData.taxINF + item.fareData.vatINF) * data.order.numberOfInfant,
                                TotalDiscount = 0,
                                TotalBaggageFee = 0,
                                TotalCommission = 0,
                                Profit = item.fareData.issueFee,
                                ProfitAdt = 0,
                                ProfitChd = 0,
                                ProfitInf = 0,
                                BookingId = 0,
                                PriceAdt = 0,
                                PriceChd = 0,
                                PriceInf = 0,
                                UpdatedBy = id_bot,
                                SalerId = user.Id,
                                Price = (item.fareData.fareADT + item.fareData.taxADT + item.fareData.vatADT) * data.order.numberOfAdult + (item.fareData.fareCHD + item.fareData.taxCHD + item.fareData.vatCHD) * data.order.numberOfChild + (item.fareData.fareINF + item.fareData.taxINF + item.fareData.vatINF) * data.order.numberOfInfant + item.fareData.otherFee,

                                Amount = (item.fareData.fareADT + item.fareData.taxADT + item.fareData.vatADT) * data.order.numberOfAdult + (item.fareData.fareCHD + item.fareData.taxCHD + item.fareData.vatCHD) * data.order.numberOfChild + (item.fareData.fareINF + item.fareData.taxINF + item.fareData.vatINF) * data.order.numberOfInfant + item.fareData.otherFee,
                                TotalNetPrice = (item.fareData.fareADT + item.fareData.taxADT + item.fareData.vatADT) * data.order.numberOfAdult + (item.fareData.fareCHD + item.fareData.taxCHD + item.fareData.vatCHD) * data.order.numberOfChild + (item.fareData.fareINF + item.fareData.taxINF + item.fareData.vatINF) * data.order.numberOfInfant + item.fareData.otherFee,


                            };
                            var airline = AirlinesDAL.getAirlinesCodes(new List<String>() { routeInfos.airCraft });
                            if (airline != null && airline.Count > 0)
                            {
                                FlyBookingDetail.SupplierId = airline[0].SupplierId;
                            }
                            var data_create_fly_book_detail = FlyBookingDetailDAL.CreateFlyBookingDetail(FlyBookingDetail);
                            list_FlyBookingDetail.Add(FlyBookingDetail);
                            group_fly_id.Add(FlyBookingDetail.Id);
                            //FlySegment

                            var seg = new FlyingSegmentViewModel();


                            seg.FlyBookingId = FlyBookingDetail.Id;
                            seg.FlightNumber = routeInfos.flightNo;
                            seg.StartPoint = routeInfos.from;
                            seg.EndPoint = routeInfos.to;
                            seg.StartTime = DateTime.ParseExact(routeInfos.departDate + " " + routeInfos.timeFrom, "dd-MM-yyyy HH:mm", null);
                            seg.EndTime = DateTime.ParseExact(routeInfos.departDate + " " + routeInfos.timeTo, "dd-MM-yyyy HH:mm", null);
                            seg.StopTime = (routeInfos.flightTime == "" || routeInfos.flightTime == null) ? 0 : ConvertToMinutes( routeInfos.flightTime);
                            seg.Class = routeInfos.Class;
                            seg.OperatingAirline = routeInfos.airCraft;
                            seg.HandBaggage = routeInfos.handBaggage == null ? "0" : routeInfos.handBaggage.description;
                            seg.Duration = (routeInfos.flightTime == "" || routeInfos.flightTime == null) ? 0 : ConvertToMinutes(routeInfos.flightTime);
                            seg.Plane = routeInfos.flightNo;


                            var data_create_fly_segment = FlightSegmentDAL.CreateFlySegment(seg);




                            dem++;
                        }


                    }
                    foreach (var fly_id in group_fly_id)
                    {
                        var FlyBooking_Detail = list_FlyBookingDetail.FirstOrDefault(s => s.Id == fly_id);
                        FlyBooking_Detail.GroupBookingId = string.Join(",", group_fly_id);
                        if (group_fly_id.Count > 1)
                        {
                            if (list_FlyBookingDetail[0].StartDate != list_FlyBookingDetail[0].StartDate)
                            {
                                if (FlyBooking_Detail.StartDate == list_FlyBookingDetail.Min(s => s.StartDate))
                                {
                                    FlyBooking_Detail.Leg = 0;
                                }
                                else
                                {
                                    FlyBooking_Detail.Leg = 1;
                                }
                            }
                        }


                        var id = FlyBookingDetailDAL.UpdateFlyBookingDetail(FlyBooking_Detail);

                    }
                    //-- Passengers:
                    foreach (var item in data.passengers)
                    {
                        var PassengerViewModel = new PassengerViewModel();
                        PassengerViewModel.Name = item.FirstName + " " + item.LastName;
                        PassengerViewModel.PersonType = item.PaxType;
                        PassengerViewModel.Gender = item.Title== "MRS"? false:true;
                        PassengerViewModel.OrderId = order_id;
                        PassengerViewModel.GroupBookingId = string.Join(",", group_fly_id);
                        var passenger = PassengerDAL.CreatePassengers(PassengerViewModel);
                        if (item.baggages != null && item.baggages.Count > 0)
                        {
                            foreach (var baggage in item.baggages)
                            {
                                var ExtraPackages = new ENTITIES.Models.FlyBookingExtraPackages();
                                ExtraPackages.GroupFlyBookingId = string.Join(",", group_fly_id);
                                ExtraPackages.PackageId = baggage.weight;
                                ExtraPackages.PackageCode = baggage.weight + "(" + baggage.segment + ")";
                                ExtraPackages.Price = Convert.ToInt32(baggage.price);
                                ExtraPackages.Amount = Convert.ToInt32(baggage.price);
                                ExtraPackages.BasePrice = Convert.ToInt32(baggage.price);
                                ExtraPackages.Profit = 0;
                                ExtraPackages.Quantity = 1;
                                ExtraPackages.CreatedBy = id_bot;
                                ExtraPackages.UpdatedBy = id_bot;
                                ExtraPackages.CreatedDate = DateTime.Now;
                                ExtraPackages.UpdatedDate = DateTime.Now;

                                FlyBookingDetailDAL.InsertFlyBookingExtraPackages(ExtraPackages);
                            }
                        }

                    }
                }

                return order_id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("saveBooking - FlyBookingMongoRepository: " + ex);
                return -1;
            }
            return -1;
        }
        public int ConvertToMinutes(string time)
        {
            // time có dạng "HH:mm"
            var parts = time.Split(':');
            int hours = int.Parse(parts[0]);
            int minutes = int.Parse(parts[1]);
            return hours * 60 + minutes;
        }
    }
}
