using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Hotel
{

    public class HotelRoomDetailModel
    {
        public string hotel_id { get; set; }

        public List<RoomDetail> rooms { get; set; } // danh sach cac phong thuoc khach san
        public string input_api_vin { get; set; }

    }

    public class RoomDetailViewModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public string bed_room_type_name { get; set; }
        public int max_adult { get; set; }
        public int max_child { get; set; }
        public double min_price { get; set; }
        public int number_bed_room { get; set; }
        public int number_extra_bed { get; set; }

        public int remainming_room { get; set; }
        public List<thumbnails> img_thumb { get; set; } // hinh anh room
    }
    public class RoomDetail
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int max_adult { get; set; }
        public int max_child { get; set; }
        public string code { get; set; }
        public double min_price { get; set; }
        public int remainming_room { get; set; }
        public int number_bed_room { get; set; }
        public int number_extra_bed { get; set; }
        public string bed_room_type_name { get; set; }
        public List<thumbnails> img_thumb { get; set; } // hinh anh room
        public List<RoomDetailRate> rates { get; set; }
        public List<string> package_includes { get; set; }

    }

    public class RoomDetailRate
    {
      
        public int room_id { get; set; }
        public string room_code { get; set; }
        public string id { get; set; }
        public string code { get; set; }
        public string allotment_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public double amount { get; set; }
        public long price_detail_id { get; set; }
        public double profit { get; set; }
        public double total_profit { get; set; }
        public double total_price { get; set; }
        public int program_id { get; set; }
        public DateTime? apply_date { get; set; }
        public List<RoomDetailPackage> package_includes { get; set; }
        public List<string> cancel_policy { get; set; }
        public string guarantee_policy { get; set; }
        public string program_name { get; set; }

    }
    public class RoomDetailCancelPolicy
    {
        public string id { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public List<RoomCancelPolicyDetail> detail { get; set; }
    }
    public class RoomCancelPolicyDetail
    {
        public string id { get; set; }
        public double amount { get; set; }
        public string type { get; set; }
        public int daysBeforeArrival { get; set; }
        public string weekdays { get; set; }
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public int order { get; set; }
    }
    public class RoomDetailPackage
    {
        public string id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public string packageType { get; set; }
       
    }
}