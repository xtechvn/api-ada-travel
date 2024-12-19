using ENTITIES.APPModels.SystemLogs;
using ENTITIES.ViewModels.Hotel;
using ENTITIES.ViewModels.Tour;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace DAL.MongoDB
{
    public class BookingTourDAL
    {
        public static string _connection;
        private IMongoCollection<BookingTourMongoViewModel> bookingCollection;

        public BookingTourDAL(string connection, string catalog)
        {
            try
            {
                _connection = connection;

                var booking = new MongoClient(_connection);
                IMongoDatabase db = booking.GetDatabase(catalog);
                bookingCollection = db.GetCollection<BookingTourMongoViewModel>("BookingTour");
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("BookingHotelDAL - BookingTour: " + ex);
                throw;
            }
        }
        public async Task<string> InsertBooking(BookingTourMongoViewModel item)
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
        public BookingTourMongoViewModel GetBookingById(string id)
        {
            try
            {

                var filter = Builders<BookingTourMongoViewModel>.Filter;
                var filterDefinition = filter.Empty;
                filterDefinition &= Builders<BookingTourMongoViewModel>.Filter.Eq(x => x._id, id);

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
        public async Task<string> UpdateBooking(BookingTourMongoViewModel item, string booking_id)
        {
            try
            {
                if (booking_id != null && booking_id.Trim() != "")
                {
                    var filter = Builders<BookingTourMongoViewModel>.Filter;
                    var filterDefinition = filter.Empty;
                    filterDefinition &= Builders<BookingTourMongoViewModel>.Filter.Eq(x => x._id, booking_id);
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
