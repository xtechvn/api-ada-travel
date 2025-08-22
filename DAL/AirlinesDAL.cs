using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Utilities;

namespace DAL
{
    public class AirlinesDAL : GenericService<Airlines>
    {
        private static DbWorker _DbWorker;
        public AirlinesDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }

        public Airlines GetByCode(string code)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.Airlines.AsNoTracking().FirstOrDefault(n => n.Code.ToLower().Equals(code.ToLower()));
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByCode - AirlinesDAL: " + ex);
                return null;
            }
        }

        public List<Airlines> GetAllData()
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.Airlines.AsNoTracking().ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByCode - AirlinesDAL: " + ex);
                return new List<Airlines>();
            }
        }
        public static List<Airlines> getAirlinesCodes(List<String> lstCode)
        {
            try
            {
                var listAirline = new List<Airlines>();
                foreach (var item in lstCode)
                {
                    SqlParameter[] objParam = new SqlParameter[1];
                    objParam[0] = new SqlParameter("@airlinesCodes", string.Join(",", lstCode));
                    DataTable tb = new DataTable();
                    _DbWorker.Fill(tb, "SP_GetAirlinesCodes", objParam);
                    var result = tb.ToList<Airlines>().FirstOrDefault();
                    if (result != null)
                        listAirline.Add(result);
                }
                return listAirline;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getAirlinesCodes - AirlinesDAL: " + ex);
                return null;
            }
        }
    }
}
