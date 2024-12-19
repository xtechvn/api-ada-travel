using DAL.Fly;
using Entities.ConfigModels;
using ENTITIES.Models;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories.Fly;
using System.Collections.Generic;

namespace REPOSITORIES.Repositories.Fly
{
    public class FlightSegmentRepository : IFlightSegmentRepository
    {
        private readonly FlightSegmentDAL flightSegmentDAL;
        private readonly IOptions<DataBaseConfig> dataBaseConfig;

        public FlightSegmentRepository(IOptions<DataBaseConfig> _dataBaseConfig)
        {
            flightSegmentDAL = new FlightSegmentDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
            dataBaseConfig = _dataBaseConfig;
        }

        public FlightSegment GetByFlyBookingDetailId(long flyBookingDetailId)
        {
            return flightSegmentDAL.GetFlyBookingDetailId(flyBookingDetailId);
        }

        public List<FlightSegment> GetByFlyBookingDetailIds(List<long> flyBookingDetailIds)
        {
            return flightSegmentDAL.GetFlyBookingDetailIds(flyBookingDetailIds);
        }
    }
}
