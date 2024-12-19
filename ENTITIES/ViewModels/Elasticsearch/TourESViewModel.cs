using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Elasticsearch
{
  public class TourESViewModel
    {
        public int id { get; set; }      
        public string name { get; set; } 
        public string code { get; set; } 
    }
    public class TourProvinceESViewModel
    {
        public int 	startpoint { get; set; }
        public int tourtype { get; set; }
        public string groupendpoint1 { get; set; }
        public string groupendpoint2 { get; set; }
        public string groupendpoint3 { get; set; }
        public string groupidendpoint1 { get; set; }
        public string groupidendpoint2 { get; set; }
        public string groupidendpoint3 { get; set; }
        public string startpoint1 { get; set; }
        public string startpoint2 { get; set; }
        public string startpoint3 { get; set; }
      
    }
}
