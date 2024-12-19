using ENTITIES.Models;
using System.Collections.Generic;

namespace REPOSITORIES.IRepositories.Fly
{
    public interface IPassengerRepository
    {
        List<Passenger> GetPassengers(long orderId);

    }
}
