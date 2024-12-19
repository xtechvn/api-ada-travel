using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Booking
{
    public class VinWonderBookingB2CModel
    {
        public VinWonderBookingB2CData data { get; set; }
        public List<VinWonderBookingB2Request> requestVin { get; set; }
    }

    public class VinWonderBookingB2Request
    {
        public string Channelcode { get; set; }
        public string Date { get; set; }
        public string BookingCode { get; set; }
        public string InvoiceCode { get; set; }
        public string ReservationCode { get; set; }
        public string PromotionCode { get; set; }
        public int SiteCode { get; set; }
        public List<VinWonderBookingB2RequestServices> Services { get; set; }
        public List<VinWonderBookingB2RequestCustomer> Customer { get; set; }

    }

    public class VinWonderBookingB2RequestCustomer
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string DateOfBirth { get; set; }
        public int? Sex { get; set; }
        public int? IdentityType { get; set; }
        public int? IdentityDetail { get; set; }
        public int? ZonePickupId { get; set; }
        public int? ZoneReleaseId { get; set; }
        public int? NationalityID { get; set; }
        public string? PlaceOfBirth { get; set; }
    }

    public class VinWonderBookingB2RequestServices
    {
        public int ServiceCode { get; set; }
        public int Number { get; set; }
        public int RateCode { get; set; }
    }

    public class VinWonderBookingB2CData
    {
        public double amount { get; set; }

        public List<List<VinWonderBookingB2CDataCart>> cart { get; set; }
        public VinWonderBookingB2CDataContact infoContact { get; set; }
    }

    public class VinWonderBookingB2CDataContact
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string area { get; set; }
        public int isUserBook { get; set; }
        public string userRequest { get; set; }
    }

    public class VinWonderBookingB2CDataCart
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string SiteName { get; set; }
        public int SiteCode { get; set; }
        public double TotalPrice { get; set; }
        public int Number { get; set; }
        public List<VinWonderBookingB2CDataCartItem> Items { get; set; }
        public bool IsShow { get; set; }
        public string RateCode { get; set; }
        public VinWonderBookingB2CDataCartNumObj PeopleNumObj { get; set; }
        public string PeopleString { get; set; }
        public double Amount { get; set; }
        public double Profit { get; set; }
    }

    public class VinWonderBookingB2CDataCartNumObj
    {
        public int adt { get; set; }
        public int child { get; set; }
        public int old { get; set; }

    }

    public class VinWonderBookingB2CDataCartItem
    {
        public string Code { get; set; }
        public string ServiceKey { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string SessionKey { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public string TypeCode { get; set; }
        public string TypeName { get; set; }
        public double BasePrice { get; set; }
        public double Price { get; set; }
        public double TotalPromotionDiscountPrice { get; set; }
        public double TotalPrice { get; set; }
        public double Profit { get; set; }
        public double RateDiscountPercent { get; set; }
        public double RateDiscountPrice { get; set; }
        public double PromotionDiscountPercent { get; set; }
        public double PromotionDiscountPrice { get; set; }
        public double VATPercent { get; set; }
        public int Availability { get; set; }
        public int NumberOfUses { get; set; }
        public int Slots { get; set; }
        public VinWonderBookingB2CDataCartItemDateTimeUsed DateTimeUsed { get; set; }
       // public List<object>? Promotions { get; set; }
       // public PromotionServiceInfo PromotionServiceInfo { get; set; }
        public VinWonderBookingB2CDataCartItemTransaction? Transaction { get; set; }
        
        public string TransactionClassification { get; set; }
        public int CardCode { get; set; }
        public bool IsTicketBonus { get; set; }
        public bool IsRevenueBonus { get; set; }
        public double OriginalPrice { get; set; }
        public string OutletsIds { get; set; }
        public string SapServiceCode { get; set; }
        public string RetailItemType { get; set; }
        public string ServiceSaleForm { get; set; }
        public string RevenueGroup { get; set; }
        public string RevenueClassificationCode { get; set; }
        public string RevenueProperty { get; set; }
        public string RateGroup { get; set; }
        public string TimeSlot { get; set; }
        public string ExpiredDate { get; set; }
        public string SessionId { get; set; }
        public string ServiceWarehouseId { get; set; }
        public VinWonderBookingB2CDataCartItemRateForm RateForm { get; set; }
       // public JObject? ComboDetails { get; set; }
       // public JObject? Outlets { get; set; }
        
    }


    public class VinWonderBookingB2CDataCartItemRateForm
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
    }

    public class VinWonderBookingB2CDataCartItemTransaction
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string TypeCode { get; set; }
        public string TypeName { get; set; }

    }

    public class VinWonderBookingB2CDataCartItemDateTimeUsed
    {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public int TimeStart { get; set; }
        public int MinuteStart { get; set; }
        public int TimeEnd { get; set; }
        public int MinuteEnd { get; set; }
        public string GateCode { get; set; }
        public string GateName { get; set; }
        public string WeekDays { get; set; }
        public int DateUsed { get; set; }
        public int? NumberOfUses { get; set; }

    }
}
