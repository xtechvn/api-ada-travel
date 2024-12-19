using ENTITIES.ViewModels.Hotel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Vinpreal
{
    public class RoomDetailsPackageViewModel
    {
        public string distributionChannelId { get; set; }
        public string propertyID { get; set; }
        public int numberOfRoom { get; set; }
        public int clientType { get; set; }
        public string arrivalDate { get; set; }
        public string departureDate { get; set; }
       
        public bool isFilteredByRoomTypeId { get; set; }
        public bool isFilteredByRatePlanId { get; set; }
        public string ratePlanId { get; set; }
        public List<string> roomTypeIds { get; set; }
        public RoomOccupancy roomOccupancy { get; set; }
      

    }

    public class RoomOccupancy
    {
        public int numberOfAdult { get; set; }

        public List<OtherOccupancies> otherOccupancies { get; set; }
    }
    public class OtherOccupancies
    {
        public string otherOccupancyRefID { get; set; }
        public string otherOccupancyRefCode { get; set; }
        public int quantity { get; set; }

    }
      public class PackagesHotelViewModel
      {
        public string hotelId { get; set; }
        public string name { get; set; }
        public string roomID { get; set; }
        public double price { get; set; }
        public string packageType { get; set; }
        public string packageId { get; set; }
        public string ratePlanId { get; set; }

       }
    public class ListPackagesHotelViewModel
    {
        
        
       // public string stayDate { get; set; }
        public double amount { get; set; }
        public double total_price { get; set; }
        public object cancelPolicy { get; set; }
       
        public List<PackagesHotelViewModel> List_packagesHotel { get; set; }

    }

}
