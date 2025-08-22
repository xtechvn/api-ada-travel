using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.BookingFly
{
    public class PassengerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MembershipCard { get; set; }
        public string PersonType { get; set; }
        public DateTime? Birthday { get; set; }
        public bool Gender { get; set; }
        public long OrderId { get; set; }
        public List<BaggageViewModel> baggages { get; set; }
        public string Note { get; set; }
        public string GroupBookingId { get; set; }

    }
    public partial class BaggageViewModel
    {
        public long? PassengerId { get; set; }
        public string Airline { get; set; }
        public int Leg { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; }
        public string Value { get; set; }
        public int FlightId { get; set; }
        public string? StartPoint { get; set; }
        public string? EndPoint { get; set; }
        public string? StatusCode { get; set; }
        public bool Confirmed { get; set; }
        public double WeightValue { get; set; }
    }
}
