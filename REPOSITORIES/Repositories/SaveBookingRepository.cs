using DAL.Fly;
using DAL.Orders;
using DAL;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using DAL.MongoDB.Flight;
using Entities.ConfigModels;
using Microsoft.Extensions.Options;
using ENTITIES.ViewModels.BookingFly;
using Utilities;
using Utilities.Contants;
using System.Threading.Tasks;
using Nest;
using ENTITIES.Models;
using Caching.Elasticsearch;
using Telegram.Bot.Types.Payments;
using ENTITIES.ViewModels.Order;
using DAL.Clients;
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
            identifierServiceRepository = new IdentifierServiceRepository(dataBaseConfig);
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
                model_order.Profit = 55000;
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
                model_order.CreatedBy = 2052;
                model_order.SalerId = 2052;
                model_order.UserUpdateId = 2052;
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
                    model_order.EndDate = DateTime.ParseExact(data.bookings[1].routeInfos[0].departDate + " " + data.bookings[1].routeInfos[0].timeFrom, "dd-MM-yyyy HH:mm", null);
                }
                else
                {
                    if (data.bookings[0].routeInfos.Count > 1)
                    {
                        model_order.EndDate = DateTime.ParseExact(data.bookings[0].routeInfos[1].departDate + " " + data.bookings[0].routeInfos[1].timeFrom, "dd-MM-yyyy HH:mm", null);

                    }
                    else
                    {
                        model_order.EndDate = DateTime.ParseExact(data.bookings[0].routeInfos[0].departDate + " " + data.bookings[0].routeInfos[0].timeFrom, "dd-MM-yyyy HH:mm", null);
                    }
                }
                if (data.bookings[0].routeInfos.Count > 1)
                {
                    model_order.Label = "Vé khứ hồi " + data.bookings[0].routeInfos[0].from + "-" + data.bookings[0].routeInfos[0].to;
                }
                else
                {
                    model_order.Label = "Vé 1 chiều " + data.bookings[0].routeInfos[0].from + "-" + data.bookings[0].routeInfos[0].to;
                }

                var order_id = OrderDAL.CreateOrder(model_order);
                if (order_id < 0) return -1;
                if (data.bookings != null && data.bookings.Count > 0)
                {
                    var dem = 0;
                    List<long> group_fly_id = new List<long>();
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
                                ExpiryDate = DateTime.Now,
                                OthersAmount = 0,
                                Adgcommission = 0,
                                SupplierId = 0,
                                Difference = 0,
                                Flight = routeInfos.flightNo,
                                GroupClass = routeInfos.cabinClass,
                                AdultNumber = data.order.numberOfAdult,
                                ChildNumber = data.order.numberOfChild,
                                InfantNumber = data.order.numberOfInfant,
                                FareAdt = item.fareData.fareADT,
                                TaxAdt = item.fareData.taxADT,
                                FeeAdt = item.fareData.vatADT,
                                FareChd = item.fareData.fareADT,
                                TaxChd = item.fareData.taxCHD,
                                FeeChd = item.fareData.vatCHD,
                                FareInf = item.fareData.fareINF,
                                TaxInf = item.fareData.taxINF,
                                FeeInf = item.fareData.fareADT,                        
                                ServiceFeeAdt = 0,
                                ServiceFeeChd = 0,
                                ServiceFeeInf = 0,
                                AmountAdt = 0,
                                AmountChd = 0,
                                AmountInf = 0,
                                TotalDiscount=0,
                                TotalBaggageFee=0,
                                TotalCommission=0,
                                Profit=0,
                                ProfitAdt=0,
                                ProfitChd=0,
                                ProfitInf=0,
                                BookingId=0,
                                PriceAdt=0,
                                PriceChd = 0,
                                PriceInf = 0,
                                UpdatedBy=2052,
                                SalerId=2052,
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
                           
                            group_fly_id.Add(FlyBookingDetail.Id);
                            //FlySegment
                            var seg = new FlyingSegmentViewModel
                            {
                                FlyBookingId = FlyBookingDetail.Id,
                                FlightNumber = routeInfos.flightNo,
                                StartPoint = routeInfos.from,
                                EndPoint = routeInfos.to,
                                StartTime = DateTime.ParseExact(routeInfos.departDate + " " + routeInfos.timeFrom, "dd-MM-yyyy HH:mm", null),
                                EndTime = DateTime.ParseExact(routeInfos.departDate + " " + routeInfos.timeTo, "dd-MM-yyyy HH:mm", null),
                                StopTime = ConvertToMinutes(routeInfos.flightTime),
                                Class = routeInfos.Class,
                                OperatingAirline = routeInfos.airCraft,
                                HandBaggage = routeInfos.handBaggage.description,
                                Duration = ConvertToMinutes(routeInfos.flightTime),
                                Plane= routeInfos.flightNo,

                            };
                            var data_create_fly_segment = FlightSegmentDAL.CreateFlySegment(seg);
                            FlyBookingDetail.GroupBookingId = string.Join(",", group_fly_id);
                            //FlyBookingDetail.Note = order_info.additional.note_go + "\n" + order_info.additional.note_back;
                            var id = FlyBookingDetailDAL.UpdateFlyBookingDetail(FlyBookingDetail);
                            dem++;
                        }

                    }
                    //-- Passengers:
                    foreach (var item in data.passengers)
                    {
                        var PassengerViewModel = new PassengerViewModel();
                        PassengerViewModel.Name = item.paxName;
                        PassengerViewModel.PersonType = item.type;
                        PassengerViewModel.Gender = false;
                        PassengerViewModel.OrderId = order_id;
                        PassengerViewModel.GroupBookingId = string.Join(",", group_fly_id);
                        var passenger = PassengerDAL.CreatePassengers(PassengerViewModel);
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
