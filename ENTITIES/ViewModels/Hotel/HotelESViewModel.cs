using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Hotel
{
    public class HotelESViewModel : HotelModel
    {
        public string _id { get; set; } // ID ElasticSearch
        public long id { get; set; } // ID ElasticSearch
        public string telephone { get; set; } // Chuỗi thương hiệu
        public DateTime? checkintime { get; set; }
        public DateTime? checkouttime { get; set; }
        public void GenID()
        {
            string datetime = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString() + (new Random().Next(100, 999)).ToString();
            _id = datetime;
        }
    }
    public class HotelModel
    {
        public string hotelid { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string imagethumb { get; set; }
        public int? numberofroooms { get; set; }

        public double? star { get; set; }
        public int? reviewcount { get; set; }
        public string city { get; set; }

        public double? reviewrate { get; set; }
        public string country { get; set; }
        public string street { get; set; }
        public string state { get; set; }
        public string hoteltype { get; set; }
        public string typeofroom { get; set; }
        public string groupname { get; set; } = null;
        public string index_search { get; set; }
        public string keyword { get; set; }
        public bool? isrefundable { get; set; }
        public bool? isinstantlyconfirmed { get; set; }
        public bool? isdisplaywebsite { get; set; }
        public bool? isvinhotel { get; set; }
        public int? position { get; set; }

    }

}
