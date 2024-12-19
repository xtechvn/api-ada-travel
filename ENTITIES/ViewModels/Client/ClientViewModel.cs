using System;

namespace ENTITIES.ViewModels.Client
{
    public class ClientViewModel
    {
        public int client_map_id { get; set; } // id khách hàng
        public int sale_map_id { get; set; } // id của nhân viên phụ trách khách hàng
        public int client_type { get; set; } // 1 : khách lẻ | 2: Đại lý | 3: nhà cung cấp
        public string address { get; set; } // địa chỉ
        public string client_name { get; set; } // tên khách hàng
        public string email { get; set; } // 
        public int gender { get; set; } // 1: nam, 2 nữ 
        public string phone { get; set; }
        public DateTime join_date { get; set; } // Ngày tạo
        public string taxno { get; set; } // Mã số thuế
    }
}
