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


namespace DAL
{
    public class HotelBookingRoomRatesDAL : GenericService<HotelBookingRoomRates>
    {
        private DbWorker dbWorker;
        public HotelBookingRoomRatesDAL(string connection) : base(connection)
        {
            dbWorker = new DbWorker(connection);
        }
      
        public async Task<List<HotelBookingRoomRates>> GetByBookingRoomsRateByHotelBookingID(long hotel_booking_id)
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
        public async Task<DataTable> GetHotelBookingRateByHotelBookingRoomID(long HotelBookingRoomID)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@HotelBookingRoomID", HotelBookingRoomID);
                return dbWorker.GetDataTable(StoreProceduresName.SP_GetHotelBookingRateByHotelBookingRoomID, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingRateByHotelBookingRoomID - HotelBookingRoomRatesDAL: " + ex);
            }
            return null;
        }
        public async Task<HotelBookingRoomRates> GetByIdAndHotelBookingRoomId(long id, long hotel_booking_room_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = await _DbContext.HotelBookingRoomRates.AsNoTracking().Where(x => x.Id== id && x.HotelBookingRoomId== hotel_booking_room_id).FirstOrDefaultAsync();
                    return detail;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByIdAndHotelBookingRoomId - HotelBookingRoomRatesDAL. " + ex);
                return null;
            }
        }
        public async Task<HotelBookingRoomRates> GetById(long id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = await _DbContext.HotelBookingRoomRates.AsNoTracking().Where(x => x.Id == id ).FirstOrDefaultAsync();
                    return detail;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetById - HotelBookingRoomRatesDAL. " + ex);
                return null;
            }
        }
        public async Task<double> GetTotalRatesAmountByHotelBookingRoomId( long hotel_booking_room_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = await _DbContext.HotelBookingRoomRates.AsNoTracking().Where(x =>  x.HotelBookingRoomId == hotel_booking_room_id).ToListAsync();
                    if (detail == null || detail.Count <= 0) return 0;

                    return detail.Sum(x=> x.UnitPrice!=null? (double)x.UnitPrice: x.TotalAmount- x.Profit);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByIdAndHotelBookingRoomId - HotelBookingRoomRatesDAL. " + ex);
                return -1;
            }
        }
        public async Task<List<HotelBookingRoomRatesOptional>> GetHotelBookingRoomRatesOptionalByHotelBookingID(long hotel_booking_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail_room = await _DbContext.HotelBookingRoomsOptional.AsNoTracking().Where(x => x.HotelBookingId == hotel_booking_id).ToListAsync();
                    var detail_room_ids = detail_room.Select(x => x.Id);
                    var detail = await _DbContext.HotelBookingRoomRatesOptional.AsNoTracking().Where(x => detail_room_ids.Contains(x.HotelBookingRoomOptionalId)).ToListAsync();
                    return detail;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByBookingRoomsRateByHotelBookingID - HotelBookingRoomRatesDAL. " + ex);
                return null;
            }
        }
        public async Task<HotelBookingRoomRatesOptional> GetOptionalByIdAndHotelBookingRoomId(long id, long hotel_booking_room_optional_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = await _DbContext.HotelBookingRoomRatesOptional.AsNoTracking().Where(x => x.Id == id && x.HotelBookingRoomOptionalId == hotel_booking_room_optional_id).FirstOrDefaultAsync();
                    return detail;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByIdAndHotelBookingRoomId - HotelBookingRoomRatesDAL. " + ex);
                return null;
            }
        }
    }
}
