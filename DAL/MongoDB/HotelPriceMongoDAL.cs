using Entities.ViewModels;
using ENTITIES.APPModels.SystemLogs;
using ENTITIES.ViewModels.Hotel;
using ENTITIES.ViewModels.MongoDb;
using ENTITIES.ViewModels.Tour;
using iTextSharp.text;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;
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
        public HotelPriceMongoDbModel GetByFilter(string hotel_id, List<int> client_types, DateTime arrivaldate, DateTime departuredate, string location=null,string stars="",double? min_price=-1, double? max_price=-1)
        {
            try
            {
                arrivaldate = arrivaldate.Date;
                departuredate = departuredate.Date;
                var filter = Builders<HotelPriceMongoDbModel>.Filter;
                var filterDefinition = filter.Empty;
                if(hotel_id!=null && hotel_id.Trim() != "")
                {
                    filterDefinition &= Builders<HotelPriceMongoDbModel>.Filter.Eq(x => x.hotel_id, hotel_id);
                }
                filterDefinition &= Builders<HotelPriceMongoDbModel>.Filter.Eq(x => x.arrival_date, arrivaldate);
                filterDefinition &= Builders<HotelPriceMongoDbModel>.Filter.In(x => x.client_type, client_types);
                filterDefinition &= Builders<HotelPriceMongoDbModel>.Filter.Eq(x => x.departure_date, departuredate);
                if (location != null && location.Trim() != "")
                {
                    // Location filter: Match either city or state
                    var locationFilter = Builders<HotelPriceMongoDbModel>.Filter.Or(
                        Builders<HotelPriceMongoDbModel>.Filter.Regex(x => x.hotel_name, new BsonRegularExpression($"^{Regex.Escape(location)}[., ]?", "i")),
                        Builders<HotelPriceMongoDbModel>.Filter.Regex(x => x.city, new BsonRegularExpression($"^{Regex.Escape(location)}[., ]?", "i")),
                        Builders<HotelPriceMongoDbModel>.Filter.Regex(x => x.state, new BsonRegularExpression($"^{Regex.Escape(location)}[., ]?", "i"))
                    );
                    filterDefinition &= locationFilter;
                }
               if(min_price>0 && max_price>0 && max_price> min_price)
               {
                    // Price range filter: min_price between min_price and max_price
                    var priceFilter = Builders<HotelPriceMongoDbModel>.Filter.And(
                        Builders<HotelPriceMongoDbModel>.Filter.Gte(x => x.min_price, min_price),
                        Builders<HotelPriceMongoDbModel>.Filter.Lte(x => x.min_price, max_price)
                    );
                    filterDefinition &= priceFilter;
               }

               if(stars!=null && stars.Trim() != "")
               {
                    // Star filter: Matches if the floor or rounded value of the star exists in the provided list
                    var starList = stars.Split(',').Select(int.Parse).ToList();
                    filterDefinition &= Builders<HotelPriceMongoDbModel>.Filter.In(x => x.star, starList);

                }
                var model = bookingCollection.Find(filterDefinition).FirstOrDefault();
                if (model != null && model._id != null && model._id.Trim() != "")
                    return model;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByFilter - BookingTour - Cannot Excute: " + ex.ToString());
            }
            return null;
        }
        public async Task<GenericViewModel<HotelPriceMongoDbModel>> GetListByFilter(string hotel_id, List<int> client_types, DateTime arrivaldate, DateTime departuredate, string location = null, string stars = "", double? min_price = -1, double? max_price = -1,int? page_index=1,int? page_size=30)
        {
            try
            {
                arrivaldate = arrivaldate.Date;
                departuredate = departuredate.Date;
                var filter = Builders<HotelPriceMongoDbModel>.Filter;
                var filterDefinition = filter.Empty;
                if (hotel_id != null && hotel_id.Trim() != "")
                {
                    filterDefinition &= Builders<HotelPriceMongoDbModel>.Filter.Eq(x => x.hotel_id, hotel_id);
                }
                filterDefinition &= Builders<HotelPriceMongoDbModel>.Filter.Eq(x => x.arrival_date, arrivaldate);
                filterDefinition &= Builders<HotelPriceMongoDbModel>.Filter.In(x => x.client_type, client_types);
                filterDefinition &= Builders<HotelPriceMongoDbModel>.Filter.Eq(x => x.departure_date, departuredate);
                if (location != null && location.Trim() != "")
                {
                    var location_nonunicode = CommonHelper.RemoveUnicode(location);

                    // Location filter: Match either city or state
                    var locationFilter = Builders<HotelPriceMongoDbModel>.Filter.Or(
                        Builders<HotelPriceMongoDbModel>.Filter.Regex(x => x.hotel_name, new BsonRegularExpression($"^{Regex.Escape(location)}[., ]?", "i")),
                        Builders<HotelPriceMongoDbModel>.Filter.Regex(x => x.city, new BsonRegularExpression($"^{Regex.Escape(location)}[., ]?", "i")),
                        Builders<HotelPriceMongoDbModel>.Filter.Regex(x => x.state, new BsonRegularExpression($"^{Regex.Escape(location)}[., ]?", "i")),
                        Builders<HotelPriceMongoDbModel>.Filter.Regex(x => x.hotel_name, new BsonRegularExpression($"^{Regex.Escape(location_nonunicode)}[., ]?", "i")),
                        Builders<HotelPriceMongoDbModel>.Filter.Regex(x => x.city, new BsonRegularExpression($"^{Regex.Escape(location_nonunicode)}[., ]?", "i")),
                        Builders<HotelPriceMongoDbModel>.Filter.Regex(x => x.state, new BsonRegularExpression($"^{Regex.Escape(location_nonunicode)}[., ]?", "i"))
                    );
                    filterDefinition &= locationFilter;
                }
                if (min_price > 0 && max_price > 0 && max_price > min_price)
                {
                    // Price range filter: min_price between min_price and max_price
                    var priceFilter = Builders<HotelPriceMongoDbModel>.Filter.And(
                        Builders<HotelPriceMongoDbModel>.Filter.Gte(x => x.min_price, min_price),
                        Builders<HotelPriceMongoDbModel>.Filter.Lte(x => x.min_price, max_price)
                    );
                    filterDefinition &= priceFilter;
                }

                if (stars != null && stars.Trim() != "")
                {
                    // Star filter: Matches if the floor or rounded value of the star exists in the provided list
                    var starList = stars.Split(',').Select(int.Parse).ToList();
                    filterDefinition &= Builders<HotelPriceMongoDbModel>.Filter.In(x => x.star, starList);

                }
                //-- Has Position B2B >0 or Vinpearl:
                var position_filter = Builders<HotelPriceMongoDbModel>.Filter.Or(
                     Builders<HotelPriceMongoDbModel>.Filter.Gt(x => x.position_b2b, 0),
                     Builders<HotelPriceMongoDbModel>.Filter.Eq(x => x.is_vinhotel, true)
                   );
                filterDefinition &= position_filter;
                //filterDefinition &= Builders<HotelPriceMongoDbModel>.Filter.Gte(x => x.position_b2b, 0);   // Greater than 0
                //-- Is Hotel Commit:
               // filterDefinition &= Builders<HotelPriceMongoDbModel>.Filter.Eq(x => x.is_commit, true); 


                // Pagination parameters
                int skip = (page_index == null|| page_size==null) ? 1: ((int)page_index - 1) * (int)page_size; // Calculate how many documents to skip
                int take = page_size == null?30: (int)page_size; // Number of documents per page
                var sortDefinition = new SortDefinitionBuilder<HotelPriceMongoDbModel>()
                        .Combine(
                            Builders<HotelPriceMongoDbModel>.Sort.Ascending(x => x.position_b2b)          // Then sort ascending
                        );
                // Query execution with pagination
                var hotels = await bookingCollection
                    .Find(filterDefinition)
                    .Sort(sortDefinition) 
                    .Skip(skip)
                    .Limit(take)
                    .ToListAsync();

                // Get total count for pagination
                var totalCount = await bookingCollection.CountDocumentsAsync(filterDefinition);

                // Prepare response with pagination metadata
                return new GenericViewModel<HotelPriceMongoDbModel>
                {
                    TotalRecord = totalCount,
                    TotalPage = Convert.ToInt32(totalCount / (int)page_size),
                    PageSize = page_size == null ? 30 : (int)page_size,
                    ListData = hotels
                };
    
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByFilter - BookingTour - Cannot Excute: " + ex.ToString());
            }
            return null;
        }
    }
}
