using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Client
{
   public class ClientDeatilModel : ENTITIES.Models.Client
    {
        public double sum_payment { get; set; }
        public string client_type_name { get; set; }
        public string UserId_name { get; set; }
        public string Create_name { get; set; }
        public string create_payment { get; set; }
        public string AgencyType_name { get; set; }
        public string PermisionType_name { get; set; }
        public string CreateDate_UserAgent { get; set; }
        public string UpdateLast { get; set; }
        public string Update_Name { get; set; }
        public string VerifyDate { get; set; }
        public string VerifyStatus_name { get; set; }
        public long UserId { get; set; }
        public long ACStatus { get; set; }

        public long TotalDebtAmount { get; set; }
        public long HotelDebtAmout { get; set; }
        public long ProductFlyTicketDebtAmount { get; set; }
        public long VinWonderDebtAmount { get; set; }
        public long TouringCarDebtAmount { get; set; }
        public long TourDebtAmount { get; set; }
        public string ServiceType { get; set; }
        public string ClientCode { get; set; }

        public long TotalAmountUse { get; set; }
        public long TotalAmountFlyUse { get; set; }
        public long TotalAmountHotelUse { get; set; }
        public long TotalAmountTourUse { get; set; }
        public long TotalAmountOtherUse { get; set; }
        public long TotalAmountVinWonderUse { get; set; }
    }
}
