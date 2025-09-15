using System;
using System.Collections.Generic;

namespace Utilities.Contants
{
    public enum ClientType
    {
        ALL = -1,
        AGENT = 1, // Đại lý
        TIER_1_AGENT = 2, // Đối tác chiến lược
        TIER_2_AGENT = 3, // Đại lý cấp 2
        TIER_3_AGENT = 4,// Đại lý cấp 3
        CUSTOMER = 5, //Khách lẻ
        SALE = 6, // nv kinh doanh
        ENTERPRISE = 7, // Doanh nghiệp
        COLLABORATORS = 8,// Cộng tác viên
        STAFF = 9//Nhân viên trong công ty

    }
    public enum ClientTypes
    {
        ALL = -1,
        AGENT = 1, // Đại lý
        TIER_1_AGENT = 2, // Đối tác chiến lược
        TIER_2_AGENT = 3, // Đại lý cấp 2
        TIER_3_AGENT = 4,// Đại lý cấp 3
        CUSTOMER = 5, //Khách lẻ
        SALE = 6, // nv kinh doanh
        ENTERPRISE = 7, // Doanh nghiệp
        COLLABORATORS = 8,// Cộng tác viên
        STAFF = 9//Nhân viên trong công ty

    }
    public enum ClientProfileType
    {
        BOOKER = 0, //Sale booking, chỉ có trong trường hợp book hộ
        CONTACT_CLIENT = 1, // Thông tin thành viên chính , sử dụng để liên hệ
        GUEST_ADULT = 2, // Thông tin thành viên trong đoàn là người lớn
        GUEST_CHILD = 2, // Thông tin thành viên trong đoàn là người lớn
        GUEST_INFANT = 2, // Thông tin thành viên trong đoàn là trẻ sơ sinh
    }
    public static class ClientTypeName
    {
        public static readonly Dictionary<Int16, string> service = new Dictionary<Int16, string>
        {
            {Convert.ToInt16(ClientType.AGENT), "DL" },
            {Convert.ToInt16(ClientType.TIER_1_AGENT), "CL" },
            {Convert.ToInt16(ClientType.TIER_2_AGENT), "DL" },
            {Convert.ToInt16(ClientType.TIER_3_AGENT), "DL" },
            {Convert.ToInt16(ClientType.CUSTOMER), "KL" },
            {Convert.ToInt16(ClientType.SALE), "SL" },
            {Convert.ToInt16(ClientType.ENTERPRISE), "DN" },
            {Convert.ToInt16(ClientType.COLLABORATORS), "CT" },
            {Convert.ToInt16(ClientType.STAFF), "NV" }
        };
    }
    public enum ClientUtmSource
    {
       B2C = 8,//b2c
      
       

    }
}
