using ENTITIES.Models;
using ENTITIES.ViewModels.FlyTicket;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories
{
    public interface IProductFlyTicketServiceRepository
    {
        public Task<List<FlyPricePolicyViewModel>> GetFlyPricePolicyActive();

    }
}
