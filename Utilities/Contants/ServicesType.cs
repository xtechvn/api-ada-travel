using System;
using System.Collections.Generic;

namespace Utilities.Contants
{
    public enum ServicesType
    {
        VINHotelRent = 1,//SERVICE_TYPE	1	Đặt phòng khách sạn
        OthersHotelRent = 2,//SERVICE_TYPE    2	Đặt phòng khách sạn
        FlyingTicket = 3, //    SERVICE_TYPE	3	Vé máy bay
        VehicleRent = 4,//SERVICE_TYPE	4	Thuê xe du lịch
        Tourist = 5,//SERVICE_TYPE	5	Tour du lịch
        VinWonderTicket = 6,//SERVICE_TYPE	5	Tour du lịch
        Other = 9//DỊCH VỤ KHÁC
    }

    public static class ServicesType2
    {
        public static readonly Dictionary<Int16, string> service = new Dictionary<Int16, string>
        {
            {Convert.ToInt16(ServicesType.VINHotelRent), "HOTEL" },
            {Convert.ToInt16(ServicesType.OthersHotelRent), "HOTEL" },
            {Convert.ToInt16(ServicesType.FlyingTicket), "FLIGHT" },
            {Convert.ToInt16(ServicesType.VehicleRent), "VEHICLE" },
            {Convert.ToInt16(ServicesType.Tourist), "TOUR" },
            {Convert.ToInt16(ServicesType.VinWonderTicket), "VINWONDER" },
            {Convert.ToInt16(ServicesType.Other), "OTHER" }

        };
    }

}
