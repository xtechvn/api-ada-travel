using DAL.MongoDB;
using DAL.MongoDB.Flight;
using DAL.MongoDB.Hotel;
using Entities.ConfigModels;
using ENTITIES.ViewModels.Booking;
using ENTITIES.ViewModels.Hotel;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories.Fly;
using REPOSITORIES.IRepositories.Hotel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace REPOSITORIES.Repositories.Hotel
{
    public class HotelBookingMongoRepository : IHotelBookingMongoRepository
    {
        private readonly BookingHotelDAL BookingMongoDAL;
        private readonly IOptions<DataBaseConfig> dataBaseConfig;

        public HotelBookingMongoRepository(IOptions<DataBaseConfig> _dataBaseConfig)
        {
            BookingMongoDAL = new BookingHotelDAL(_dataBaseConfig.Value.MongoServer.connection_string, _dataBaseConfig.Value.MongoServer.catalog_core);
            dataBaseConfig = _dataBaseConfig;
        }

        public async Task<List<BookingHotelMongoViewModel>> getBookingByID(string[] booking_id)
        {
            try
            {
                List<BookingHotelMongoViewModel> data = new List<BookingHotelMongoViewModel>();

                foreach (var item in booking_id)
                {
                    var a = BookingMongoDAL.GetBookingById(item);
                    data.Add(a);
                }
                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> saveBooking(BookingHotelMongoViewModel data,string booking_id)
        {
            try
            {
                if(booking_id!=null && booking_id.Trim() != "")
                {
                    var exists = BookingMongoDAL.GetBookingById(booking_id);
                    if(exists!=null && exists._id!=null&& exists._id.Trim()!="")
                    {
                      return await BookingMongoDAL.UpdateBooking(data,booking_id);
                    }
                }
                return await BookingMongoDAL.InsertBooking(data);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("saveBooking - FlyBookingMongoRepository: " + ex);
            }
            return null;
        }
    }
}
