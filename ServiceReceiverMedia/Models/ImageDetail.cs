using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceReceiverMedia.Models
{
    //Class thông tin ảnh gửi lên.
    [System.Serializable]
    public class ImageDetail
    {
        public string data_file;
        public string extend;
    }
    //Class thông tin ảnh gửi lên.
    [System.Serializable]
    public class PaymentImageDetail
    {
        public string data_file;
        public string extend;

    }
    [System.Serializable]
    public class TicketImageDetail : ImageDetail
    {
        public string file_name;
    }
    //Class thông tin ảnh gửi lên.
    [System.Serializable]
    public class ImageDetailOrder: ImageDetail
    {
        public string order_no;
        public double amount;
        public string type;
    }
}
