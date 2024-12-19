using DAL;
using Entities.ConfigModels;
using ENTITIES.APPModels.Client;
using ENTITIES.Models;
using ENTITIES.ViewModels.B2B;
using ENTITIES.ViewModels.Client;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using static ENTITIES.ViewModels.B2C.AccountB2CViewModel.AcconutViewModel;
using static Utilities.Contants.CommonConstant;

namespace REPOSITORIES.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ClientDAL clientDAL;
        private readonly AddressClientDAL addressClientDAL;
        private readonly AccountClientDAL accountClientDAL;
        private readonly UserDAL userDAL;
        private readonly UserAgenDAL userAgenDAL;
        private readonly IOptions<DataBaseConfig> dataBaseConfig;

        public ClientRepository(IOptions<DataBaseConfig> _dataBaseConfig)
        {
            clientDAL = new ClientDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
            userDAL = new UserDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
            userAgenDAL = new UserAgenDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
            addressClientDAL = new AddressClientDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
            accountClientDAL = new AccountClientDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
            dataBaseConfig = _dataBaseConfig;
        }
        public ClientB2BModel GetClientByUserAndPassword(string userName, string password, int client_type, int type)
        {
            try
            {
                var user = accountClientDAL.GetByAccountUserAndPassword(userName, password, client_type, type);
                if (user == null) return null;
                var address = addressClientDAL.GetByClientMapId((int)user.ClientId);
                var client = clientDAL.GetByClientId((long)user.ClientId);
                if (client == null) return null;

                var clientB2BModel = new ClientB2BModel();
                clientB2BModel.UserName = user.UserName;
                clientB2BModel.Avatar = client.Avartar;
                clientB2BModel.ClientName = client?.ClientName;
                clientB2BModel.ClientId = user.Id;
                clientB2BModel.ClientType = (int)client.ClientType;
                clientB2BModel.Status = user.Status == null ? 0 : (int)user.Status;
                clientB2BModel.Phone = client.Phone;
                clientB2BModel.gender = client.Gender == null ? 0 : (int)client.Gender;
                clientB2BModel.IsRegisterAffiliate = client.IsRegisterAffiliate == null ? false:(bool)client.IsRegisterAffiliate;
                clientB2BModel.ReferralId = client.ReferralId;
                clientB2BModel.ParentId = client.ParentId==null? -1 :(int)client.ParentId;
                if (client.Birthday != null)
                {
                    clientB2BModel.Birthday = client.Birthday;
                }
                else
                {
                    clientB2BModel.Birthday = DateTime.Now;
                }
                if (address != null)
                {
                    clientB2BModel.ProvinceId = address.ProvinceId;
                    clientB2BModel.DistrictId = address.DistrictId;
                    clientB2BModel.WardId = address.WardId;
                    clientB2BModel.Address = address.Address;
                }
                clientB2BModel.Email = client.Email;

                return clientB2BModel;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetClientByUserAndPassword - ClientRepository: " + ex);
                return null;
            }
        }

        public Client GetDetail(long clientId)
        {
            return clientDAL.GetDetail(clientId);
        }

        public long InsertOrUpdate(ClientViewModel model, out bool isUpdate)
        {
            try
            {
                isUpdate = false;
                long result = 0;
                var client = clientDAL.GetByClientMapId(model.client_map_id);
                var checkExists = client != null;
                if (client == null)
                {
                    client = new Client();
                    client.Status = (int)CommonStatus.ACTIVE;
                }
                client.ClientMapId = model.client_map_id;
                client.ClientName = model.client_name;
                client.Email = model.email;
                client.Gender = model.gender;
                client.JoinDate = model.join_date;
                client.Phone = model.phone;
                client.TaxNo = model.taxno;
                client.SaleMapId = model.sale_map_id;
                client.ClientType = model.client_type;
                client.PermisionType = 0;

                if (!checkExists)
                    result = clientDAL.Insert(client);
                else
                {
                    isUpdate = true;
                    result = clientDAL.Update(client);
                }
                if (result == -1) return -1;
                var addressClient = addressClientDAL.GetByClientMapId(client.ClientMapId.Value);
                var checkExistsAddressClient = addressClient != null;
                if (addressClient == null)
                    addressClient = new AddressClient();
                addressClient.ClientId = (int)client.ClientMapId;
                addressClient.ReceiverName = client.ClientName;
                addressClient.Phone = client.Phone;
                addressClient.Address = model.address;
                addressClient.IsActive = true;
                addressClient.ProvinceId = "-1";
                addressClient.DistrictId = "-1";
                addressClient.WardId = "-1";
                addressClient.CreatedOn = DateTime.Now;
                addressClient.Status = (int)CommonStatus.ACTIVE;
                if (!checkExistsAddressClient)
                    result = addressClientDAL.Insert(addressClient);
                else
                    result = addressClientDAL.Update(addressClient);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertOrUpdate - ClientRepository: " + ex);
                isUpdate = false;
                return -1;
            }
        }
        public async Task<string> ResetPassword(long accountClientId)
        {
            var rs = string.Empty;
            try
            {
                var _model = await accountClientDAL.FindAsync(accountClientId);
                var _newPassword = StringHelpers.CreateRandomPassword();
                _model.Password = EncodeHelpers.MD5Hash(_newPassword);
                LogHelper.InsertLogTelegram("ResetPassword." + JsonConvert.SerializeObject(_model));
                await accountClientDAL.UpdateAsync(_model);
                rs = _newPassword;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ResetPassword - ClientRepository: " + ex);
            }
            return rs;
        }

        public async Task<string> ResetPasswordDefault(long accountClientId)
        {
            var rs = string.Empty;
            try
            {
                var _model = await accountClientDAL.FindAsync(accountClientId);
                var _newPassword = "123456";
                _model.PasswordBackup = EncodeHelpers.MD5Hash(_newPassword);
                LogHelper.InsertLogTelegram("ResetPassword." + JsonConvert.SerializeObject(_model));
                await accountClientDAL.UpdateAsync(_model);
                rs = _newPassword;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ResetPassword - ClientRepository: " + ex);
            }
            return rs;
        }
        public async Task<string> Updata(ClientB2CViewModel clientB2CView, List<int> client_type)
        {
            try
            {
                var account_b2c = await accountClientDAL.GetbyAccountClientID(clientB2CView.account_client_id, client_type);
                var clientB2C = clientDAL.GetByClientId((long)account_b2c.ClientId);
                clientB2C.ClientName = clientB2CView.client_name;
                clientB2C.Birthday = clientB2CView.Birthday;
                clientB2C.Gender = clientB2CView.gender;
                clientB2C.UpdateTime = DateTime.Now;
                var result = clientDAL.Update(clientB2C);

                var addressClient = new AddressClient();
                addressClient.Address = clientB2CView.Address;
                addressClient.WardId = clientB2CView.Wards;
                addressClient.DistrictId = clientB2CView.District;
                addressClient.ProvinceId = clientB2CView.ProvinceId;
                addressClient.Phone = clientB2CView.Phone;
                addressClient.ReceiverName = clientB2CView.client_name;
                //-- clientB2CView.ClientId -> clientB2C.Id 
                addressClient.ClientId = clientB2C.Id;
                result = addressClientDAL.Update(addressClient);
                return addressClient.ClientId.ToString();
            }
            catch
            {
                return null;
            }
        }


        public async Task<int> UpdateAccountPassword(string password_old, string password_new, string confirm_password_new, long account_client_id, int client_type)
        {
            int rs = 0;
            try
            {
                var exist_account = await accountClientDAL.GetbyClientIDAndPassword(account_client_id, password_old, client_type);
                if (exist_account == null || exist_account.Id <= 0) return rs;

                //-- Update Password
                if (password_new.Trim() != "" && password_new.Trim() != exist_account.Password.Trim() && exist_account.Password != password_new
                    && password_old.Trim() == exist_account.Password.Trim() && password_new == confirm_password_new)
                {
                    exist_account.Password = password_new;
                    accountClientDAL.Update(exist_account);
                    rs = 1;
                }


            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("updateClientInfo - ClientRepository: " + ex);
                rs = -1;
            }
            return rs;
        }
        public async Task<int> UpdateAccountInfomationB2B(ClientInfoViewModel model)
        {
            int rs = 0;
            try
            {
                List<int> client_types = new List<int>() { (int)ClientType.TIER_1_AGENT, (int)ClientType.TIER_2_AGENT, (int)ClientType.TIER_3_AGENT, (int)ClientType.AGENT };

                var exist_account = await accountClientDAL.GetbyAccountClientID(model.account_client_id, client_types);
                if (exist_account == null || exist_account.Id <= 0) return rs;
                //-- client_id -> account_client_id
                // var client =  clientDAL.GetByClientId(model.client_id);
                var client = clientDAL.GetByClientId((long)exist_account.ClientId);
                if (client == null || client.Id <= 0) return rs;
                client.Email = model.email;
                client.Phone = model.phone;
                client.Gender = model.gender;
                client.ClientName = model.name;
                client.Birthday = Convert.ToDateTime(model.birthday_year + "/" + model.birthday_month + "/" + model.birthday_day);
                var address_client = addressClientDAL.GetByClientMapId((long)exist_account.ClientId);
                if (address_client == null)
                {
                    address_client = new AddressClient();
                    address_client.ClientId = (long)exist_account.ClientId;
                    address_client.CreatedOn = DateTime.Now;
                    address_client.UpdateTime = DateTime.Now;
                    address_client.ProvinceId = model.province_id.ToString();
                    address_client.DistrictId = model.district_id.ToString();
                    address_client.WardId = model.ward_id.ToString();
                    address_client.Address = model.address;
                    clientDAL.Insert(client);

                }
                else
                {
                    address_client.UpdateTime = DateTime.Now;
                    address_client.ProvinceId = model.province_id.ToString();
                    address_client.DistrictId = model.district_id.ToString();
                    address_client.WardId = model.ward_id.ToString();
                    address_client.Address = model.address;
                    await addressClientDAL.UpdateAsync(address_client);
                }
                await clientDAL.UpdateAsync(client);
                rs = 1;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateAccountInfomation - ClientRepository: " + ex);
                rs = -1;
            }
            return rs;
        }
        public async Task<int> UpdateAccountInfomationB2C(ClientInfoViewModel model)
        {
            int rs = 0;
            try
            {
                //-- Change to get By Account Client Id
                // var exist_account = await accountClientDAL.GetB2CbyClientID( model.client_id);
                var exist_account = await accountClientDAL.GetB2CByID(model.client_id);
                if (exist_account == null || exist_account.Id <= 0) return rs;
                // var client = clientDAL.GetByClientId(model.client_id);
                var client = clientDAL.GetByClientId((long)exist_account.ClientId);
                if (client == null || client.Id <= 0) return rs;
                client.Email = model.email;
                client.Phone = model.phone;
                client.Gender = model.gender;
                client.ClientName = model.name;
                client.Birthday = Convert.ToDateTime(model.birthday_year + "/" + model.birthday_month + "/" + model.birthday_day);
                var address_client = addressClientDAL.GetByClientMapId(client.Id);
                if (address_client == null)
                {
                    var accountclient = await accountClientDAL.GetByID(model.client_id);

                    address_client = new AddressClient();
                    address_client.ClientId = (long)accountclient.ClientId;
                    address_client.ReceiverName = model.name;
                    address_client.Phone = model.phone;

                    address_client.CreatedOn = DateTime.Now;
                    address_client.UpdateTime = DateTime.Now;
                    address_client.ProvinceId = model.province_id.ToString();
                    address_client.DistrictId = model.district_id.ToString();
                    address_client.WardId = model.ward_id.ToString();
                    address_client.Address = model.address;
                    addressClientDAL.Insert(address_client);

                }
                else
                {
                    address_client.UpdateTime = DateTime.Now;
                    address_client.ProvinceId = model.province_id.ToString();
                    address_client.DistrictId = model.district_id.ToString();
                    address_client.WardId = model.ward_id.ToString();
                    address_client.Address = model.address;
                    await addressClientDAL.UpdateAsync(address_client);
                }
                await clientDAL.UpdateAsync(client);
                rs = 1;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateAccountInfomation - ClientRepository: " + ex);
                rs = -1;
            }
            return rs;
        }
        public async Task<long> registerAffiliate(long client_id, string ReferralId)
        {
            try
            {
                var _client_id = await clientDAL.registerAffiliate(client_id, ReferralId);

                return _client_id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("[ClientRepository ] -- > registerAffiliate/ Lỗi update client: " + ex + " client_id =" + client_id);
                return -1;
            }
        }
        public async Task<long> InsertClientB2b(AccountB2BViewModel model)
        {
            try
            {
                var _client_id =await accountClientDAL.AddAccountB2B(model);

                return _client_id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram(" InsertClient - ClientRepository " + ex );
                return -1;
            }
        }
        public long CountClientByParentId(int ParentId)
        {
            try
            {
                var Count =  clientDAL.CountClientByParentId(ParentId);

                return Count;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram(" CountClientByParentId - ClientRepository " + ex);
                return 0;
            }
        }
        public Client GetClientDetailByEmail(string Email)
        {
            try
            {
                return clientDAL.GetClientDetailByEmail(Email);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram(" GetClientDetailByEmail - ClientRepository " + ex);
                return null;
            }
        }
    }
}
