using ENTITIES.APPModels.PushHotel;
using ENTITIES.Models;
using ENTITIES.ViewModels.Booking;
using ENTITIES.ViewModels.Hotel;
using ENTITIES.ViewModels.MongoDb;
using ENTITIES.ViewModels.Programs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REPOSITORIES.IRepositories.Hotel
{
    public interface IHotelDetailRepository
    {
        Dictionary<string, string> getLocationByStreet(string street);

        List<HotelFEDataModel> GetFEHotelList(HotelFESearchModel model);

        List<HotelFERoomDataModel> GetFEHotelRoomList(int hotel_id);

        List<HotelFERoomPackageDataModel> GetFEHotelRoomPackageList(int hotel_id, DateTime? from_date, DateTime? to_date);

        List<HotelFERoomPackageDataModel> GetFERoomPackageListByRoomId(int room_id, DateTime fromDate, DateTime toDate);
        Task<ENTITIES.Models.Hotel> GetByHotelId(string hotel_id);
        Task<ENTITIES.Models.Hotel> GetById(int id);
        public List<HotelPricePolicyViewModel> GetHotelRoomPricePolicy(string hotel_id,  string client_types, bool get_all=false /*, DateTime arrival_date, DateTime departure_date*/);
        Task<int> SummitHotelDetail(HotelSummit model);
        Task<ENTITIES.Models.Hotel> GetHotelContainRoomid(int room_id);
        IEnumerable<HotelRoomGridModel> GetHotelRoomList(int hotel_id, int page_index, int page_size);
        Task<List<ENTITIES.Models.Hotel>> GetByType(bool isVinHotel = false);
        List<HotelFERoomPackageDataModel> GetFERoomPackageDaiLyListByRoomId(int room_id, DateTime fromDate, DateTime toDate);
        List<HotelFEDataModel> GetFEHotelDetails(string name, bool? is_commit_fund, string province_id, int page_index, int page_size);
        Task<List<ProgramsExpiredViewModel>> GetListProgramsPackageExpired();
        List<HotelFEDataModel> GetFEHotelDetailPosition(string name, bool? is_commit_fund, string province_id, int page_index, int page_size);
        List<HotelFEDataModel> GetFEHotelListPosition(HotelFESearchModel model);
        IEnumerable<HotelSurchargeGridModel> GetHotelSurchargeList(int hotel_id, int page_index, int page_size);
        Task<List<HotelPosition>> GetByPositionType(int type);
        public HotelPriceMongoDbModel GetHotelPriceByHotel(string hotel_id, List<int> client_type, DateTime arrivaldate, DateTime departuredate);
        Task<string> UpSertHotelPrice(HotelPriceMongoDbModel item);
    }
}
