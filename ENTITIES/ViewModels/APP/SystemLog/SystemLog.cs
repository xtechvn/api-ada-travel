using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.APPModels
{
    public class SystemLog
    {
        public int SourceID { get; set; } // log từ nguồn nào, quy định trong SystemLogSourceID
        public string Type { get; set; } // nội dung: booking, order,....
        public string KeyID { get; set; } // Key: mã đơn, mã khách hàng, mã booking,....
        public string ObjectType { get; set; } // ObjectType: Dùng để phân biệt các đối tượng cần log với nhau. Ví dụ: log cho đơn hàng, khách hàng, hợp đồng, Phiếu thu...
        public string Log { get; set; } // nội dung log
        public int CompanyType { get; set; } // nội dung log
    }
}
