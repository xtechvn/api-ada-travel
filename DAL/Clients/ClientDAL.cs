using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using ENTITIES.ViewModels.BookingFly;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{
    public class ClientDAL : GenericService<Client>
    {
        private static DbWorker _DbWorker;
        public ClientDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }

        public Client GetDetail(long clientId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.Client.AsNoTracking().FirstOrDefault(s => s.Id == clientId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailAsync - ClientDAL: " + ex);
                return null;
            }
        }


        public long Insert(Client client)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var result = _DbContext.Client.Add(client);
                    _DbContext.SaveChanges();
                    return client.Id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Insert - ClientDAL: " + ex);
                return -1;
            }
        }

        public long Update(Client client)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var entity = _DbContext.Client.AsNoTracking().FirstOrDefault(s => s.Id == client.Id);
                    if (entity != null)
                    {
                        entity.ClientName = client.ClientName;
                        entity.Email = client.Email;
                        entity.Gender = client.Gender;
                        entity.Status = client.Status;
                        entity.Phone = client.Phone;
                        entity.SaleMapId = client.SaleMapId;
                        entity.ClientType = client.ClientType;
                        entity.UpdateTime = DateTime.Now;
                        entity.TaxNo = client.TaxNo;
                        _DbContext.Update(entity);
                        _DbContext.SaveChanges();
                        return client.Id;
                    }
                    return -1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Update - ClientDAL: " + ex);
                return -1;
            }
        }

        public Client GetByClientMapId(int? clientMapId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.Client.AsNoTracking().Where(s => s.ClientMapId == clientMapId).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByClientMapId - ClientDAL: " + ex);
                return null;
            }
        }
        public Client GetByClientId(long id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.Client.AsNoTracking().Where(s => s.Id == id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByClientId - ClientDAL: " + ex);
                return null;
            }
        }
        public async Task<long> registerAffiliate(long clientId, string ReferralId)
        {
            try
            {
                var clientModel = await FindAsync(clientId);
                clientModel.IsRegisterAffiliate = true;
                clientModel.ReferralId = ReferralId;
                await UpdateAsync(clientModel);
                return clientId;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("registerAffiliate - ClientDAL: " + ex);
                return 0;
            }
        }
        public async Task<DataTable> GetClientByID(long Client)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@ClientID", Client);

                return _DbWorker.GetDataTable(StoreProceduresName.GetClientByID, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getClientid - ClientDal: " + ex);
            }
            return null;
        }
        public long CountClientByParentId(int ParentId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var entity = _DbContext.Client.AsNoTracking().Where(s => s.ParentId == ParentId).ToList();
                    return entity.Count;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CountClientByParentId - ClientDAL: " + ex);
                return 0;
            }
        }
        public long UpdateClientB2B(Client client)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var entity = _DbContext.Client.AsNoTracking().FirstOrDefault(s => s.Id == client.Id);
                    if (entity != null)
                    {
                        entity.ClientName = client.ClientName;
                        entity.Email = client.Email;
                        entity.Status = client.Status;
                        entity.Phone = client.Phone;
                        entity.UpdateTime = DateTime.Now;

                        _DbContext.Update(entity);
                        _DbContext.SaveChanges();
                        return client.Id;
                    }
                    return -1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Update - ClientDAL: " + ex);
                return -1;
            }
        }

        public bool CheckEmailClient(string email, long id)
        {
            try
            {

                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var entity = _DbContext.Client.Where(s => s.Email.Equals(email) && s.Id != id).ToList();
                    if (entity.Count == 0 || entity == null)
                    {
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CountClientByParentId - ClientDAL: " + ex);
                return true;
            }
        }
        public Client GetClientDetailByEmail(string Email)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.Client.AsNoTracking().FirstOrDefault(s => s.Email == Email);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailAsync - ClientDAL: " + ex);
                return null;
            }
        }
        public bool CheckPhoneClient(string Phone)
        {
            try
            {

                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var entity = _DbContext.Client.Where(s => s.Phone == Phone).ToList();
                    if (entity.Count > 0)
                    {
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CountClientByParentId - ClientDAL: " + ex);
                return true;
            }
        }
        public Client GetByPhone(string Phone)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.Client.AsNoTracking().Where(s => s.Phone == Phone).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByClientId - ClientDAL: " + ex);
                return null;
            }
        }
        public static int CreateContactClients(ContactClientViewModel obj_contact_client)
        {
            try
            {

                SqlParameter[] objParam_cclient = new SqlParameter[6];
                objParam_cclient[0] = new SqlParameter("@Name", obj_contact_client.Name);
                objParam_cclient[1] = new SqlParameter("@Mobile", obj_contact_client.Mobile);
                objParam_cclient[2] = new SqlParameter("@Email", obj_contact_client.Email);
                objParam_cclient[3] = new SqlParameter("@CreateDate", obj_contact_client.CreateDate);
                objParam_cclient[4] = new SqlParameter("@AccountClientId", obj_contact_client.ClientId);
                objParam_cclient[5] = new SqlParameter("@OrderId", obj_contact_client.OrderId);
                var id = _DbWorker.ExecuteNonQuery(ProcedureConstants.CreateContactClients, objParam_cclient);
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateContactClients - ClientDAL: " + ex);
                return -1;
            }
        }
    }
}
