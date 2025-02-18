using DAL;
using DAL.Hotel;
using Entities.ConfigModels;
using ENTITIES.Models;
using ENTITIES.ViewModels.HotelBookingRoom;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace Repositories.Repositories
{
    public class HotelBookingRoomsRepository : IHotelBookingRoomRepository
    {
        
        private readonly HotelBookingDAL _hotelBookingDAL;
        private readonly HotelBookingRoomDAL _hotelBookingRoomDAL;
        private readonly HotelBookingRoomRatesDAL _hotelBookingRoomRatesDAL;
        private readonly HotelBookingRoomExtraPackagesDAL _hotelBookingRoomExtraPackagesDAL;

        public HotelBookingRoomsRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {

            _hotelBookingRoomDAL = new HotelBookingRoomDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _hotelBookingRoomRatesDAL = new HotelBookingRoomRatesDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _hotelBookingDAL = new HotelBookingDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _hotelBookingRoomExtraPackagesDAL = new HotelBookingRoomExtraPackagesDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }

        public async Task<List<HotelBookingRooms>> GetByHotelBookingID(long hotel_booking_id)
        {
            return await _hotelBookingRoomDAL.GetByHotelBookingID(hotel_booking_id);
        }
        public async Task<List<HotelBookingRoomViewModel>> GetHotelBookingRoomByHotelBookingID(long HotelBookingId,long status)
        {
            var model = new List<HotelBookingRoomViewModel>();
            var model2 = new List<HotelBookingRoomRates>();
            try
            {
                DataTable dt = await _hotelBookingRoomDAL.GetHotelBookingRoomByHotelBookingID(HotelBookingId, status);
                if (dt != null && dt.Rows.Count > 0)
                {
                    model = dt.ToList<HotelBookingRoomViewModel>();

                }
                foreach(var item in model)
                {
                    DataTable dt2 = await _hotelBookingRoomRatesDAL.GetHotelBookingRateByHotelBookingRoomID(Convert.ToInt32(item.Id));
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        item.HotelBookingRoomRates = dt2.ToList<HotelBookingRoomRatesViewModel>();
                        item.SumAmount = item.HotelBookingRoomRates.Sum(x => x.TotalAmount);
                        item.SumUnitPrice = item.SumUnitPrice +Convert.ToDouble(item.HotelBookingRoomRates.Sum(x => x.UnitPrice));

                    }
                }
                var list = new List<HotelBookingRoomRatesViewModel>();
                foreach (var item in model)
                {
                    
                    list= item.HotelBookingRoomRates;
                    if (item.HotelBookingRoomRates!=null && item.HotelBookingRoomRates.Count > 1)
                    {
                        for(int i=0; i<item.HotelBookingRoomRates.Count;i++)
                        {
                            item.HotelBookingRoomRates[i].EndDate = item.HotelBookingRoomRates[i].StayDate;
                            for (int i2=1;i2 < item.HotelBookingRoomRates.Count;i2++)
                            {
                                if (item.HotelBookingRoomRates[i].RatePlanCode == item.HotelBookingRoomRates[i2].RatePlanCode && item.HotelBookingRoomRates[i].StayDate.Month == item.HotelBookingRoomRates[i2].StayDate.Month)
                                {
                                    if ((item.HotelBookingRoomRates[i].EndDate.Day + 1) == item.HotelBookingRoomRates[i2].StayDate.Day)
                                    {
                                        item.HotelBookingRoomRates[i].EndDate = item.HotelBookingRoomRates[i2].StayDate;
                                        item.HotelBookingRoomRates.Remove(item.HotelBookingRoomRates[i2]);
                                        i2--;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if(item.HotelBookingRoomRates != null && item.HotelBookingRoomRates.Count>0)
                            item.HotelBookingRoomRates[0].EndDate = item.HotelBookingRoomRates[0].StayDate;
                    }
                   
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingById - HotelBookingRepository: " + ex);
            }
            return model;
        }
      
       
        
        public async Task<List<HotelBookingRoomRatesOptionalViewModel>> GetHotelBookingRoomRatesOptionalByBookingId(long HotelBookingId)
        {
            var model = new List<HotelBookingRoomRatesOptionalViewModel>();
            try
            {
                DataTable dt = await _hotelBookingDAL.GetHotelBookingRoomRatesOptionalByBookingId(HotelBookingId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    model = dt.ToList<HotelBookingRoomRatesOptionalViewModel>();

                }
                
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingRoomRatesOptionalByBookingId - HotelBookingRepository: " + ex);
            }
            return model;
        }

    }
}
