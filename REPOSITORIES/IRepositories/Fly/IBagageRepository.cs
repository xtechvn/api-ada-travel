using ENTITIES.Models;
using System.Collections.Generic;

namespace REPOSITORIES.IRepositories.Fly
{
    public interface IBagageRepository
    {
        List<Baggage> GetBaggages(List<int> passengerIdList);
    }
}
