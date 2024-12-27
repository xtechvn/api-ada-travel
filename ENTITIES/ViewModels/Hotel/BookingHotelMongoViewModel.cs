using ENTITIES.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Hotel
{
    public class BookingHotelMongoViewModel
    {
        [BsonElement("_id")]
        public string _id { get; set; } 
        public long account_client_id { get; set; } 
        public DateTime create_booking { get; set; }
        public double total_amount { get; set; }
        public MongoBookingData booking_data { get; set; }
        public HotelMongoBookingOrder booking_order { get; set; }
        public BookingHotelB2BViewModel booking_b2b_data { get; set; }
        public List<HotelBookingRoomExtraPackages> extrapackages { get; set; }

        public string? voucher_code { get; set; }

        public void GenID()
        {
            _id = ObjectId.GenerateNewId(DateTime.Now).ToString();
        }
    }
    
    public class HotelMongoBookingOrder
    {
        public string arrivalDate { get; set; }
        public string departureDate { get; set; }
        public string hotelID { get; set; }
        public int numberOfRoom { get; set; }
        public int numberOfAdult { get; set; }
        public int numberOfChild { get; set; }
        public int numberOfInfant { get; set; }
        public string clientType { get; set; }
        
    }
    
    public class MongoBookingData
    {
        public string propertyId { get; set; }
        public string propertyName { get; set; }
        public string arrivalDate { get; set; }
        public string departureDate { get; set; }
        public List<HotelMongoReservation> reservations { get; set; }
        public string distributionChannel { get; set; }
        public string sourceCode { get; set; }
    }

    public class HotelMongoOtherOccupancy
    {
        public string otherOccupancyRefID { get; set; }
        public string otherOccupancyRefCode { get; set; }
        public int quantity { get; set; }
    }

    public class HotelMongoPackage
    {
        public string usedDate { get; set; }
        public string packageRefId { get; set; }
        public string ratePlanId { get; set; }
        public int quantity { get; set; }
        public string packageCode { get; set; }
    }

    public class HotelMongoPrimarySearchValues
    {
        public string email { get; set; }
        public string phoneNumber { get; set; }
    }

    public class HotelMongoProfile
    {
        public string profileRefID { get; set; }
        public string firstName { get; set; }
        public string profileType { get; set; }
        public bool isPrimary { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string birthday { get; set; }
        public string phoneNumber { get; set; }
        public HotelMongoPrimarySearchValues primarySearchValues { get; set; }
        public string travelAgentCode { get; set; }
    }

    public class HotelMongoReservation
    {
        public HotelMongoRoomOccupancy roomOccupancy { get; set; }
        public HotelMongoTotalAmount totalAmount { get; set; }
        public bool isSpecialRequestSpecified { get; set; }
        public int numberOfRoom { get; set; }
        public List<HotelMongoSpecialRequest> specialRequests { get; set; }
        public bool isProfilesSpecified { get; set; }
        public List<HotelMongoProfile> profiles { get; set; }
        public bool isRoomRatesSpecified { get; set; }
        public List<HotelMongoRoomRate> roomRates { get; set; }
        public bool isPackagesSpecified { get; set; }
        public List<HotelMongoPackage> packages { get; set; }
    }

    public class HotelMongoRoomOccupancy
    {
        public int numberOfAdult { get; set; }
        public List<HotelMongoOtherOccupancy> otherOccupancies { get; set; }
    }

    public class HotelMongoRoomRate
    {
        public string stayDate { get; set; }
        public string roomTypeRefID { get; set; }
        public string allotmentId { get; set; }
        public string ratePlanRefID { get; set; }
        public string roomTypeCode { get; set; }
        public string ratePlanCode { get; set; }
    }

    

    public class HotelMongoSpecialRequest
    {
        public string requestType { get; set; }
        public string requestContent { get; set; }
    }

    public class HotelMongoTotalAmount
    {
        public int amount { get; set; }
        public string currencyCode { get; set; }
    }


}
