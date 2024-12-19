using DAL;
using Entities.ConfigModels;
using ENTITIES.Models;
using ENTITIES.ViewModels;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Utilities;

namespace Repositories.Repositories
{
    
    public class OtherBookingRepository: IOtherBookingRepository
    {
        private readonly OtherBookingDAL otherBookingDAL;
        private readonly AllCodeDAL AllCodeDAL;

        public OtherBookingRepository(IOptions<DataBaseConfig> dataBaseConfig )
        {
            otherBookingDAL = new OtherBookingDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            AllCodeDAL = new AllCodeDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
        public async Task<OtherBooking> GetOtherBookingById(long booking_id)
        {
            return await otherBookingDAL.GetOtherBookingById(booking_id);
        }
        public async Task<List<OtherBooking>> GetOtherBookingByOrderId(long order_id)
        {
            return await otherBookingDAL.GetOtherBookingByOrderId(order_id);
        }
        public async Task<List<OtherBookingViewModel>> GetDetailOtherBookingById(int OtherBookingId)
        {
            try
            {
                DataTable dt = await otherBookingDAL.GetDetailOtherBookingById(OtherBookingId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<OtherBookingViewModel>();
                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CancelTourByID - TourRepository: " + ex);
                return null;
            }
        }
    } 
}
