using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{
    
    public class OtherBookingDAL : GenericService<OtherBooking>
    {
        private DbWorker dbWorker;

        public OtherBookingDAL(string connection) : base(connection)
        {
            dbWorker = new DbWorker(connection);

        }
        public async Task<OtherBooking> GetOtherBookingById(long booking_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.OtherBooking.AsNoTracking().FirstOrDefaultAsync(x => x.Id == booking_id);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetOtherBookingById - OtherBookingDAL: " + ex);
                return null;
            }
        }
        public async Task<List<OtherBooking>> GetOtherBookingByOrderId(long order_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.OtherBooking.AsNoTracking().Where(x => x.OrderId == order_id).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetOtherBookingById - OtherBookingDAL: " + ex);
                return null;
            }
        }
        public async Task<DataTable> GetDetailOtherBookingById(long OtherBookingId)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@OtherBookingId ", OtherBookingId);
                return dbWorker.GetDataTable(StoreProceduresName.SP_GetDetailOtherBookingById, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailOtherBookingById - OtherBookingDAL: " + ex);
            }
            return null;
        }
    }
}