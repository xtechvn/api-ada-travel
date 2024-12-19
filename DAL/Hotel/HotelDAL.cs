using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.APPModels.PushHotel;
using ENTITIES.Models;
using ENTITIES.ViewModels.Hotel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL.Hotel
{
    public class HotelDAL : GenericService<ENTITIES.Models.Hotel>
    {
        private static DbWorker _DbWorker;
        public HotelDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public async Task<ENTITIES.Models.Hotel> GetIDByHotelID(string hotel_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var hotel = await _DbContext.Hotel.AsNoTracking().Where(x => x.HotelId == hotel_id).FirstOrDefaultAsync();
                    if (hotel == null || hotel.Id <= 0)
                    {
                        return null;
                    }
                    return hotel;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelDetail - HotelDAL. " + ex);
                return null;
            }
        }
        public async Task<long> UpdateHotelDetail(ENTITIES.Models.Hotel hotel)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    _DbContext.Hotel.Update(hotel);
                    await _DbContext.SaveChangesAsync();
                    return hotel.Id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelDetail - HotelDAL. " + ex);
                return -1;
            }
        }
        public int CreateHotel(ENTITIES.Models.Hotel model)
        {
            try
            {
                int position = 0;
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@HotelId", model.HotelId ?? (object)DBNull.Value),
                    new SqlParameter("@Name", model.Name ?? (object)DBNull.Value),
                    new SqlParameter("@Email", model.Email ?? (object)DBNull.Value),
                    new SqlParameter("@ImageThumb", model.ImageThumb ?? (object)DBNull.Value),
                    new SqlParameter("@NumberOfRoooms", model.NumberOfRoooms?? (object)DBNull.Value),
                    new SqlParameter("@Star", model.Star ?? (object)DBNull.Value),
                    new SqlParameter("@ReviewCount", model.ReviewCount ?? (object)DBNull.Value),
                    new SqlParameter("@ReviewRate", model.ReviewRate ?? (object)DBNull.Value),
                    new SqlParameter("@City", model.City?? (object)DBNull.Value),
                    new SqlParameter("@Country", model.Country ?? (object)DBNull.Value),
                    new SqlParameter("@Street", model.Street ?? (object)DBNull.Value),
                    new SqlParameter("@State", model.State?? (object)DBNull.Value),
                    new SqlParameter("@HotelType", model.HotelType ?? (object)DBNull.Value),
                    new SqlParameter("@TypeOfRoom", model.TypeOfRoom?? (object)DBNull.Value),
                    new SqlParameter("@IsRefundable", model.IsRefundable ?? (object)DBNull.Value),
                    new SqlParameter("@IsInstantlyConfirmed ", model.IsInstantlyConfirmed ?? (object)DBNull.Value),
                    new SqlParameter("@GroupName", model.GroupName?? (object)DBNull.Value),
                    new SqlParameter("@Telephone", model.Telephone?? (object)DBNull.Value),
                    new SqlParameter("@CheckinTime", DateTime.Now),
                    new SqlParameter("@CheckoutTime", DateTime.Now),
                    new SqlParameter("@SupplierId",model.SupplierId ?? (object)DBNull.Value),
                    new SqlParameter("@ProvinceId", model.ProvinceId ?? (object)DBNull.Value),
                    new SqlParameter("@TaxCode",  model.TaxCode ?? (object)DBNull.Value),
                    new SqlParameter("@EstablishedYear",  model.EstablishedYear ?? (object)DBNull.Value),
                    new SqlParameter("@RatingStar", model.RatingStar ?? (object)DBNull.Value),
                    new SqlParameter("@ChainBrands", model.ChainBrands ?? (object)DBNull.Value),
                    new SqlParameter("@VerifyDate", model.VerifyDate ?? (object)DBNull.Value),
                    new SqlParameter("@SalerId", model.SalerId ?? (object)DBNull.Value),
                    new SqlParameter("@IsDisplayWebsite", model.IsDisplayWebsite),
                    new SqlParameter("@ShortName", model.ShortName ?? (object)DBNull.Value),
                    new SqlParameter("@Description", model.Description ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedBy", model.CreatedBy?? (object)DBNull.Value),
                    new SqlParameter("@IsCommitFund",model.IsCommitFund?? (object)DBNull.Value),
                    new SqlParameter("@CreatedDate", DateTime.Now),
                    new SqlParameter("@Position", position)
                };
                var id = _DbWorker.ExecuteNonQuery(StoreProceduresName.SP_InsertHotel, objParam);
                if (model.HotelId!=null && model.HotelId.Trim() != "")
                {
                    using (var _DbContext = new EntityDataContext(_connection))
                    {
                        var hotel =  _DbContext.Hotel.AsNoTracking().Where(x => x.Id == id).FirstOrDefault();
                        if (hotel != null && hotel.Id > 0)
                        {
                            hotel.HotelId = model.HotelId;
                            hotel.IsVinHotel = model.IsVinHotel;
                            hotel.Extends = model.Extends;
                            _DbContext.Hotel.Update(hotel);
                            _DbContext.SaveChanges();
                        }
                       
                    }
                }
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateHotel - HotelDAL. " + ex);
                throw;
            }
        }

        public int UpdateHotel(ENTITIES.Models.Hotel model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@HotelId", model.HotelId ?? (object)DBNull.Value ),
                    new SqlParameter("@Name", model.Name ?? (object)DBNull.Value),
                    new SqlParameter("@Email", model.Email ?? (object)DBNull.Value),
                    new SqlParameter("@ImageThumb", model.ImageThumb ?? (object)DBNull.Value),
                    new SqlParameter("@NumberOfRoooms", model.NumberOfRoooms?? (object)DBNull.Value),
                    new SqlParameter("@Star", model.Star ?? (object)DBNull.Value),
                    new SqlParameter("@ReviewCount", model.ReviewCount ?? (object)DBNull.Value),
                    new SqlParameter("@ReviewRate", model.ReviewRate ?? (object)DBNull.Value),
                    new SqlParameter("@City", model.City?? (object)DBNull.Value),
                    new SqlParameter("@Country", model.Country ?? (object)DBNull.Value),
                    new SqlParameter("@Street", model.Street ?? (object)DBNull.Value),
                    new SqlParameter("@State", model.State?? (object)DBNull.Value),
                    new SqlParameter("@HotelType", model.HotelType ?? (object)DBNull.Value),
                    new SqlParameter("@TypeOfRoom", model.TypeOfRoom?? (object)DBNull.Value),
                    new SqlParameter("@IsRefundable", model.IsRefundable ?? (object)DBNull.Value),
                    new SqlParameter("@IsInstantlyConfirmed ", model.IsInstantlyConfirmed ?? (object)DBNull.Value),
                    new SqlParameter("@GroupName", model.GroupName?? (object)DBNull.Value),
                    new SqlParameter("@Telephone", model.Telephone?? (object)DBNull.Value),
                    new SqlParameter("@CheckinTime", DateTime.Now),
                    new SqlParameter("@CheckoutTime", DateTime.Now),
                    new SqlParameter("@SupplierId",model.SupplierId ?? (object)DBNull.Value),
                    new SqlParameter("@ProvinceId", model.ProvinceId ?? (object)DBNull.Value),
                    new SqlParameter("@TaxCode",  model.TaxCode ?? (object)DBNull.Value),
                    new SqlParameter("@EstablishedYear",  model.EstablishedYear ?? (object)DBNull.Value),
                    new SqlParameter("@RatingStar", model.RatingStar ?? (object)DBNull.Value),
                    new SqlParameter("@ChainBrands", model.ChainBrands ?? (object)DBNull.Value),
                    new SqlParameter("@VerifyDate", model.VerifyDate ?? (object)DBNull.Value),
                    new SqlParameter("@SalerId", model.SalerId ?? (object)DBNull.Value),
                    new SqlParameter("@IsDisplayWebsite", model.IsDisplayWebsite),
                    new SqlParameter("@ShortName", model.ShortName ?? (object)DBNull.Value),
                    new SqlParameter("@Description", model.Description ?? (object)DBNull.Value),
                    new SqlParameter("@UpdatedBy",model.UpdatedBy?? (object)DBNull.Value),
                    new SqlParameter("@IsCommitFund",model.IsCommitFund?? (object)DBNull.Value)
                };

                return _DbWorker.ExecuteNonQuery(StoreProceduresName.SP_UpdateHotel, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotel - HotelDAL. " + ex);
                throw;
            }
        }

        // TÌm kiếm tỉnh thành theo street
        public DataTable getProvinceByStreet(string street_name)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[1];
                objParam_order[0] = new SqlParameter("@street_name", street_name);

                var rs = _DbWorker.GetDataTable(StoreProceduresName.sp_getProvinceByStreet, objParam_order);
                return rs;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getProvinceByStreet - HotelDAL. " + ex);
                return null;
            }
        }

        // TÌm kiếm Quận theo street
        public DataTable getDistrictByStreet(string street_name, int province_id)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[2];
                objParam_order[0] = new SqlParameter("@street_name", street_name);
                objParam_order[1] = new SqlParameter("@province_id", province_id);
                var rs = _DbWorker.GetDataTable(StoreProceduresName.sp_getDistrictByStreet, objParam_order);
                return rs;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getDistrictByStreet - HotelDAL. " + ex);
                return null;
            }
        }

        // TÌm kiếm phường theo street
        public DataTable getWardByStreet(string street_name, int province_id)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[2];
                objParam_order[0] = new SqlParameter("@street_name", street_name);
                objParam_order[1] = new SqlParameter("@province_id", province_id);

                var rs = _DbWorker.GetDataTable(StoreProceduresName.sp_getWardByStreet, objParam_order);
                return rs;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getWardByStreet - HotelDAL. " + ex);
                return null;
            }
        }


        // TÌm kiếm phường theo street
        public DataTable GetFEHotelList(HotelFESearchModel model)
        {
            try
            {
                SqlParameter[] objParams = new SqlParameter[]
                {
                    new SqlParameter("@HotelId", model.HotelId ?? (object) DBNull.Value),
                    new SqlParameter("@ProvinceId", model.ProvinceId ?? (object) DBNull.Value),
                    new SqlParameter("@FromDate", model.FromDate ?? (object) DBNull.Value),
                    new SqlParameter("@ToDate", model.ToDate ?? (object) DBNull.Value),
                    new SqlParameter("@RatingStar", model.RatingStar ?? (object) DBNull.Value),
                    new SqlParameter("@Extend", model.Extend ?? (object) DBNull.Value),
                    new SqlParameter("@HotelType", model.HotelType ?? (object) DBNull.Value),
                    new SqlParameter("@PageIndex", model.PageIndex),
                    new SqlParameter("@PageSize", model.PageSize),
                };

                return _DbWorker.GetDataTable(StoreProceduresName.SP_fe_GetListHotel, objParams);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SP_fe_GetListHotel - HotelDAL. " + ex);
                return null;
            }
        }
        public DataTable GetFEHotelListPosition(HotelFESearchModel model)
        {
            try
            {
                SqlParameter[] objParams = new SqlParameter[]
                {
                    new SqlParameter("@HotelId", model.HotelId ?? (object) DBNull.Value),
                    new SqlParameter("@ProvinceId", model.ProvinceId ?? (object) DBNull.Value),
                    new SqlParameter("@FromDate", model.FromDate ?? (object) DBNull.Value),
                    new SqlParameter("@ToDate", model.ToDate ?? (object) DBNull.Value),
                    new SqlParameter("@RatingStar", model.RatingStar ?? (object) DBNull.Value),
                    new SqlParameter("@Extend", model.Extend ?? (object) DBNull.Value),
                    new SqlParameter("@HotelType", model.HotelType ?? (object) DBNull.Value),
                    new SqlParameter("@PositionType", model.PositionType),
                    new SqlParameter("@PageIndex", model.PageIndex),
                    new SqlParameter("@PageSize", model.PageSize),
                };

                return _DbWorker.GetDataTable(StoreProceduresName.SP_fe_GetListHotelPosition, objParams);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SP_fe_GetListHotel - HotelDAL. " + ex);
                return null;
            }
        }
        public DataTable GetFEHotelDetail(string name,bool? is_commit_fund,string province_id,int page_index,int page_size)
        {
            try
            {
                SqlParameter[] objParams = new SqlParameter[]
                {
                    new SqlParameter("@Name", name ?? (object) DBNull.Value),
                    new SqlParameter("@IsCommitFund", is_commit_fund ?? (object) DBNull.Value),
                    new SqlParameter("@ProvinceId", province_id ?? (object) DBNull.Value),
                    new SqlParameter("@PageIndex", page_index),
                    new SqlParameter("@PageSize", page_size),
                };

                return _DbWorker.GetDataTable(StoreProceduresName.SP_fe_SearchHotels, objParams);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetFEHotelDetail - HotelDAL. " + ex);
                return null;
            }
        }
        public DataTable GetFEHotelDetailPosition(string name, bool? is_commit_fund, string province_id, int page_index, int page_size)
        {
            try
            {
                SqlParameter[] objParams = new SqlParameter[]
                {
                    new SqlParameter("@Name", name ?? (object) DBNull.Value),
                    new SqlParameter("@IsCommitFund", is_commit_fund ?? (object) DBNull.Value),
                    new SqlParameter("@ProvinceId", province_id ?? (object) DBNull.Value),
                    new SqlParameter("@PageIndex", page_index),
                    new SqlParameter("@PageSize", page_size),
                };

                return _DbWorker.GetDataTable(StoreProceduresName.SP_fe_SearchHotelsPosition, objParams);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetFEHotelDetail - HotelDAL. " + ex);
                return null;
            }
        }
        public DataTable GetFEHotelRoomByHotelId(int hotel_id)
        {
            try
            {
                SqlParameter[] objParams = new SqlParameter[]
                {
                    new SqlParameter("@HotelId", hotel_id),
                   
                };

                return _DbWorker.GetDataTable(StoreProceduresName.SP_fe_GetHotelRoomByHotelId, objParams);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SP_fe_GetHotelRoomByHotelId - HotelDAL. " + ex);
                return null;
            }
        }

        public DataTable GetFEHotelRoomPackageByHotelId(int hotel_id, DateTime? from_date, DateTime? to_date)
        {
            try
            {
                SqlParameter[] objParams = new SqlParameter[]
                {
                    new SqlParameter("@HotelId", hotel_id),
                    new SqlParameter("@FromDate", from_date ?? (object)DBNull.Value),
                    new SqlParameter("@ToDate", to_date ?? (object)DBNull.Value)
                };

                return _DbWorker.GetDataTable(StoreProceduresName.SP_FE_GetHotelRoomPrice, objParams);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SP_FE_GetHotelRoomPrice - HotelDAL. " + ex);
                return null;
            }
        }

        public DataTable GetFEHotelRoomPackageByRoomId(int room_id, DateTime fromDate, DateTime toDate)
        {
            try
            {
                SqlParameter[] objParams = new SqlParameter[]
                {
                    new SqlParameter("@RoomId", room_id),
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate),
                    new SqlParameter("@PageIndex", 1),
                    new SqlParameter("@PageSize", 1000),
                };

                return _DbWorker.GetDataTable(StoreProceduresName.SP_GetListProgramsPackageByRoomId, objParams);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SP_GetListProgramsPackageByRoomId - HotelDAL. " + ex);
                return null;
            }
        }
        public DataTable GetFERoomPackageDaiLyListByRoomId(int room_id, DateTime fromDate, DateTime toDate)
        {
            try
            {
                SqlParameter[] objParams = new SqlParameter[]
                {
                    new SqlParameter("@RoomId", room_id),
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate),
                    new SqlParameter("@PageIndex", 1),
                    new SqlParameter("@PageSize", 1000),
                };

                return _DbWorker.GetDataTable(StoreProceduresName.SP_GetListProgramsPackageDailyByRoomId, objParams);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SP_GetListProgramsPackageByRoomId - HotelDAL. " + ex);
                return null;
            }
        }
        public async Task<ENTITIES.Models.Hotel> GetByHotelId(string hotel_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var hotel = await _DbContext.Hotel.AsNoTracking().Where(x => x.HotelId == hotel_id).FirstOrDefaultAsync();
                    if (hotel == null || hotel.Id <= 0)
                    {
                        return null;
                    }
                    return hotel;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelDetail - GetByHotelId. " + ex);
                return null;
            }
        }
        public async Task<ENTITIES.Models.Hotel> GetById(int hotel_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var hotel = await _DbContext.Hotel.AsNoTracking().Where(x => x.Id == hotel_id).FirstOrDefaultAsync();
                    if (hotel == null || hotel.Id <= 0)
                    {
                        return null;
                    }
                    return hotel;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelDetail - GetByHotelId. " + ex);
                return null;
            }
        }
        public DataTable GetHotelPricePolicy(string hotel_id, string client_types/*, DateTime arrival_date, DateTime departure_date*/)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[3];
                objParam_order[0] = new SqlParameter("@HotelID", hotel_id);
                objParam_order[1] = new SqlParameter("@RoomIds", DBNull.Value);


                if (client_types==null || client_types.Trim() == "")
                {
                    objParam_order[2] = new SqlParameter("@ClientTypes", DBNull.Value);

                }
                else
                {
                    objParam_order[2] = new SqlParameter("@ClientTypes", client_types);

                }
                //objParam_order[3] = new SqlParameter("@ArrivalDate", arrival_date);
                //objParam_order[4] = new SqlParameter("@DepartureDate", departure_date);

                var rs = _DbWorker.GetDataTable(StoreProceduresName.GetListHotelPricePolicyByHotelID, objParam_order);
                return rs;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelPricePolicy - HotelDAL. " + ex);
                return null;
            }
        } 
        public DataTable GetHotelPricePolicyDaily(string hotel_id, string client_types/*,DateTime arrival_date, DateTime departure_date*/)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[3];
                objParam_order[0] = new SqlParameter("@HotelID", hotel_id);
                objParam_order[1] = new SqlParameter("@RoomIds", DBNull.Value);


                if (client_types==null || client_types.Trim() == "")
                {
                    objParam_order[2] = new SqlParameter("@ClientTypes", DBNull.Value);

                }
                else
                {
                    objParam_order[2] = new SqlParameter("@ClientTypes", client_types);

                }
                //objParam_order[3] = new SqlParameter("@ArrivalDate", arrival_date);
                //objParam_order[4] = new SqlParameter("@DepartureDate", departure_date);

                var rs = _DbWorker.GetDataTable(StoreProceduresName.GetListHotelPricePolicyDailyByHotelID, objParam_order);
                return rs;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelPricePolicy - HotelDAL. " + ex);
                return null;
            }
        }
        public int CreateHotelRoom(HotelRoom model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@HotelId", model.HotelId),
                    new SqlParameter("@RoomId", model.RoomId ?? String.Empty),
                    new SqlParameter("@Name", model.Name ?? model.Name),
                    new SqlParameter("@Code", model.Code ?? String.Empty),
                    new SqlParameter("@Avatar", model.Avatar ?? (object)DBNull.Value),
                    new SqlParameter("@NumberOfBedRoom", model.NumberOfBedRoom ?? (object)DBNull.Value),
                    new SqlParameter("@Description", model.Description ?? (object)DBNull.Value),
                    new SqlParameter("@TypeOfRoom", model.TypeOfRoom ?? (object)DBNull.Value),
                    new SqlParameter("@Thumbnails", model.Thumbnails ?? (object)DBNull.Value),
                    new SqlParameter("@Extends", model.Extends ?? (object)DBNull.Value),
                    new SqlParameter("@BedRoomType", model.BedRoomType ?? (object)DBNull.Value),
                    new SqlParameter("@NumberOfAdult", model.NumberOfAdult ?? (object)DBNull.Value),
                    new SqlParameter("@NumberOfChild", model.NumberOfChild ?? (object)DBNull.Value),
                    new SqlParameter("@NumberOfRoom", model.NumberOfRoom ?? (object)DBNull.Value),
                    new SqlParameter("@RoomArea", model.RoomArea ?? (object)DBNull.Value),
                    new SqlParameter("@RoomAvatar", model.RoomAvatar ?? (object)DBNull.Value),
                    new SqlParameter("@IsActive", model.IsActive),
                    new SqlParameter("@IsDisplayWebsite", model.IsDisplayWebsite),
                    new SqlParameter("@CreatedBy", model.CreatedBy),
                    new SqlParameter("@CreatedDate", model.CreatedDate ?? (object)DBNull.Value),
                };

                return _DbWorker.ExecuteNonQuery(StoreProceduresName.SP_InsertHotelRoom, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateHotelRoom:" + ex);
                return -1;
            }
        }

        public int UpdateHotelRoom(HotelRoom model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@Id", model.Id),
                     new SqlParameter("@HotelId", model.HotelId),
                     new SqlParameter("@RoomId", model.RoomId ?? (object)DBNull.Value),
                     new SqlParameter("@Name", model.Name ?? (object)DBNull.Value),
                     new SqlParameter("@Code", model.Code ?? (object)DBNull.Value),
                     new SqlParameter("@Avatar", model.Avatar ?? (object)DBNull.Value),
                     new SqlParameter("@NumberOfBedRoom", model.NumberOfBedRoom ?? (object)DBNull.Value),
                     new SqlParameter("@Description", model.Description ?? (object)DBNull.Value),
                     new SqlParameter("@TypeOfRoom", model.TypeOfRoom ?? (object)DBNull.Value),
                     new SqlParameter("@Thumbnails", model.Thumbnails ?? (object)DBNull.Value),
                     new SqlParameter("@Extends", model.Extends ?? (object)DBNull.Value),
                     new SqlParameter("@BedRoomType", model.BedRoomType ?? (object)DBNull.Value),
                     new SqlParameter("@NumberOfAdult", model.NumberOfAdult ?? (object)DBNull.Value),
                     new SqlParameter("@NumberOfChild", model.NumberOfChild ?? (object)DBNull.Value),
                     new SqlParameter("@NumberOfRoom", model.NumberOfRoom ?? (object)DBNull.Value),
                     new SqlParameter("@RoomArea", model.RoomArea ?? (object)DBNull.Value),
                     new SqlParameter("@RoomAvatar", model.RoomAvatar ?? (object)DBNull.Value),
                     new SqlParameter("@IsActive", model.IsActive),
                     new SqlParameter("@IsDisplayWebsite", model.IsDisplayWebsite ),
                     new SqlParameter("@UpdatedBy", model.UpdatedBy)
                };

                return _DbWorker.ExecuteNonQuery(StoreProceduresName.SP_UpdateHotelRoom, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelRoom" + ex);
                return -1;
            }
        }
        public int DeleteHotelRoom(long room_id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@RoomId", room_id),
                };

                return _DbWorker.ExecuteNonQuery(StoreProceduresName.SP_DeleteHotelRoom, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteHotelRoom - HotelDAL. " + ex);
                return -1;
            }
        }
        public async Task<ENTITIES.Models.HotelRoom> GetHotelRoomByRoomCode(string room_code)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var hotel = await _DbContext.HotelRoom.AsNoTracking().Where(x => x.RoomId == room_code).FirstOrDefaultAsync();
                    if (hotel == null || hotel.Id <= 0)
                    {
                        return null;
                    }
                    return hotel;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelDetail - GetByHotelId. " + ex);
                return null;
            }
        }
        public async Task<ENTITIES.Models.HotelRoom> GetHotelRoomByRoomName(string name)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var hotel = await _DbContext.HotelRoom.AsNoTracking().Where(x => x.Name == name).FirstOrDefaultAsync();
                    if (hotel == null || hotel.Id <= 0)
                    {
                        return null;
                    }
                    return hotel;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelDetail - GetByHotelId. " + ex);
                return null;
            }
        }
        public async Task<int> SummitHotelDetail(HotelSummit model)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    if (model.hotel_detail.hotel.Id <= 0)
                    {
                        model.hotel_detail.hotel.Id = CreateHotel(model.hotel_detail.hotel);
                    }
                    else
                    {
                        UpdateHotel(model.hotel_detail.hotel);
                    }
                    foreach(var room in model.hotel_detail.rooms)
                    {
                        if (room.HotelId <= 0) room.HotelId = model.hotel_detail.hotel.Id;

                    }
                }
                return model.hotel_detail.hotel.Id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SummitHotelDetail - GetByHotelId. " + ex);
                return -1;
            }
        }
        public async Task<ENTITIES.Models.Hotel> GetHotelContainRoomid(int room_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var hotel_room = await _DbContext.HotelRoom.AsNoTracking().Where(x => x.Id == room_id).FirstOrDefaultAsync();
                    if (hotel_room == null || hotel_room.Id <= 0)
                    {
                    }
                    else
                    {
                        var hotel = await _DbContext.Hotel.AsNoTracking().Where(x => x.Id == hotel_room.HotelId).FirstOrDefaultAsync();
                        return hotel;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelDetail - HotelDAL. " + ex);
                return null;
            }
            return null;

        }
        public DataTable GetHotelRoomByHotelId(int hotel_id, int page_index, int page_size)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@HotelId",hotel_id),
                    new SqlParameter("@PageIndex",page_index),
                    new SqlParameter("@PageSize",page_size)
                };

                return _DbWorker.GetDataTable(StoreProceduresName.SP_GetHotelRoomByHotelId, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateSupplierRoom - HotelDAL. " + ex);
                throw;
            }
        }
        public async Task<List<ENTITIES.Models.Hotel>> GetByType(bool isVinHotel=false)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var hotel = await _DbContext.Hotel.AsNoTracking().Where(x => x.IsVinHotel == isVinHotel).ToListAsync();
                    if (hotel == null || hotel.Count <= 0)
                    {
                        return null;
                    }
                    return hotel;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByType - GetByHotelId. " + ex);
                return null;
            }
        }
    }
}
