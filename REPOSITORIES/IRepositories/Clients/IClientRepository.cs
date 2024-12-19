using ENTITIES.APPModels.Client;
using ENTITIES.Models;
using ENTITIES.ViewModels.B2B;
using ENTITIES.ViewModels.Client;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ENTITIES.ViewModels.B2C.AccountB2CViewModel.AcconutViewModel;

namespace REPOSITORIES.IRepositories
{
    public interface IClientRepository
    {
        Client GetDetail(long clientId);
        ClientB2BModel GetClientByUserAndPassword(string userName, string password, int client_type,int type);
        long InsertOrUpdate(ClientViewModel model, out bool isUpdate);
        Task<string> Updata(ClientB2CViewModel clientB2CView, List<int> client_type);
        Task<int> UpdateAccountPassword(string password_old, string password_new, string confirm_password_new, long account_client_id,int client_type);
        Task<int> UpdateAccountInfomationB2B(ClientInfoViewModel model);
        Task<int> UpdateAccountInfomationB2C(ClientInfoViewModel model);
        Task<long> registerAffiliate(long client_id, string ReferralId);
        Task<long> InsertClientB2b(AccountB2BViewModel model);
        long CountClientByParentId(int ParentId);
        Client GetClientDetailByEmail(string Email);

    }
}
