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
    public class AirPortCodeRepository : IAirPortCodeRepository
    {
        private readonly AirPortCodeDAL airPortCodeDAL;
        private readonly IOptions<DataBaseConfig> dataBaseConfig;

        public AirPortCodeRepository(IOptions<DataBaseConfig> _dataBaseConfig)
        {
            airPortCodeDAL = new AirPortCodeDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
            dataBaseConfig = _dataBaseConfig;
        }

        public List<AirPortCode> GetAirPortCodes()
        {
            return airPortCodeDAL.GetAirPortCodes();
        }
    }
}
