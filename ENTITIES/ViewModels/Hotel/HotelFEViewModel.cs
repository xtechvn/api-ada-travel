using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Hotel
{
    public class HotelFEViewModel
    {
    }

    public class HotelFESearchModel
    {
        public string HotelId { get; set; }
        public string ProvinceId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public string RatingStar { get; set; }
        public string Extend { get; set; }
        public string HotelType { get; set; }
        public int PositionType { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class HotelFEDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string HotelId { get; set; }
        public string ShortName { get; set; }
        public string ProvinceName { get; set; }
        public int Star { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string State { get; set; }
        public string HotelType { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string HotelRoomType { get; set; }
        public int NumberOfRoooms { get; set; }
        public string ImageThumb { get; set; }
        public int NumberOfAdult { get; set; }
        public int NumberOfBedRoom { get; set; }
        public int NumberOfChild { get; set; }
        public int NumberOfRoomType { get; set; }
        public string RoomType { get; set; }

        public bool IsRefundable { get; set; }
        public bool IsInstantlyConfirmed { get; set; }
        public bool IsVinHotel { get; set; }
        public int? VerifyDate { get; set; }
        public decimal? ReviewRate { get; set; }
        public int? ReviewCount { get; set; }
        public int? Hotelposition { get; set; }

        public bool IsCommitFund { get; set; }

        public int TotalRow { get; set; }
    }

    public class HotelFERoomDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TypeOfRoom { get; set; }
        public int? NumberOfAdult { get; set; }
        public int? NumberOfChild { get; set; }
        public int? NumberOfRoom { get; set; }
        public string Description { get; set; }
        public int BedRoomType { get; set; }
        public string BedRoomTypeName { get; set; }
        public double RoomArea { get; set; }
        public string Avatar { get; set; }
        public string RoomAvatar { get; set; }
    }

    public class HotelFERoomPackageDataModel
    {
        public int Id { get; set; }
        public string PackageCode { get; set; }
        public string PackageName { get; set; }
        public int ProgramId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int WeekDay { get; set; }
        public string RoomType { get; set; }
        public string Description { get; set; }
        public string StatusName { get; set; }
        public string HotelId { get; set; }
        public string RoomName { get; set; }
        public string ProgramName { get; set; }
        public int RoomTypeId { get; set; }
        public double Amount { get; set; }
        public DateTime? ApplyDate { get; set; }
        public double? TotalProfit { get; set; }
        public double? TotalPrice { get; set; }

    }
}
