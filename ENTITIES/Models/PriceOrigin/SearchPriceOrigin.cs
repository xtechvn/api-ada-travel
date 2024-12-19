using System.Collections.Generic;

namespace ENTITIES.Model.PriceOrigin
{
    public class SearchPriceOrigin
    {
        public string departureDate { get; set; }
        public string arrivalDate { get; set; }
        public string numberOfRoom { get; set; }
        public string hotelId { get; set; }
        public int clientType { get; set; }
    }
}
