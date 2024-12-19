using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using ENTITIES.ViewModels.Tour;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{
    
    public class TourDAL : GenericService<Tour>
    {
        private DbWorker dbWorker;

        public TourDAL(string connection) : base(connection)
        {
            dbWorker = new DbWorker(connection);

        }
        public async Task<Tour> GetTourById(long tour_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Tour.Where(x => x.Id == tour_id).FirstOrDefaultAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTourById - TourDAL: " + ex);
                return null;
            }
        }
        public async Task<DataTable> GetDetailTourByID(long TourId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@TourId", TourId);
                return dbWorker.GetDataTable(StoreProceduresName.SP_GetDetailTourByID, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTourById - TourDAL: " + ex);
                return null;
            }
        }
        public async Task<DataTable> GetListTourByAccountId(long Id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@AccountId", Id);
                return dbWorker.GetDataTable(StoreProceduresName.SP_fe_GetListTourByAccountId, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTourById - TourDAL: " + ex);
                return null;
            }
        }
    }
    
}