using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL.Hotel
{
    public class HotelBookingDAL : GenericService<HotelBooking>
    {
        private static DbWorker _DbWorker;
        public HotelBookingDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public List<HotelBooking> GetListByOrderId(long orderId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.HotelBooking.AsNoTracking().Where(s => s.OrderId == orderId).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListByOrderId - HotelBookingDAL: " + ex);
                return null;
            }
        }
        public async Task<HotelBooking> GetHotelBookingByID(long id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = await _DbContext.HotelBooking.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                    return detail;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingByID - HotelBookingDAL: " + ex);
                return null;
            }
        }
        public async Task<DataTable> GetHotelBookingById(long HotelBookingId)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@HotelBookingId", HotelBookingId);
                return _DbWorker.GetDataTable(StoreProceduresName.SP_GetHotelBookingById, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingById - HotelBookingDAL: " + ex);
            }
            return null;
        }
        public async Task<DataTable> GetServiceDeclinesByServiceId(string ServiceId, int type)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[2];
                objParam[0] = new SqlParameter("@ServiceId", ServiceId);
                objParam[1] = new SqlParameter("@Type", type);

                return _DbWorker.GetDataTable(StoreProceduresName.Sp_GetServiceDeclinesByOrderId, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailHotelBookingByID - HotelBookingDAL: " + ex);
            }
            return null;
        }
        public async Task<DataTable> GetDetailHotelBookingByID(long HotelBookingId)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@HotelBookingId", HotelBookingId);

                return _DbWorker.GetDataTable(StoreProceduresName.SP_GetDetailHotelBookingByID, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailHotelBookingByID - HotelBookingDAL: " + ex);
            }
            return null;
        }
        public async Task<List<HotelBookingRooms>> GetHotelBookingRoomsByID(long id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = _DbContext.HotelBookingRooms.Where(x => x.HotelBookingId == id).ToList();
                    return detail;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingRoomsByID - HotelBookingDAL: " + ex);
                return null;
            }
        }

        public int CreateHotelBooking(HotelBooking booking)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[31];
                objParam_order[0] = new SqlParameter("@OrderId", booking.OrderId);
                objParam_order[1] = new SqlParameter("@BookingId", booking.BookingId);
                objParam_order[2] = new SqlParameter("@PropertyId", booking.PropertyId);
                if (booking.HotelType != null)
                {
                    objParam_order[3] = new SqlParameter("@HotelType", booking.HotelType);
                }
                else
                {
                    objParam_order[3] = new SqlParameter("@HotelType", (int)ServicesType.VINHotelRent);
                }
                objParam_order[4] = new SqlParameter("@ArrivalDate", booking.ArrivalDate);
                objParam_order[5] = new SqlParameter("@DepartureDate", booking.DepartureDate);
                objParam_order[6] = new SqlParameter("@numberOfRoom", booking.NumberOfRoom);
                objParam_order[7] = new SqlParameter("@numberOfAdult", booking.NumberOfAdult);
                objParam_order[8] = new SqlParameter("@numberOfChild", booking.NumberOfChild);
                objParam_order[9] = new SqlParameter("@numberOfInfant", booking.NumberOfInfant);
                objParam_order[10] = new SqlParameter("@totalPrice", booking.TotalPrice);
                objParam_order[11] = new SqlParameter("@totalProfit", booking.TotalProfit);
                objParam_order[12] = new SqlParameter("@totalAmount", booking.TotalAmount);
                objParam_order[13] = new SqlParameter("@Status", booking.Status);
                objParam_order[14] = new SqlParameter("@HotelName", booking.HotelName);
                if (booking.Telephone != null)
                {
                    objParam_order[15] = new SqlParameter("@Telephone", booking.Telephone);
                }
                else
                {
                    objParam_order[15] = new SqlParameter("@Telephone", DBNull.Value);
                }
                if (booking.Email != null)
                {
                    objParam_order[16] = new SqlParameter("@Email", booking.Email);
                }
                else
                {
                    objParam_order[16] = new SqlParameter("@Email", DBNull.Value);
                }
                if (booking.Address != null)
                {
                    objParam_order[17] = new SqlParameter("@Address", booking.Address);
                }
                else
                {
                    objParam_order[17] = new SqlParameter("@Address", DBNull.Value);
                }
                if (booking.ImageThumb != null)
                {
                    objParam_order[18] = new SqlParameter("@ImageThumb", booking.ImageThumb);
                }
                else
                {
                    objParam_order[18] = new SqlParameter("@ImageThumb", DBNull.Value);
                }
                objParam_order[19] = new SqlParameter("@CheckinTime", booking.CheckinTime);
                objParam_order[20] = new SqlParameter("@CheckoutTime", booking.CheckoutTime);
                objParam_order[21] = new SqlParameter("@ExtraPackageAmount", booking.ExtraPackageAmount);
                if (booking.SalerId != null)
                {
                    objParam_order[22] = new SqlParameter("@SalerId", booking.SalerId);
                }
                else
                {
                    objParam_order[22] = new SqlParameter("@SalerId", DBNull.Value);
                }
                objParam_order[23] = new SqlParameter("@CreatedBy", booking.CreatedBy);
                objParam_order[24] = new SqlParameter("@CreatedDate", DateTime.Now);
                if (booking.ServiceCode != null && booking.ServiceCode.Trim() != "")
                {
                    objParam_order[25] = new SqlParameter("@ServiceCode", booking.ServiceCode);
                }
                else
                {
                    objParam_order[25] = new SqlParameter("@ServiceCode", DBNull.Value);
                }
                objParam_order[26] = new SqlParameter("@Price", booking.TotalPrice);
                if (booking.SupplierId != null)
                {
                    objParam_order[27] = new SqlParameter("@SupplierId", booking.SupplierId);
                }
                else
                {
                    objParam_order[27] = new SqlParameter("@SupplierId", DBNull.Value);

                }
                if (booking.Note != null)
                {
                    objParam_order[28] = new SqlParameter("@Note", booking.Note);
                }
                else
                {
                    objParam_order[28] = new SqlParameter("@Note", DBNull.Value);
                }
                objParam_order[29] = new SqlParameter("@TotalDiscount", booking.TotalDiscount);
                objParam_order[30] = new SqlParameter("@TotalOthersAmount", booking.TotalOthersAmount);
                var id = _DbWorker.ExecuteNonQuery(StoreProceduresName.CreateHotelBooking, objParam_order);
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Repository HotelBookingDAL" + ex.ToString());
                return -1;
            }
        }

        public int CreateHotelBookingRooms(HotelBookingRooms booking)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[19];
                objParam_order[0] = new SqlParameter("@HotelBookingId", booking.HotelBookingId);
                objParam_order[1] = new SqlParameter("@RoomTypeID", booking.RoomTypeId);
                objParam_order[2] = new SqlParameter("@Price", booking.Price);
                objParam_order[3] = new SqlParameter("@Profit", booking.Profit);
                objParam_order[4] = new SqlParameter("@TotalAmount", booking.TotalAmount);
                objParam_order[5] = new SqlParameter("@RoomTypeCode", booking.RoomTypeCode);
                objParam_order[6] = new SqlParameter("@RoomTypeName", booking.RoomTypeName);
                objParam_order[7] = new SqlParameter("@numberOfAdult", booking.NumberOfAdult);
                objParam_order[8] = new SqlParameter("@numberOfChild", booking.NumberOfChild);
                objParam_order[9] = new SqlParameter("@numberOfInfant", booking.NumberOfInfant);
                objParam_order[10] = new SqlParameter("@PackageIncludes", booking.PackageIncludes);
                objParam_order[11] = new SqlParameter("@ExtraPackageAmount", booking.ExtraPackageAmount);
                objParam_order[12] = new SqlParameter("@Status", booking.Status);
                objParam_order[13] = new SqlParameter("@TotalUnitPrice", booking.TotalUnitPrice);
                objParam_order[14] = new SqlParameter("@CreateBy", booking.CreatedBy);
                objParam_order[15] = new SqlParameter("@CreatedDate", DBNull.Value);
                objParam_order[16] = new SqlParameter("@NumberOfRooms", booking.NumberOfRooms);
                if (booking.SupplierId != null)
                {
                    objParam_order[17] = new SqlParameter("@SupplierId", booking.SupplierId);
                }
                else
                {
                    objParam_order[17] = new SqlParameter("@SupplierId", DBNull.Value);
                }
                objParam_order[18] = new SqlParameter("@IsRoomAvailable", booking.IsRoomAvailable);
                var id = _DbWorker.ExecuteNonQuery(StoreProceduresName.CreateHotelBookingRooms, objParam_order);
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Repository HotelBookingDAL" + ex.ToString());
                return -1;
            }
        }

        public int CreateHotelBookingRoomRates(HotelBookingRoomRates booking)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[15];
                objParam_order[0] = new SqlParameter("@HotelBookingRoomId", booking.HotelBookingRoomId);
                objParam_order[1] = new SqlParameter("@RatePlanId", booking.RatePlanId);
                objParam_order[2] = new SqlParameter("@StayDate", booking.StayDate);
                objParam_order[3] = new SqlParameter("@Price", booking.Price);
                objParam_order[4] = new SqlParameter("@Profit", booking.Profit);
                objParam_order[5] = new SqlParameter("@TotalAmount", booking.TotalAmount);
                if (booking.AllotmentId != null)
                {
                    objParam_order[6] = new SqlParameter("@AllotmentId", booking.AllotmentId);
                }
                else
                {
                    objParam_order[6] = new SqlParameter("@AllotmentId", DBNull.Value);
                }
                if (booking.RatePlanCode != null)
                {
                    objParam_order[7] = new SqlParameter("@RatePlanCode", booking.RatePlanCode);
                }
                else
                {
                    objParam_order[7] = new SqlParameter("@RatePlanCode", DBNull.Value);
                }
                if (booking.PackagesInclude != null)
                {
                    objParam_order[8] = new SqlParameter("@PackagesInclude", booking.PackagesInclude);
                }
                else
                {
                    objParam_order[8] = new SqlParameter("@PackagesInclude", DBNull.Value);
                }
                objParam_order[9] = new SqlParameter("@Nights", booking.Nights);
                objParam_order[10] = new SqlParameter("@StartDate", booking.StartDate);
                objParam_order[11] = new SqlParameter("@EndDate", booking.EndDate);
                objParam_order[12] = new SqlParameter("@OperatorPrice", booking.OperatorPrice);
                objParam_order[13] = new SqlParameter("@SalePrice", booking.SalePrice);
                objParam_order[14] = new SqlParameter("@CreatedBy", booking.CreatedBy);

                var id = _DbWorker.ExecuteNonQuery(StoreProceduresName.CreateHotelBookingRoomRates, objParam_order);
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Repository HotelBookingDAL" + ex.ToString());
                return -1;
            }
        }
        public async Task<DataTable> GetListHotelBookingRoomsExtraPackageByBookingId(long HotelBookingId)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@HotelBookingId", HotelBookingId);
                return _DbWorker.GetDataTable(StoreProceduresName.SP_GetListHotelBookingRoomsExtraPackageByBookingId, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingById - HotelBookingDAL: " + ex);
            }
            return null;
        }
        public async Task<List<HotelBookingRooms>> GetHotelBookingRoomsByHotelBookingID(long hotel_booking_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = await _DbContext.HotelBookingRooms.AsNoTracking().Where(x => x.HotelBookingId == hotel_booking_id).ToListAsync();
                    return detail;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByHotelBookingID - HotelBookingRoomDAL. " + ex);
                return null;
            }
        }
        public async Task<List<HotelBookingRoomRates>> GetHotelBookingRoomRatesByBookingRoomsRateByHotelBookingID(long hotel_booking_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail_room = await _DbContext.HotelBookingRooms.AsNoTracking().Where(x => x.HotelBookingId == hotel_booking_id).ToListAsync();
                    var detail_room_ids = detail_room.Select(x => x.Id);
                    var detail = await _DbContext.HotelBookingRoomRates.AsNoTracking().Where(x => detail_room_ids.Contains(x.HotelBookingRoomId)).ToListAsync();
                    return detail;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByBookingRoomsRateByHotelBookingID - HotelBookingRoomRatesDAL. " + ex);
                return null;
            }
        }
    }
}
