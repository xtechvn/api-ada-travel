using ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.APPModels.PushHotel
{
    public class HotelContract
    {
        public RoomFun contract { get; set; }
        public List<RoomPackagesDetail> packages_list { get; set; }
    }
    public class RoomPackagesDetail
    {
        public RoomPackage package { get; set; }
        public List<ServicePiceRoom> room_list { get; set; }
    }
    public class HotelSummit
    {
        public HotelDetail hotel_detail { get; set; }
        public List<HotelProgram> hotel_program { get; set; }
    }
    public class HotelDetail
    {
        public Hotel hotel { get; set; }
        public List<HotelRoom> rooms { get; set; }
    }
    public class HotelProgram
    {
        public Programs program { get; set; }
        public List<ProgramPackage> packages { get; set; }
    }
}
