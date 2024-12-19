using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using ENTITIES.ViewModels.B2B;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using static ENTITIES.ViewModels.B2C.AccountB2CViewModel;
using static Utilities.Contants.UserConstant;

namespace DAL
{
    public class AccountClientDAL : GenericService<AccountClient>
    {
        private static DbWorker _DbWorker;
        private static string sqlInsertUserAndClient = String.Empty;
        public AccountClientDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public AccountClient GetByAccountUserAndPassword(string userName, string password, int client_type, int type)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    if (type == 1)
                    {
                        return _DbContext.AccountClient.AsNoTracking().FirstOrDefault(s => s.UserName.Equals(userName) && s.Password.Equals(password) && s.ClientType != client_type);
                    }
                    else
                    {
                        return _DbContext.AccountClient.AsNoTracking().FirstOrDefault(s => s.UserName.Equals(userName) && s.Password.Equals(password) && s.ClientType == client_type);
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByAccountUserAndPassword - AccountClientDAL: " + ex);
                return null;
            }
        }
        public async Task<bool> checkEmailExtisB2c(string email)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var data = await _DbContext.Client.AsNoTracking().FirstOrDefaultAsync(x => x.ClientType == (Int16)ClientType.CUSTOMER && x.Email.ToLower() == email.ToLower());
                    return data == null ? false : true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("checkEmailExtisB2c - AccountClientDAL: " + ex);
                return true;
            }
        }
        public async Task<int> AddAccountB2C(AccountB2C accountB2)
        {

            try
            {
                AccountClient accountClient = new AccountClient();
                Client client = new Client();

                accountClient.UserName = accountB2.Email;
                accountClient.ClientType = (byte?)ClientType.CUSTOMER;
                accountClient.Password = accountB2.Password;
                accountClient.PasswordBackup = accountB2.PasswordBackup;
                accountClient.Status = (int)UserStatus.ACTIVE;
                accountClient.GroupPermission = GroupPermissionType.Admin;


                client.ClientName = accountB2.ClientName;
                client.Birthday = DateTime.Now;
                var time = client.Birthday;
                client.JoinDate = DateTime.Now;
                client.UpdateTime = DateTime.Now;
                client.IsReceiverInfoEmail = accountB2.isReceiverInfoEmail;
                client.ClientType = (int)ClientType.CUSTOMER;
                client.AgencyType = AgencyType.CA_NHAN;
                client.PermisionType = PermisionType.KHONG_DC_CONG_NO;
                client.Phone = accountB2.Phone;
                client.Status = (int)UserStatus.ACTIVE;
                client.Email = accountB2.Email;
                client.Note = "Khách hàng được khởi tạo từ hệ thống B2C";
                client.ClientCode = accountB2.ClientCode;
                client.ParentId = -1;

                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var i = _DbContext.User.FirstOrDefault(a => a.UserName.Equals("CtyADAVIGO"));
                    client.SaleMapId = i.Id;
                    var datalist = _DbContext.AccountClient.AsQueryable();
                    var data = _DbContext.Client.Add(client);
                    await _DbContext.SaveChangesAsync();
                    var data2 = _DbContext.Client.AsQueryable();
                    var a = data2.FirstOrDefault(s => s.Email.Equals(accountB2.Email));
                    accountClient.ClientId = a.Id;
                    var data3 = _DbContext.AccountClient.Add(accountClient);
                    await _DbContext.SaveChangesAsync();
                    return 0;

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddAccountB2C - AccountClientDAL: " + ex);
                return -1;
            }
        }
        public async Task<AccountClient> GetbyClientIDAndPassword(long account_client_id, string password, int client_type)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.AccountClient.AsNoTracking().FirstOrDefault(s => s.Id == account_client_id && s.Password == password && s.ClientType == client_type);

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetbyClientIDAndPassword - AccountClientDAL: " + ex);
                return null;
            }
        }
        public async Task<AccountClient> GetbyAccountClientID(long account_client_id, List<int> client_type)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.AccountClient.AsNoTracking().FirstOrDefault(s => s.Id == account_client_id && client_type.Contains((int)s.ClientType));
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetbyAccountClientID - AccountClientDAL: " + ex);
                return null;
            }
        }
        public async Task<AccountClient> GetB2CbyClientID(long client_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.AccountClient.AsNoTracking().FirstOrDefault(s => s.ClientId == client_id && s.ClientType == (int)ClientType.CUSTOMER);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetB2CbyClientID - AccountClientDAL: " + ex);
                return null;
            }
        }
        public async Task<AccountClient> GetB2CByID(long account_client_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.AccountClient.AsNoTracking().FirstOrDefault(s => s.Id == account_client_id && s.ClientType == (int)ClientType.CUSTOMER);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetB2CByID - AccountClientDAL: " + ex);
                return null;
            }
        }
        public async Task<AccountClient> GetB2BById(long account_client_id)
        {
            try
            {
                var list_b2b = new List<int>() { (int)ClientType.AGENT, (int)ClientType.TIER_1_AGENT, (int)ClientType.TIER_2_AGENT, (int)ClientType.TIER_3_AGENT , (int)ClientType.SALE , (int)ClientType.COLLABORATORS , (int)ClientType.ENTERPRISE };
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.AccountClient.AsNoTracking().FirstOrDefault(s => s.Id == account_client_id && list_b2b.Contains((int)s.ClientType));
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetB2BById - AccountClientDAL: " + ex);
                return null;
            }
        }

        public async Task<AccountClient> GetByID(long account_client_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.AccountClient.AsNoTracking().FirstOrDefault(s => s.Id == account_client_id);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByID - AccountClientDAL: " + ex);
                return null;
            }
        }
        public async Task<AccountClient> GetByClientId(long client_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.AccountClient.AsNoTracking().FirstOrDefault(s => s.ClientId == client_id);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddAccountB2C - AccountClientDAL: " + ex);
                return null;
            }
        }
        public AccountClient GetByUserName(string userName)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.AccountClient.AsNoTracking().FirstOrDefault(s => s.UserName == userName);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByUserName - AccountClientDAL: " + ex);
                return null;
            }
        }
        public long UpdataAccountClient(AccountClient model)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {

                    _DbContext.AccountClient.Update(model);
                    _DbContext.SaveChanges();
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddAccountB2C - AccountClientDAL: " + ex);
                return -1;
            }
        }

        public int UpdatePassword(string email, string password)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var accountClient = _DbContext.AccountClient.AsNoTracking().FirstOrDefault(s => s.UserName == email);
                    accountClient.Password = EncodeHelpers.MD5Hash(password);
                    accountClient.PasswordBackup = EncodeHelpers.MD5Hash(password);
                    _DbContext.AccountClient.Update(accountClient);
                    _DbContext.SaveChanges();
                }
                return 1;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdatePassword - AccountClientDAL: " + ex);
                return -1;
            }
        }

        public async Task<int> AddAccountB2B(AccountB2BViewModel accountB2B)
        {

            try
            {
                AccountClient accountClient = new AccountClient();
                Client client = new Client();

                accountClient.UserName = accountB2B.UserName;
                accountClient.ClientType = (byte?)accountB2B.ClientType;
                accountClient.Password = accountB2B.Password;
                accountClient.PasswordBackup = accountB2B.PasswordBackup;
                accountClient.Status = (int)UserStatus.ACTIVE;
                accountClient.GroupPermission = accountB2B.Type;


                client.ClientName = accountB2B.ClientName;
                client.Birthday = DateTime.Now;
                var time = client.Birthday;
                client.JoinDate = DateTime.Now;
                client.UpdateTime = DateTime.Now;

                client.ClientType = accountB2B.ClientType;
                client.AgencyType = AgencyType.TO_CHUC;
                client.PermisionType = PermisionType.KHONG_DC_CONG_NO;
                client.Phone = accountB2B.Phone;
                client.Status = (int)UserStatus.ACTIVE;
                client.Email = accountB2B.Email;
                client.Note = "Khách hàng được khởi tạo từ hệ thống B2B";
                client.ClientCode = accountB2B.ClientCode;
                client.ParentId = accountB2B.ParentId;

                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var i = _DbContext.User.FirstOrDefault(a => a.UserName.Equals("CtyADAVIGO"));
                    client.SaleMapId = i.Id;
                    var datalist = _DbContext.AccountClient.AsQueryable();
                    var data = _DbContext.Client.Add(client);
                    await _DbContext.SaveChangesAsync();
                    var data2 = _DbContext.Client.AsQueryable();
                    var a = data2.FirstOrDefault(s => s.ClientCode.Equals(accountB2B.ClientCode));
                    accountClient.ClientId = a.Id;
                    var data3 = _DbContext.AccountClient.Add(accountClient);
                    await _DbContext.SaveChangesAsync();
                    return 0;

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddAccountB2B - AccountClientDAL: " + ex);
                return -1;
            }
        }
        public async Task<DataTable> GetListAccountClient(long ClientId, long GroupPermission, long Status, long PageIndex, long PageSize, string TextSearch)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[7];
                objParam[0] = new SqlParameter("@ClientID", ClientId);
                objParam[1] = GroupPermission != -1 ? new SqlParameter("@GroupPermission ", GroupPermission): new SqlParameter("@GroupPermission ", DBNull.Value);
                objParam[2] = Status != -1 ? new SqlParameter("@Status ", Status) : new SqlParameter("@Status ", DBNull.Value);
                objParam[3] = new SqlParameter("@PageIndex ", PageIndex);
                objParam[4] = new SqlParameter("@PageSize ", PageSize);
                objParam[5] = new SqlParameter("@TextSearch ", TextSearch);
                if (TextSearch != null && TextSearch.Contains("@"))
                {
                    objParam[5] = new SqlParameter("@TextSearch ", DBNull.Value);
                    objParam[6] = new SqlParameter("@TextSearchEmail ", TextSearch);
                }
                else
                {
                    objParam[5] = new SqlParameter("@TextSearch ", TextSearch);
                    objParam[6] = new SqlParameter("@TextSearchEmail ", DBNull.Value);
                }

                return _DbWorker.GetDataTable(StoreProceduresName.SP_GetListAccountClient, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListAccountClient - ClientDal: " + ex);
            }
            return null;
        }
        public async Task<bool> checkEmailExtisB2B(string email)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var data = await _DbContext.Client.AsNoTracking().FirstOrDefaultAsync(x => x.ClientType != (Int16)ClientType.CUSTOMER && x.Email.ToLower() == email.ToLower());
                    return data == null ? false : true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("checkEmailExtisB2B - AccountClientDAL: " + ex);
                return true;
            }
        }
        public async Task<bool> checkUserNameExtisB2B(string UserName)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var data = await _DbContext.AccountClient.AsNoTracking().FirstOrDefaultAsync(x => x.ClientType != (Int16)ClientType.CUSTOMER && x.UserName.ToLower() == UserName.ToLower());
                    return data == null ? false : true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("checkUserNameExtisB2B - AccountClientDAL: " + ex);
                return true;
            }
        }
        public AccountClient GetAccountClientByUserName(string userName, int type)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    if (type == 1)
                    {
                        return _DbContext.AccountClient.AsNoTracking().FirstOrDefault(s => s.UserName == userName && s.ClientType == (int)ClientType.CUSTOMER);
                    }
                    else
                    {
                        return _DbContext.AccountClient.AsNoTracking().FirstOrDefault(s => s.UserName == userName && s.ClientType != (int)ClientType.CUSTOMER);

                    }
                   
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAccountClientByUserName - AccountClientDAL: " + ex);
                return null;
            }
        }
    }

}
