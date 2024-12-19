using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Tour
{
    public class TourViewModel : ENTITIES.Models.Tour
    {
        public string SalerIdName { get; set; }
        public string TourName { get; set; }
        public string StatusName { get; set; }
        public string TourProductName { get; set; }
        public string ContactClientId { get; set; }
        public string ORGANIZINGName { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public string StartPoint1 { get; set; }
        public string StartPoint2 { get; set; }
        public string StartPoint3 { get; set; }
        public string GroupEndPoint1 { get; set; }
        public string GroupEndPoint2 { get; set; }
        public string GroupEndPoint3 { get; set; }
        public string SuplierName { get; set; }
        public string OperatorName { get; set; }
        public double TotalBaby { get; set; }
        public double TotalAdult { get; set; }
        public double TotalChildren { get; set; }
        public double OrderAmount { get; set; }
     
    }
    public class TourSearchViewModel
    {
        public string ServiceCode { get; set; }
        public string OrderCode { get; set; }
        public string StatusBooking { get; set; }
        public string TourType { get; set; }
        public string OrganizingType { get; set; }
        public string StartDateFrom { get; set; }
        public string StartDateTo { get; set; }
        public string EndDateFrom { get; set; }
        public string EndDateTo { get; set; }
        public string UserCreate { get; set; }
        public string CreateDateFrom { get; set; }
        public string CreateDateTo { get; set; }
        public string SalerPermission { get; set; }
        public int Days { get; set; } = -1;
        public int Star { get; set; } = -1;
        public int IsDisplayWeb { get; set; } = -1;
        public int StartPoint { get; set; } = -1;
        public int Endpoint { get; set; } = -1;
        public int SalerId { get; set; } = -1;
        public int OperatorId { get; set; } = -1;
        public int PageIndex { get; set; } = -1;
        public int PageSize { get; set; } = -1;
    }
    public class TourGetListViewModel
    {
        public string ServiceCode { get; set; }
        public string OrderNo { get; set; }
        public string OrderStatusName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string SalerName { get; set; }
        public string OperatorName { get; set; }
        public string OrganizingTypeName { get; set; }
        public string TourTypeName { get; set; }
        public string StartPoint1 { get; set; }
        public string StartPoint2 { get; set; }
        public string StartPoint3 { get; set; }
        public string GroupEndPoint1 { get; set; }
        public string GroupEndPoint2 { get; set; }
        public string GroupEndPoint3 { get; set; }
        public string StatusName { get; set; }
        public int TourType { get; set; }
        public int Status { get; set; }
        public int OrderStatus { get; set; }
        public int OrderId { get; set; }
        public int Id { get; set; }
        public double TotalAdult { get; set; }
        public double TotalChildren { get; set; }
        public double TotalBaby { get; set; }

    }
    public class CountTourviewModel
    {
        public double Total { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }

    }
}
