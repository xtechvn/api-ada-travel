using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.APPModels.ReadBankMessages;
using ENTITIES.Models;
using ENTITIES.ViewModels.APP.ReadBankMessages;
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
    public class PlayGroundDetailDAL : GenericService<PlaygroundDetail>
    {
        private static DbWorker _DbWorker;

        public PlayGroundDetailDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public DataTable GetPlayGroundDetail(string location_code,int service_type)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[2];
                objParam[0] = new SqlParameter("@Code", location_code);
                objParam[1] = new SqlParameter("@ServiceType", service_type);


                return _DbWorker.GetDataTable(ProcedureConstants.sp_getLocationServiceDetailByCode, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetVinWonderPlayGroundDetail - ArticleDAL: " + ex);
            }
            return null;
        }
    }
}
