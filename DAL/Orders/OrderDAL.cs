using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.APPModels.ReadBankMessages;
using ENTITIES.Models;
using ENTITIES.ViewModels.APP.ReadBankMessages;
using ENTITIES.ViewModels.Order;
using Microsoft.EntityFrameworkCore;
using Nest;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using Utilities.Helpers;

namespace DAL.Orders
{
    public class OrderDAL : GenericService<Order>
    {
        private static DbWorker _DbWorker;


        public OrderDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }

        public Order GetDetail(long orderId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.Order.AsNoTracking().FirstOrDefault(s => s.OrderId == orderId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetail - OrderDAL: " + ex.ToString());
                return null;
            }

        }
        public async Task<List<Order>> GetListOrder(List<long> orderIds)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Order.AsNoTracking().Where(s => orderIds.Contains(s.OrderId)).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListOrder - OrderDAL: " + ex.ToString());
                return null;
            }

        }
        public Order GetOrderByOrderNo(string order_no)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.Order.AsNoTracking().FirstOrDefault(s => s.OrderNo.ToLower().Trim()== order_no.ToLower().Trim());
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetOrderByOrderNo - OrderDAL: " + ex.ToString());
                return null;
            }

        }
        public List<OrderViewModel> getOrderByOrderId(long client_id, long orderId)
        {
            try
            {
                List<OrderViewModel> ListOrder = new List<OrderViewModel>();
                using (var _DbContext = new EntityDataContext(_connection))
                {

                    var data = (from b in _DbContext.FlyBookingDetail.AsNoTracking().Where(s => s.OrderId == orderId)
                                join c in _DbContext.FlightSegment.AsNoTracking() on b.Id equals c.FlyBookingId
                                join d in _DbContext.AirPortCode.AsNoTracking() on c.StartPoint equals d.Code
                                join e in _DbContext.AirPortCode.AsNoTracking() on c.EndPoint equals e.Code
                                join f in _DbContext.Airlines.AsNoTracking() on b.Airline equals f.Code
                                select new OrderViewModel
                                {
                                    HasStop = c.HasStop,
                                    StartPoint = c.StartPoint,
                                    Startime = c.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                    EndPoint = c.EndPoint,
                                    Endtime = c.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                    StartDistrict = d.DistrictVi,
                                    EndDistrict = e.DistrictVi,
                                    FlightNumber = c.FlightNumber,
                                    Amount = b.Amount,
                                    BookingId = b.BookingId,
                                    AirlineLogo = f.Logo,
                                    Duration = c.Duration,
                                    Leg = b.Leg,
                                    AirlineName_Vi = f.NameVi,
                                });
                    if (data != null)
                    {


                        var listdata = _DbContext.FlyBookingDetail.AsNoTracking().Where(s => s.OrderId == orderId).ToList();

                        foreach (var item in data)
                        {
                            if (listdata.Count > 0)
                                if (item.Startime == ((DateTime)listdata[0].StartDate).ToString("yyyy-MM-dd HH:mm:ss"))
                                {
                                    ListOrder.Add(item);
                                }
                            if (listdata.Count > 1)
                                if (item.Startime == ((DateTime)listdata[1].StartDate).ToString("yyyy-MM-dd HH:mm:ss"))
                                {
                                    ListOrder.Add(item);
                                }
                        }
                    }
                    else
                    {
                        LogHelper.InsertLogTelegram("getOrderByOrderId - OrderDAL:100" + orderId + "  data=null");
                    }

                    return ListOrder;


                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getOrderByOrderId - OrderDAL: " + ex.ToString());
                return null;
            }
        }
        public async Task<List<List_OrderViewModel>> getOrderByClientId(long client_id)
        {

            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    List<List_OrderViewModel> datalist = new List<List_OrderViewModel>();
                    if (client_id != -1)
                    {
                        var data = (from b in _DbContext.AllCode.AsNoTracking().Where(s => s.Type == AllCodeType.ORDER_STATUS)
                                    join a in _DbContext.Order.AsNoTracking().Where(s => s.ClientId == client_id) on b.CodeValue equals (int)a.OrderStatus
                                    select new OrderDetailViewModel
                                    {
                                        AccountClientId = a.AccountClientId,
                                        ClientId = a.ClientId,
                                        OrderNo = a.OrderNo,
                                        OrderId = a.OrderId,
                                        OrderStatus = a.OrderStatus,
                                        order_status_name = b.Description,
                                        Amount = a.Amount,
                                        color_code = a.ColorCode,
                                        CreateTime = a.CreateTime,
                                        ExpiryDate = a.ExpriryDate,
                                        VoucherId = (int)a.VoucherId,
                                        PercentDecrease = (int)a.PercentDecrease,
                                        Discount = (int)a.Discount,
                                    }).Distinct().OrderByDescending(s => s.CreateTime).ToList();
                        //for (int i = 0; i < (data.Count-1); i++)
                        //{
                        //    if (data[i].OrderId == data[i + 1].OrderId)
                        //    {
                        //        if ((DateTime)data[i].ExpiryDate > (DateTime)data[i + 1].ExpiryDate)
                        //        {
                        //            data.Remove(data[i]);
                        //        }
                        //        else
                        //        {
                        //            data.Remove(data[i + 1]);
                        //        }

                        //    }
                        //}
                        if (data != null)
                        {
                            foreach (var item in data)
                            {
                                List_OrderViewModel a = new List_OrderViewModel();

                                if (item.OrderStatus == (int)OrderStatus.CREATED_ORDER || item.OrderStatus == (int)OrderStatus.CONFIRMED_SALE)
                                {

                                    if ((DateTime)item.ExpiryDate < DateTime.Now)
                                    {
                                        Order order = _DbContext.Order.FirstOrDefault(s => s.OrderId == item.OrderId);
                                        order.OrderStatus = (byte?)OrderStatus.CANCEL;
                                        var updata = _DbContext.Order.Update(order);
                                        await _DbContext.SaveChangesAsync();
                                        a.ClientId = item.ClientId;
                                        a.OrderNo = item.OrderNo;
                                        a.OrderId = item.OrderId;
                                        a.OrderStatus = (byte?)OrderStatus.CANCEL;
                                        a.order_status_name = CommonHelper.GetDescriptionFromEnumValue(OrderStatus.CANCEL);
                                        a.OrderAmount = item.Amount;
                                        a.color_code = item.color_code;
                                        a.ExpiryDate = item.ExpiryDate;
                                        a.list_Order = getOrderByOrderId((long)item.ClientId, (long)item.OrderId);
                                        a.VoucherId = item.VoucherId;
                                        a.PercentDecrease = item.PercentDecrease;
                                        a.Discount = (int)item.Discount;
                                        datalist.Add(a);
                                    }
                                    else
                                    {
                                        a.ClientId = item.ClientId;
                                        a.OrderNo = item.OrderNo;
                                        a.OrderId = item.OrderId;
                                        a.OrderStatus = item.OrderStatus;
                                        a.order_status_name = item.order_status_name;
                                        a.OrderAmount = item.Amount;
                                        a.color_code = item.color_code;
                                        a.ExpiryDate = item.ExpiryDate;
                                        a.list_Order = getOrderByOrderId((long)item.ClientId, (long)item.OrderId);
                                        a.VoucherId = item.VoucherId;
                                        a.PercentDecrease = item.PercentDecrease;
                                        a.Discount = (int)item.Discount;
                                        datalist.Add(a);
                                    }
                                }
                                else
                                {
                                    a.ClientId = item.ClientId;
                                    a.OrderNo = item.OrderNo;
                                    a.OrderId = item.OrderId;
                                    a.OrderStatus = item.OrderStatus;
                                    a.order_status_name = item.order_status_name;
                                    a.OrderAmount = item.Amount;
                                    a.color_code = item.color_code;
                                    a.ExpiryDate = item.ExpiryDate;
                                    a.list_Order = getOrderByOrderId((long)item.ClientId, (long)item.OrderId);
                                    a.VoucherId = item.VoucherId;
                                    a.PercentDecrease = item.PercentDecrease;
                                    a.Discount = (int)item.Discount;
                                    datalist.Add(a);
                                }


                            }
                        }
                        else
                        {
                            LogHelper.InsertLogTelegram("getOrderByClientId - OrderDAL: 222 client_id:" + client_id + " data=null ");
                        }
                    }
                    else
                    {
                        var data = (from a in _DbContext.Order.AsNoTracking().OrderByDescending(x => x.CreateTime)
                                    join b in _DbContext.AccountClient.AsNoTracking() on a.ClientId equals b.ClientId
                                    join f in _DbContext.Client.AsNoTracking() on a.ClientId equals f.Id
                                    join c in _DbContext.AllCode.AsNoTracking().Where(s => s.Type == AllCodeType.ORDER_STATUS) on (int)a.OrderStatus equals c.CodeValue
                                    select new List_OrderViewModel
                                    {
                                        ClientId = a.ClientId,
                                        OrderNo = a.OrderNo,
                                        OrderId = a.OrderId,
                                        OrderStatus = a.OrderStatus,
                                        order_status_name = c.Description,
                                        OrderAmount = a.Amount,
                                        color_code = a.ColorCode,
                                        clientName = f.ClientName,
                                        email = f.Email,
                                        phone = f.Phone,
                                        Profit = a.Profit,
                                        UserName = b.UserName,
                                        PaymentDate = ((DateTime)a.PaymentDate).ToString("yyyy-MM-dd HH:mm:ss"),
                                        CreateTime = ((DateTime)a.CreateTime).ToString("yyyy-MM-dd HH:mm:ss"),
                                        ExpiryDate = a.ExpriryDate,
                                        VoucherId = Convert.ToInt32(a.VoucherId),
                                        PercentDecrease = (int)a.PercentDecrease,
                                        Discount = (int)a.Discount,
                                    }).Take(200).ToList();
                        foreach (var item in data)
                        {
                            item.list_Order = getOrderByOrderId((long)item.ClientId, (long)item.OrderId);
                        }
                        datalist = data;
                    }
                    return datalist;
                }
            }
            catch (Exception ex)
            {

                LogHelper.InsertLogTelegram("getOrderByClientId - OrderDAL: " + ex.ToString());
                return null;
            }
        }

        public async Task<List<List_OrderViewModel>> getOrderByOrderIdPagingList(int PageSize, int pageNumb, long client_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    List<List_OrderViewModel> datalist = new List<List_OrderViewModel>();

                    var data = (from a in _DbContext.Order.AsNoTracking().OrderByDescending(x => x.CreateTime)
                                join b in _DbContext.AccountClient.AsNoTracking() on a.ClientId equals b.ClientId
                                join f in _DbContext.Client.AsNoTracking() on a.ClientId equals f.Id
                                join c in _DbContext.AllCode.AsNoTracking().Where(s => s.Type == AllCodeType.ORDER_STATUS) on (int)a.OrderStatus equals c.CodeValue
                                select new List_OrderViewModel
                                {
                                    ClientId = a.ClientId,
                                    OrderNo = a.OrderNo,
                                    OrderId = a.OrderId,
                                    OrderStatus = a.OrderStatus,
                                    order_status_name = c.Description,
                                    OrderAmount = a.Amount,
                                    color_code = a.ColorCode,
                                    clientName = f.ClientName,
                                    email = f.Email,
                                    phone = f.Phone,
                                    Profit = a.Profit,
                                    UserName = b.UserName,
                                    PaymentDate = ((DateTime)a.PaymentDate).ToString("yyyy-MM-dd HH:mm:ss"),
                                    CreateTime = ((DateTime)a.CreateTime).ToString("yyyy-MM-dd HH:mm:ss"),

                                }).Skip(PageSize * (pageNumb - 1)).Take(PageSize).ToList();
                    var list_status = new List<int>() { (int)OrderStatus.ACCOUNTANT_DECLINE, (int)OrderStatus.CONFIRMED_SALE, (int)OrderStatus.OPERATOR_DECLINE, (int)OrderStatus.WAITING_FOR_ACCOUNTANT, (int)OrderStatus.WAITING_FOR_OPERATOR, (int)OrderStatus.CONFIRMED_SALE };
                    foreach (var item in data)
                    {
                        item.list_Order = getOrderByOrderId((long)item.ClientId, (long)item.OrderId);
                        if (list_status.Contains((int)item.OrderStatus))
                        {
                            item.order_status_name = "Đang xử lý";
                        }
                    }
                    return data;
                }
            }
            catch (Exception ex)
            {

                LogHelper.InsertLogTelegram("getOrderByOrderIdPagingList - OrderDAL: " + ex.ToString());
                return null;
            }
        }
        public async Task<List<OrderDetailViewModel>> getOrderDetail(long order_id, long client_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var Update = UpdateOrderExprire(order_id);

                    var listPassenger = _DbContext.Passenger.AsNoTracking().Where(s => s.OrderId == order_id).ToList();

                    var data = (from a in _DbContext.Order.AsNoTracking().Where(s => s.OrderId == order_id && s.ClientId == client_id)
                                join b in _DbContext.FlyBookingDetail.AsNoTracking() on a.OrderId equals b.OrderId
                                join f in _DbContext.ContactClient.AsNoTracking() on a.ContactClientId equals f.Id
                                join h in _DbContext.Airlines.AsNoTracking() on b.Airline equals h.Code
                                join k in _DbContext.AllCode.AsNoTracking().Where(s => s.Type == AllCodeType.PAYMENT_TYPE) on a.PaymentType equals k.CodeValue
                                join j in _DbContext.AllCode.AsNoTracking().Where(s => s.Type == AllCodeType.PAYMENT_STATUS) on a.PaymentStatus equals j.CodeValue
                                select new OrderDetailViewModel
                                {
                                    Name = f.Name,
                                    Phone = f.Mobile,
                                    Email = f.Email,
                                    OrderId = a.OrderId,
                                    OrderNo = a.OrderNo,
                                    ClientId = a.ClientId,
                                    AirlineLogo = h.Logo,
                                    AirlineName_Vi = h.NameVi,
                                    FlyBookingID = b.Id,
                                    BookingCode = b.BookingCode,
                                    CreateTime = a.CreateTime,
                                    PaymentType = a.PaymentType,
                                    PaymentTypeName = k.Description,
                                    PaymentStatus = a.PaymentStatus,
                                    OrderStatus = a.OrderStatus,
                                    Amount = b.Amount,
                                    BookingId = b.BookingId,
                                    Leg = b.Leg,
                                    PaymentStatusName = j.Description,
                                    Sessionid = b.Session,
                                    VoucherId = Convert.ToInt32(a.VoucherId),
                                    Price = Convert.ToInt32(a.Price),
                                    PercentDecrease = (int)a.PercentDecrease,
                                    Discount = (int)a.Discount,
                                    Passenger = (List<Passenger>)listPassenger
                                }).OrderByDescending(x => x.CreateTime).ToList();

                    if (data.Count > 1)
                    {
                        if (data[0].Leg != 0)
                        {
                            OrderDetailViewModel a = data[0];
                            data[0] = data[1];
                            data[1] = a;
                        }

                    }
                    var listdata = (from a in data
                                    join c in _DbContext.FlightSegment.AsNoTracking() on a.FlyBookingID equals c.FlyBookingId
                                    join d in _DbContext.AirPortCode.AsNoTracking() on c.StartPoint equals d.Code
                                    join e in _DbContext.AirPortCode.AsNoTracking() on c.EndPoint equals e.Code
                                    select new OrderDetailViewModel
                                    {
                                        Name = a.Name,
                                        Phone = a.Phone,
                                        Email = a.Email,
                                        OrderId = a.OrderId,
                                        OrderNo = a.OrderNo,
                                        ClientId = a.ClientId,
                                        AirlineLogo = a.AirlineLogo,
                                        AirlineName_Vi = a.AirlineName_Vi,
                                        Duration = c.Duration,
                                        BookingCode = a.BookingCode,
                                        CreateTime = a.CreateTime,
                                        PaymentType = a.PaymentType,
                                        PaymentTypeName = a.PaymentTypeName,
                                        StartPoint = c.StartPoint,
                                        Startime = c.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                        EndPoint = c.EndPoint,
                                        Endtime = c.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                        StartDistrict = d.DistrictVi,
                                        EndDistrict = e.DistrictVi,
                                        FlightNumber = c.FlightNumber,
                                        PaymentStatus = a.PaymentStatus,
                                        OrderStatus = a.OrderStatus,
                                        Amount = a.Amount,
                                        BookingId = a.BookingId,
                                        Leg = a.Leg,
                                        PaymentStatusName = a.PaymentStatusName,
                                        Sessionid = a.Sessionid,
                                        Passenger = a.Passenger,
                                        VoucherId = a.VoucherId,
                                        Price = a.Price,
                                        PercentDecrease = a.PercentDecrease,
                                        Discount = a.Discount,
                                        is_lock = a.OrderStatus == (byte?)OrderStatus.CANCEL ? 1 : 0,
                                    }).ToList();

                    return listdata;
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getOrderDetail - OrderDAL: " + ex);
                return null;
            }
        }

        public OrderViewAPIdetail getOrderOrderViewdetail(long order_id, int type)
        {
            try
            {
                OrderViewAPIdetail orderViewdetail = null;
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    switch (type)
                    {

                        case (int)ServicesType.FlyingTicket:
                            {
                                var listPassenger = (from a in _DbContext.Passenger.AsNoTracking().Where(s => s.OrderId == order_id)
                                                     select new ListPassenger
                                                     {
                                                         Id = a.Id,
                                                         Birthday = a.Birthday,
                                                         Gender = a.Gender,
                                                         MembershipCard = a.MembershipCard,
                                                         Name = a.Name,
                                                         OrderId = a.OrderId,
                                                         PersonType = a.PersonType,
                                                     }).ToList();
                                foreach (var item in listPassenger)
                                {
                                    if (item.Gender == true)
                                    {
                                        item.Gender_Name = "Nam";
                                    }
                                    if (item.Gender == false)
                                    {
                                        item.Gender_Name = "Nữ";
                                    }
                                    switch (item.PersonType.ToLower())
                                    {
                                        case "adt":
                                            {
                                                item.Person_Type_Name = "Người lớn";
                                                break;
                                            }
                                        case "chd":
                                            {
                                                item.Person_Type_Name = "Trẻ em";
                                                break;
                                            }
                                        case "inf":
                                            {
                                                item.Person_Type_Name = "Trẻ sơ sinh";
                                                break;
                                            }
                                        default:
                                            {
                                                break;
                                            }
                                    }
                                }
                                var ListPayment = (from a in _DbContext.Payment.AsNoTracking().Where(s => s.OrderId == order_id)
                                                   join b in _DbContext.AllCode.AsNoTracking().Where(s => s.Type == AllCodeType.PAYMENT_TYPE) on a.PaymentType equals b.CodeValue
                                                   select new ListPayment
                                                   {
                                                       Id = a.Id,
                                                       PaymentDate = a.PaymentDate.ToString("dd/MM/yyyy HH:mm:ss"),
                                                       Amount = a.Amount,
                                                       CreatedOn = a.CreatedOn,
                                                       ModifiedOn = a.ModifiedOn,
                                                       Note = a.Note,
                                                       OrderId = a.OrderId,
                                                       PaymentType = a.PaymentType,
                                                       Payment_Type_Name = b.Description,
                                                       ClientId = a.ClientId,
                                                       TransferContent = a.TransferContent,
                                                   }).ToList();
                                double Sum = 0;
                                if (ListPayment != null && ListPayment.Count > 0)
                                    Sum = ListPayment.Sum(x => x.Amount);
                                var listbooking = (from a in _DbContext.FlyBookingDetail.AsNoTracking().Where(s => s.OrderId == order_id)
                                                   join b in _DbContext.FlightSegment.AsNoTracking() on a.Id equals b.FlyBookingId
                                                   join c in _DbContext.Airlines.AsNoTracking() on a.Airline equals c.Code
                                                   join d in _DbContext.AirPortCode.AsNoTracking() on b.StartPoint equals d.Code
                                                   join e in _DbContext.AirPortCode.AsNoTracking() on b.EndPoint equals e.Code
                                                   join f in _DbContext.Order.AsNoTracking().Where(s => s.OrderId == order_id) on a.OrderId equals f.OrderId
                                                   select new Bookingdetail
                                                   {
                                                       OrderId = a.OrderId,
                                                       EndPoint = b.EndPoint,
                                                       EndDistrict = e.DistrictVi,
                                                       StartPoint = b.StartPoint,
                                                       StartDistrict = d.DistrictVi,
                                                       Leg = a.Leg,
                                                       AirlineLogo = c.Logo,
                                                       AirlineName_Vi = c.NameVi,
                                                       FlightNumber = b.FlightNumber,
                                                       BookingCode = a.BookingCode,
                                                       Amount = (double)f.Amount,
                                                       Discount = (double)f.Discount,
                                                       StartTime = b.StartTime.ToString("dd/MM/yyyy HH:mm:ss"),
                                                       EndTime = b.EndTime.ToString("dd/MM/yyyy HH:mm:ss"),
                                                   }).ToList();

                                var data = (from a in _DbContext.Order.AsNoTracking().Where(s => s.OrderId == order_id)
                                            join b in _DbContext.Client.AsNoTracking() on a.ClientId equals b.Id
                                            join c in _DbContext.AllCode.AsNoTracking().Where(s => s.Type == AllCodeType.ORDER_STATUS) on (int)a.OrderStatus equals c.CodeValue
                                            join d in _DbContext.AccountClient.AsNoTracking() on a.ClientId equals d.ClientId
                                            join e in _DbContext.AllCode.AsNoTracking().Where(s => s.Type == AllCodeType.PAYMENT_STATUS) on a.PaymentStatus equals e.CodeValue

                                            select new OrderViewAPIdetail
                                            {
                                                OrderId = a.OrderId,
                                                UserUpdateId = a.UserUpdateId,
                                                ClientId = a.ClientId,
                                                AccountClientId = a.AccountClientId,
                                                OrderNo = a.OrderNo,
                                                ExpriryDate = ((DateTime)a.ExpriryDate).ToString("dd/MM/yyyy HH:mm:ss"),
                                                OrderStatus = a.OrderStatus,
                                                CreateTime = ((DateTime)a.CreateTime).ToString("dd/MM/yyyy HH:mm:ss"),
                                                PaymentDate = ((DateTime)a.PaymentDate).ToString("dd/MM/yyyy HH:mm:ss"),
                                                UpdateLast = ((DateTime)a.UpdateLast).ToString("dd/MM/yyyy HH:mm:ss"),
                                                Amount = a.Price,
                                                order_status_name = c.Description,
                                                PaymentStatus = a.PaymentStatus,
                                                Profit = a.Profit,
                                                ClientName = b.ClientName,
                                                Phone = b.Phone,
                                                Email = b.Email,
                                                UserName = d.UserName,
                                                PaymentNo = a.PaymentNo,
                                                Payment_Status_name = e.Description,
                                                ListPayment = ListPayment,
                                                Passenger = listPassenger,
                                                bookingdetail = listbooking,
                                                PaymentAmount = (int)Sum,
                                                Price = (double)a.Price,
                                                Discount = (double)a.Discount,
                                                VoucherId = (int)a.VoucherId,
                                                OrderAmount = (double)a.Amount,

                                            }).ToList();
                                orderViewdetail = data[0];
                            }
                            break;
                        case (int)ServicesType.VinWonderTicket:
                            {
                                var VinWonderBooking = (from a in _DbContext.VinWonderBooking.AsNoTracking().Where(s => s.OrderId == order_id)
                                                        select new vinWonderdetail
                                                        {
                                                            OrderId = a.OrderId,
                                                            BookingId = a.Id,
                                                            SiteName = a.SiteName,
                                                        }).ToList();
                                if (VinWonderBooking != null && VinWonderBooking.Count > 0)
                                {
                                    foreach (var item in VinWonderBooking)
                                    {
                                        var VinWonderBookingTicket = (from a in _DbContext.VinWonderBookingTicket.AsNoTracking().Where(s => s.BookingId == item.BookingId)
                                                                      join b in _DbContext.VinWonderBookingTicketDetail.AsNoTracking() on a.Id equals b.BookingTicketId
                                                                      select new VinWonderBookingTicketViewModel
                                                                      {
                                                                          DateUsed = Convert.ToDateTime(a.DateUsed).ToString("dd/MM/yyyy"),
                                                                          adt = (int)a.Adt,
                                                                          child = (int)a.Child,
                                                                          old = (int)a.Old,
                                                                          BookingTicketId = a.Id,
                                                                          Name=b.Name,
                                                                      }).ToList();

                                        
                                        item.VinWonderBookingTicket = VinWonderBookingTicket;
                                    }
                                }
                                var data = (from a in _DbContext.Order.AsNoTracking().Where(s => s.OrderId == order_id)
                                            join b in _DbContext.Client.AsNoTracking() on a.ClientId equals b.Id
                                            join c in _DbContext.AllCode.AsNoTracking().Where(s => s.Type == AllCodeType.ORDER_STATUS) on (int)a.OrderStatus equals c.CodeValue
                                            join d in _DbContext.AccountClient.AsNoTracking() on a.ClientId equals d.ClientId
                                            join e in _DbContext.AllCode.AsNoTracking().Where(s => s.Type == AllCodeType.PAYMENT_STATUS) on a.PaymentStatus equals e.CodeValue

                                            select new OrderViewAPIdetail
                                            {
                                                OrderId = a.OrderId,
                                                UserUpdateId = a.UserUpdateId,
                                                ClientId = a.ClientId,
                                                AccountClientId = a.AccountClientId,
                                                OrderNo = a.OrderNo,
                                                ExpriryDate = ((DateTime)a.ExpriryDate).ToString("dd/MM/yyyy HH:mm:ss"),
                                                OrderStatus = a.OrderStatus,
                                                CreateTime = ((DateTime)a.CreateTime).ToString("dd/MM/yyyy HH:mm:ss"),
                                                PaymentDate = ((DateTime)a.PaymentDate).ToString("dd/MM/yyyy HH:mm:ss"),
                                                UpdateLast = ((DateTime)a.UpdateLast).ToString("dd/MM/yyyy HH:mm:ss"),
                                                Amount = a.Price,
                                                order_status_name = c.Description,
                                                PaymentStatus = a.PaymentStatus,
                                                Profit = a.Profit,
                                                ClientName = b.ClientName,
                                                Phone = b.Phone,
                                                Email = b.Email,
                                                UserName = d.UserName,
                                                PaymentNo = a.PaymentNo,
                                                Payment_Status_name = e.Description,
                                                vinWonderdetail = VinWonderBooking,
                                                Price = (double)a.Price,
                                                Discount = (double)a.Discount,
                                                VoucherId = (int)a.VoucherId,
                                                OrderAmount = (double)a.Amount,

                                            }).ToList();
                                orderViewdetail = data[0];
                            }
                            break;
                    }

                    return orderViewdetail;
                }
            }
            catch (Exception ex)
            {

                LogHelper.InsertLogTelegram("getOrderOrderViewdetail - OrderDAL: " + ex);
                return null;
            }
        }


        public async Task<long> CreateOrderNo(string order_no)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var order = new Order()
                    {
                        CreateTime = DateTime.Now,
                        OrderNo = order_no
                    };
                    _DbContext.Order.Add(order);
                    await _DbContext.SaveChangesAsync();
                    return order.OrderId;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateOrderNo - OrderDAL: " + ex.ToString());
                return -1;
            }
        }

        public async Task<string> getOrderNoByOrderId(long order_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {

                    var data = _DbContext.Order.AsNoTracking().FirstOrDefault(s => s.OrderId == order_id);
                    return data.OrderNo;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getOrderNoByOrderId - OrderDAL: " + ex.ToString());
                return "";
            }
        }



        public async Task<List<OrderDetailViewModel>> getOrderDetailBySessionId(string session_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var flyBookingDetails = _DbContext.FlyBookingDetail.AsNoTracking().FirstOrDefault(s => s.Session == session_id);

                    if (flyBookingDetails != null)
                    {
                        var listPassenger = _DbContext.Passenger.AsNoTracking().Where(s => s.OrderId == flyBookingDetails.OrderId).ToList();

                        var data = (from b in _DbContext.FlyBookingDetail.AsNoTracking().Where(s => s.Session == session_id)
                                    join a in _DbContext.Order.AsNoTracking() on b.OrderId equals a.OrderId
                                    join f in _DbContext.ContactClient.AsNoTracking() on a.ContactClientId equals f.Id
                                    join h in _DbContext.Airlines.AsNoTracking() on b.Airline equals h.Code
                                    join k in _DbContext.AllCode.AsNoTracking().Where(s => s.Type == AllCodeType.PAYMENT_TYPE) on a.PaymentType equals k.CodeValue
                                    join j in _DbContext.AllCode.AsNoTracking().Where(s => s.Type == AllCodeType.PAYMENT_STATUS) on a.PaymentStatus equals j.CodeValue
                                    select new OrderDetailViewModel
                                    {
                                        Name = f.Name,
                                        Phone = f.Mobile,
                                        Email = f.Email,
                                        OrderId = a.OrderId,
                                        OrderNo = a.OrderNo,
                                        ClientId = a.ClientId,
                                        AirlineLogo = h.Logo,
                                        AirlineName_Vi = h.NameVi,
                                        FlyBookingID = b.Id,
                                        BookingCode = b.BookingCode,
                                        CreateTime = a.CreateTime,
                                        PaymentType = a.PaymentType,
                                        PaymentTypeName = k.Description,
                                        PaymentStatus = a.PaymentStatus,
                                        OrderStatus = a.OrderStatus,
                                        Amount = b.Amount,
                                        BookingId = b.BookingId,
                                        Leg = b.Leg,
                                        PaymentStatusName = j.Description,
                                        Sessionid = b.Session,
                                        VoucherId = (int)a.VoucherId,
                                        Discount = (double)a.Discount,
                                        Price = (double)a.Price,
                                        Passenger = (List<Passenger>)listPassenger
                                    }).OrderByDescending(x => x.CreateTime).ToList();

                        if (data.Count > 1)
                        {
                            if (data[0].Leg != 0)
                            {
                                OrderDetailViewModel a = data[0];
                                data[0] = data[1];
                                data[1] = a;
                            }

                        }
                        var listdata = (from a in data
                                        join c in _DbContext.FlightSegment.AsNoTracking() on a.FlyBookingID equals c.FlyBookingId
                                        join d in _DbContext.AirPortCode.AsNoTracking() on c.StartPoint equals d.Code
                                        join e in _DbContext.AirPortCode.AsNoTracking() on c.EndPoint equals e.Code
                                        select new OrderDetailViewModel
                                        {
                                            Name = a.Name,
                                            Phone = a.Phone,
                                            Email = a.Email,
                                            OrderId = a.OrderId,
                                            OrderNo = a.OrderNo,
                                            ClientId = a.ClientId,
                                            AirlineLogo = a.AirlineLogo,
                                            AirlineName_Vi = a.AirlineName_Vi,
                                            Duration = c.Duration,
                                            BookingCode = a.BookingCode,
                                            CreateTime = a.CreateTime,
                                            PaymentType = a.PaymentType,
                                            PaymentTypeName = a.PaymentTypeName,
                                            StartPoint = c.StartPoint,
                                            Startime = c.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                            EndPoint = c.EndPoint,
                                            Endtime = c.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                            StartDistrict = d.DistrictVi,
                                            EndDistrict = e.DistrictVi,
                                            FlightNumber = c.FlightNumber,
                                            PaymentStatus = a.PaymentStatus,
                                            OrderStatus = a.OrderStatus,
                                            Amount = a.Amount,
                                            BookingId = a.BookingId,
                                            Leg = a.Leg,
                                            PaymentStatusName = a.PaymentStatusName,
                                            Sessionid = a.Sessionid,
                                            Passenger = a.Passenger,
                                            VoucherId = a.VoucherId,
                                            Price = a.Price,
                                            Discount = (double)a.Discount,
                                        }).ToList();

                        return listdata;
                    }
                    return null;
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getOrderDetailBySessionId - OrderDAL: " + ex.ToString());
                return null;
            }
        }
        public async Task<bool> UpdateOrderExprire(long order_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    Order orders = await _DbContext.Order.FirstOrDefaultAsync(x => x.OrderId == order_id);
                    if (orders != null && orders.OrderId > 0 && orders.ExpriryDate <= DateTime.Now)
                    {
                        orders.OrderStatus = (int)OrderStatus.CANCEL;
                        _DbContext.Update(orders);
                        await _DbContext.SaveChangesAsync();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateOrderExprire - OrderDAL: " + ex.ToString());
                return false;
            }
        }


        public async Task<long> CountOrderInYear()
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.Order.AsNoTracking().Where(x => (x.CreateTime ?? DateTime.Now).Year == DateTime.Now.Year).Count();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CountOrderInYear - OrderDAL: " + ex.ToString());
                return -1;
            }
        }
        public async Task<long> CountOrderBySystemTypeInYear(int system_type)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.Order.AsNoTracking().Where(x => (x.CreateTime ?? DateTime.Now).Year == DateTime.Now.Year && x.SystemType == system_type).Count();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CountOrderBySystemTypeInYear - OrderDAL: " + ex.ToString());
                return -1;
            }
        }
        public async Task<string> getOrderNoByOrderNo(string order_no)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {

                    var data = _DbContext.Order.AsNoTracking().FirstOrDefault(s => s.OrderNo == order_no);
                    return data == null ? "" : data.OrderNo;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getOrderNoByOrderNo - OrderDAL: " + ex);
                return "";
            }
        }

        public async Task<bool> UpdateBookingInfoByOrderId(long order_id, string j_booking_info)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    Order orders = await _DbContext.Order.FirstOrDefaultAsync(x => x.OrderId == order_id);
                    if (orders != null)
                    {
                        orders.BookingInfo = j_booking_info;
                        _DbContext.Update(orders);
                        await _DbContext.SaveChangesAsync();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateBookingInfoByOrderId - OrderDAL: " + ex.ToString());
                return false;
            }
        }
        public async Task<HotelBookingB2BPagingViewModel> GetHotelOrderB2BPaging(int PageSize, int pageNumb, string account_client_id, DateTime start_date, DateTime end_date)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    HotelBookingB2BPagingViewModel datalist = new HotelBookingB2BPagingViewModel();
                    datalist.data = GetOrderB2BPagingSP(PageSize, pageNumb, account_client_id, (int)ServicesType.VINHotelRent, start_date, end_date);
                    datalist.page = pageNumb;
                    datalist.total_record = datalist.data != null && datalist.data.Count > 0 ? datalist.data[0].TotalRecord : 0;
                    await UpdateOrderDetail(datalist.data);
                    return datalist;
                }
            }
            catch (Exception ex)
            {

                LogHelper.InsertLogTelegram("GetHotelOrderB2BPaging - OrderDAL: " + ex.ToString());
                return null;
            }
        }
        public async Task<HotelB2BOrderDetailViewModel> GetHotelOrderDetailB2B(long order_id)
        {
            HotelB2BOrderDetailViewModel result = new HotelB2BOrderDetailViewModel();
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    result.order = _DbContext.Order.AsNoTracking().FirstOrDefault(s => s.OrderId == order_id);
                    result.booking = _DbContext.HotelBooking.AsNoTracking().FirstOrDefault(s => s.OrderId == order_id);
                    result.contact = _DbContext.ContactClient.AsNoTracking().FirstOrDefault(s => s.Id == result.order.ContactClientId);
                    result.rooms = new List<HotelB2BOrderDetailRooms>();
                    var r_list = await _DbContext.HotelBookingRooms.AsNoTracking().Where(s => s.HotelBookingId == result.booking.Id).ToListAsync();
                    foreach (var room in r_list)
                    {
                        result.rooms.Add(new HotelB2BOrderDetailRooms()
                        {
                            detail = room,
                            rates = await _DbContext.HotelBookingRoomRates.Where(s => s.HotelBookingRoomId == room.Id).ToListAsync(),
                            guest = await _DbContext.HotelGuest.Where(s => s.HotelBookingRoomsId == room.Id).ToListAsync(),
                            packages = await _DbContext.HotelBookingRoomExtraPackages.Where(s => s.HotelBookingRoomId == room.Id).ToListAsync(),
                        });
                    }
                    result.extras = await _DbContext.HotelBookingRoomExtraPackages.Where(s => s.HotelBookingRoomId == 0 && s.HotelBookingId == result.booking.Id).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelOrderDetailB2B - OrderDAL: " + ex.ToString());
            }
            return result;

        }

        private List<HotelBookingB2BViewModel> GetOrderB2BPagingSP(int PageSize, int pageNumb, string account_client_id, int service_type, DateTime start_date, DateTime end_date)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[6];
                objParam[0] = new SqlParameter("@AccountClientId", account_client_id);
                objParam[1] = new SqlParameter("@PageIndex", pageNumb);
                objParam[2] = new SqlParameter("@PageSize", PageSize);
                objParam[3] = new SqlParameter("@ServiceType", service_type);
                if (start_date == DateTime.MinValue)
                {
                    objParam[4] = new SqlParameter("@StartDate", DBNull.Value);
                }
                else
                {
                    objParam[4] = new SqlParameter("@StartDate", start_date);

                }
                if (end_date == DateTime.MinValue)
                {
                    objParam[5] = new SqlParameter("@EndDate", DBNull.Value);
                }
                else
                {
                    objParam[5] = new SqlParameter("@EndDate", end_date);
                }
                DataTable tb = new DataTable();
                _DbWorker.Fill(tb, StoreProceduresName.SP_GetHotelOrderByAccountClientId, objParam);

                return tb.ToList<HotelBookingB2BViewModel>();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetOrderB2BPagingSP - OrderDAL: " + ex.ToString());
                return new List<HotelBookingB2BViewModel>();
            }

        }
        /// <summary>
        /// Lấy ra số lần voucher được dùng. Trừ những đơn HỦY
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<int> GetTotalVoucherUse(long voucher_id, long account_client_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = _DbContext.Order.AsNoTracking().Where(x => x.VoucherId == voucher_id && x.AccountClientId == account_client_id);

                    return detail.Count();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTotalVoucherUse - OrderDAL:(account_client_id = " + account_client_id + ", voucher_id =" + voucher_id + ") " + ex);
                return -1;
            }
        }
        private async Task UpdateOrderDetail(List<HotelBookingB2BViewModel> list)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var all_code = _DbContext.AllCode.AsNoTracking().Where(s => s.Type == AllCodeType.ORDER_STATUS);
                    foreach (var order in list)
                    {

                        if (order.OrderStatus == (int)OrderStatus.CONFIRMED_SALE || order.OrderStatus == (int)OrderStatus.CREATED_ORDER)
                        {
                            var exists = _DbContext.Order.FirstOrDefault(s => s.OrderId == order.OrderId);
                            if ((DateTime)exists.ExpriryDate < DateTime.Now)
                            {
                                order.OrderStatus = (int)OrderStatus.CANCEL;
                                order.HotelBookingStatusName = all_code == null || all_code.Count() < 1 ? order.HotelBookingStatusName : all_code.Where(x => x.CodeValue == order.OrderStatus).First().Description;
                                exists.OrderStatus = (int)OrderStatus.CANCEL;
                                var updata = _DbContext.Order.Update(exists);
                                await _DbContext.SaveChangesAsync();

                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateOrderDetail - OrderDAL: " + ex);
            }

        }
        public async Task<int> UpdateOrderStatus(long OrderId, long Status, long UpdatedBy, long UserVerify)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[4];
                objParam[0] = new SqlParameter("@OrderId", OrderId);
                objParam[1] = new SqlParameter("@Status", Status);
                objParam[2] = new SqlParameter("@UpdatedBy", UpdatedBy);
                objParam[3] = UserVerify == 0 ? new SqlParameter("@UserVerify", DBNull.Value) : new SqlParameter("@UserVerify", UserVerify);

                return _DbWorker.ExecuteNonQuery(StoreProceduresName.SP_UpdateOrderStatus, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailOrderServiceByOrderId - OrderDal: " + ex);
            }
            return 0;
        }
        public async Task<List<OrderVinWonderDetailViewModel>> getOrderVinWonderDetail(long order_id, long client_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var listPassenger = _DbContext.Passenger.AsNoTracking().Where(s => s.OrderId == order_id).ToList();
                    var data = (from a in _DbContext.Order.AsNoTracking().Where(s => s.OrderId == order_id && s.ClientId == client_id)
                                join f in _DbContext.ContactClient.AsNoTracking() on a.ContactClientId equals f.Id
                                join k in _DbContext.AllCode.AsNoTracking().Where(s => s.Type == AllCodeType.PAYMENT_TYPE) on a.PaymentType equals k.CodeValue
                                join j in _DbContext.AllCode.AsNoTracking().Where(s => s.Type == AllCodeType.PAYMENT_STATUS) on a.PaymentStatus equals j.CodeValue
                                select new OrderVinWonderDetailViewModel
                                {
                                    Name = f.Name,
                                    Phone = f.Mobile,
                                    Email = f.Email,
                                    OrderId = a.OrderId,
                                    OrderNo = a.OrderNo,
                                    ClientId = a.ClientId,
                                    CreateTime = a.CreateTime,
                                    PaymentType = a.PaymentType,
                                    PaymentTypeName = k.Description,
                                    PaymentStatus = a.PaymentStatus,
                                    OrderStatus = a.OrderStatus,
                                    PaymentStatusName = j.Description,
                                    VoucherId = Convert.ToInt32(a.VoucherId),
                                    Price = Convert.ToInt32(a.Price),
                                    Amount=(double)a.Amount,
                                    Discount = (int)a.Discount,
                                    Passenger= listPassenger,
                                }).OrderByDescending(x => x.CreateTime).ToList();
                    var VinWonderBooking = (from a in _DbContext.VinWonderBooking.AsNoTracking().Where(s => s.OrderId == order_id)
                                            select new vinWonderdetail
                                            {
                                                OrderId = a.OrderId,
                                                BookingId = a.Id,
                                                SiteName = a.SiteName,
                                            }).FirstOrDefault();
                    if (VinWonderBooking != null )
                    {
                       
                            var VinWonderBookingTicket = (from a in _DbContext.VinWonderBookingTicket.AsNoTracking().Where(s => s.BookingId == VinWonderBooking.BookingId)
                                                          join b in _DbContext.VinWonderBookingTicketDetail.AsNoTracking() on  a.Id equals b.BookingTicketId
                                                          select new VinWonderBookingTicketViewModel
                                                          {
                                                              DateUsed = Convert.ToDateTime(a.DateUsed).ToString("dd/MM/yyyy"),
                                                              adt = (int)a.Adt,
                                                              child = (int)a.Child,
                                                              old = (int)a.Old,
                                                              BookingTicketId = a.Id,
                                                              Name=b.Name
                                                          }).ToList();


                        VinWonderBooking.VinWonderBookingTicket = VinWonderBookingTicket;
                        
                    }
                    var Listdata = (from a in data
                                    join b in _DbContext.Client.AsNoTracking() on a.ClientId equals b.Id
                                    join c in _DbContext.AllCode.AsNoTracking().Where(s => s.Type == AllCodeType.ORDER_STATUS) on (int)a.OrderStatus equals c.CodeValue
                                    join d in _DbContext.AccountClient.AsNoTracking() on a.ClientId equals d.ClientId
                                    join e in _DbContext.AllCode.AsNoTracking().Where(s => s.Type == AllCodeType.PAYMENT_STATUS) on a.PaymentStatus equals e.CodeValue
                                    join f in _DbContext.VinWonderBooking.AsNoTracking() on a.OrderId equals f.OrderId
                                    select new OrderVinWonderDetailViewModel
                                    {
                                        Name = a.Name,
                                        Phone = a.Phone,
                                        Email = a.Email,
                                        OrderId = a.OrderId,
                                        OrderNo = a.OrderNo,
                                        ClientId = a.ClientId,

                                        BookingCode = a.BookingCode,
                                        CreateTime = a.CreateTime,
                                        PaymentType = a.PaymentType,
                                        PaymentTypeName = a.PaymentTypeName,
                                        PaymentStatus = a.PaymentStatus,
                                        OrderStatus = a.OrderStatus,
                                        Amount = (double)a.Amount,
                                        PaymentStatusName = a.PaymentStatusName,
                                        Passenger = a.Passenger,
                                        VoucherId = a.VoucherId,
                                        Price = a.Price,
                                        Discount = a.Discount,
                                        vinWonderdetail = VinWonderBooking,
                                        BookingId=f.AdavigoBookingId,
                                        is_lock = a.OrderStatus == (byte?)OrderStatus.CANCEL ? 1 : 0,
                                    }).ToList();
                    return Listdata;
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getOrderDetail - OrderDAL: " + ex);
                return null;
            }
        }
        public async Task<DataTable> GetAllServiceByOrderId(long OrderId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@OrderId", OrderId);
                return _DbWorker.GetDataTable(StoreProceduresName.SP_GetAllServiceByOrderId, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAllServiceByOrderId - OrderDal: " + ex);
            }
            return null;
        }
        public async Task<DataTable> GetContractPayDetailByOrderId(long orderId)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@OrderId", orderId);

                return _DbWorker.GetDataTable(StoreProceduresName.GetListContractPayDetailByOrderId, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetContractPayDetailByOrderId - HotelBookingDAL: " + ex);
            }
            return null;
        }
        public List<ContractPayDetail> GetContractPayByOrderID(long orderId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var Listdata = (from a in _DbContext.ContractPayDetail.AsNoTracking()
                                    join b in _DbContext.ContractPay.AsNoTracking().Where(x => x.Type == 1 && x.IsDelete == false) on a.PayId equals b.PayId
                                    where a.DataId == orderId
                                    select new ContractPayDetail
                                    {
                                        Amount = a.Amount,
                                        PayId = a.PayId,
                                        Id = a.Id,
                                        DataId = a.DataId
                                    }).ToList();
                    return Listdata;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetContractPayDetailByOrderId - HotelBookingDAL: " + ex);
            }
            return null;
        }
        public async Task<Payment> GetOrderPayment(long orderId,double amount,DateTime receiveTime)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Payment.AsNoTracking().Where(x => x.OrderId == orderId && x.Amount == amount && x.PaymentDate == receiveTime).FirstOrDefaultAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPayment - HotelBookingDAL: " + ex);
            }
            return null;
        } 
        public async Task<Payment> GetDepositPayment(long orderId,double amount,DateTime receiveTime)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var exists_payment = await _DbContext.Payment.AsNoTracking().Where(x => x.OrderId == orderId && x.Amount == amount && x.PaymentDate == receiveTime && x.DepositPaymentType == (short)DepositPaymentType.DEPOSIT_FUND).FirstOrDefaultAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPayment - HotelBookingDAL: " + ex);
            }
            return null;
        }
        public async Task<long> UpdateOrder(Order order)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    _DbContext.Order.Update(order);
                    await _DbContext.SaveChangesAsync();
                    var OrderId = order.OrderId;
                    return OrderId;

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdataOrder - OrderDal: " + ex.ToString());
                return 0;
            }
        }
        public async Task<long> UpdateOrderFinishPayment(long OrderId, long Status)
        {
            try
            {
                SqlParameter[] objParam_updateFinishPayment = new SqlParameter[5];
                objParam_updateFinishPayment[0] = new SqlParameter("@OrderId", OrderId);
                objParam_updateFinishPayment[1] = new SqlParameter("@IsFinishPayment", DBNull.Value);
                objParam_updateFinishPayment[2] = new SqlParameter("@PaymentStatus", DBNull.Value);
                objParam_updateFinishPayment[3] = new SqlParameter("@Status", Status);
                objParam_updateFinishPayment[4] = new SqlParameter("@DebtStatus", (int)DepositHistoryConstant.ORDER_DEBT_STATUS.GACH_NO_DU);
                var data = _DbWorker.ExecuteNonQuery(StoreProceduresName.SP_UpdateOrderFinishPayment, objParam_updateFinishPayment);
                return data;


            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateOrderFinishPayment - OrderDal: " + ex);
                return -1;
            }
        }
        public int IsClientAllowedToDebtNewService(double service_amount, long client_id, long order_id, int service_type)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[4];
                objParam_order[0] = new SqlParameter("@ClientId", client_id);
                objParam_order[1] = new SqlParameter("@Amount", service_amount);
                objParam_order[2] = new SqlParameter("@OrderId", order_id);
                objParam_order[3] = new SqlParameter("@ServiceType", service_type);

                var table = _DbWorker.GetDataTable(StoreProceduresName.SP_CheckClientDebt, objParam_order);
                if (table != null && table.Rows.Count > 0)
                {
                    int id = -1;
                    id = (from row in table.AsEnumerable()
                          select new
                          {
                              IsPayable = Convert.ToInt32(row["IsPayable"])
                          }).First().IsPayable;
                    return id;
                }
                return 0;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("IsClientAllowedToDebtNewService - OrderDal: " + ex);
                return -1;
            }
        }
        public async Task<DataTable> GetOrrderbyUtmSource(OrderViewSearchModel searchModel, int currentPage, int pageSize, string proc)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[24];


                objParam[0] = new SqlParameter("@CreateTime", DBNull.Value) ;
                objParam[1] = new SqlParameter("@ToDateTime", DBNull.Value) ;
                objParam[2] = new SqlParameter("@SysTemType", searchModel.SysTemType);
                objParam[3] = new SqlParameter("@OrderNo", searchModel.OrderNo);
                objParam[4] = new SqlParameter("@Note", searchModel.Note);
                objParam[5] = new SqlParameter("@ServiceType", searchModel.ServiceType == null ? "" : string.Join(",", searchModel.ServiceType));
                objParam[6] = new SqlParameter("@UtmSource", searchModel.UtmSource == null ? "" : searchModel.UtmSource);
                objParam[7] = new SqlParameter("@status", searchModel.Status == null ? "" : string.Join(",", searchModel.Status));
                objParam[8] = new SqlParameter("@CreateName", searchModel.CreateName);
                if (searchModel.Sale == null)
                {
                    objParam[9] = new SqlParameter("@Sale", DBNull.Value);

                }
                else
                {
                    objParam[9] = new SqlParameter("@Sale", searchModel.Sale);

                }
                objParam[10] = new SqlParameter("@SaleGroup", searchModel.SaleGroup);
                objParam[11] = new SqlParameter("@PageIndex", currentPage);
                objParam[12] = new SqlParameter("@PageSize", pageSize);
                objParam[13] = new SqlParameter("@StatusTab", searchModel.StatusTab);
                objParam[14] = new SqlParameter("@ClientId", searchModel.ClientId);
                objParam[15] = new SqlParameter("@SalerPermission", searchModel.SalerPermission);
                objParam[16] = new SqlParameter("@OperatorId", searchModel.OperatorId);
                if (searchModel.StartDateFrom == DateTime.MinValue)
                {
                    objParam[17] = new SqlParameter("@StartDateFrom", DBNull.Value);

                }
                else
                {
                    objParam[17] = new SqlParameter("@StartDateFrom", searchModel.StartDateFrom);

                }
                if (searchModel.StartDateTo == DateTime.MinValue)
                {
                    objParam[18] = new SqlParameter("@StartDateTo", DBNull.Value);

                }
                else
                {
                    objParam[18] = new SqlParameter("@StartDateTo", searchModel.StartDateTo);

                }
                if (searchModel.EndDateFrom == DateTime.MinValue)
                {
                    objParam[19] = new SqlParameter("@EndDateFrom", DBNull.Value);

                }
                else
                {
                    objParam[19] = new SqlParameter("@EndDateFrom", searchModel.EndDateFrom);

                }
                if (searchModel.EndDateTo == DateTime.MinValue)
                {
                    objParam[20] = new SqlParameter("@EndDateTo", DBNull.Value);

                }
                else
                {
                    objParam[20] = new SqlParameter("@EndDateTo", searchModel.EndDateTo);

                }
                if (searchModel.PermisionType == null)
                {
                    objParam[21] = new SqlParameter("@PermisionType", DBNull.Value);

                }
                else
                {
                    objParam[21] = new SqlParameter("@PermisionType", searchModel.PermisionType);

                }
                if (searchModel.PaymentStatus == null)
                {
                    objParam[22] = new SqlParameter("@PaymentStatus", DBNull.Value);

                }
                else
                {
                    objParam[22] = new SqlParameter("@PaymentStatus", searchModel.PaymentStatus);

                }
                objParam[23] = new SqlParameter("@UtmMedium", searchModel.UtmMedium);
                
                return _DbWorker.GetDataTable(proc, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - OrderDal: " + ex);
            }
            return null;
        }
        public DataTable GetListOrder(long clientId, DateTime fromDate, DateTime toDate,string OrderStatus="")
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[13];
                objParam[0] = new SqlParameter("@StartDateFrom", DBNull.Value);
                objParam[1] = new SqlParameter("@StartDateTo", DBNull.Value);
                objParam[2] = new SqlParameter("@EndDateFrom", DBNull.Value);
                objParam[3] = new SqlParameter("@EndDateTo", DBNull.Value);
                if (fromDate == null || fromDate<new DateTime(2020,01,01))
                {
                    objParam[4] = new SqlParameter("@CreateDateFrom", DBNull.Value);

                }
                else
                {
                    objParam[4] = new SqlParameter("@CreateDateFrom", fromDate);

                }
                if (toDate == null || toDate < new DateTime(2020, 01, 01))
                {
                    objParam[5] = new SqlParameter("@CreateDateTo", DBNull.Value);

                }
                else
                {
                    objParam[5] = new SqlParameter("@CreateDateTo", toDate);

                }
                objParam[6] = new SqlParameter("@ClientId", clientId);
                objParam[7] = new SqlParameter("@SupplierId", DBNull.Value);
                objParam[8] = new SqlParameter("@OrderId", DBNull.Value);
                if (OrderStatus == null || OrderStatus.Trim()=="")
                {
                    objParam[9] = new SqlParameter("@OrderStatus", DBNull.Value);

                }
                else
                {
                    objParam[9] = new SqlParameter("@OrderStatus", OrderStatus);

                }
                objParam[10] = new SqlParameter("@Module", DBNull.Value);
                objParam[11] = new SqlParameter("@PageIndex", 1);
                objParam[12] = new SqlParameter("@PageSize", 1000);
                return _DbWorker.GetDataTable(StoreProceduresName.SP_GetListOrder, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetOrderListByClientId - DebtStatisticDAL: " + ex);
            }
            return null;
        }
    }

}
