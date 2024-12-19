using DAL.MongoDB.Flight;
using Entities.ConfigModels;
using ENTITIES.ViewModels.Booking;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories.Fly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace REPOSITORIES.Repositories.Fly
{
    public class FlyBookingMongoRepository : IFlyBookingMongoRepository
    {
        private readonly BookingDAL BookingMongoDAL;
        private readonly IOptions<DataBaseConfig> dataBaseConfig;

        public FlyBookingMongoRepository(IOptions<DataBaseConfig> _dataBaseConfig)
        {
            BookingMongoDAL = new BookingDAL(_dataBaseConfig.Value.MongoServer.connection_string, _dataBaseConfig.Value.MongoServer.catalog_core);
            dataBaseConfig = _dataBaseConfig;
        }

        public string saveBooking(BookingFlyMongoDbModel data)
        {
            try
            {
                data.GenID();
                var result = BookingMongoDAL.saveBooking(data);
                if (result != null)
                    return "-1";
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("saveBooking - FlyBookingMongoRepository: " + ex);
                return null;
            }
            return null;
        }

        //-- int client_id ->int account_client_id
        public async Task<List<BookingFlyMongoDbModel>> getBookingByBookingIdAsync(List<int> booking_id, int account_client_id)
        {
            try
            {
                foreach (var item in booking_id)
                {
                    if (item == -1 && account_client_id > 0)
                    {
                        var datalist = await BookingMongoDAL.getBookingByAccountClientId(account_client_id);
                        List<BookingFlyMongoDbModel> data2 = new List<BookingFlyMongoDbModel>();
                        for (int i = 0; i < datalist.Count; i++)
                        {
                            var b = datalist.ToList()[i].booking_id;
                            int dem = 0;
                            var a = datalist.ToList()[i];
                            if (data2.Count() == 0)
                            {
                                data2.Add(a);
                            }
                            else
                            {
                                for (int j = 0; j < data2.Count(); j++)
                                {
                                    if (b == data2[j].booking_id)
                                    {
                                        dem++;
                                        break;
                                    }
                                }
                                if (dem == 0)
                                {
                                    data2.Add(a);
                                }
                            }

                        }

                        return data2.ToList();
                    }
                    if (item > 0 && account_client_id > 0)
                    {
                        return await BookingMongoDAL.getBookingByBookingId(item);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<List<BookingFlyMongoDbModel>> getBookingBySessionId(string[] list_session_id, int account_client_id)
        {
            List<BookingFlyMongoDbModel> data = new List<BookingFlyMongoDbModel>();

            try
            {

                foreach (var item in list_session_id)
                {
                    var a = await BookingMongoDAL.getBookingBySessionId(item, account_client_id);
                    data.AddRange(a);
                }
                if (data.Count == 0)
                {
                    account_client_id = 0;
                    foreach (var item in list_session_id)
                    {
                        var a = await BookingMongoDAL.getBookingBySessionId(item, account_client_id);
                        data.AddRange(a);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return data;

        }

        public async Task<string> DeleteBookingBySessionId(BookingFlyMongoDbModel data)
        {
            try
            {
               
                return await BookingMongoDAL.DeleteBookingBySessionId(data);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getBookingBySessionId - BookingDAL: " + ex);
                return null;

            }

        }
    }
}
