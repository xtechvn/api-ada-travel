using ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Hotel
{
    public class BookingHotelB2BViewModel
    {
        public string booking_id { get; set; }
        public long account_client_id { get; set; }
        public BookingHotelB2BViewModelDetail detail { get; set; }
        public BookingHotelB2BViewModelSearch search { get; set; }
        public List<BookingHotelB2BViewModelRooms> rooms { get; set; }
        public BookingHotelB2BViewModelContact contact { get; set; }
        public BookingHotelB2BViewModelPickUp pickup { get; set; }
        public List<HotelBookingRoomExtraPackages> extrapackages { get; set; }
        public string voucher_code { get; set; }
        public HotelOrderDataVoucher voucher { get; set; }

    }
    public class HotelOrderDataVoucher
    {
        public int id { get; set; }
        public string code { get; set; }
        public double discount { get; set; }
    }

    public class BookingHotelB2BViewModelDetail
    {
        public DateTime check_in_time { get; set; }
        public DateTime check_out_time { get; set; }
        public string address { get; set; }
        public string image_thumb { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
        public string note { get; set; }
    }

    public class BookingHotelB2BViewModelPickUp
    {
        public BookingHotelB2BViewModelPickUpForm arrive { get; set; }
        public BookingHotelB2BViewModelPickUpForm departure { get; set; }
    }
    public class BookingHotelB2BViewModelPickUpForm
    {
        public int required { get; set; }
        public string id_request { get; set; }
        public string vehicle { get; set; }
        public string stop_point_code { get; set; }
        public string fly_code { get; set; }
        public int amount_of_people { get; set; }
        public string? date { get; set; }
        public string time { get; set; }
        public string note { get; set; }
    }
    public class BookingHotelB2BViewModelPackage
    {
        public string used_date { get; set; }
        public string package_id { get; set; }
        public string rate_plan_id { get; set; }
        public string package_code { get; set; }
        public double total_amount { get; set; }
        public string quanity { get; set; }
    }

    public class BookingHotelB2BViewModelContact
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string? birthday { get; set; }
        public string? email { get; set; }
        public string? phoneNumber { get; set; }
        public string? country { get; set; }
        public string? province { get; set; }
        public string? district { get; set; }
        public string? ward { get; set; }
        public string? note { get; set; }

    }
    public class BookingHotelB2BViewModelGuest
    {
        public int profile_type { get; set; }
        public int room { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string birthday { get; set; }
        public short? type { get; set; }
        public string note { get; set; }

    }


    public class BookingHotelB2BViewModelRooms
    {
        public int numberOfAdult { get; set; }
        public int numberOfChild { get; set; }
        public int numberOfInfant { get; set; }
        public short? numberOfRooms { get; set; }

        public string room_type_id { get; set; }
        public string room_type_code { get; set; }
        public string room_type_name { get; set; }
        public double price { get; set; }
        public double profit { get; set; }
        public double total_amount { get; set; }
        public string  special_request { get; set; }
        public List<string> package_includes { get; set; }
        public List<BookingHotelB2BViewModelRates> rates { get; set; }
        public List<BookingHotelB2BViewModelGuest> guests { get; set; }

    }

    public class BookingHotelB2BViewModelRates
    {
        public string arrivalDate { get; set; }
        public string departureDate { get; set; }
        public string rate_plan_code { get; set; }
        public string rate_plan_id { get; set; }
        public string allotment_id { get; set; }
        public double price { get; set; }
        public double profit { get; set; }
        public double total_amount { get; set; }
        public List<BookingHotelB2BViewModelPackage> packages { get; set; }
        public List<string> package_includes { get; set; }

    }

    public class BookingHotelB2BViewModelSearch
    {
        public string arrivalDate { get; set; }
        public string departureDate { get; set; }
        public string hotelID { get; set; }
        public int numberOfRoom { get; set; }
        public int numberOfAdult { get; set; }
        public int numberOfChild { get; set; }
        public int numberOfInfant { get; set; }
    }
}
