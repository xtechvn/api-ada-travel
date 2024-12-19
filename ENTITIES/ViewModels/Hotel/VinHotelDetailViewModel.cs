using ENTITIES.ViewModels.Vinpreal;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Hotel
{
    public class VinHotelDetailViewModel
    {
        public List<ListPackagesHotelViewModel> packages { get; set; }
        public string surcharges{ get; set; }
    }
}
