using DAL;
using Entities.ConfigModels;
using ENTITIES.Models;
using ENTITIES.ViewModels.FlyTicket;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Utilities;

namespace REPOSITORIES.Repositories
{
    public class ProductFlyTicketServiceRepository : IProductFlyTicketServiceRepository
    {

        private readonly ProductFlyTicketServiceDAL _productFlyTicketServiceDAL;

        public ProductFlyTicketServiceRepository(IOptions<DataBaseConfig> dataBaseConfig, IOptions<MailConfig> mailConfig)
        {
           _productFlyTicketServiceDAL = new ProductFlyTicketServiceDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
        
        public async Task<List<FlyPricePolicyViewModel>> GetFlyPricePolicyActive()
        {
            try
            {
                DataTable dt = await _productFlyTicketServiceDAL.GetFlyPricePolicyActive();
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<FlyPricePolicyViewModel>();
                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetFlyPricePolicyActive - ProductFlyTicketServiceRepository: " + ex);
                return null;
            }
        }
    }
}
