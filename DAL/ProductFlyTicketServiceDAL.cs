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

namespace DAL
{
    public class ProductFlyTicketServiceDAL : GenericService<ProductFlyTicketService>
    {
        private static DbWorker _DbWorker;
        public ProductFlyTicketServiceDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
      
        public async Task<DataTable> GetFlyPricePolicyActive()
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[0];
                return _DbWorker.GetDataTable(StoreProceduresName.SP_GetFlyPricePolicyActive, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetFlyPricePolicyActive - OtherBookingDAL: " + ex);
            }
            return null;
        }
    }
}
