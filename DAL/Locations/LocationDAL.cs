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

namespace DAL.Locations
{
    public class LocationDAL: GenericService<Province>
    {
        private static DbWorker _DbWorker;
        public LocationDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public List<Province> GetProvinceByListID(List<int> provinces)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.Province.AsNoTracking().Where(x=> provinces.Contains( x.Id)).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetProvinceByListID - LocationDAL: " + ex);
                return null;
            }
        }
        public List<National> GetNationalByListID(List<int> nationals)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.National.AsNoTracking().Where(x => nationals.Contains(x.Id)).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetNationalByListID - LocationDAL: " + ex);
                return null;
            }
        }
        public async Task<DataTable> GetNationalByListID(string ids)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@Ids", ids)
                };

                return _DbWorker.GetDataTable(StoreProceduresName.SP_GetListNational, objParam);
            }
            catch
            {
                throw;
            }
        }
        public async Task<DataTable> GetProvinceByListID(string ids)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@Ids", ids)
                };

                return _DbWorker.GetDataTable(StoreProceduresName.SP_GetListProvinces, objParam);
            }
            catch
            {
                throw;
            }
        }
    }
}
