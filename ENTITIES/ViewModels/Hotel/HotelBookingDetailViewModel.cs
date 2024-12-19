using System;

public class HotelBookingDetailViewModel : ENTITIES.Models.HotelBooking
	{
		public string UserUpdate { get; set; }
		public string UserCreate { get; set; }
		public string SalerName { get; set; }
		public string StatusName { get; set; }
		public string ContactClientId { get; set; }
		public string OrderNo { get; set; }
		public double Amount { get; set; }
		public double OrderPrice { get; set; }
		public string SalerPhone { get; set; }
		public string ContactClientEmail { get; set; }
		public string ContactClientPhone { get; set; }
		public string ContactClientName { get; set; }
		public DateTime UpdatedDate { get; set; }
		public string SalerEmail { get; set; }
		public int OrderStatus { get; set; }
		public string SuplierName { get; set; }

	}