using DAL;
using Entities.ConfigModels;
using ENTITIES.Models;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories.Fly;
using System;

namespace REPOSITORIES.Repositories.Fly
{
    public class GroupClassAirlinesDetailRepository : IGroupClassAirlinesDetailRepository
    {
        private readonly GroupClassAirlinesDetailDAL groupClassAirlinesDetailDAL;
        public GroupClassAirlinesDetailRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            groupClassAirlinesDetailDAL = new GroupClassAirlinesDetailDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }

        public GroupClassAirlinesDetail GetDetailGroupClassAirlines(int groupClassId)
        {
            return groupClassAirlinesDetailDAL.GetDetail(groupClassId);
        }
    }
}
