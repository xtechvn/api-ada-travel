using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using ENTITIES.ViewModels.DepositHistory;
using ENTITIES.ViewModels.Hotel;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL.DepositHistory
{
    public class DepositHistoryDAL : GenericService<ENTITIES.Models.DepositHistory>
    {

        private static DbWorker _DbWorker;

        public DepositHistoryDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public async Task<List<DepositHistoryViewMdel>> getDepositHistory(long clientId,string service_type, DateTime from_date, DateTime to_date, int page=1,int size=10)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@ClientId", clientId),
                    new SqlParameter("@ServiceType", service_type),
                    new SqlParameter("@PageIndex", page),
                    new SqlParameter("@PageSize", size),
                    new SqlParameter("@FromDate", from_date),
                    new SqlParameter("@ToDate", to_date),

                };

                var row= _DbWorker.GetDataTable(StoreProceduresName.SP_FE_GetContractPayDepositByClientId, objParam);
                if(row!=null && row.Rows!=null && row.Rows.Count > 0)
                {
                    return row.ToList<DepositHistoryViewMdel>();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDepositHistory - DepositHistoryDAL: " + ex.ToString());
            }
            return new List<DepositHistoryViewMdel>();
        }
        public AmountDeposit amountDeposit(long clientid, int SERVICE_TYPE)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    AmountDeposit data = new AmountDeposit();
                    List<AllotmentUse> listallotmentUses = new List<AllotmentUse>();
                    List<ENTITIES.Models.AllotmentFund> allotmentFund = new List<ENTITIES.Models.AllotmentFund>();
                    switch (SERVICE_TYPE)
                    {
                        case (int)ServicesType.VINHotelRent:
                            allotmentFund = _DbContext.AllotmentFund.AsNoTracking().Where(s => s.AccountClientId == clientid && s.FundType == (int)ServicesType.VINHotelRent).ToList();
                            break;
                        case (int)ServicesType.OthersHotelRent:
                            allotmentFund = _DbContext.AllotmentFund.AsNoTracking().Where(s => s.AccountClientId == clientid && s.FundType == (int)ServicesType.OthersHotelRent).ToList();
                            break;
                        case (int)ServicesType.FlyingTicket:
                            allotmentFund = _DbContext.AllotmentFund.AsNoTracking().Where(s => s.AccountClientId == clientid && s.FundType == (int)ServicesType.FlyingTicket).ToList();
                            break;
                        case (int)ServicesType.VehicleRent:
                            allotmentFund = _DbContext.AllotmentFund.AsNoTracking().Where(s => s.AccountClientId == clientid && s.FundType == (int)ServicesType.VehicleRent).ToList();
                            break;
                        case (int)ServicesType.Tourist:
                            allotmentFund = _DbContext.AllotmentFund.AsNoTracking().Where(s => s.AccountClientId == clientid && s.FundType == (int)ServicesType.Tourist).ToList();
                            break;

                        default:
                            break;
                    }
                    //List<AllotmentFund> allotmentFund = _DbContext.AllotmentFund.AsNoTracking().Where(s => s.ClientId == clientid).ToList();

                    foreach (var item in allotmentFund)
                    {
                        var allotmentUses = (from a in _DbContext.AllotmentUse.AsNoTracking().Where(s => s.AllomentFundId == item.Id)
                                             join b in _DbContext.Order.AsNoTracking().Where(s => s.OrderStatus != (int)OrderStatus.CANCEL && s.ServiceType == item.FundType) on a.DataId equals (b.OrderId)
                                             select new AllotmentUse
                                             {
                                                 Id = a.Id,
                                                 DataId = a.DataId,
                                                 CreateDate = a.CreateDate,
                                                 AllomentFundId = a.AllomentFundId,
                                                 AmountUse = a.AmountUse,
                                                 AccountClientId = a.AccountClientId,
                                             }).ToList();
                        if (allotmentUses.Count > 0)
                            listallotmentUses.AddRange(allotmentUses);
                    }


                    var dataAllCode = _DbContext.AllCode.AsNoTracking().Where(s => s.Type == AllCodeType.SERVICE_TYPE).ToList();
                    foreach (var i in dataAllCode)
                    {
                        if (i.CodeValue == SERVICE_TYPE)
                            data.fundtypeName = i.Description;
                    }
                    data.AllotmentUse = listallotmentUses;
                    data.AllotmentFund = allotmentFund;

                    return data;
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("amountDeposit - DepositHistoryDAL: " + ex.ToString());
                return null;
            }

        }
        

        public List<ENTITIES.Models.AllotmentFund> getAllotmentFund(long clientid)
        {

            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {

                    return _DbContext.AllotmentFund.AsNoTracking().Where(s => s.AccountClientId == clientid).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("amountDeposit - DepositHistoryDAL: " + ex.ToString());
                return null;
            }


        }
        public async Task<int> CreateDepositHistory(ENTITIES.Models.DepositHistory model)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    _DbContext.DepositHistory.Add(model);
                    _DbContext.SaveChanges();
                    return (int)ResponseType.SUCCESS;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("amountDeposit - DepositHistoryDAL: " + ex.ToString());
                return (int)ResponseType.ERROR;
            }

        }
        public long CountOrderInYear()
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.DepositHistory.AsNoTracking().Where(x => (x.CreateDate ?? DateTime.Now).Year == DateTime.Now.Year).Count();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CountOrderInYear - DepositHistoryDAL: " + ex.ToString());
                return -1;
            }
        }
        public async Task<string> getDepositHistoryByTransNo(string order_no)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {

                    var data = _DbContext.DepositHistory.AsNoTracking().FirstOrDefault(s => s.TransNo == order_no);
                    return data == null ? "" : data.TransNo;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getDepositHistoryByTransNo - DepositHistoryDAL: " + ex);
                return "";
            }
        }

        //cập nhật trạng thái chờ thanh toán. Sau khi bấm nút thanh toán
        // Veirify bước trước đó có phải là đang trạng thái tạo mới ko. Tránh nhảy cóc
        public async Task<bool> checkOutDeposit(Int64 user_id, string trans_no, string bank_name)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var deposit = await _DbContext.DepositHistory.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == user_id && x.TransNo == trans_no && x.Status == TransStatusType.CREATE_NEW_TRANS);
                    if (deposit != null)
                    {
                        deposit.Status = TransStatusType.WAITING_VERIFY_PAYMENT;
                        deposit.BankName = bank_name;
                        deposit.UpdateLast = DateTime.Now;
                        _DbContext.Update(deposit);
                        await _DbContext.SaveChangesAsync();

                        return deposit.Id > 0 ? true : false;
                    }
                    else
                    {
                        LogHelper.InsertLogTelegram("checkOutDeposit - DepositHistoryDAL: ko tim thay thong tin repository theo user_id = " + user_id + ", trans_no=" + trans_no + ", bank_name=" + bank_name);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("amountDeposit - DepositHistoryDAL: " + ex.ToString());
                return false;
            }

        }
        //cập nhật trạng thái chờ thanh toán. Sau khi bấm nút thanh toán        
        public async Task<bool> updateProofTrans(Int64 user_id, string trans_no, string link_proof)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var deposit = await _DbContext.DepositHistory.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == user_id && x.TransNo == trans_no);
                    if (deposit != null)
                    {
                        deposit.ImageScreen = link_proof;
                        deposit.UpdateLast = DateTime.Now;
                        _DbContext.Update(deposit);
                        await _DbContext.SaveChangesAsync();

                        return deposit.Id > 0 ? true : false;
                    }
                    else
                    {
                        LogHelper.InsertLogTelegram("updateProofTrans - DepositHistoryDAL: ko tim thay thong tin repository theo user_id = " + user_id + ", trans_no=" + trans_no + ", link_proof=" + link_proof);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("updateProofTrans - DepositHistoryDAL: " + ex.ToString());
                return false;
            }

        }
        //cập nhật trạng thái chờ kế toán duyệt sau khi bot verify   
        public async Task<bool> updateStatusBotVerifyTrans(string trans_no)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var deposit = await _DbContext.DepositHistory.AsNoTracking().FirstOrDefaultAsync(x => x.TransNo == trans_no && x.Status == TransStatusType.WAITING_VERIFY_PAYMENT); // kiểm tra trans có đang là chờ thanh toán ko
                    if (deposit != null)
                    {
                        deposit.Status = TransStatusType.WAITING_VERIFY_ACCOUNTANT;
                        deposit.UpdateLast = DateTime.Now;
                        _DbContext.Update(deposit);
                        await _DbContext.SaveChangesAsync();

                        return deposit.Id > 0 ? true : false;
                    }
                    else
                    {
                        LogHelper.InsertLogTelegram("updateStatusBotVerifyTrans - DepositHistoryDAL: ko tim thay thong tin repository theo  trans_no=" + trans_no);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("updateStatusBotVerifyTrans - DepositHistoryDAL: " + ex.ToString());
                return false;
            }

        }
        //cập nhật trạng thái  kế toán duyệt tu cms
        public async Task<bool> VerifyTrans(string trans_no, Int16 is_verify, string note, Int16 user_verify, int contract_pay_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    decimal price_verify = 0;
                    var deposit = await _DbContext.DepositHistory.AsNoTracking().FirstOrDefaultAsync(x => x.TransNo == trans_no && x.Status == TransStatusType.WAITING_VERIFY_ACCOUNTANT); // kiểm tra trans có đang là chờ thanh toán ko
                    var pay_detail = _DbContext.ContractPayDetail.Where(x => x.DataId == deposit.Id);

                    // Kiểm tra chi tiết phiếu thu đã khớp với mã trans chưa.
                    if (pay_detail.Count() == 0)
                    {
                        LogHelper.InsertLogTelegram("[API] VerifyTrans - DepositHistoryDAL: ma trans_no=" + trans_no + ", data_id = " + deposit.Id + " khong khop voi chi tiet phieu thu Payid = " + contract_pay_id);
                        return false;
                    }
                    else
                    {
                        // Lấy ra số tiền đã được duyệt cho mã trans
                        price_verify = pay_detail.Sum(x => x.Amount ?? 0);
                    }

                    if (deposit != null)
                    {
                        deposit.Status = is_verify;
                        deposit.UpdateLast = DateTime.Now;
                        deposit.NoteReject = is_verify == TransStatusType.REJECT_VERIFY_ACCOUNTANT ? note : deposit.NoteReject;
                        deposit.UserVerifyId = user_verify;
                        deposit.VerifyDate = DateTime.Now;
                        _DbContext.Update(deposit);
                        await _DbContext.SaveChangesAsync();

                        if (deposit.Id > 0)
                        {
                            // Sau khi kế toán duyệt thành công sẽ thực hiện chuyển đổi số tiền vào quỹ
                            // cuonglv update: 08-7-2023
                            // Kiểm tra khách hàng này đã có khởi tạo quỹ lần nào chưa
                            var allotmentFundDetail = _DbContext.AllotmentFund.FirstOrDefault(x => x.AccountClientId == deposit.UserId && x.FundType == deposit.ServiceType);
                            if (allotmentFundDetail != null)
                            {
                                // Cộng dồn số tiền nạp vào quỹ 
                                allotmentFundDetail.AccountBalance += Convert.ToDouble(price_verify);
                                allotmentFundDetail.UpdateTime = DateTime.Now;
                                await _DbContext.SaveChangesAsync();
                                return allotmentFundDetail.Id > 0 ? true : false;
                            }
                            else
                            {
                                // Khởi tạo quỹ với số tiền cần nạp
                                var model = new ENTITIES.Models.AllotmentFund
                                {
                                    FundType = Convert.ToInt16(deposit.ServiceType), // Loại quỹ cần nạp
                                    AccountBalance = Convert.ToDouble(price_verify),
                                    AccountClientId = deposit.UserId,
                                    CreateDate = DateTime.Now
                                };
                                _DbContext.AllotmentFund.Add(model);
                                await _DbContext.SaveChangesAsync();
                                return model.Id > 0 ? true : false;

                            }
                        }
                        return false;
                    }
                    else
                    {
                        LogHelper.InsertLogTelegram("VerifyTrans - DepositHistoryDAL: ko tim thay thong tin repository theo  trans_no=" + trans_no);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("VerifyTrans - DepositHistoryDAL: " + ex.ToString());
                return false;
            }

        }
        public async Task<ENTITIES.Models.DepositHistory> GetDepositHistoryByTransNo(string trans_no)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {

                    var data = _DbContext.DepositHistory.AsNoTracking().FirstOrDefault(s => s.TransNo.ToLower() == trans_no.ToLower());
                    return data;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getDepositHistoryByTransNo - DepositHistoryDAL: " + ex);
                return null;
            }
        }

    }
}
