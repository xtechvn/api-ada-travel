using DAL.Clients;
using Entities.ConfigModels;
using ENTITIES.Models;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories.Clients;
using System;
using System.Collections.Generic;
using System.Text;

namespace REPOSITORIES.Repositories.Clients
{
    public class ContactClientRepository : IContactClientRepository
    {
        private readonly ContactClientDAL contactClientDAL;
        private readonly IOptions<DataBaseConfig> dataBaseConfig;

        public ContactClientRepository(IOptions<DataBaseConfig> _dataBaseConfig)
        {
            contactClientDAL = new ContactClientDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
            dataBaseConfig = _dataBaseConfig;
        }
        public ContactClient GetByClientId(long clientId)
        {
            return contactClientDAL.GetByClientId(clientId);
        }
        public ContactClient GetByContactClientId(long Id)
        {
            return contactClientDAL.GetByContactClientId(Id);

        }
    }
}
