using DAL.Generic;
using ENTITIES.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Utilities;

namespace DAL
{
    public class GroupClassAirlinesDetailDAL : GenericService<GroupClassAirlinesDetail>
    {
        public GroupClassAirlinesDetailDAL(string connection) : base(connection)
        {
        }

        public GroupClassAirlinesDetail GetDetail(int groupClassId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.GroupClassAirlinesDetail.AsNoTracking().FirstOrDefault(n => n.GroupClassAirlinesId == groupClassId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetail - GroupClassAirlinesDetailDAL: " + ex);
                return null;
            }
        }
    }
}
