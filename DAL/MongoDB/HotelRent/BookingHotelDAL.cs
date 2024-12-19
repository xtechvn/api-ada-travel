using ENTITIES.APPModels.SystemLogs;
using ENTITIES.ViewModels.Hotel;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace DAL.MongoDB.Hotel
{
    public class BookingHotelDAL
    {
        public static string _connection;
        private IMongoCollection<BookingHotelMongoViewModel> bookingCollection;

        public BookingHotelDAL(string connection, string catalog)
        {
            try
            {
                _connection = connection;

                var booking = new MongoClient(_connection);
                IMongoDatabase db = booking.GetDatabase(catalog);
                bookingCollection = db.GetCollection<BookingHotelMongoViewModel>("BookingHotel");
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("BookingHotelDAL - BookingHotelDAL: " + ex);
                throw;
            }
        }
        public async Task<string> InsertBooking(BookingHotelMongoViewModel item)
        {
            try
            {
                item.GenID();
                await bookingCollection.InsertOneAsync(item);
                return item._id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertBooking - BookingHotelDAL - Cannot Excute: " + ex.ToString());
                return null;
            }
        }
        public BookingHotelMongoViewModel GetBookingById(string id)
        {
            try
            {
            
                var filter = Builders<BookingHotelMongoViewModel>.Filter;
                var filterDefinition = filter.Empty;
                filterDefinition &= Builders<BookingHotelMongoViewModel>.Filter.Eq(x => x._id, id);
               
                var model = bookingCollection.Find(filterDefinition).FirstOrDefault();
                if (model != null && model._id != null && model._id.Trim() != "")
                    return model;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetBookingById - BookingHotelDAL - Cannot Excute: " + ex.ToString());
            }
            return null;
        }
        public async Task<string> UpdateBooking(BookingHotelMongoViewModel item,string booking_id)
        {
            try
            {
                if(booking_id!=null && booking_id.Trim() != "")
                {
                    var filter = Builders<BookingHotelMongoViewModel>.Filter;
                    var filterDefinition = filter.Empty;
                    filterDefinition &= Builders<BookingHotelMongoViewModel>.Filter.Eq(x => x._id, booking_id);
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
                LogHelper.InsertLogTelegram("InsertBooking - BookingHotelDAL - Cannot Excute: " + ex.ToString());
                return null;
            }
        }
    }
}
