using DAL.Fly;
using Entities.ConfigModels;
using ENTITIES.Models;
using ENTITIES.ViewModels.Order;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories.Fly;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Utilities;

namespace REPOSITORIES.Repositories.Fly
{
    public class FlyBookingDetailRepository : IFlyBookingDetailRepository
    {
        private readonly FlyBookingDetailDAL flyBookingDetailDAL;
        private readonly IOptions<DataBaseConfig> dataBaseConfig;

        public FlyBookingDetailRepository(IOptions<DataBaseConfig> _dataBaseConfig)
        {
            flyBookingDetailDAL = new FlyBookingDetailDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
            dataBaseConfig = _dataBaseConfig;
        }

        public FlyBookingDetail GetByOrderId(long orderId)
        {
            return flyBookingDetailDAL.GetDetail(orderId);
        }

        public List<FlyBookingDetail> GetListByOrderId(long orderId)
        {
            return flyBookingDetailDAL.GetListByOrderId(orderId);
        }
        public async Task<List<FlyBookingDetail>> GetFlyBookingById(long fly_booking_id)
        {
            return await flyBookingDetailDAL.GetFlyBookingById(fly_booking_id);
        }
        public async Task<FlyBookingdetail> GetDetailFlyBookingDetailById(int FlyBookingId)
        {
            try
            {
                DataTable dt = await flyBookingDetailDAL.GetDetailFlyBookingDetailById(FlyBookingId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var ListData = dt.ToList<FlyBookingdetail>();
                    return ListData[0];
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailFlyBookingDetailById- FlyBookingDetailRepository: " + ex);
            }
            return null;
        }
    }
}
