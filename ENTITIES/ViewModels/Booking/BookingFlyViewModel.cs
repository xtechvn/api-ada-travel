using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Booking
{
    public class BookingFlyViewModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; } // khóa chính của bảng bookingFly
        //--- client_id -> account_client_id
        [BsonElement]
        public int account_client_id { get; set; } // mã tài khoản khách hàng
        [BsonElement]
        public string session_id { get; set; }
        [BsonElement]
        public string voucher_name { get; set; }
        public int is_checkout { get; set; } 

        [BsonElement]
        public int booking_id { get; set; } // id booking của client
        [BsonElement]
        public object booking_data { get; set; } // là 1 list object chứa thông tin booking
        [BsonElement]
        public object booking_session { get; set; } // là 1 list object chứa thông tin booking
        [BsonElement] 
        public object booking_order { get; set; } //Thông tin đặt chỗ sau khi tiền về
        [BsonElement]
        public DateTime create_date { get; set; } // ngày lưu booking
    }
}
