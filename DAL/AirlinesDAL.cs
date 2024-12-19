using DAL.Generic;
using ENTITIES.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace DAL
{
    public class AirlinesDAL : GenericService<Airlines>
    {
        public AirlinesDAL(string connection) : base(connection)
        {
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
    }
}
