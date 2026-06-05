using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using ENTITIES.ViewModels.Hotel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL.Hotel
{
    public class HotelRoomFundDetailDAL : GenericService<HotelBooking>
    {
        private static DbWorker _DbWorker;
        public HotelRoomFundDetailDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public async Task<List<HotelRoomFundDetailModel>> GetListHotelRoomFundDetailByHotelIdAndSupplierId(int HotelId, int SupplierId, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[4];

                objParam[0] = new SqlParameter("@HotelId", HotelId);
                objParam[1] = new SqlParameter("@SupplierId", SupplierId);
                if (StartDate != null )
                {
                    objParam[2] = new SqlParameter("@StartDate", StartDate);

                }
                else
                {
                    objParam[2] = new SqlParameter("@StartDate", DBNull.Value);

                }
                if (EndDate != null )
                {
                    objParam[3] = new SqlParameter("@EndDate",  EndDate);

                }
                else
                {
                    objParam[3] = new SqlParameter("@EndDate", DBNull.Value );

                }


                var dt = _DbWorker.GetDataTable(StoreProceduresName.sp_GetListHotelRoomFundDetailByHotelIdAndSupplierId, objParam);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<HotelRoomFundDetailModel>();
                    return data;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - ClientDAL: " + ex);
            }
            return null;
        }
    }
}
