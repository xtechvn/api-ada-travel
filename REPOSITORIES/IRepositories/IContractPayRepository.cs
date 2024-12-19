using Entities.ViewModels;
using ENTITIES.APPModels.ReadBankMessages;
using ENTITIES.Models;
using ENTITIES.ViewModels.APP.ReadBankMessages;
using ENTITIES.ViewModels.ContractPay;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories
{
    public interface IContractPayRepository
    {
        Task<PaymentSuccessDataViewModel> UpdateOrderBankTransferPayment(BankMessageDetail detail,string contract_pay_code);
        Task<List<ContractPayDetaiByOrderIdlViewModel>> GetContractPayByOrderId(long OrderId);
        Task<List<ENTITIES.Models.BankingAccount>> GetListBankingAccountByAccountClientId(int AccountClientId);
        Task<List<ENTITIES.Models.BankingAccount>> GetBankAccountDataTableBySupplierId(int suplier_id);
    }
}
