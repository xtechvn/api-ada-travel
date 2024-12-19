using DAL.Fly;
using Entities.ConfigModels;
using ENTITIES.Models;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories.Fly;
using System.Collections.Generic;

namespace REPOSITORIES.Repositories.Fly
{
    public class PassengerRepository : IPassengerRepository
    {
        private readonly PassengerDAL passengerDAL;
        private readonly IOptions<DataBaseConfig> dataBaseConfig;

        public PassengerRepository(IOptions<DataBaseConfig> _dataBaseConfig)
        {
            passengerDAL = new PassengerDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
            dataBaseConfig = _dataBaseConfig;
        }

        public List<Passenger> GetPassengers(long orderId)
        {
            return passengerDAL.GetPassengers(orderId);
        }
    }
}
