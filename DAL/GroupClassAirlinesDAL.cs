using DAL.Generic;
using ENTITIES.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace DAL
{
    public class GroupClassAirlinesDAL : GenericService<GroupClassAirlines>
    {
        public GroupClassAirlinesDAL(string connection) : base(connection)
        {
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
    }
}
