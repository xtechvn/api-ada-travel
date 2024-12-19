using DAL.Generic;
using DAL.Orders;
using DAL.StoreProcedure;
using Entities.ViewModels;
using ENTITIES.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{
    public class ContractPayDAL : GenericService<ContractPay>
    {
        private static DbWorker _DbWorker;
        private static OrderDAL orderDAL;

        public ContractPayDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
            orderDAL = new OrderDAL(connection);

        }

        public async Task<bool> CreateContractPay(List<ContractPay> contracts_list)
        {
            try
            {

                using (var _DbContext = new EntityDataContext(_connection))
                { 
                    foreach(var contract in contracts_list)
                    {
                        var exists = _DbContext.ContractPay.Where(x =>  x.ExportDate == contract.ExportDate && x.Amount==contract.Amount).FirstOrDefault();
                        if (exists !=null && exists.PayId > 0)
                        {
                            return false;
                        }
                       
                    }
                    foreach (var contract in contracts_list)
                    {
                        _DbContext.ContractPay.Add(contract);
                        await _DbContext.SaveChangesAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateContractPay - ContractPayDAL: " + ex.ToString());

                return false;
            }
        }

        public long CountContractPayInYear()
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.ContractPay.AsNoTracking().Where(x => ((DateTime)x.CreatedDate).Year == DateTime.Now.Year).Count();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CountContractPayInYear - ContractPayDAL: " + ex.ToString());
                return -1;
            }
        }


        public long CountPaymentVoucherInYear()
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.PaymentVoucher.AsNoTracking().Where(x => ((DateTime)x.CreatedDate).Year == DateTime.Now.Year).Count();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CountPaymentVoucherInYear - ContractPayDAL: " + ex.ToString());
                return -1;
            }
        }
        public long CountPaymentRequest()
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.PaymentRequest.AsNoTracking().Count();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CountPaymentVoucherInYear - ContractPayDAL: " + ex.ToString());
                return -1;
            }
        }
        public long CountInvoiceRequest()
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.InvoiceRequest.AsNoTracking().Count();
                }
               
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CountPaymentVoucherInYear - ContractPayDAL: " + ex.ToString());
                return -1;
            }
        }
    

        public async Task<string> getContractPayByBillNo(string bill_no)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {

                    var data = _DbContext.ContractPay.AsNoTracking().FirstOrDefault(s => s.BillNo == bill_no);
                    return data == null ? "" : data.BillNo;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getContractPayByBillNo - ContractPayDAL: " + ex);
                return "";
            }
        }
        
        
        public int CreateContractPay(ContractPayViewModel model)
        {
            int id = 0;
            List<int> detailIds = new List<int>();
            try
            {
                SqlParameter[] objParam_contractPay = new SqlParameter[16];
                objParam_contractPay[0] = new SqlParameter("@BillNo", model.BillNo);
                if (model.ClientId == null || model.ClientId == 0)
                    objParam_contractPay[1] = new SqlParameter("@ClientId", DBNull.Value);
                else
                    objParam_contractPay[1] = new SqlParameter("@ClientId", model.ClientId);
                objParam_contractPay[2] = new SqlParameter("@Note", string.IsNullOrEmpty(model.Note) ? DBNull.Value.ToString() :
                    model.Note);
                if (model.Type == (int)DepositHistoryConstant.CONTRACT_PAY_TYPE.THU_TIEN_NCC_HOAN_TRA
                    || model.Type == (int)DepositHistoryConstant.CONTRACT_PAY_TYPE.THU_TIEN_HOA_HONG_NCC)
                {
                    objParam_contractPay[3] = new SqlParameter("@Amount", model.ContractPayDetails.Sum(n => n.Amount));
                }
                else
                {
                    objParam_contractPay[3] = new SqlParameter("@Amount", model.Amount);
                }
                objParam_contractPay[4] = new SqlParameter("@Type", model.Type);
                objParam_contractPay[5] = new SqlParameter("@PayType", model.PayType);
                if (model.PayType == (int)DepositHistoryConstant.CONTRACT_PAYMENT_TYPE.CHUYEN_KHOAN)
                {
                    objParam_contractPay[6] = new SqlParameter("@BankingAccountId", model.BankingAccountId);
                }
                else
                {
                    objParam_contractPay[6] = new SqlParameter("@BankingAccountId", DBNull.Value);
                }
                objParam_contractPay[7] = new SqlParameter("@Description", string.IsNullOrEmpty(model.Description)
                    ? DBNull.Value.ToString() : model.Description);
                objParam_contractPay[8] = new SqlParameter("@AttatchmentFile", string.IsNullOrEmpty(model.AttatchmentFile)
                    ? DBNull.Value.ToString() : model.AttatchmentFile);
                objParam_contractPay[9] = new SqlParameter("@ExportDate", DateTime.Now);
                objParam_contractPay[10] = new SqlParameter("@PayStatus", (int)DepositHistoryConstant.CONTRACT_PAY_STATUS.KE_TOAN_DUYET);
                objParam_contractPay[11] = new SqlParameter("@CreatedBy", model.CreatedBy);
                objParam_contractPay[12] = new SqlParameter("@CreatedDate", DateTime.Now);
                objParam_contractPay[13] = new SqlParameter("@SupplierId", Convert.ToInt32(model.SupplierId));
                objParam_contractPay[14] = new SqlParameter("@ObjectType", Convert.ToInt32(model.ObjectType));
                objParam_contractPay[15] = new SqlParameter("@EmployeeId", Convert.ToInt32(model.EmployeeId));
                id = _DbWorker.ExecuteNonQuery(StoreProceduresName.SP_InsertContractPay, objParam_contractPay);
                if (id > 0)
                {
                    foreach (var item in model.ContractPayDetails)
                    {
                        var detailId = 0;
                        SqlParameter[] objParam_contractPayDetail = new SqlParameter[8];
                        objParam_contractPayDetail[0] = new SqlParameter("@PayId", id);
                        if (model.Type == (int)DepositHistoryConstant.CONTRACT_PAY_TYPE.THU_TIEN_KY_QUY)
                        {
                            objParam_contractPayDetail[1] = new SqlParameter("@DataId", item.Id);
                        }
                        else
                        {
                            objParam_contractPayDetail[1] = new SqlParameter("@DataId", item.OrderId);

                        }
                        objParam_contractPayDetail[2] = new SqlParameter("@CreatedBy", model.CreatedBy);
                        objParam_contractPayDetail[3] = new SqlParameter("@Amount", item.Amount);
                        objParam_contractPayDetail[4] = new SqlParameter("@CreatedDate", DateTime.Now);
                        objParam_contractPayDetail[5] = new SqlParameter("@ServiceId", Convert.ToInt64(0));
                        objParam_contractPayDetail[6] = new SqlParameter("@ServiceType", Convert.ToInt32(0));
                        objParam_contractPayDetail[7] = new SqlParameter("@ServiceCode", DBNull.Value);


                        detailId = _DbWorker.ExecuteNonQuery(StoreProceduresName.SP_InsertContractPayDetail, objParam_contractPayDetail);
                        if (detailId > 0)
                            detailIds.Add(detailId);
                        if (detailId <= 0)
                        {
                            using (var _DbContext = new EntityDataContext(_connection))
                            {
                                var entity = _DbContext.ContractPay.Find(id);
                                _DbContext.ContractPay.Remove(entity);
                                foreach (var idDetail in detailIds)
                                {
                                    var detail = _DbContext.ContractPayDetail.Find(idDetail);
                                    _DbContext.ContractPayDetail.Remove(detail);
                                }
                                _DbContext.SaveChanges();
                            }
                            return -1;
                        }

                        //nếu thanh toán đủ thì cập nhật trạng thái của đơn hàng - Đơn hàng đã thanh toán
                        if (model.Type == (int)DepositHistoryConstant.CONTRACT_PAY_TYPE.THU_TIEN_DON_HANG && item.Amount >= item.TotalNeedPayment)
                        {
                            SqlParameter[] objParam_updateFinishPayment = new SqlParameter[5];
                            objParam_updateFinishPayment[0] = new SqlParameter("@OrderId", item.OrderId);
                            objParam_updateFinishPayment[1] = new SqlParameter("@IsFinishPayment", true);
                            objParam_updateFinishPayment[2] = new SqlParameter("@PaymentStatus", (int)PaymentStatus.PAID);
                            objParam_updateFinishPayment[3] = new SqlParameter("@Status", (int)OrderStatus.WAITING_FOR_OPERATOR);
                            objParam_updateFinishPayment[4] = new SqlParameter("@DebtStatus", (int)DepositHistoryConstant.ORDER_DEBT_STATUS.GACH_NO_DU);
                            _DbWorker.ExecuteNonQuery(StoreProceduresName.SP_UpdateOrderFinishPayment, objParam_updateFinishPayment);
                        }
                        if (model.Type == (int)DepositHistoryConstant.CONTRACT_PAY_TYPE.THU_TIEN_DON_HANG && item.Amount < item.TotalNeedPayment)
                        {
                            SqlParameter[] objParam_updateFinishPayment = new SqlParameter[5];
                            objParam_updateFinishPayment[0] = new SqlParameter("@OrderId", item.OrderId);
                            objParam_updateFinishPayment[1] = new SqlParameter("@IsFinishPayment", false);
                            objParam_updateFinishPayment[2] = new SqlParameter("@PaymentStatus", (int)PaymentStatus.PAID_NOT_ENOUGH);
                            objParam_updateFinishPayment[3] = new SqlParameter("@Status", (int)OrderStatus.WAITING_FOR_OPERATOR);
                            objParam_updateFinishPayment[4] = new SqlParameter("@DebtStatus", (int)DepositHistoryConstant.ORDER_DEBT_STATUS.GACH_NO_CHUA_DU);
                            _DbWorker.ExecuteNonQuery(StoreProceduresName.SP_UpdateOrderFinishPayment, objParam_updateFinishPayment);
                        }
                        if (model.Type == (int)DepositHistoryConstant.CONTRACT_PAY_TYPE.THU_TIEN_DON_HANG)
                        {
                            orderDAL.UpdateOrderStatus(item.OrderId, (int)OrderStatus.WAITING_FOR_OPERATOR,
                                model.CreatedBy.Value, model.CreatedBy.Value).Wait();
                        }
                        //nếu thanh toán đủ thì cập nhật trạng thái của nạp quỹ - Chờ duyệt
                        if (model.Type == (int)DepositHistoryConstant.CONTRACT_PAY_TYPE.THU_TIEN_KY_QUY)
                        {
                            SqlParameter[] objParam_updateFinishPayment = new SqlParameter[3];
                            objParam_updateFinishPayment[0] = new SqlParameter("@DepositHistoryId", item.Id);
                            objParam_updateFinishPayment[1] = new SqlParameter("@IsFinishPayment", true);
                            objParam_updateFinishPayment[2] = new SqlParameter("@Status", (int)DepositHistoryConstant.DEPOSIT_STATUS.CHO_DUYET);
                            _DbWorker.ExecuteNonQuery(StoreProceduresName.SP_UpdateDepositFinishPayment, objParam_updateFinishPayment);
                        }
                    }

                    SqlParameter[] objParam_UpdateContractPayDebtStatus = new SqlParameter[3];
                    objParam_UpdateContractPayDebtStatus[0] = new SqlParameter("@PayId", id);
                    objParam_UpdateContractPayDebtStatus[1] = new SqlParameter("@DebtStatus", model.ContractPayDetails.Sum(n => n.Amount) >= model.Amount ?
                        (int)DepositHistoryConstant.CONTRACTPAY_DEBT_STATUS.DA_GACH_HET :
                         (int)DepositHistoryConstant.CONTRACTPAY_DEBT_STATUS.CHUA_GACH_HET);
                    objParam_UpdateContractPayDebtStatus[2] = new SqlParameter("@UpdatedBy", model.CreatedBy);
                    _DbWorker.ExecuteNonQuery(StoreProceduresName.Sp_UpdateDebtStatusByPayId, objParam_UpdateContractPayDebtStatus);
                }
                return id;
            }
            catch (Exception ex)
            {
                DeleteContractPayFail(id, detailIds);
                LogHelper.InsertLogTelegram("CreateContactPay - ContractPayDAL. " + ex);
                return -1;
            }
        }
        private void DeleteContractPayFail(int id, List<int> detailIds)
        {
            using (var _DbContext = new EntityDataContext(_connection))
            {
                var entity = _DbContext.ContractPay.Find(id);
                _DbContext.ContractPay.Remove(entity);
                foreach (var idDetail in detailIds)
                {
                    var detail = _DbContext.ContractPayDetail.Find(idDetail);
                    _DbContext.ContractPayDetail.Remove(detail);
                }
                _DbContext.SaveChanges();
            }
        }
        public async Task<DataTable> GetContractPayByOrderId(long OrderId)
        {
            try
            {

                SqlParameter[] objParam_contractPay = new SqlParameter[1];
                objParam_contractPay[0] = new SqlParameter("@OrderId", OrderId);

                return _DbWorker.GetDataTable(StoreProceduresName.SP_GetContractPayByOrderId, objParam_contractPay);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetContractPayByOrderId - ContractPayDAL. " + ex);
                return null;
            }
        }


    }
}
