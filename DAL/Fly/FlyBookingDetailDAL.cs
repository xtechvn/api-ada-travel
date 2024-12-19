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

namespace DAL.Fly
{
    public class FlyBookingDetailDAL : GenericService<FlyBookingDetail>
    {
        private static DbWorker _DbWorker;
        public FlyBookingDetailDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }

        public FlyBookingDetail GetDetail(long orderId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.FlyBookingDetail.AsNoTracking().FirstOrDefault(s => s.OrderId == orderId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetail - FlyBookingDetailDAL: " + ex);
                return null;
            }
        }

        public List<FlyBookingDetail> GetListByOrderId(long orderId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.FlyBookingDetail.AsNoTracking().Where(s => s.OrderId == orderId).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetail - FlyBookingDetailDAL: " + ex);
                return null;
            }
        }
        public async Task<List<FlyBookingDetail>> GetFlyBookingById(long fly_booking_id)
        {
            try
            {

                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var exists_fly= await _DbContext.FlyBookingDetail.AsNoTracking().Where(s => s.Id == fly_booking_id).FirstOrDefaultAsync();
                    if(exists_fly!=null && exists_fly.Id > 0)
                    {
                        return  await _DbContext.FlyBookingDetail.AsNoTracking().Where(s => s.GroupBookingId == exists_fly.GroupBookingId).ToListAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetFlyBookingById - FlyBookingDetailDAL: " + ex);
            }
            return null;
        }
        public async Task<DataTable> GetDetailFlyBookingDetailById(long FlyBookingDetailId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@FlyBookingDetailId", FlyBookingDetailId);

                return _DbWorker.GetDataTable(StoreProceduresName.SP_GetDetailFlyBookingDetailById, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailOrderServiceByOrderId - FlyBookingDetailDAL: " + ex);
            }
            return null;
        }
    }
}
