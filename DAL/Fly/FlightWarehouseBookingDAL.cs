using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using ENTITIES.ViewModels.B2B;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL.Fly
{
    public class FlightWarehouseBookingDAL : GenericService<FlightSegment>
    {
        private static DbWorker _DbWorker;
        public FlightWarehouseBookingDAL(string connection) : base(connection)
        {

            _DbWorker = new DbWorker(connection);
        }
        public async Task<List<FlightWarehouseBookingViewModel>> GetListFlightWarehouse(GetListFlightWarehouseModel searchModel)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[8];

                objParam[0] = new SqlParameter("@BookingCode", searchModel.BookingCode ?? (object)DBNull.Value);
                objParam[1] = new SqlParameter("@DeparturePoint", searchModel.DeparturePoint ?? (object)DBNull.Value);
                objParam[2] = new SqlParameter("@ArrivalPoint", searchModel.ArrivalPoint ?? (object)DBNull.Value);
                objParam[3] = new SqlParameter("@Airline", searchModel.Airline ?? (object)DBNull.Value);
                objParam[4] = new SqlParameter("@PageIndex", searchModel.pageIndex);
                objParam[5] = new SqlParameter("@PageSize", searchModel.pageSize);
                objParam[6] = new SqlParameter("@Date", searchModel.Date);
                objParam[7] = new SqlParameter("@FundType", searchModel.FundType);
                var dt = _DbWorker.GetDataTable(StoreProceduresName.SP_GetListFlightWarehouse, objParam);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<FlightWarehouseBookingViewModel>();
                    return data;
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListFlightWarehouse - FlightWarehouseBookingDAL: " + ex);
            }
            return null;
        }
        public async Task<FlightWarehouseBookingModel> GetById(int id)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];

                objParam[0] = new SqlParameter("@Id", id);



                var result = _DbWorker.GetDataTable(StoreProceduresName.sp_GetDetailFlightWarehouseBooking, objParam);
                if (result != null && result.Rows.Count > 0)
                {
                    var data = result.ToList<FlightWarehouseBookingModel>();
                    return data[0];
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetById - FlightWarehouseBookingDAL: " + ex);
            }
            return null;
        }
        public async Task<List<FlightWarehouseSegmentModel>> GetSegmentsByBookingId(int bookingId)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];

                objParam[0] = new SqlParameter("@BookingId", bookingId);



                var result = _DbWorker.GetDataTable(StoreProceduresName.sp_GetDetailFlightWarehouseSegmentByBookingId, objParam);
                if (result != null && result.Rows.Count > 0)
                {
                    var data = result.ToList<FlightWarehouseSegmentModel>();
                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByBookingId - FlightWarehouseSegmentDAL: " + ex);
            }
            return null;
        }
    }
}
