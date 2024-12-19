using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Order
{
    public class HotelBookingB2BViewModel
    {
       public long OrderId { get; set; }
       public DateTime CreateTime { get; set; }
       public string OrderNo { get; set; }
       public string HotelName { get; set; }
       public int OrderStatus { get; set; }
       public int HotelBookingStatus { get; set; }
       public string HotelBookingStatusName { get; set; }
       public string ContactClientName { get; set; }
       public DateTime ArrivalDate { get; set; }
       public DateTime DepartureDate { get; set; }
       public int NumberOfRoom { get; set; }
       public int RoomNights { get; set; }
       public int NumberOfPeople { get; set; }
       public double Amount { get; set; }
       public int TotalRecord { get; set; }

    }
    public class HotelBookingB2BPagingViewModel
    {
      public List<HotelBookingB2BViewModel> data { get; set; }
      public int page { get; set; }   
      public int total_record { get; set; }   
    }

}
