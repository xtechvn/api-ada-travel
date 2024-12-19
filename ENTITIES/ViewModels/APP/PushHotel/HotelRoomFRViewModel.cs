// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
using System.Collections.Generic;

public class HotelRoomFRContract
{
    public string date { get; set; }
    public int timestamp { get; set; }
    public int price { get; set; }
    public int allotment { get; set; }
}

public class HotelRoomFRData
{
    public int id { get; set; }
    public int mapping_code { get; set; }
    public string vin_id { get; set; }
    public string name { get; set; }
    public HotelRoomFRDestination destination { get; set; }
    public string address { get; set; }
    public List<HotelRoomFRRoom> rooms { get; set; }
}

public class HotelRoomFRDestination
{
    public int id { get; set; }
    public string name { get; set; }
}

public class HotelRoomFRPriceSummary
{
    public List<HotelRoomFRContract> contract { get; set; }
    public List<HotelRoomFRPromotion> promotion { get; set; }
}

public class HotelRoomFRPromotion
{
    public string date { get; set; }
    public int timestamp { get; set; }
    public int price { get; set; }
    public int allotment { get; set; }
}

public class HotelRoomFRRoom
{
    public int id { get; set; }
    public int mapping_code { get; set; }
    public string name { get; set; }
    public string vin_id { get; set; }
    public HotelRoomFRPriceSummary price_summary { get; set; }
}

public class HotelRoomFRViewModel
{
    public HotelRoomFRData data { get; set; }
}

