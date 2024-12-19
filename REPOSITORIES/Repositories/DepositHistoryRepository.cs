using DAL;
using DAL.AllotmentFund;
using DAL.DepositHistory;
using Entities.ConfigModels;
using Entities.ViewModels;
using ENTITIES.Models;
using ENTITIES.ViewModels.Client;
using ENTITIES.ViewModels.DepositHistory;
using Microsoft.Extensions.Options;
using Nest;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using static MongoDB.Driver.WriteConcern;

namespace REPOSITORIES.Repositories
{
    public class DepositHistoryRepository : IDepositHistoryRepository
    {
        private readonly DepositHistoryDAL depositHistoryDAL;
        private readonly AllCodeDAL AllCodeDAL;
        private readonly AllotmentFundDAL allotmentFundDAL;
        private readonly ClientDAL clientDAL;
        public DepositHistoryRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            depositHistoryDAL = new DepositHistoryDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            AllCodeDAL = new AllCodeDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            clientDAL = new ClientDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            allotmentFundDAL = new AllotmentFundDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
        public async Task<GenericViewModel<DepositHistoryViewMdel>> getDepositHistory(long clientId,string service_types, DateTime from_date, DateTime to_date, int page=1, int size=10)
        {
            try
            {
                var data= await depositHistoryDAL.getDepositHistory(clientId, service_types, from_date, to_date, page,size);
                int total_page = 0;
                int total_record = 0;
                if (data!=null && data.Count > 0)
                {
                    total_record = data[0].TotalRow;
                    total_page = (int)(data[0].TotalRow / size);
                }
                return new GenericViewModel<DepositHistoryViewMdel>()
                {
                    CurrentPage = page,
                    ListData = data,
                    PageSize = size,
                    TotalPage = total_page,
                    TotalRecord = total_record
                };
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getDepositHistory - DepositHistoryRepository: " + ex.ToString());
                return null;
            }
        }
        public async Task<List<AmountServiceDeposit>> amountDeposit(long clientid)
        {
            try
            {

                List<AmountServiceDeposit> list_amount = new List<AmountServiceDeposit>();
                var data_list = depositHistoryDAL.getAllotmentFund(clientid);
                var Allcode = AllCodeDAL.GetAllCodeByType(AllCodeType.SERVICE_TYPE).Result;
                if (data_list.Count > 0)
                {
                    foreach (var i in Allcode)
                    {
                        AmountServiceDeposit amount = new AmountServiceDeposit();
                        var data = depositHistoryDAL.amountDeposit(clientid, i.CodeValue);

                        DataTable dt = await clientDAL.GetClientByID(clientid);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            var ListClient = dt.ToList<ClientDeatilModel>();
                            var detailClient = ListClient[0];
                            switch (i.CodeValue)
                            {
                                case (short)ServicesType.FlyingTicket:
                                    {
                                        amount.TotalAmount = detailClient.TotalAmountFlyUse;
                                        amount.TotalDebtAmount = detailClient.ProductFlyTicketDebtAmount;
                                    }
                                    break;
                                case (short)ServicesType.OthersHotelRent:
                                case (short)ServicesType.VINHotelRent:
                                    {
                                        amount.TotalAmount = detailClient.TotalAmountHotelUse;
                                        amount.TotalDebtAmount = detailClient.HotelDebtAmout;
                                    }
                                    break;
                                case (short)ServicesType.Tourist:
                                    {
                                        amount.TotalAmount = detailClient.TotalAmountTourUse;
                                        amount.TotalDebtAmount = detailClient.TourDebtAmount;
                                    }
                                    break;
                                case (short)ServicesType.VinWonderTicket:
                                    {
                                        amount.TotalAmount = detailClient.TotalAmountVinWonderUse;
                                        amount.TotalDebtAmount = detailClient.VinWonderDebtAmount;
                                    }
                                    break;
                                case (short)ServicesType.Other:
                                case (short)ServicesType.VehicleRent:
                                    {
                                        amount.TotalAmount = detailClient.TotalAmountOtherUse;
                                        amount.TotalDebtAmount = detailClient.TouringCarDebtAmount;
                                    }
                                    break;

                            }
                        }
                        double sumallotmentFund = 0;
                        double sumallotmentUse = 0;
                        foreach (var item in data.AllotmentFund)
                        {
                            sumallotmentFund += item.AccountBalance;
                        }
                        foreach (var item in data.AllotmentUse)
                        {
                            sumallotmentUse += item.AmountUse;
                        }

                        amount.account_blance = (float)(sumallotmentFund - sumallotmentUse);
                        amount.service_name = data.fundtypeName;
                        amount.service_type = i.CodeValue;
                        list_amount.Add(amount);
                    }
                }
                else
                {


                    foreach (var item in Allcode)
                    {
                        AmountServiceDeposit amount = new AmountServiceDeposit();
                        DataTable dt = await clientDAL.GetClientByID(clientid);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            var ListClient = dt.ToList<ClientDeatilModel>();
                            var detailClient = ListClient[0];
                            switch (item.CodeValue)
                            {
                                case (short)ServicesType.FlyingTicket:
                                    {
                                        amount.TotalAmount = detailClient.TotalAmountFlyUse;
                                        amount.TotalDebtAmount = detailClient.ProductFlyTicketDebtAmount;
                                    }
                                    break;
                                case (short)ServicesType.OthersHotelRent:
                                case (short)ServicesType.VINHotelRent:
                                    {
                                        amount.TotalAmount = detailClient.TotalAmountHotelUse;
                                        amount.TotalDebtAmount = detailClient.HotelDebtAmout;
                                    }
                                    break;
                                case (short)ServicesType.Tourist:
                                    {
                                        amount.TotalAmount = detailClient.TotalAmountTourUse;
                                        amount.TotalDebtAmount = detailClient.TourDebtAmount;
                                    }
                                    break;
                                case (short)ServicesType.VinWonderTicket:
                                    {
                                        amount.TotalAmount = detailClient.TotalAmountVinWonderUse;
                                        amount.TotalDebtAmount = detailClient.VinWonderDebtAmount;
                                    }
                                    break;
                                case (short)ServicesType.Other:
                                case (short)ServicesType.VehicleRent:
                                    {
                                        amount.TotalAmount = detailClient.TotalAmountOtherUse;
                                        amount.TotalDebtAmount = detailClient.TouringCarDebtAmount;
                                    }
                                    break;

                            }
                        }
                        amount.account_blance = 0;
                        amount.service_name = item.Description;
                        amount.service_type = item.CodeValue;
                        list_amount.Add(amount);
                    }
                }
                return list_amount;

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("amountDeposit - DepositHistoryRepository: " + ex.ToString());
                return null;
            }
        }
        public async Task<List<AmountServiceDeposit>> GetAmountDepositB2B(long clientid,long account_client_id,  List<int> SERVICES_TYPE_B2B)
        {
            List<AmountServiceDeposit> result = new List<AmountServiceDeposit>();
            try
            {
                var data_list = depositHistoryDAL.getAllotmentFund(account_client_id);
                var Allcode = AllCodeDAL.GetAllCodeByType(AllCodeType.SERVICE_TYPE).Result;
                DataTable dt = await clientDAL.GetClientByID(clientid);
                ClientDeatilModel detail = new ClientDeatilModel()
                {

                };
                if (dt != null && dt.Rows.Count > 0)
                {
                    var ListClient = dt.ToList<ClientDeatilModel>();
                    detail = ListClient[0];
                   
                }
                foreach (var type in SERVICES_TYPE_B2B)
                {
                    double balance = 0;
                    double total_amount = 0;
                    double total_debt_amount = 0;
                    var fund_exists = data_list.Count <= 0 || data_list.FirstOrDefault(x => x.FundType == type) == null ? null : data_list.FirstOrDefault(y => y.FundType == type);
                    if (fund_exists == null)
                    {

                        //-- Calucate Amount Balanced
                        //-- Deposit:
                        DateTime from_date = DateTime.ParseExact("01/01/2020", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime to_date = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        var deposit = await depositHistoryDAL.getDepositHistory(clientid, string.Join(",", SERVICES_TYPE_B2B), from_date,to_date, 1,100);
                        if (deposit != null && deposit.Count > 0)
                        {
                            balance += deposit.Where(x=>x.ServiceType==type).Sum(x => x.Amount);
                        }
                        //-- Transfer Fund (updated on itself):
                        //-- Withdraw for Order:
                        var withdraw = allotmentFundDAL.GetAllotmentUseDepositByClientId(clientid,type);
                        if (withdraw != null && withdraw.Rows != null && withdraw.Rows.Count > 0)
                        {
                            var list_withdraw = withdraw.ToList<AllotmentUse>();
                            balance -= list_withdraw.Sum(x => x.AmountUse);
                        }
                       //-- Add new
                        fund_exists = new ENTITIES.Models.AllotmentFund()
                        {
                            AccountBalance = balance,
                            AccountClientId = account_client_id,
                            CreateDate = DateTime.Now,
                            FundType = type,
                            UpdateTime = DateTime.Now
                        };
                        fund_exists.Id = await allotmentFundDAL.AddAllotmentFund(fund_exists);
                    }
                    else
                    {
                        balance = fund_exists.AccountBalance;
                    }
                    //-- Calucate total balance and debt
                    switch (type)
                    {
                        case (short)ServicesType.FlyingTicket:
                            {
                                total_amount = detail.TotalAmountFlyUse;
                                total_debt_amount = detail.ProductFlyTicketDebtAmount;
                            }
                            break;
                        case (short)ServicesType.OthersHotelRent:
                        case (short)ServicesType.VINHotelRent:
                            {
                                total_amount = detail.TotalAmountHotelUse;
                                total_debt_amount = detail.HotelDebtAmout;
                            }
                            break;
                        case (short)ServicesType.Tourist:
                            {
                                total_amount = detail.TotalAmountTourUse;
                                total_debt_amount = detail.TourDebtAmount;
                            }
                            break;
                        case (short)ServicesType.VinWonderTicket:
                            {
                                total_amount = detail.TotalAmountVinWonderUse;
                                total_debt_amount = detail.VinWonderDebtAmount;
                            }
                            break;
                        case (short)ServicesType.Other:
                        case (short)ServicesType.VehicleRent:
                            {
                                total_amount = detail.TotalAmountOtherUse;
                                total_debt_amount = detail.TouringCarDebtAmount;
                            }
                            break;

                    }
                    result.Add(new AmountServiceDeposit()
                    {
                        id= fund_exists.Id,
                        account_blance = balance,
                        service_name=Allcode.First(x=>x.CodeValue== type).Description,
                        service_type=type,
                        TotalAmount= total_amount,
                        TotalDebtAmount=total_debt_amount,
                    });
                }
                

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAmountDepositB2B - DepositHistoryRepository: " + ex.ToString());
            }
            return result;

        }

        public async Task<int> CreateDepositHistory(ENTITIES.Models.DepositHistory model)
        {
            try
            {
                var data = await depositHistoryDAL.getDepositHistoryByTransNo(model.TransNo);
                if (data == "")
                {
                    model.CreateDate = DateTime.Now;
                    model.Status = TransStatusType.CREATE_NEW_TRANS;
                    model.PaymentType = (short?)PaymentType.CHUYEN_KHOAN_TRUC_TIEP;
                    var result = await depositHistoryDAL.CreateDepositHistory(model);
                    return result;
                }
                else
                {
                    LogHelper.InsertLogTelegram("amountDeposit - DepositHistoryRepository: Mã TransNo :" + model.TransNo + " đã tồn tại");
                    return (int)ResponseType.ERROR; ;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("amountDeposit - DepositHistoryRepository: " + ex.ToString());
                return (int)ResponseType.ERROR; ;
            }
        }

        public async Task<bool> checkOutDeposit(Int64 user_id, string trans_no, string bank_name)
        {
            try
            {
                var result = await depositHistoryDAL.checkOutDeposit(user_id, trans_no, bank_name);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("amountDeposit - DepositHistoryRepository: " + ex.ToString());
                return false;
            }
        }

        public async Task<bool> updateProofTrans(Int64 user_id, string trans_no, string link_proof)
        {
            try
            {
                var result = await depositHistoryDAL.updateProofTrans(user_id, trans_no, link_proof);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("updateProofTrans - DepositHistoryRepository: " + ex.ToString());
                return false;
            }
        }

        public async Task<bool> BotVerifyTrans(string trans_no)
        {
            try
            {
                var result = await depositHistoryDAL.updateStatusBotVerifyTrans(trans_no);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("BotVerifyTrans - DepositHistoryRepository: " + ex.ToString());
                return false;
            }
        }

        public async Task<bool> VerifyTrans(string trans_no, Int16 is_verify, string note, Int16 user_verify, int contract_pay_id)
        {
            try
            {
                var result = await depositHistoryDAL.VerifyTrans(trans_no, is_verify, note, user_verify, contract_pay_id);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("VerifyTrans - DepositHistoryRepository: " + ex.ToString());
                return false;
            }
        }
        public async Task<ENTITIES.Models.DepositHistory> GetDepositHistoryByTransNo(string trans_no)
        {
            try
            {
               return await  depositHistoryDAL.GetDepositHistoryByTransNo(trans_no);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getDepositHistoryByTransNo - DepositHistoryRepository: " + ex);
                return null;
            }
        }
    }
}
