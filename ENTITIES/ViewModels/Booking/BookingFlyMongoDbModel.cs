using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace ENTITIES.ViewModels.Booking
{
    public class BookingFlyMongoDbModel
    {
        [BsonElement("_id")]
        public ObjectId _id { get; set; }
        public int account_client_id { get; set; }
    
        public int is_checkout { get; set; }
        public int booking_id { get; set; }
        public string voucher_name { get; set; }
        public string session_id { get; set; }
        public string? utmmedium { get; set; }
        public string? utm_source { get; set; }
        public BookingFlyData booking_data { get; set; }
        public BookingFlyOrder booking_order { get; set; }
        public BookingFlySessionString booking_session { get; set; }
        public DateTime create_date { get; set; }
        public void GenID()
        {
            _id = MongoDB.Bson.ObjectId.GenerateNewId();
        }
    }
    public class BookingFlySessionString
    {
        public string search { get; set; }
        public string info { get; set; }

    }
    public class BookingFlySession
    {
        public BookingFlySessionGo go { get; set; }
        public BookingFlySessionBack back { get; set; }
    }

    public class BookingFlySessionBack
    {
        public double Amount { get; set; }
        public int Adt { get; set; }
        public int Chd { get; set; }
        public int Inf { get; set; }
        public double FareAdt { get; set; }
        public double FareChd { get; set; }
        public double FareInf { get; set; }
        public double TaxAdt { get; set; }
        public double TaxChd { get; set; }
        public double TaxInf { get; set; }
        public double FeeAdt { get; set; }
        public double FeeChd { get; set; }
        public double FeeInf { get; set; }
        public double ServiceFeeAdt { get; set; }
        public double ServiceFeeChd { get; set; }
        public double ServiceFeeInf { get; set; }
        public double TotalNetPrice { get; set; }
        public double TotalServiceFee { get; set; }
        public double TotalDiscount { get; set; }
        public double TotalCommission { get; set; }
        public double TotalPrice { get; set; }
        public double Profit { get; set; }
        public BookingFlyDataFareDataAdavigoPrice AdavigoPrice { get; set; }
    }

    public class BookingFlySessionGo
    {
        public double Amount { get; set; }
        public int Adt { get; set; }
        public int Chd { get; set; }
        public int Inf { get; set; }
        public double FareAdt { get; set; }
        public double FareChd { get; set; }
        public double FareInf { get; set; }
        public double TaxAdt { get; set; }
        public double TaxChd { get; set; }
        public double TaxInf { get; set; }
        public double FeeAdt { get; set; }
        public double FeeChd { get; set; }
        public double FeeInf { get; set; }
        public double ServiceFeeAdt { get; set; }
        public double ServiceFeeChd { get; set; }
        public double ServiceFeeInf { get; set; }
        public double TotalNetPrice { get; set; }
        public double TotalServiceFee { get; set; }
        public double TotalDiscount { get; set; }
        public double TotalCommission { get; set; }
        public double TotalPrice { get; set; }
        public double Profit { get; set; }

        public BookingFlyDataFareDataAdavigoPrice AdavigoPrice { get; set; }

    }

    public class BookingFlyData
    {
        public int BookingId { get; set; }
        public string OrderCode { get; set; }
        public List<BookingFlyDataBooking> ListBooking { get; set; }
    }
    public class BookingFlyDataBooking
    {
        public string Status { get; set; }
        public bool AutoIssue { get; set; }
        public string Airline { get; set; }
        public string BookingCode { get; set; }
        public string GdsCode { get; set; }
        public string Flight { get; set; }
        public string Route { get; set; }
        public DateTimeOffset ExpiryDate { get; set; }
        public int ExpiryTime { get; set; }
        public DateTimeOffset TimePurchase { get; set; }
        public double ResponseTime { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
        public long PriceId { get; set; }
        public double Profit { get; set; }
        public double Difference { get; set; }
        public List<BookingFlyOrderListPassenger> ListPassenger { get; set; }
        public List<BookingFlyDataListFareData> ListFareData { get; set; }
        public BookingFlyDataFareData FareData { get; set; }
        public string Session { get; set; }

    }
    public class BookingFlyDataFareData
    {
        public int FareDataId { get; set; }
        public List<BookingFlyDataFareDataListFlight> ListFlight { get; set; }
        public int Leg { get; set; }
        public int Adt { get; set; }
        public int Chd { get; set; }
        public int Inf { get; set; }
        public double FareAdt { get; set; }
        public double FareChd { get; set; }
        public double FareInf { get; set; }
        public double TaxAdt { get; set; }
        public double TaxChd { get; set; }
        public double TaxInf { get; set; }
        public double FeeAdt { get; set; }
        public double FeeChd { get; set; }
        public double FeeInf { get; set; }
        public double ServiceFeeAdt { get; set; }
        public double ServiceFeeChd { get; set; }
        public double ServiceFeeInf { get; set; }
        public double TotalNetPrice { get; set; }
        public double TotalServiceFee { get; set; }
        public double TotalDiscount { get; set; }
        public double TotalCommission { get; set; }
        public double TotalPrice { get; set; }
        public BookingFlyDataFareDataAdavigoPrice AdavigoPrice { get; set; }
    }
    public class BookingFlyDataFareDataAdavigoPrice
    {
        public double price { get; set; }
        public double amount { get; set; }
        public long price_id { get; set; }
        public double profit { get; set; }

    }
    public class BookingFlyDataFareDataListFlight
    {
        public string Airline { get; set; }
        public string Operating { get; set; }
        public string StartPoint { get; set; }
        public int? Leg { get; set; }
        public string EndPoint { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string FlightNumber { get; set; }
        public int Duration { get; set; }
        public int StopNum { get; set; }
        public string FlightValue { get; set; }
        public List<BookingFlyDataFareDataListFlightListSegment> ListSegment { get; set; }
        public bool NoRefund { get; set; }
        public string GroupClass { get; set; }
        public string FareClass { get; set; }
        public int SeatRemain { get; set; }

    }
    public class BookingFlyDataFareDataListFlightListSegment
    {
        public int Id { get; set; }
        public string Airline { get; set; }
        public string MarketingAirline { get; set; }
        public string OperatingAirline { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public string Duration { get; set; }
        public string FlightNumber { get; set; }
        public string Plane { get; set; }
        public string? StartTerminal { get; set; }
        public string? EndTerminal { get; set; }
        public bool HasStop { get; set; }
        public string? StopPoint { get; set; }
        public double? StopTime { get; set; }
        public bool DayChange { get; set; }
        public bool StopOvernight { get; set; }
        public bool ChangeStation { get; set; }
        public bool ChangeAirport { get; set; }
        public bool LastItem { get; set; }
        public string Cabin { get; set; }
        public string Class { get; set; }
        public string HandBaggage { get; set; }
        public string AllowanceBaggage { get; set; }

    }
    public class BookingFlyDataListPassenger
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Gender { get; set; }
    }
    public class BookingFlyDataListFareData
    {
        public int FareDataId { get; set; }
        public double TotalPrice { get; set; }
    }
    public class BookingFlyOrder
    {
        public string BookType { get; set; }
        public bool UseAgentContact { get; set; }
        public BookingFlyOrderContact Contact { get; set; }
        public List<BookingFlyOrderListPassenger> ListPassenger { get; set; }
        public List<BookingFlyOrderListFareDatum> ListFareData { get; set; }
        public string AccountCode { get; set; }
        public string Remark { get; set; }
        public string HeaderUser { get; set; }
        public string HeaderPass { get; set; }
        public string AgentAccount { get; set; }
        public string AgentPassword { get; set; }
        public string ProductKey { get; set; }
        public string Currency { get; set; }
        public string Language { get; set; }
        public string IpRequest { get; set; }
    }
    public class BookingFlyOrderContact
    {
        public bool Gender { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Area { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }

    public class BookingFlyOrderListFareDatum
    {
        public string Session { get; set; }
        public int FareDataId { get; set; }
        public List<BookingFlyOrderListFlight> ListFlight { get; set; }
    }

    public class BookingFlyOrderListFlight
    {
        public string FlightValue { get; set; }
        public int Leg { get; set; }
    }

    public class BookingFlyOrderListPassenger
    {
        public int Index { get; set; }
        public int ParentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Type { get; set; }
        public bool Gender { get; set; }
        public string Birthday { get; set; }
        public string Nationality { get; set; }
        public string PassportNumber { get; set; }
        public string PassportExpirationDate { get; set; }
        public string Membership { get; set; }
        public bool Wheelchair { get; set; }
        public bool Vegetarian { get; set; }
        public List<BookingFlyOrderListPassengerListBaggage> ListBaggage { get; set; }
    }
    public class BookingFlyOrderListPassengerListBaggage
    {
        public string Airline { get; set; }
        public int Leg { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; }
        public string Value { get; set; }
        public int FlightId { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public string StatusCode { get; set; }
        public bool Confirmed { get; set; }
    }

}
