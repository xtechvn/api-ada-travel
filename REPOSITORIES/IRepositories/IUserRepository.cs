using ENTITIES.Models;
using ENTITIES.ViewModels.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories
{
    public interface IUserRepository
    {
        User GetByUserAndPassword(string userName, string password);

        int InsertUserAndClient(List<UserClientModel> listUser);
        public User GetDetail(long userId);
        Task<User> GetChiefofDepartmentByServiceType(int service_type);
        List<string> getManagerEmailByUserId(int user_id);
        List<string> GetLeaderEmailByUserId(int user_id);
        long? GetUserAgentByClientId(long clientId);
        User? GetHeadOfDepartmentByRoleID(long role_id);
    } 
}
