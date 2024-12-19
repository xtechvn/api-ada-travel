using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.B2C
{
    public class AccountB2CViewModel
    {
        public class AccountB2C
        {
            public string ClientName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Password { get; set; }
            public string PasswordBackup { get; set; }
            public string ClientCode { get; set; }
            public Boolean isReceiverInfoEmail { get; set; }
            public int ClientType { get; set; }
        }
        public class AcconutViewModel
        {
            public class ClientB2CViewModel
            {
                //-- client_id -> account_client_id
                public long client_id { get; set; } // id khách hàng
                public long account_client_id { get; set; } // id khách hàng (ID tài khoản khách hàng - AccountClientId) 
                public string Address { get; set; } // địa chỉ
                public string Wards { get; set; } // địa chỉ
                public string District { get; set; } // địa chỉ
                public string ProvinceId { get; set; } // địa chỉ
                public string client_name { get; set; } // tên khách hàng               
                public int gender { get; set; } // 1: nam, 2 nữ 
                public string Phone { get; set; } //
                public DateTime UpdateTime { get; set; } // Ngày tạo
                public DateTime Birthday { get; set; } // Ngày tạo
               
            }
        }
    }
}
