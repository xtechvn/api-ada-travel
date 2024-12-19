using DAL;
using Entities.ConfigModels;
using ENTITIES.Models;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories;
using System.Collections.Generic;

namespace REPOSITORIES.Repositories
{
    public class AirlinesRepository : IAirlinesRepository
    {
        private readonly AirlinesDAL airlinesDAL;
        public AirlinesRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            airlinesDAL = new AirlinesDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }

        public List<Airlines> GetAllData()
        {
            return airlinesDAL.GetAllData();
        }

        public Airlines GetByCode(string code)
        {
            return airlinesDAL.GetByCode(code);
        }
    }
}
