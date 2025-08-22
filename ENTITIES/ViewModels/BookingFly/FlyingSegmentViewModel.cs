using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.BookingFly
{
    public class FlyingSegmentViewModel
    {
        public long FlyBookingId { get; set; }
        public string OperatingAirline { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public string FlightNumber { get; set; }
        public int Duration { get; set; }
        public string Class { get; set; }
        public string Plane { get; set; }
        public string? StartTerminal { get; set; }
        public string? EndTerminal { get; set; }
        public string? StopPoint { get; set; }
        public double? StopTime { get; set; }
        public string AllowanceBaggage { get; set; }
        public string HandBaggage { get; set; }
        public bool HasStop { get; set; }
        public bool ChangeStation { get; set; }
        public bool ChangeAirport { get; set; }
        public bool StopOvernight { get; set; }
        public double AllowanceBaggageValue { get; set; }
        public double HandBaggageValue { get; set; }
    }
}
