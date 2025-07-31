using DAL.Hotel;
using DAL.MongoDB;
using Entities.ConfigModels;
using Entities.ViewModels;
using ENTITIES.APPModels.PushHotel;
using ENTITIES.Models;
using ENTITIES.ViewModels.Hotel;
using ENTITIES.ViewModels.MongoDb;
using ENTITIES.ViewModels.Programs;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories.Hotel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace REPOSITORIES.Repositories.Hotel
{
    public class HotelDetailRepository : IHotelDetailRepository
    {
        private readonly DAL.Hotel.HotelDAL _hotelDAL;
        private readonly DAL.Programs.ProgramsDAL _programsDAL;
        private readonly HotelPositionDAL _hotelPositionDAL;
        private readonly HotelPriceMongoDAL hotelPriceMongoDAL;

        public HotelDetailRepository(IOptions<DataBaseConfig> _dataBaseConfig)
        {
            _hotelDAL = new DAL.Hotel.HotelDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
            _programsDAL = new DAL.Programs.ProgramsDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
            _hotelPositionDAL = new HotelPositionDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
            hotelPriceMongoDAL = new HotelPriceMongoDAL(_dataBaseConfig.Value.MongoServer.connection_string, _dataBaseConfig.Value.MongoServer.catalog_core);

        }

        /// <summary>
        /// cuonglv: lấy ra thông tin tỉnh thành, quận huyện
        /// </summary>
        /// <param name="hotel"></param>
        /// <returns></returns>
        public Dictionary<string, string> getLocationByStreet(string street)
        {
            try
            {
                string city = string.Empty; // Tinh thanh
                string state = string.Empty; // quan huyen
                int provinceId = -1;
                var province = _hotelDAL.getProvinceByStreet(street);
                if (province != null && province.Rows.Count > 0)
                {
                    city = province.Rows[0]["province_name"].ToString();
                    provinceId = Convert.ToInt32(province.Rows[0]["ProvinceId"]);

                    // lay ra quan huyen
                    var district = _hotelDAL.getDistrictByStreet(street, provinceId);
                    if (district != null && district.Rows.Count > 0)
                    {
                        state = district.Rows[0]["district_name"].ToString();
                    }
                    else
                    {
                        // lấy ra các quận thuộc tỉnh thành

                        // khi ko có quận huyện
                        // kiem tra co phuong xa khong
                        var Ward = _hotelDAL.getWardByStreet(street, provinceId);
                        if (Ward != null && Ward.Rows.Count > 0)
                        {
                            state = Ward.Rows[0]["district_name"].ToString();
                        }
                    }
                }
                else
                {
                    // Kiểm tra có quận huyện ko
                    // lay ra quan huyen
                    var district = _hotelDAL.getDistrictByStreet(street, -1);
                    if (district != null && district.Rows.Count > 0)
                    {
                        city = district.Rows[0]["province_name"].ToString();
                        state = district.Rows[0]["district_name"].ToString();
                    }
                    else
                    {
                        LogHelper.InsertLogTelegram("getLocationByStreet - HotelDetailRepository: khong tach duoc street = " + street);
                        return null;
                    }
                }
                var data = new Dictionary<string, string>
                {
                    { "city" , city},
                    { "state" , state }
                };
                return data;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getLocationByStreet - HotelDetailRepository. " + ex + " street = " + street);
                return null;
            }
        }

        public List<HotelFEDataModel> GetFEHotelList(HotelFESearchModel model)
        {
            try
            {
                var dataTable = _hotelDAL.GetFEHotelList(model);
                return dataTable.ToList<HotelFEDataModel>();
            }
            catch
            {
                throw;
            }
        }
        public List<HotelFEDataModel> GetFEHotelListPosition(HotelFESearchModel model)
        {
            try
            {
                var dataTable = _hotelDAL.GetFEHotelListPosition(model);
                return dataTable.ToList<HotelFEDataModel>();
            }
            catch
            {
                throw;
            }
        }
        public List<HotelFERoomDataModel> GetFEHotelRoomList(int hotel_id)
        {
            try
            {
                var dataTable = _hotelDAL.GetFEHotelRoomByHotelId(hotel_id);
                return dataTable.ToList<HotelFERoomDataModel>();
            }
            catch
            {
                throw;
            }
        }

        public List<HotelFERoomPackageDataModel> GetFEHotelRoomPackageList(int hotel_id, DateTime? from_date, DateTime? to_date)
        {
            try
            {
                var dataTable = _hotelDAL.GetFEHotelRoomPackageByHotelId(hotel_id, from_date, to_date);
                return dataTable.ToList<HotelFERoomPackageDataModel>();
            }
            catch
            {
                throw;
            }
        }

        public List<HotelFERoomPackageDataModel> GetFERoomPackageListByRoomId(int room_id, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var dataTable = _hotelDAL.GetFEHotelRoomPackageByRoomId(room_id, fromDate, toDate);
                return dataTable.ToList<HotelFERoomPackageDataModel>();
            }
            catch
            {
                throw;
            }
        }
        public List<HotelFERoomPackageDataModel> GetFERoomPackageDaiLyListByRoomId(int room_id, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var dataTable = _hotelDAL.GetFERoomPackageDaiLyListByRoomId(room_id, fromDate, toDate);
                return dataTable.ToList<HotelFERoomPackageDataModel>();
            }
            catch
            {
                throw;
            }
        }
        public async Task<ENTITIES.Models.Hotel> GetByHotelId(string hotel_id)
        {
            return await _hotelDAL.GetByHotelId(hotel_id);

        }
        public async Task<ENTITIES.Models.Hotel> GetById(int id)
        {
            return await _hotelDAL.GetById(id);

        }
        public List<HotelPricePolicyViewModel> GetHotelRoomPricePolicy(string hotel_id, string client_types, bool get_all = false/*, DateTime arrival_date, DateTime departure_date*/)
        {
            try
            {
                var dataTable = _hotelDAL.GetHotelPricePolicyDaily(hotel_id, client_types/*,  arrival_date,  departure_date*/);
                if (dataTable == null || dataTable.Rows.Count <= 0)
                {
                    dataTable = _hotelDAL.GetHotelPricePolicy(hotel_id, client_types/*,  arrival_date,  departure_date*/);
                }
                if (dataTable == null || dataTable.Rows.Count > 0) { get_all = true; }
                if (get_all)
                {
                    var list = dataTable.ToList<HotelPricePolicyViewModel>();
                    var data_table_special = _hotelDAL.GetHotelPricePolicy(hotel_id, client_types/*,  arrival_date,  departure_date*/);
                    if (data_table_special != null && data_table_special.Rows.Count > 0)
                    {
                        list.AddRange(data_table_special.ToList<HotelPricePolicyViewModel>());
                    }
                    return list;
                }
                return dataTable.ToList<HotelPricePolicyViewModel>();
            }
            catch
            {
                return new List<HotelPricePolicyViewModel>();
            }
        }
        public async Task<int> SummitHotelDetail(HotelSummit model)
        {
            try
            {
                var ex_hotel = await _hotelDAL.GetByHotelId(model.hotel_detail.hotel.HotelId);
                if (ex_hotel != null && ex_hotel.Id > 0)
                {
                    model.hotel_detail.hotel.Id = ex_hotel.Id;
                    model.hotel_detail.hotel.CreatedBy = ex_hotel.CreatedBy;
                    model.hotel_detail.hotel.CreatedDate = ex_hotel.CreatedDate;
                    model.hotel_detail.hotel.SupplierId = ex_hotel.SupplierId;
                    model.hotel_detail.hotel.ListSupplierId = ex_hotel.ListSupplierId;
                    model.hotel_detail.hotel.SalerId = ex_hotel.SalerId;
                    var id = _hotelDAL.UpdateHotel(model.hotel_detail.hotel);

                }
                else
                {
                    model.hotel_detail.hotel.Id = _hotelDAL.CreateHotel(model.hotel_detail.hotel);
                }
                foreach (var r in model.hotel_detail.rooms)
                {
                    r.HotelId = model.hotel_detail.hotel.Id;
                    var exists_room = await _hotelDAL.GetHotelRoomByRoomCode(r.RoomId.Trim());
                    if (exists_room != null && exists_room.Id > 0)
                    {
                        r.Id = exists_room.Id;
                        r.CreatedBy = exists_room.CreatedBy;
                        r.CreatedDate = exists_room.CreatedDate;
                        _hotelDAL.UpdateHotelRoom(r);
                    }
                    else
                    {
                        r.Id = _hotelDAL.CreateHotelRoom(r);

                    }
                }
                if (model.hotel_program != null && model.hotel_program.Count > 0)
                {
                    foreach (var p in model.hotel_program)
                    {
                        p.program.HotelId = model.hotel_detail.hotel.Id;
                        p.program.SupplierId = model.hotel_detail.hotel.SupplierId != null ? (int)model.hotel_detail.hotel.SupplierId : 0;
                        p.program.UserVerify = 18;
                        p.program.VerifyDate = DateTime.Now;
                        p.program.Status = 2;
                        var exists_program = _programsDAL.GetProgramByCode(p.program.ProgramCode, model.hotel_detail.hotel.Id);
                        if (exists_program != null && exists_program.Id > 0)
                        {
                            p.program.CreatedBy = exists_program.CreatedBy;
                            p.program.CreatedDate = exists_program.CreatedDate;

                            p.program.Id = exists_program.Id;

                            await _programsDAL.UpdatePrograms(p.program);
                        }
                        else
                        {
                            p.program.Id = await _programsDAL.InsertPrograms(p.program);
                        }
                        foreach (var package in p.packages)
                        {

                            package.ProgramId = p.program.Id;
                            var match_room = model.hotel_detail.rooms.First(x => x.RoomId == package.RoomType);
                            var exists_package = _programsDAL.GetProgramPackagesByCode(package.PackageName, p.program.Id, match_room.Id);
                            package.RoomTypeId = match_room.Id;
                            package.RoomType = match_room.Code + " - " + match_room.Name;
                            if (exists_package != null && exists_package.Id > 0)
                            {
                                package.CreatedBy = exists_package.CreatedBy;
                                package.CreatedDate = exists_package.CreatedDate;
                                package.Id = exists_package.Id;
                                await _programsDAL.UpdateProgramPackage(package);
                            }
                            else
                            {
                                package.Id = await _programsDAL.InsertProgramPackage(package);
                            }
                        }
                    }
                }
                return model.hotel_detail.hotel.Id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SummitHotelDetail - HotelDetailRepository. " + ex);
                return -1;
            }
        }
        public async Task<ENTITIES.Models.Hotel> GetHotelContainRoomid(int room_id)
        {
            return await _hotelDAL.GetHotelContainRoomid(room_id);

        }
        public IEnumerable<HotelRoomGridModel> GetHotelRoomList(int hotel_id, int page_index, int page_size)
        {
            try
            {
                var dataTable = _hotelDAL.GetHotelRoomByHotelId(hotel_id, page_index, page_size);
                return dataTable.ToList<HotelRoomGridModel>();
            }
            catch
            {
                throw;
            }
        }
        public async Task<List<ENTITIES.Models.Hotel>> GetByType(bool isVinHotel = false)
        {
            return await _hotelDAL.GetByType(isVinHotel);

        }
        public List<HotelFEDataModel> GetFEHotelDetails(string name, bool? is_commit_fund, string province_id, int page_index, int page_size)
        {
            try
            {
                var dataTable = _hotelDAL.GetFEHotelDetail(name, is_commit_fund, province_id, page_index, page_size);
                return dataTable.ToList<HotelFEDataModel>();
            }
            catch
            {
                throw;
            }
        }
        public List<HotelFEDataModel> GetFEHotelDetailPosition(string name, bool? is_commit_fund, string province_id, int page_index, int page_size)
        {
            try
            {
                var dataTable = _hotelDAL.GetFEHotelDetailPosition(name, is_commit_fund, province_id, page_index, page_size);
                return dataTable.ToList<HotelFEDataModel>();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<ProgramsExpiredViewModel>> GetListProgramsPackageExpired()
        {
            try
            {
                var dataTable = await _programsDAL.GetListProgramsPackageExpired();
                if (dataTable == null || dataTable.Rows.Count > 0)
                {
                    var data = dataTable.ToList<ProgramsExpiredViewModel>();
                    return data;
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListProgramsPackageExpired - HotelDetailRepository. " + ex);
            }
            return null;
        }
        public IEnumerable<HotelSurchargeGridModel> GetHotelSurchargeList(int hotel_id, int page_index, int page_size)
        {
            try
            {
                var dataTable = _hotelDAL.GetHotelSurchargeDataTable(hotel_id, page_index, page_size);
                return dataTable.ToList<HotelSurchargeGridModel>();
            }
            catch
            {
                throw;
            }
        }
        public async Task<List<HotelPosition>> GetListHotelActivePosition()
        {
            return await _hotelPositionDAL.GetListHotelActivePosition();
        }
        public async Task<GenericViewModel<HotelPriceMongoDbModel>> GetListHotelPriceByFilter(string hotel_id, List<int> client_types, DateTime arrivaldate, DateTime departuredate, string location = null, string stars = "", double? min_price = -1, double? max_price = -1, int? page_index = 1, int? page_size = 30)
        {
            try
            {

                return await hotelPriceMongoDAL.GetListByFilter(hotel_id, client_types, arrivaldate, departuredate, location, stars, min_price, max_price, page_index, page_size);
            }
            catch
            {

            }
            return new GenericViewModel<HotelPriceMongoDbModel>();
        }
        public async Task<GenericViewModel<HotelPriceMongoDbModel>> GetListAllHotelPriceByFilter(string hotel_id, List<int> client_types, DateTime arrivaldate, DateTime departuredate, string location = null, string stars = "", double? min_price = -1, double? max_price = -1, int? page_index = 1, int? page_size = 30, bool? is_commit = false)
        {
            try
            {

                return await hotelPriceMongoDAL.GetListAllByFilter(hotel_id, client_types, arrivaldate, departuredate, location, stars, min_price, max_price, page_index, page_size, is_commit);
            }
            catch
            {

            }
            return new GenericViewModel<HotelPriceMongoDbModel>();
        }
        public async Task<string> UpSertHotelPrice(HotelPriceMongoDbModel item)
        {
            try
            {
                var exists = hotelPriceMongoDAL.GetByFilter(item.hotel_id, new List<int>() { item.client_type });
                if (exists != null && exists._id != null)
                {
                    await hotelPriceMongoDAL.DeleteByFilter(exists._id, item.hotel_id, new List<int>() { item.client_type });
                    var _id = await hotelPriceMongoDAL.Update(item, exists._id);
                    if (_id != null && _id.Trim() != "") return _id;
                }

                return await hotelPriceMongoDAL.Insert(item);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
