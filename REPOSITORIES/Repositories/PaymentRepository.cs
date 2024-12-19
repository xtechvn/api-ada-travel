using DAL;
using DAL.MongoDB.Flight;
using DAL.Orders;
using Entities.ConfigModels;
using ENTITIES.APPModels.ReadBankMessages;
using ENTITIES.Models;
using ENTITIES.ViewModels.APP.ReadBankMessages;
using ENTITIES.ViewModels.Order;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace REPOSITORIES.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentDAL paymentDAL;

        public PaymentRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            paymentDAL = new PaymentDAL(dataBaseConfig.Value.SqlServer.ConnectionString);

        }
      
        
     
    }
}
