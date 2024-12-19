using ENTITIES.ViewModels.MongoDb;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace DAL.MongoDB.VinWonder
{
    public class VinWonderMongoBookingDAL
    {
        public static string _connection;
        private IMongoCollection<BookingVinWonderMongoDbModel> bookingCollection;

        public VinWonderMongoBookingDAL(string connection, string catalog)
        {
            try
            {
                _connection = connection;

                var booking = new MongoClient(_connection);
                IMongoDatabase db = booking.GetDatabase(catalog);
                bookingCollection = db.GetCollection<BookingVinWonderMongoDbModel>("BookingVinWonder");
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("VinWonderBookingDAL - VinWonderBookingDAL: " + ex);
                throw;
            }
        }
        public async Task<string> InsertBooking(BookingVinWonderMongoDbModel item)
        {
            try
            {
                item.GenID();
                await bookingCollection.InsertOneAsync(item);
                return item._id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertBooking - VinWonderBookingDAL - Cannot Excute: " + ex.ToString());
                return null;
            }
        }
        public BookingVinWonderMongoDbModel GetBookingById(string id)
        {
            try
            {

                var filter = Builders<BookingVinWonderMongoDbModel>.Filter;
                var filterDefinition = filter.Empty;
                filterDefinition &= Builders<BookingVinWonderMongoDbModel>.Filter.Eq(x => x._id, id);

                var model = bookingCollection.Find(filterDefinition).FirstOrDefault();
                if (model != null && model._id != null && model._id.Trim() != "")
                    return model;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetBookingById - VinWonderBookingDAL - Cannot Excute: " + ex.ToString());
            }
            return null;
        }
    }
}
