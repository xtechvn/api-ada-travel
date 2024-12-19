using DAL.AllotmentFund;
using Entities.ConfigModels;
using ENTITIES.Models;
using ENTITIES.ViewModels.AllotmentFunds;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using REPOSITORIES.IRepositories;
using System;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using Utilities.Content.DisplayText;

namespace REPOSITORIES.Repositories
{
    public class AllotmentFundRepository : IAllotmentFundRepository
    {
        private readonly AllotmentFundDAL allotmentFundDAL;
        private readonly AllotmentHistoryDAL allotmentHistoryDAL;
        private readonly AllotmentUseDAL allotmentUseDAL;

        public AllotmentFundRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            allotmentFundDAL = new AllotmentFundDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            allotmentHistoryDAL = new AllotmentHistoryDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            allotmentUseDAL = new AllotmentUseDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
        public async Task<long> AddAllotmentHistory(AllotmentHistory model)
        {
            return await allotmentHistoryDAL.AddAllotmentHistory(model);
        }

        public async Task<long> UpdateFundBalanceByTransfer(AllotmentFundTransferViewModel model)
        {
            try
            {
                //-- Get Fund from DB
                var to_fund_obj = await allotmentFundDAL.GetAllotmentFundByClientID(model.AccountClientId, model.to_fund_type);
                var from_fund_obj= await allotmentFundDAL.GetAllotmentFundByClientID(model.AccountClientId, model.FundType);
                if(from_fund_obj ==null)
                {
                    LogHelper.InsertLogTelegram("AddOrUpdateAllotmentFund - AllotmentFundRepository: Cannot Find Source Fund with: Client_id="+ model.AccountClientId +" - Fund_type="+ model.to_fund_type);
                    return -1;
                }
                if (to_fund_obj == null)
                {
                    AllotmentFund new_to_fund = new AllotmentFund()
                    {
                        AccountBalance = 0,
                        AccountClientId = model.AccountClientId,
                        CreateDate = DateTime.Now,
                        FundType = model.to_fund_type
                    };
                    var new_id = await allotmentFundDAL.AddAllotmentFund(new_to_fund);
                    to_fund_obj = await allotmentFundDAL.GetAllotmentFundByClientID(model.AccountClientId, model.to_fund_type);
                    if (new_id < 0)
                    {
                        LogHelper.InsertLogTelegram("AddOrUpdateAllotmentFund - AllotmentFundRepository: Cannot create Fund with "+JsonConvert.SerializeObject(new_to_fund));
                        return -1;
                    }
                }
                if(from_fund_obj.AccountBalance< model.amount_move)
                {
                    LogHelper.InsertLogTelegram("AddOrUpdateAllotmentFund - AllotmentFundRepository: Source Fund Balace is not enough to transfer "+model.amount_move+" - "+JsonConvert.SerializeObject(from_fund_obj));
                    return -1;
                }
                from_fund_obj.AccountBalance -= model.amount_move;
                to_fund_obj.AccountBalance += model.amount_move;
                AllotmentHistory history = new AllotmentHistory()
                {
                    AmountMove = model.amount_move,
                    AccountClientId = (long)model.AccountClientId,
                    CreateDate = DateTime.Now,
                    AccountBalance = to_fund_obj.AccountBalance,
                    AllotmentFundId = from_fund_obj.Id,
                    FundTypeFrom = model.FundType,
                    FundTypeTo = model.to_fund_type,
                    PaymentType = (int)PaymentType.KY_QUY,
                    Description = ViVnText.TransferFund
                };
                var history_id = await allotmentFundDAL.UpdateFundBalance(from_fund_obj, to_fund_obj, history);
                return history_id;
            }
            catch(Exception ex)
            {
                LogHelper.InsertLogTelegram("AddOrUpdateAllotmentFund - AllotmentFundRepository: " + ex.ToString());
                return -3;
            }
        }
        public int AddAllotmentUse(AllotmentUse model)
        {
            return allotmentUseDAL.CreateAllotmentUse(model);
        }
        public async Task<long> UpdateFundBalanceByAllotmentUse(AllotmentUse model)
        {
            try
            {
                //-- Get Fund from DB
                var fund = await allotmentFundDAL.GetAllotmentFundByClientID(model.AccountClientId, model.ServiceType);
                if(model.Id<=0 || fund==null || fund.AccountBalance < model.AmountUse)
                {
                    return -1;
                }
                fund.AccountBalance-=model.AmountUse;
                fund.UpdateTime = DateTime.Now;
                allotmentFundDAL.Update(fund);
                return fund.Id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateFundBalanceByAllotmentUse - AllotmentFundRepository: " + ex.ToString());
                return -3;
            }
        }

    }
}
