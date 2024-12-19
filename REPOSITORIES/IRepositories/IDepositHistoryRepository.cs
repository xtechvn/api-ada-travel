using Entities.ViewModels;
using ENTITIES.ViewModels.DepositHistory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories
{
    public interface IDepositHistoryRepository
    {
        Task<GenericViewModel<DepositHistoryViewMdel>> getDepositHistory(long clientId, string service_types,DateTime from_date,DateTime to_date, int page = 1, int size = 10);
        Task<List<AmountServiceDeposit>> amountDeposit(long clientid);
        Task<List<AmountServiceDeposit>> GetAmountDepositB2B(long clientid, long account_client_id,  List<int> SERVICES_TYPE_B2B);
        Task<int> CreateDepositHistory(ENTITIES.Models.DepositHistory model);
        Task<bool> checkOutDeposit(Int64 user_id, string trans_no, string bank_name);
        Task<bool> updateProofTrans(Int64 user_id, string trans_no, string link_proof);
        Task<bool> BotVerifyTrans(string trans_no);
        Task<bool> VerifyTrans(string trans_no, Int16 is_verify, string note,Int16 user_verify, int contract_pay_id); //accountant verify
        Task<ENTITIES.Models.DepositHistory> GetDepositHistoryByTransNo(string trans_no);
    }
}
