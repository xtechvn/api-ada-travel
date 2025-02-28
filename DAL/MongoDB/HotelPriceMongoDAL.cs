using ENTITIES.APPModels.SystemLogs;
using ENTITIES.ViewModels.Hotel;
using ENTITIES.ViewModels.MongoDb;
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
    public class HotelPriceMongoDAL
    {
        public static string _connection;
        private IMongoCollection<HotelPriceMongoDbModel> bookingCollection;

        public HotelPriceMongoDAL(string connection, string catalog)
        {
            try
            {
                _connection = connection;

                var booking = new MongoClient(_connection);
                IMongoDatabase db = booking.GetDatabase(catalog);
                bookingCollection = db.GetCollection<HotelPriceMongoDbModel>("HotelPrice");
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("HotelPriceMongoDAL - HotelPrice: " + ex);
                throw;
            }
        }
        public async Task<string> Insert(HotelPriceMongoDbModel item)
        {
            try
            {
                item.GenID();
                item.arrival_date = item.arrival_date.Date;
                item.departure_date = item.departure_date.Date;
                await bookingCollection.InsertOneAsync(item);
                return item._id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertBooking - BookingTour - Cannot Excute: " + ex.ToString());
                return null;
            }
        }
        public HotelPriceMongoDbModel GetById(string id)
        {
            try
            {

                var filter = Builders<HotelPriceMongoDbModel>.Filter;
                var filterDefinition = filter.Empty;
                filterDefinition &= Builders<HotelPriceMongoDbModel>.Filter.Eq(x => x._id, id);

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
        public async Task<string> Update(HotelPriceMongoDbModel item, string booking_id)
        {
            try
            {
                if (booking_id != null && booking_id.Trim() != "")
                {
                    var filter = Builders<HotelPriceMongoDbModel>.Filter;
                    var filterDefinition = filter.Empty;
                    filterDefinition &= Builders<HotelPriceMongoDbModel>.Filter.Eq(x => x._id, booking_id);
                    item._id = booking_id;
                    item.arrival_date = item.arrival_date.Date;
                    item.departure_date = item.departure_date.Date;
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
        public HotelPriceMongoDbModel GetByHotel(string hotel_id, List<int> client_types, DateTime arrivaldate,DateTime departuredate)
        {
            try
            {
                arrivaldate = arrivaldate.Date;
                departuredate = departuredate.Date;
                var filter = Builders<HotelPriceMongoDbModel>.Filter;
                var filterDefinition = filter.Empty;
                filterDefinition &= Builders<HotelPriceMongoDbModel>.Filter.Eq(x => x.hotel_id, hotel_id);
                filterDefinition &= Builders<HotelPriceMongoDbModel>.Filter.Eq(x => x.arrival_date, arrivaldate);
                filterDefinition &= Builders<HotelPriceMongoDbModel>.Filter.In(x => x.client_type, client_types);
                filterDefinition &= Builders<HotelPriceMongoDbModel>.Filter.Eq(x => x.departure_date, departuredate);

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
    }
}
