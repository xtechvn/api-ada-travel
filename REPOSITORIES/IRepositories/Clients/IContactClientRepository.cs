using ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace REPOSITORIES.IRepositories.Clients
{
    public interface IContactClientRepository
    {
        ContactClient GetByClientId(long clientId);
        public ContactClient GetByContactClientId(long Id);
    }
}
