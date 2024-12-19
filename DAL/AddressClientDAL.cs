using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace DAL
{
    public class AddressClientDAL : GenericService<AddressClient>
    {
        private static DbWorker _DbWorker;
        public AddressClientDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }

        public AddressClient GetByClientMapId(int clientMapId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.AddressClient.FirstOrDefault(n => n.ClientId == clientMapId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Insert - AddressClientDAL: " + ex);
                return null;
            }
        }
        public AddressClient GetByClientMapId(long clientMapId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.AddressClient.FirstOrDefault(n => n.ClientId == clientMapId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Insert - AddressClientDAL: " + ex);
                return null;
            }
        }
        public long Insert(AddressClient entity)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var result = _DbContext.AddressClient.Add(entity);
                    _DbContext.SaveChanges();
                    return entity.Id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Insert - AddressClientDAL: " + ex);
                return -1;
            }
        }

        public long Update(AddressClient entity)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var addressClient = _DbContext.AddressClient.FirstOrDefault(n => n.ClientId == Convert.ToInt32(entity.ClientId));
                    if (addressClient != null)
                    {
                        addressClient.Address = entity.Address;
                        addressClient.ReceiverName = entity.ReceiverName;
                        addressClient.Phone = entity.Phone;
                        addressClient.UpdateTime = DateTime.Now;
                        addressClient.Address = entity.Address;
                        addressClient.WardId = entity.WardId;
                        addressClient.DistrictId = entity.DistrictId;
                        addressClient.ProvinceId = entity.ProvinceId;
                        _DbContext.AddressClient.Update(addressClient);
                        _DbContext.SaveChanges();
                        return addressClient.Id;
                    }
                    return -1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Insert - AddressClientDAL: " + ex);
                return -1;
            }
        }
    }
}
