using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using ENTITIES.ViewModels.BookingFly;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Utilities;

namespace DAL.Clients
{
    public class ContactClientDAL : GenericService<Client>
    {
        private static DbWorker _DbWorker;
        public ContactClientDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }

        public ContactClient GetByClientId(long clientId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.ContactClient.AsNoTracking().FirstOrDefault(s => s.ClientId == clientId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetail - ContactClientDAL: " + ex);
                return null;
            }
        }
        public ContactClient GetByContactClientId(long Id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.ContactClient.AsNoTracking().FirstOrDefault(s => s.Id == Id);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByContactClientId - ContactClientDAL: " + ex);
                return null;
            }
        }
        public long CreateContactClients(ContactClientViewModel client)
        {
            try
            {
                var ContactClient =new ContactClient();
                ContactClient.ClientId = client.ClientId;
                ContactClient.Name = client.Name;
                ContactClient.Mobile = client.Mobile;
                ContactClient.Email = client.Email;
                ContactClient.CreateDate = client.CreateDate;
                ContactClient.OrderId = client.OrderId;
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var result = _DbContext.ContactClient.Add(ContactClient);
                    _DbContext.SaveChanges();
                    return ContactClient.Id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Insert - ClientDAL: " + ex);
                return -1;
            }
        }

    }
}
