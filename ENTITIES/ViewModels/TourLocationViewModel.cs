namespace ENTITIES.ViewModels
{
    public  class TourLocationViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public int location_type { get; set; } // 0: start | 1:end
    }
}
