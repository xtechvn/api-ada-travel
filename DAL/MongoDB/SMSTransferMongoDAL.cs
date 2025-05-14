using ENTITIES.ViewModels.MongoDb;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace DAL.MongoDB
{
    public class SMSTransferMongoDAL
    {
        public static string _connection;
        private IMongoCollection<SMSN8NMongoModel> bookingCollection;
        private readonly IConfiguration _configuration;

        public SMSTransferMongoDAL(string connection, string catalog)
        {
            try
            {
                _connection = connection;

                var booking = new MongoClient(_connection);
                IMongoDatabase db = booking.GetDatabase(catalog);
                bookingCollection = db.GetCollection<SMSN8NMongoModel>("SMSN8NTransfer");
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SMSTransferMongoDAL - BookingTour: " + ex);
                throw;
            }
        }
        public async Task<string> Insert(SMSN8NMongoModel item)
        {
            try
            {
                item.GenID();
                await bookingCollection.InsertOneAsync(item);
                return item._id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertBooking - BookingTour - Cannot Excute: " + ex.ToString());
                return null;
            }
        }
        public SMSN8NMongoModel GetById(string id)
        {
            try
            {

                var filter = Builders<SMSN8NMongoModel>.Filter;
                var filterDefinition = filter.Empty;
                filterDefinition &= Builders<SMSN8NMongoModel>.Filter.Eq(x => x._id, id);

                var model = bookingCollection.Find(filterDefinition).FirstOrDefault();
                if (model != null && model._id != null && model._id.Trim() != "")
                    return model;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetBookingById - BookingTour - Cannot Excute: " + ex.ToString());
            }
            return null;
        }
        public async Task<string> Update(SMSN8NMongoModel item, string booking_id)
        {
            try
            {
                if (booking_id != null && booking_id.Trim() != "")
                {
                    var filter = Builders<SMSN8NMongoModel>.Filter;
                    var filterDefinition = filter.Empty;
                    filterDefinition &= Builders<SMSN8NMongoModel>.Filter.Eq(x => x._id, booking_id);
                    item._id = booking_id;
                    await bookingCollection.ReplaceOneAsync(filterDefinition, item);
                    return item._id;
                }
                item.GenID();
                await bookingCollection.InsertOneAsync(item);
                return item._id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertBooking - BookingTour - Cannot Excute: " + ex.ToString());
                return null;
            }
        }
    }
}
