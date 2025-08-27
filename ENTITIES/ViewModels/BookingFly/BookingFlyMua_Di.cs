using ENTITIES.Models;
using ENTITIES.ViewModels.Order;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.BookingFly
{
    public class BookingFlyMua_Di
    {
        public order order { get; set; }
       
        public List<passengers> passengers { get; set; }
        public List<bookings> bookings { get; set; }
    
    }
    public class order
    {
        public string orderCode { get; set; }
        public string transactionTime { get; set; }
        public string totalPrice { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public string remark { get; set; }
        public int numberOfAdult { get; set; }
        public int numberOfChild { get; set; }
        public int numberOfInfant { get; set; }
        public string companyName { get; set; }
        public string companyAddress { get; set; }
        public string taxCode { get; set; }
        public string invoiceEmail { get; set; }
        public string paymentType { get; set; }
        public string customerName { get; set; }
        public string customerEmail { get; set; }
        public string customerPhone { get; set; }
        public string clentid { get; set; }
    }
    public class passengers
    {
        public string PaxType { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string paxName { get; set; }
        public string birthday { get; set; }
        public string loyalty { get; set; }
        public string type { get; set; }
        public List<baggages> baggages { get; set; }
    }
    public class baggages
    {
        public string segment { get; set; }
        public string weight { get; set; }
        public string price { get; set; }
    }  
    public class bookings
    {
        public string pnr { get; set; }
        public string recordStatus { get; set; }
        public string timeLimit { get; set; }
        public string airline { get; set; }
        public string purchaseTimeLimit { get; set; }
        public List<routeInfos> routeInfos { get; set; }
        public fareData fareData { get; set; }
        public string system { get; set; }
    }  
    public class routeInfos
    {
        public string routeID { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string departDate { get; set; }
        public string timeFrom { get; set; }
        public string timeTo { get; set; }
        public string airCraft { get; set; }
        public string flightNo { get; set; }
        public string aircraftType { get; set; }
        public string Class { get; set; }
        public string cabinClass { get; set; }
        public string flightTime { get; set; }
        public freeBaggage freeBaggage { get; set; }
        public handBaggage handBaggage { get; set; }
       
    }
    public class freeBaggage
    {
        public int pieces { get; set; }
        public string description { get; set; }
    }
    public class handBaggage
    {
        public int pieces { get; set; }
        public string description { get; set; }
    } 
    public class fareData
    {
        public int fareADT { get; set; }
        public int taxADT { get; set; }
        public int vatADT { get; set; }
        public int fareCHD { get; set; }
        public int taxCHD { get; set; }
        public int vatCHD { get; set; }
        public int fareINF { get; set; }
        public int taxINF { get; set; }
        public int vatINF { get; set; }
        public int otherFee { get; set; }
        public int totalPrice { get; set; }
        public int issueFee { get; set; }

    }

}