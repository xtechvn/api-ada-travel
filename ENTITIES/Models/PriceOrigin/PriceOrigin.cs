using System;
using System.Collections.Generic;

namespace ENTITIES.Model.PriceOrigin
{
    public class HotelInfo
    {
        public DateTime arrivalDate { get; set; }
        public DateTime departureDate { get; set; }
        public Rate rates { get; set; }
    }
    public class Rate
    {
        public List<Propety> property { get; set; }
        public List<Rates> rates { get; set; }
    }
    public class Propety
    {
        public string id { get; set; }
        public string organizationId { get; set; }
        public string logo { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string description { get; set; }
        public int numberOfRooms { get; set; }
        public string checkInTime { get; set; }
        public string checkOutTime { get; set; }
        public int status { get; set; }
        public List<RoomType> roomTypes { get; set; }
    }
    public class RoomType
    {
        public string id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public int numberOfRoom { get; set; }
        public int numberOfTotalRoom { get; set; }
        public int maxAdult { get; set; }
        public int defaultOccupancy { get; set; }
        public int maxOccupancy { get; set; }
        public int maxChild { get; set; }
        public int hotelId { get; set; }
        public int organizationId { get; set; }
    }
    public class Rates
    {
        public int roomTypeID { get; set; }
        public int ratePlanID { get; set; }
        public double totalAmount { get; set; }
        public double currencyCode { get; set; }
        public double averageAmount { get; set; }
        public double totalTaxAmount { get; set; }
        public int quantity { get; set; }
        public RateAvailablity rateAvailablity { get; set; }
    }
    public class RateAvailablity
    {
        public int propertyId { get; set; }
        public int roomTypeId { get; set; }
        public int ratePlanId { get; set; }
        public int quantity { get; set; }
        public int amount { get; set; }
        public string ratePlanCode { get; set; }
        public string roomTypeCode { get; set; }
        public string arrivalDate { get; set; }
        public string departureDate { get; set; }
        public RatePlan ratePlan { get; set; }
        public Allotments allotments { get; set; }
    }
    public class RatePlan
    {
        public string id { get; set; }
        public string ratePlanId { get; set; }
        public string ratePlanCategoryId { get; set; }
        public DateTime beginSellDate { get; set; }
        public DateTime endSellDate { get; set; }
    }
    public class Allotments
    {
        public string id { get; set; }
        public string allotmentId { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string dcId { get; set; }
        public int quantity { get; set; }
        public double roomRateAmount { get; set; }
        public double amountIncludePackage { get; set; }
        public double amountExcludePackage { get; set; }
    }
}
