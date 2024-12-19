using ENTITIES.ViewModels.Booking;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace DAL.MongoDB.Flight
{
    public class BookingDAL
    {
        public static string _connection;
        private IMongoCollection<BookingFlyMongoDbModel> bookingCollection;

        //Gọi phương thức GetDatabase() trên MongoClient và chỉ định tên cơ sở dữ liệu để kết nối(FirstDatabase trong trường hợp này)
        //GetDatabase trả về đối tượng .IMongoDatabase.Tiếp theo bookingCollection collection được truy suất sử dụng phương thức GetCollection của IMongoDatabase.
        public BookingDAL(string connection, string catalog)
        {
            try
            {
                _connection = connection;

                var booking = new MongoClient(_connection);
                IMongoDatabase db = booking.GetDatabase(catalog);
                bookingCollection = db.GetCollection<BookingFlyMongoDbModel>("BookingFly");
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("BookingDAL - BookingDAL: " + ex);
                throw;
            }
        }

        public async Task<string> saveBooking(BookingFlyMongoDbModel data)
        {
            try
            {


                await bookingCollection.InsertOneAsync(data);

                return data._id.ToString();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("saveBooking - BookingDAL: " + ex);
                return null;
                throw;
            }
            return null;
        }
        public async Task<List<BookingFlyMongoDbModel>> getBookingByBookingId(int booking_id)
        {
            try
            {
                var filter = Builders<BookingFlyMongoDbModel>.Filter.Where(x => x.booking_id == booking_id);
                var result_document = bookingCollection.Find(filter).ToList();
                IOrderedEnumerable<BookingFlyMongoDbModel> data = result_document.OrderByDescending(x => x.create_date);
                return data.ToList();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getBookingByBookingId - BookingDAL: " + ex);
                return null;
                throw;
            }

        }
        public async Task<List<BookingFlyMongoDbModel>> getBookingByAccountClientId(long account_client_id)
        {
            try
            {
                var filter = Builders<BookingFlyMongoDbModel>.Filter.Where(x => x.account_client_id == account_client_id);
                var result_document = bookingCollection.Find(filter).ToList();
                IOrderedEnumerable<BookingFlyMongoDbModel> data = result_document.OrderByDescending(x => x.create_date);
                return data.ToList();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getBookingByClientId - BookingDAL: " + ex);
                return null;
                throw;
            }

        }
        public async Task<List<BookingFlyMongoDbModel>> getBookingBySessionId(string session_id, int account_client_id)
        {
            try
            {
                var filter = Builders<BookingFlyMongoDbModel>.Filter.Where(x => x.session_id == session_id && x.account_client_id == account_client_id);
                var result_document = bookingCollection.Find(filter).ToList();
                IOrderedEnumerable<BookingFlyMongoDbModel> data = result_document.OrderByDescending(x => x.create_date);
                return data.ToList();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getBookingBySessionId - BookingDAL: " + ex);
                return null;
               
            }

        }
        public async Task<bool> UpdateCheckedBookingSession(string session_id, long account_client_id)
        {
            try
            {
                var filter = Builders<BookingFlyMongoDbModel>.Filter.Where(x => x.session_id == session_id && x.account_client_id == account_client_id);
                var result_document = bookingCollection.Find(filter).ToList();
                if (result_document.Count > 0)
                {
                    foreach(var booking in result_document)
                    {
                        booking.is_checkout = 1;
                        var filter_replace = Builders<BookingFlyMongoDbModel>.Filter.Eq(s => s._id, booking._id);
                        var result = await bookingCollection.ReplaceOneAsync(filter_replace, booking);
                    }
                    
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateCheckedBookingSession - BookingDAL: " + ex);
                return false;
            }

        }
        public async Task<string> DeleteBookingBySessionId(BookingFlyMongoDbModel data)
        {
            try
            {
                await bookingCollection.DeleteOneAsync(s => s.session_id.Equals(data.session_id) && s.is_checkout.Equals(0));
                return data._id.ToString();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getBookingBySessionId - BookingDAL: " + ex);
                return null;

            }

        }
    }

}
