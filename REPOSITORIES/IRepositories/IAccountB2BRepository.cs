using ENTITIES.Models;
using ENTITIES.ViewModels.B2B;
using ENTITIES.ViewModels.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories
{
    public interface IAccountB2BRepository
    {
        Task<AccountClient> GetAccountClientById(long accountClientId);
        Task<ClientB2BDetailUpdateViewModel> GetClientB2BDetailViewModel(long clientId);
        Task<long> UpdateClientDetail(ClientB2BDetailUpdateViewModel model, long clientId);
        Task<List<ListAccountClientModel>> GetListAccountClient(long ClientId, long GroupPermission, long Status, long PageIndex, long PageSize, string TextSearch);
        Task<long> UpdataAccountClientB2B(AccountClientB2BViewModel model);
        Task<bool> checkEmailExtisB2B(string email);
        Task<bool> checkUserNameExtisB2B(string UserName);
        Task<bool> checkEmailClient(string email, long id);
    }
}
