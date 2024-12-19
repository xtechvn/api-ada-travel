using ENTITIES.Models;
using System.Collections.Generic;

namespace REPOSITORIES.IRepositories.Fly
{
    public interface IFlightSegmentRepository
    {
        FlightSegment GetByFlyBookingDetailId(long flyBookingDetailId);
        List<FlightSegment> GetByFlyBookingDetailIds(List<long> flyBookingDetailIds);
    }
}
