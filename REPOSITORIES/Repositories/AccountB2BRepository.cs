using DAL;
using DAL.PaymentAccounts;
using Entities.ConfigModels;
using ENTITIES.Models;
using ENTITIES.ViewModels.B2B;
using ENTITIES.ViewModels.Client;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace REPOSITORIES.Repositories
{
    public class AccountB2BRepository : IAccountB2BRepository
    {
        private readonly AccountClientDAL accountClientDAL;
        private readonly AllCodeDAL allCodeDAL;
        private readonly ClientDAL clientDAL;
        private readonly AddressClientDAL addressClientDAL;
        private readonly PaymentAccountDAL paymentAccountDAL;

        public AccountB2BRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            allCodeDAL = new AllCodeDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            accountClientDAL = new AccountClientDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            clientDAL = new ClientDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            addressClientDAL = new AddressClientDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            paymentAccountDAL = new PaymentAccountDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
        public async Task<AccountClient> GetAccountClientById(long accountClientId)
        {
            try
            {
                return await accountClientDAL.GetB2BById(accountClientId);
                
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAccountClientById - ClientRepository: " + ex.ToString());
            }
            return null;
        }
      
        public async Task<ClientB2BDetailUpdateViewModel> GetClientB2BDetailViewModel (long clientId)
        {
            try
            {
                if (clientId <= 0) return null;
               var accountClient=await accountClientDAL.GetByClientId(clientId);
                var all_code = await allCodeDAL.GetAllCodeByType("CLIENT_TYPE");
                var client =  clientDAL.GetByClientId(clientId);
                var address_client = addressClientDAL.GetByClientMapId(clientId);
                var payment_account = paymentAccountDAL.GetByClientMapId(clientId);
                var type_all_code = all_code.Count > 0 ? all_code.FirstOrDefault(x => x.CodeValue == client.ClientType) : null;
                var data = new ClientB2BDetailUpdateViewModel()
                {
                    name = client.ClientName,
                    phone = client.Phone,
                    client_type = (int)client.ClientType,
                    indentifer_no = client.TaxNo == null ? "" : client.TaxNo,
                    country = "0",
                    district_id = address_client != null ? address_client.DistrictId : "0",
                    provinced_id = address_client != null ? address_client.ProvinceId : "0",
                    ward_id = address_client != null ? address_client.WardId : "0",
                    address = address_client != null ? address_client.Address : "",
                    account_name = payment_account != null ? payment_account.AccountName : "",
                    account_number = payment_account != null ? payment_account.AccountNumb : "",
                    bank_name = payment_account != null ? payment_account.BankName : "",
                    email = client.Email ?? "",
                    client_type_name = type_all_code != null ? type_all_code.Description : "",
                    GroupPermission = accountClient.GroupPermission!=null?(int)accountClient.GroupPermission:-1,
                    ParentId = client.ParentId != null ? (int)client.ParentId:-1,
                };
                return data;

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAddressByClientId - ClientRepository: " + ex.ToString());
            }
            return null;
        }
        public async Task<long> UpdateClientDetail(ClientB2BDetailUpdateViewModel model, long clientId)
        {
            try
            {
                var client = clientDAL.GetByClientId(clientId);
                
                if (client != null)
                {
                    client.ClientName = model.name; 
                    client.TaxNo = model.indentifer_no; 
                    await clientDAL.UpdateAsync(client);
                }
                var address_client = addressClientDAL.GetByClientMapId(clientId);
                if (address_client != null)
                {
                    address_client.ProvinceId = model.provinced_id;
                    address_client.DistrictId = model.district_id;
                    address_client.WardId = model.ward_id;
                    address_client.Address = model.address;
                    await addressClientDAL.UpdateAsync(address_client);

                }
                else
                {
                    address_client = new AddressClient()
                    {
                        ProvinceId = model.provinced_id,
                        DistrictId = model.district_id,
                        WardId = model.ward_id,
                        Address = model.address,
                        ReceiverName = client.ClientName,
                        Phone = client.Phone,
                        ClientId = client.Id,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        Status = 0,
                        UpdateTime = DateTime.Now
                    };
                    await addressClientDAL.CreateAsync(address_client);

                }
                var payment_account = paymentAccountDAL.GetByClientMapId(clientId);
                if (payment_account != null)
                {
                    payment_account.AccountName = model.account_name;
                    payment_account.AccountNumb = model.account_number;
                    payment_account.BankName = model.bank_name;
                    await paymentAccountDAL.UpdateAsync(payment_account);

                }
                else
                {
                    payment_account = new PaymentAccount()
                    {
                        AccountName = model.account_name,
                        AccountNumb = model.account_number,
                        BankName = model.bank_name,
                        Branch="",
                        ClientId= client.Id,
                        
                    };
                    await paymentAccountDAL.CreateAsync(payment_account);

                }
                return client.Id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateClientDetail - ClientRepository: " + ex.ToString());
            }
            return -1;
        }

        public async Task<List<ListAccountClientModel>> GetListAccountClient(long ClientId, long GroupPermission, long Status, long PageIndex, long PageSize, string TextSearch)
        {
            try
            {
                DataTable dt = await accountClientDAL.GetListAccountClient(ClientId, GroupPermission, Status, PageIndex, PageSize, TextSearch);
                if(dt != null && dt.Rows.Count > 0)
                {
                    var model = dt.ToList<ListAccountClientModel>();
                    return model;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram(" CountClientByParentId - ClientRepository " + ex);
                return null;
            }         
        }
        public async Task<long> UpdataAccountClientB2B(AccountClientB2BViewModel model)
        {
            try
            {
               var account_client = await accountClientDAL.GetB2BById(model.id);
                account_client.Password = model.Password;
                account_client.GroupPermission = model.Type;
                account_client.Status = (byte?)model.Status;
                account_client.PasswordBackup = model.PasswordBackup;
                var updateAc = accountClientDAL.UpdataAccountClient(account_client);

                var client_model = clientDAL.GetDetail((long)account_client.ClientId);
                client_model.ClientName = model.ClientName;
                client_model.Email = model.Email;
                client_model.Phone = model.Phone;
                var updateClient = clientDAL.UpdateClientB2B(client_model);
                return 0;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdataAccountClientB2B - AccountClientDAL: " + ex);
                return -1;
            }
        }
        public async Task<bool> checkEmailExtisB2B(string email)
        {
            return await accountClientDAL.checkEmailExtisB2B(email);
        }
        public async Task<bool> checkUserNameExtisB2B(string UserName)
        {
            return await accountClientDAL.checkUserNameExtisB2B(UserName);
        } 
        public async Task<bool> checkEmailClient(string email,long id)
        {
            return  clientDAL.CheckEmailClient(email,id);
        }
    }
}
