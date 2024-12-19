using ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Tour
{
    public class TourProductViewModel
    {
        public TourProduct tour_product { get; set; }
        public List<TourDestination> destinations { get; set; }
    }

    public class TourProductGridModel
    {
        public long id { get; set; }
        public string TourName { get; set; }
        public string ServiceCode { get; set; }
        public string OrganizingTypeName { get; set; }
        public string TourTypeName { get; set; }
        public string TourType { get; set; }
        public string StartPoint { get; set; }
        public string SupplierId { get; set; }
        public string FullName { get; set; }
        public string DateDeparture { get; set; }
        public string StartPoint1 { get; set; }
        public string StartPoint2 { get; set; }
        public string StartPoint3 { get; set; }
        public string GroupEndPoint1 { get; set; }
        public string GroupEndPoint2 { get; set; }
        public string GroupEndPoint3 { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UserCreate { get; set; }
        public string UserUpdate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int IsDisplayWeb { get; set; }
        public int TotalRow { get; set; }
    }

    public class TourProductDetailModel
    {
        public int Id { get; set; }
        public double Price { get; set; }
        public int? Status { get; set; }
        public int? Days { get; set; }
        public double? OldPrice { get; set; }
        public string Avatar { get; set; }
        public int? Star { get; set; }
        public int? Tourtype { get; set; }
        public string datedeparture { get; set; }
        public string Image { get; set; }
        public string listimage { get; set; }
        public string Schedule { get; set; }
        public string Description { get; set; }
        public string Include { get; set; }
        public string Exclude { get; set; }
        public string Refund { get; set; }
        public string Surcharge { get; set; }
        public string Note { get; set; }
        public IEnumerable<TourProductScheduleModel> TourSchedule { get; set; }
        public IEnumerable<string> OtherImages { get; set; }
        public IEnumerable<int> EndPoints { get; set; }
        public string SupplierName { get; set; }
        public string Startpoint1 { get; set; }
        public string Startpoint2 { get; set; }
        public string Startpoint3 { get; set; }
        public string Groupendpoint1 { get; set; }
        public string Groupendpoint2 { get; set; }
        public string Groupendpoint3 { get; set; }
        public string transportname { get; set; }
        public string tourname { get; set; }
        public string tourtypename { get; set; }
        public string organizingname { get; set; }
        public double? min_adultprice { get; set; }
        public double? min_childprice { get; set; }
        public List<TourProgramPackages> packages { get; set; }
        public double? daily_adultprice { get; set; }
        public double? daily_childprice { get; set; }
        public long? daily_package_id { get; set; }

    }

    public class TourProductScheduleModel
    {
        public int day_num { get; set; }
        public string day_title { get; set; }
        public string day_description { get; set; }
    }

    public class TourProductSearchModel
    {
        public string ServiceCode { get; set; }
        public string TourName { get; set; }
        public string TourType { get; set; }
        public string OrganizingType { get; set; }
        public int? Days { get; set; }
        public int? Star { get; set; }
        public bool? IsDisplayWeb { get; set; }
        public string StartPoint { get; set; }
        public string Endpoint { get; set; }
        public bool? IsSelfDesign { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
    public class ListTourProductViewModel
    {
        public long Id { get; set; }
        public string tourname { get; set; }
        public double price { get; set; }
        public int? status { get; set; }
        public int? days { get; set; }
        public double? oldprice { get; set; }
        public string avatar { get; set; }
        public int? star { get; set; }
        public int? tourtype { get; set; }
        public int startpoint { get; set; }
        public string startpoint1 { get; set; }
        public string startpoint2 { get; set; }
        public string startpoint3 { get; set; }
        public string organizingname { get; set; }
        public string tourtypename { get; set; }
        public string updateddate { get; set; }

        public string groupendpoint { get; set; }
        public string groupendpoint1 { get; set; }
        public string groupendpoint2 { get; set; }
        public string groupendpoint3 { get; set; }

        public string location_key { get; set; }
        public string listimage { get; set; }
        public long TotalRow { get; set; }
        public bool isselfdesigned { get; set; }
        public bool isdelete { get; set; }
        public bool isdisplayweb { get; set; }
        public string position { get; set; }
        public double? minprice { get; set; }
        public int? packages { get; set; }

    }
    public class ListFavoriteTourProductViewModel
    {
        public int TourProductId { get; set; }
        public string TourName { get; set; }
        public string Avatar { get; set; }
    }
    public class TourDtailFeViewModel
    {
        public long OrderId { get; set; }
        public long Id { get; set; }
        public string OrderNo { get; set; }
        public double Amount { get; set; }
        public string CreateTime { get; set; }
        public double OrderAmount { get; set; }
        public int OrderStatus { get; set; }
        public int PaymentStatus { get; set; }
        public string OrderStatusName { get; set; }
        public string PaymentStatusName { get; set; }
        public string TourProductName { get; set; }
        public string DateDeparture { get; set; }
        public DateTime? StartDate { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ClientName { get; set; }
        public string Note { get; set; }
       
        public int TotalChildren { get; set; }
        public int TotalAdult { get; set; }
        public int TotalBaby { get; set; }
        public int Days { get; set; }
    }
    public class OrderListTourViewModel
    {
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
        public string TourId { get; set; }
        public string TourName { get; set; }
        public string Amount { get; set; }
        public string OrderStatus { get; set; }
        public string OrderStatusName { get; set; }
    }
    public class TourProductSearchCacheModel
    {
       public List<ListTourProductViewModel> data { get; set; }
       public List<string> listimages { get; set; }
    }
}

