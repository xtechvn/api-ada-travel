using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Elasticsearch
{
   public class ProductViewModel
    {
        //public long id { get; set; }
        public string hotel_id { get; set; }//ID khách sạn
        public string name { get; set; }//tên khách sạn
        public Int16 number_of_roooms { get; set; }//số phòng
        public string check_in_time { get; set; }
        public string check_out_time { get; set; }
        public int number_of_adults { get; set; }//Số người lớn
        public int number_of_children { get; set; }//Số trẻ em
        public string conscious { get; set; }//tỉnh thành
        public string district { get; set; }//quận huyện
        public string type_of_room { get; set; }//Loại hình cư trú
        public string product_source_type { get; set; }//nguồn api  từ đâu


    }
    public class ElasticsearchHotelViewModel
    {
        //public long id { get; set; }
        public string hotel_id { get; set; }//ID khách sạn
        public string name { get; set; }//tên khách sạn  
        public string address { get; set; }//địa chỉ
        //public string conscious { get; set; }//tỉnh thành
        public string district { get; set; }//quận huyện
        public List<string> type_of_room { get; set; }//Loại hình cư trú
        public List<string> type_of_room_name { get; set; }//Loại hình cư trú
        public string product_source_type { get; set; }//nguồn api  từ đâu
        public int product_type { get; set; } // Phân loại : 1- Khách sạn cụ thể, 2 - Chỉ là địa danh

    }
}
