using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ENTITIES.ViewModels.Hotel
{
    public class HotelEntities
    {
        public HotelModel hotel { get; set; }
        public List<thumbnails> img_thumb { get; set; } // hinh anh khach san
        public List<amenitie> amenities { get; set; } // tien ich khach san
        public List<Room> room { get; set; } // danh sach cac phong thuoc khach san
    }

    public class amenitie
    {
        public string name { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public string icon { get; set; }

    }
    public class thumbnails
    {
        public string id { get; set; }
        public string url { get; set; }
    }

    public class Room
    {
        public string id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public int number_of_room { get; set; }
        public int number_of_total_room { get; set; }
        public int number_of_bed { get; set; }
        public string short_description { get; set; }
        public int default_occupancy { get; set; }
        public int max_occupancy { get; set; }
        public int max_adult { get; set; }
        public int max_child { get; set; }
        public string hotel_id { get; set; }
        public string type_of_room { get; set; }
        public int number_of_bed_room { get; set; }
        public List<thumbnails> img_thumb { get; set; } // hinh anh room
        public List<RoomRate> rates { get; set; }
    }

    public class RoomRate
    {
        public string hotel_id { get; set; }
        public string room_id { get; set; }
        public string rate_plan_id { get; set; }
        public string allotment_id { get; set; }
        public string allotment_name { get; set; }
        public string rate_plan_code { get; set; }
        public double amount { get; set; }
        public long price_detail_id { get; set; }
        public double profit { get; set; }
        public double total_profit { get; set; }
        public double total_price { get; set; }
        public List<RoomPackages> packages_include { get; set; }

    }

    public class RoomPackages
    {
        public string id { get; set; }
        public string propertyId { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string calculationRule { get; set; }
        public string postingRhythm { get; set; }
        public string packageType { get; set; }
        public bool isBreakfast { get; set; }
        public bool isLunch { get; set; }
        public bool isDinner { get; set; }
        public double amount { get; set; }

    }
}
