using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using ENTITIES.ViewModels.Order;
using ENTITIES.ViewModels.VinWonder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using Utilities.Helpers;

namespace DAL.VinWonder
{
    public class VinWonderBookingDAL : GenericService<VinWonderBooking>
    {
        private static DbWorker _DbWorker;
        public VinWonderBookingDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public async Task<List<ListVinWonderViewModel>> GetVinWonderByAccountClientId(long AccountClientId, long PageIndex, long PageSize, string keyword)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[4];
                objParam_order[0] = new SqlParameter("@AccountClientId", AccountClientId);
                objParam_order[1] = new SqlParameter("@PageIndex", PageIndex);
                objParam_order[2] = new SqlParameter("@PageSize", PageSize);
                objParam_order[3] = new SqlParameter("@keyword", keyword);
                var dt = _DbWorker.GetDataTable(StoreProceduresName.Sp_GetListOrderByAccountClientId, objParam_order);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var listData = dt.ToList<ListVinWonderViewModel>();
                    return listData;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelDetail - HotelDAL. " + ex);
                return null;
            }
        }

        // lấy ra nhóm useri của 1 quyền
        public async Task<List<PriceVinWonderViewModels>> getVinWonderPricePolicyByServiceId(string rate_code, string service_id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[2];
                objParam[0] = new SqlParameter("@RateCode", rate_code);
                objParam[1] = new SqlParameter("@ServiceId", service_id);
                var dt = _DbWorker.GetDataTable("SP_GetVinWonderPricePolicyByServiceId", objParam);

                if (dt != null && dt.Rows.Count > 0)
                {
                    var listData = dt.ToList<PriceVinWonderViewModels>();
                    return listData;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("VinWonderBookingDAL - getVinWonderPricePolicyByServiceId: " + ex);
                return null;
            }
        }
        public async Task<List<OrderVinWonderDetailViewModel>> GetVinWonderByBookingId(string bookingId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@BookingId", bookingId);
                var data = new List<OrderVinWonderDetailViewModel>();
                var dt = _DbWorker.GetDataTable(StoreProceduresName.SP_GetVinWonderBookingByBookingId, objParam);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var listData = dt.ToList<OrderVinWonderDetailViewModel>();
                    data=listData;
                }
               
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    
                    if (data != null && data.Count > 0)
                    {
                        foreach(var item in data)
                        {

                            var listPassenger = _DbContext.Passenger.Where(s => s.OrderId == (long)item.OrderId).ToList();
                            item.Passenger = listPassenger;
                            var VinWonderBooking =( from a in _DbContext.VinWonderBooking.Where(s=>s.AdavigoBookingId==bookingId)
                                                    select new vinWonderdetail
                                                    {
                                                        
                                                        BookingId=a.Id,
                                                        OrderId=a.OrderId,
                                                        SiteCode=a.SiteCode,
                                                        SiteName=a.SiteName
                                                    }
                                                    ).FirstOrDefault();
                            var VinWonderBookingTicket = (from a in _DbContext.VinWonderBookingTicket.Where(s => s.BookingId == VinWonderBooking.BookingId)
                                                          join b in _DbContext.VinWonderBookingTicketDetail on a.Id equals b.BookingTicketId
                                                          select new VinWonderBookingTicketViewModel
                                                          {
                                                              DateUsed = Convert.ToDateTime(a.DateUsed).ToString("dd/MM/yyyy"),
                                                              adt = (int)a.Adt,
                                                              child = (int)a.Child,
                                                              old = (int)a.Old,
                                                              BookingTicketId = a.Id,
                                                              Name = b.Name
                                                          }).ToList();
                            VinWonderBooking.VinWonderBookingTicket = VinWonderBookingTicket;
                            item.vinWonderdetail = VinWonderBooking;
                        }
                      
                    }
                    return data;
                }

               
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelDetail - HotelDAL. " + ex);
                return null;
            }
        }
        public async Task<List<VinWonderBooking>> GetVinWonderBookingByOrderId(long order_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.VinWonderBooking.AsNoTracking().Where(x => x.OrderId == order_id).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetVinWonderBookingByOrderId - OtherBookingDAL: " + ex);
                return null;
            }
        }
        public async Task<List<ListVinWonderemialViewModel>> GetVinWonderBookingEmailByOrderID(long orderid)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@OrderID", orderid);
                
                var dt = _DbWorker.GetDataTable(StoreProceduresName.SP_GetVinWonderBookingEmailByOrderID, objParam);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var listData = dt.ToList<ListVinWonderemialViewModel>();
                    return listData;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelDetail - HotelDAL. " + ex);
                return null;
            }
        }
        public async Task<List<VinWonderBooking>> GetVinWonderBookingByOrderID(long orderid)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@OrderID", orderid);

                var dt = _DbWorker.GetDataTable(StoreProceduresName.SP_GetVinWonderBookingByOrderID, objParam);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var listData = dt.ToList<VinWonderBooking>();
                    return listData;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetVinWonderBookingByOrderID - HotelDAL. " + ex);
                return null;
            }
        }
        public async Task<List<VinWonderBookingTicket>> GetVinWonderBookingTicketByBookingID(long BookingId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@BookingId", BookingId);

                var dt = _DbWorker.GetDataTable(StoreProceduresName.SP_GetVinWonderBookingTicketByBookingID, objParam);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var listData = dt.ToList<VinWonderBookingTicket>();
                    return listData;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetVinWonderBookingByOrderID - HotelDAL. " + ex);
                return null;
            }
        }
        public async Task<List<VinWonderBookingTicketCustomer>> GetVinWondeCustomerByBookingId(long BookingId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@BookingId", BookingId);

                var dt = _DbWorker.GetDataTable(StoreProceduresName.SP_GetVinWonderBookingCustomerByBookingId, objParam);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var listData = dt.ToList<VinWonderBookingTicketCustomer>();
                    return listData;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetVinWonderBookingByOrderID - HotelDAL. " + ex);
                return null;
            }
        }
    }
}
