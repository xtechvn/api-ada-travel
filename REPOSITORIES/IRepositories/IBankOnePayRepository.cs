using ENTITIES.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories
{
    public interface IBankOnePayRepository
    {
       Task<List<BankOnePay>> GetAllBankOnePay();
    }
}
