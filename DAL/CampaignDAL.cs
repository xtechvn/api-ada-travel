using DAL.Generic;
using DAL.StoreProcedure;
using Entities.ViewModels;
using ENTITIES.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{
    public class CampaignDAL : GenericService<Campaign>
    {
        private static DbWorker _DbWorker;
        public CampaignDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        
    }
}
