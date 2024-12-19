using REPOSITORIES.IRepositories;
using System.Collections.Generic;
using DAL;
using Microsoft.Extensions.Options;
using Entities.ConfigModels;
using System.Threading.Tasks;
using ENTITIES.Models;

namespace REPOSITORIES.Repositories
{
   public class BankOnePayRepository : IBankOnePayRepository
    {
        private readonly BankOnePayDAL _bankOnePayDAL;

        public BankOnePayRepository(IOptions<DataBaseConfig> dataBaseConfig, IOptions<MailConfig> mailConfig)
        {
            _bankOnePayDAL = new BankOnePayDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
        public async Task<List<BankOnePay>> GetAllBankOnePay()
        {
            return await _bankOnePayDAL.GetAllBankOnePay();
        }
    }
}
