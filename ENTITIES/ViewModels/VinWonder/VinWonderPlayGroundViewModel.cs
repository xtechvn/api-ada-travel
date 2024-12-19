using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.VinWonder
{
    public class VinWonderPlayGroundViewModel
    {
        public List<string> images { get; set; }
        public string title { get; set; }
        public string lead { get; set; }
        public string content { get; set; }
    }
    public class ListVinWonderViewModel
    {
    
        public long OrderId { get; set; }
        public string OrderNo { get; set; }
        public double Amount { get; set; }
        public int IsFinishPayment { get; set; }
        public string IsFinishPaymentName { get; set; }
        public string code { get; set; }
        public string SiteName { get; set; }
        public long TotalRow { get; set; }
    }
    public class ListVinWonderemialViewModel
    {
        public string SiteName { get; set; }
        public DateTime DateUsed { get; set; }
        public int adt { get; set; }
        public int child { get; set; }
        public int old { get; set; }
        public string Name { get; set; }
        public string typeCode { get; set; }
        public long BookingId { get; set; }
        public long BookingTicketId { get; set; }
    }
    public class ListVinWonderUrlViewModel
    {
   
        public string CardID { get; set; }
        public string QrCode { get; set; }
        public string QrCodeUrl { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public string BookingCode { get; set; }
        public DateTimeUsed? DateTimeUsed { get; set; }
      

    }
    public class DateTimeUsed
    {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string GateCode { get; set; }
        public string GateName { get; set; }
        public string WeekDays { get; set; }
        public int? NumberOfUses { get; set; }
        public int? DateUsed { get; set; }
        public string TimeStart { get; set; }
        public string MinuteStart { get; set; }
        public string TimeEnd { get; set; }
        public string MinuteEnd { get; set; }
    }
}
