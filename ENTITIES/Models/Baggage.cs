using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class Baggage
    {
        public long Id { get; set; }
        public long PassengerId { get; set; }
        public string Airline { get; set; }
        public int Leg { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; }
        public string Value { get; set; }
        public int FlightId { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public string StatusCode { get; set; }
        public bool Confirmed { get; set; }
        public double? WeightValue { get; set; }
    }
}
