using DAL.Contracts;
using Entities.ConfigModels;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories.Contract;
using System;
using System.Threading.Tasks;
using Utilities;

namespace REPOSITORIES.Repositories
{
    public class ContractRepository : IContractRepository
    {

        private readonly ContractDAL contractDAL;


        public ContractRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            contractDAL = new ContractDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }


        public async Task<long> CountContract()
        {
            try
            {
                return await contractDAL.CountContractInYear();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CountContract - OrderRepository" + ex.ToString());
                return -1;
            }
        }


    }
}
