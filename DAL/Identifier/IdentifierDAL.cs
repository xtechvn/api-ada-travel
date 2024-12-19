using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using ENTITIES.ViewModels.Order;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Utilities;
using Utilities.Contants;
using Utilities.Helpers;

namespace DAL.Identifier
{
    public class IdentifierDAL : GenericService<Order>
    {
        private static DbWorker _DbWorker;


        public IdentifierDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }

        public int countServiceUse(int service_type)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@ServiceType", service_type);

                DataTable tb = new DataTable();
                return _DbWorker.ExecuteNonQuery(StoreProceduresName.sp_countServiceUse, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("service_type - IdentifierDAL: " + ex.ToString());
                return -1;
            }
        }
        public long CountIdentity(int code_type)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@ServiceType", code_type);

                DataTable tb = new DataTable();
                return _DbWorker.ExecuteNonQuery("Sp_CountServiceUse", objParam);

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CountIdentity - IdentifierDAL: " + ex.ToString());
                return -1;
            }
        }

        public int countClientTypeUse(int client_type)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@ClientType", client_type);

                DataTable tb = new DataTable();
                return _DbWorker.ExecuteNonQuery("Sp_CountClientByType", objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("countClientTypeUse - IdentifierDAL: " + ex.ToString());
                return -1;
            }
        }  
        public int CountRequestByDay(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[2];
                objParam[0] = new SqlParameter("@FromDate", FromDate);
                objParam[1] = new SqlParameter("@ToDate", ToDate);

                DataTable tb = new DataTable();
                return _DbWorker.ExecuteNonQuery("Sp_CountRequestByDay", objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("countClientTypeUse - IdentifierDAL: " + ex.ToString());
                return -1;
            }
        }
    }
}
