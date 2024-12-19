using DAL;
using Entities.ConfigModels;
using ENTITIES.Models;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories;
using System.Collections.Generic;

namespace REPOSITORIES.Repositories
{
    public class GroupClassAirlinesRepository : IGroupClassAirlinesRepository
    {
        private readonly GroupClassAirlinesDAL groupClassAirlinesDAL;
        public GroupClassAirlinesRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            groupClassAirlinesDAL = new GroupClassAirlinesDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }

        public List<GroupClassAirlines> GetAllData()
        {
            return groupClassAirlinesDAL.GetAllData();
        }

        public GroupClassAirlines GetGroupClassAirlines(string air_line, string class_code, string fare_type)
        {
            return groupClassAirlinesDAL.GetGroupClassAirlines(air_line, class_code, fare_type);
        }
    }
}
