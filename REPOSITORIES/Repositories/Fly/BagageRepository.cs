using DAL.Fly;
using Entities.ConfigModels;
using ENTITIES.Models;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories.Fly;
using System;
using System.Collections.Generic;
using System.Text;

namespace REPOSITORIES.Repositories.Fly
{
    public class BagageRepository : IBagageRepository
    {
        private readonly BaggageDAL baggageDAL;
        private readonly IOptions<DataBaseConfig> dataBaseConfig;

        public BagageRepository(IOptions<DataBaseConfig> _dataBaseConfig)
        {
            baggageDAL = new BaggageDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
            dataBaseConfig = _dataBaseConfig;
        }
        public List<Baggage> GetBaggages(List<int> passengerIdList)
        {
            return baggageDAL.GetBaggages(passengerIdList);
        }
    }
}
