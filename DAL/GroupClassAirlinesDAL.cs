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
    public class GroupClassAirlinesDAL : GenericService<GroupClassAirlines>
    {
        private static DbWorker _DbWorker;
        public GroupClassAirlinesDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }

        public GroupClassAirlines GetGroupClassAirlines(string air_line, string class_code, string fare_type)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.GroupClassAirlines.AsNoTracking().FirstOrDefault(n =>
                                        n.Airline.ToLower().Equals(air_line.ToLower())
                                        && n.ClassCode.ToLower().Equals(class_code.ToLower())
                                        && n.FareType.ToLower().Equals(fare_type.ToLower())
                    );
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetGroupClassAirlines - GroupClassAirlinesDAL: " + ex);
                return null;
            }
        }

        public List<GroupClassAirlines> GetAllData()
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.GroupClassAirlines.AsNoTracking().ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAllData - GroupClassAirlinesDAL: " + ex);
                return new List<GroupClassAirlines>(); ;
            }
        }
        public  GroupClassAirlines getDetailGroupClassAirlines(string classCode, string airline, string fairtype)
        {
            try
            {
                if (classCode.Contains("_ECO")) classCode = "_ECO";
                if (classCode.Contains("_DLX")) classCode = "_DLX";
                if (classCode.Contains("_BOSS")) classCode = "_BOSS";
                if (classCode.Contains("_SBOSS")) classCode = "_SBOSS";
                if (classCode.Contains("_Combo")) classCode = "_Combo";
                if (airline.ToLower().Equals("vu")) classCode = "";
                SqlParameter[] objParam = new SqlParameter[3];
                objParam[0] = new SqlParameter("@classCode", classCode);
                objParam[1] = new SqlParameter("@airline", airline);
                objParam[2] = new SqlParameter("@fairtype", fairtype);
                DataTable tb = new DataTable();
                _DbWorker.Fill(tb, "SP_GetGroupClassAirlines", objParam);
                return tb.ToList<GroupClassAirlines>().FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAllData getAllAirportCode" + ex);
                return null;
            }
        }
    }
}
