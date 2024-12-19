
using ENTITIES.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ENTITIES.ViewModels.B2C.AccountB2CViewModel;

namespace REPOSITORIES.IRepositories
{
    public interface IAccountB2CRepository
    {
        Task<int> AddAccountB2C(AccountB2C accountB2);
        Task<bool> checkEmailExtisB2c(string email);
    }
}
