using ENTITIES.Models;
using ENTITIES.ViewModels.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories
{
    public interface IUserCoreRepository
    {
        Task<UserMasterViewModel> checkAuthent(string username, string password);
        Task<long> upsertUser(UserMasterViewModel model);
        Task<List<UserMasterViewModel>> getDetail(long user_id, string username, string email);
        Task<long> updateActive2Fa(long user_id);
        Task<long> changePassword(string username, string password);
        
    }
}
