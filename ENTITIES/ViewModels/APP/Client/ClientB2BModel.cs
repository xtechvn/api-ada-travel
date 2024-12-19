using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.APPModels.Client
{
    public class ClientB2BModel
    {
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public string ClientName { get; set; }
        public long ClientId { get; set; }
        public int ClientType { get; set; }
        public int gender { get; set; } // 1: nam, 2 nữ 
        public int Status { get; set; }
        public string Phone { get; set; }
        public DateTime? Birthday { get; set; }
        public string ProvinceId { get; set; }
        public string DistrictId { get; set; }
        public string WardId { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public bool IsRegisterAffiliate { get; set; }
        public string ReferralId { get; set; }
        public int ParentId { get; set; }
        

    }
}
