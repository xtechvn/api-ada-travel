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
    public class HotelBookingRoomDAL : GenericService<HotelBookingRooms>
    {
        private static DbWorker dbWorker;
        public HotelBookingRoomDAL(string connection) : base(connection)
        {
            dbWorker = new DbWorker(connection);
        }
        public async Task<List<HotelBookingRooms>> GetByHotelBookingID(long hotel_booking_id)
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
        public async Task<DataTable> GetHotelBookingRoomByHotelBookingID(long HotelBookingId, long status)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[2];
                objParam[0] = new SqlParameter("@HotelBookingID", HotelBookingId);
                objParam[1] = new SqlParameter("@Status", status);
                return dbWorker.GetDataTable(StoreProceduresName.SP_GetHotelBookingRoomByHotelBookingID, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingRoomByHotelBookingID - HotelBookingRoomDAL: " + ex);
            }
            return null;
        }
        public async Task<HotelBookingRooms> GetByIDAndHotelBookingID(long hotel_booking_room_id, long hotel_booking_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = await _DbContext.HotelBookingRooms.AsNoTracking().Where(x => x.Id == hotel_booking_room_id && x.HotelBookingId== hotel_booking_id).FirstOrDefaultAsync();
                    return detail;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByIDAndHotelBookingID - HotelBookingRoomDAL. " + ex);
                return null;
            }
        }
        public async Task<HotelBookingRoomsOptional> GetHotelBookingRoomOptionalById(long hotel_booking_room_optional)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = await _DbContext.HotelBookingRoomsOptional.AsNoTracking().FirstOrDefaultAsync(x => x.Id == hotel_booking_room_optional);
                    return detail;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByHotelBookingID - HotelBookingRoomDAL. " + ex);
                return null;
            }
        }
        public async Task<List<HotelBookingRoomsOptional>> GetHotelBookingRoomOptionalByBookingId(long hotel_booking_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = await _DbContext.HotelBookingRoomsOptional.AsNoTracking().Where(x => x.HotelBookingId == hotel_booking_id).ToListAsync();
                    return detail;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingRoomOptionalByBookingId - HotelBookingRoomDAL. " + ex);
                return null;
            }
        }
        public async Task<HotelBookingRooms> GetByID(long hotel_booking_room_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = await _DbContext.HotelBookingRooms.AsNoTracking().Where(x => x.Id == hotel_booking_room_id).FirstOrDefaultAsync();
                    return detail;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByIDAndHotelBookingID - HotelBookingRoomDAL. " + ex);
                return null;
            }
        }
    }
}
