using DAL.Generic;
using DAL.StoreProcedure;
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
    public class BankingAccountDAL : GenericService<BankingAccount>
    {
        private static DbWorker _DbWorker;
        public BankingAccountDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }

        public List<BankingAccount> GetAllBankingAccount()
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.BankingAccount.AsNoTracking().ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAllBankingAccount - BankingAccountDAL: " + ex);
                return null;
            }
        }

        public BankingAccount GetById(int bankAccountId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.BankingAccount.AsNoTracking().FirstOrDefault(n => n.Id == bankAccountId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetById - BankingAccountDAL: " + ex);
                return null;
            }
        }

        public int InsertBankingAccount(BankingAccount model)
        {
            try
            {
                SqlParameter[] objParam_contractPay = new SqlParameter[]
                {
                    new SqlParameter("@BankId", model.BankId ?? (object)DBNull.Value),
                    new SqlParameter("@AccountNumber", model.AccountNumber ?? (object)DBNull.Value),
                    new SqlParameter("@AccountName", model.AccountName ?? (object)DBNull.Value),
                    new SqlParameter("@Branch", model.Branch ?? (object)DBNull.Value),
                    new SqlParameter("@SupplierId", model.SupplierId ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedBy", model.CreatedBy),
                    new SqlParameter("@CreatedDate",DateTime.Now),
                    new SqlParameter("@ClientId",model.ClientId)
                };
                return _DbWorker.ExecuteNonQuery(StoreProceduresName.SP_InsertBankingAccount, objParam_contractPay);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateSupplier - SupplierDAL. " + ex);
                return -1;
            }
        }

        public int UpdateBankingAccount(BankingAccount model)
        {
            try
            {
                SqlParameter[] objParam_contractPay = new SqlParameter[]
                {
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@BankId", model.BankId ?? (object)DBNull.Value),
                    new SqlParameter("@AccountNumber", model.AccountNumber ?? (object)DBNull.Value),
                    new SqlParameter("@AccountName", model.AccountName ?? (object)DBNull.Value),
                    new SqlParameter("@Branch", model.Branch ?? (object)DBNull.Value),
                    new SqlParameter("@SupplierId", model.SupplierId ?? (object)DBNull.Value),
                    new SqlParameter("@UpdatedBy", model.UpdatedBy),
                    new SqlParameter("@ClientId", model.ClientId),
                };
                return _DbWorker.ExecuteNonQuery(StoreProceduresName.SP_UpdateBankingAccount, objParam_contractPay);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateSupplier - SupplierDAL. " + ex);
                return -1;
            }
        }

        public DataTable GetBankAccountDataTableBySupplierId(int supplier_id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@SupplierId", supplier_id)
                };

                return _DbWorker.GetDataTable(StoreProceduresName.SP_GetListBankingAccountBySupplierId, objParam);
            }
            catch
            {
                throw;
            }
        }
        public BankingAccount GetByAccountNumber(string bank_id,string account_number)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.BankingAccount.AsNoTracking().FirstOrDefault(n => n.BankId.ToLower().Trim() == bank_id.ToLower().Trim() && n.AccountNumber.ToLower().Trim()==account_number.ToLower().Trim());
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByAccountNumber - BankingAccountDAL: " + ex);
                return null;
            }
        }
        public async Task<DataTable>  GetListBankingAccountByAccountClientId(int AccountClientId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@AccountClientId", AccountClientId)
                };

                return _DbWorker.GetDataTable(StoreProceduresName.SP_GetListBankingAccountByAccountClientId, objParam);
            }
            catch
            {
                throw;
            }
        }
    }
}
