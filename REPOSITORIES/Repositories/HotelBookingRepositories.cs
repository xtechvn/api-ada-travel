using Caching.Elasticsearch;
using DAL;
using DAL.Hotel;
using Entities.ConfigModels;
using ENTITIES.Models;
using ENTITIES.ViewModels;
using ENTITIES.ViewModels.HotelBooking;
using ENTITIES.ViewModels.HotelBookingRoom;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace Repositories.Repositories
{
    public class HotelBookingRepositories : IHotelBookingRepositories
    {

        private readonly HotelBookingDAL _hotelBookingDAL;
        private readonly ClientDAL _clientDAL;
        private HotelESRepository _hotelESRepository;
        private IConfiguration _configuration;


        public HotelBookingRepositories(IOptions<DataBaseConfig> dataBaseConfig, IConfiguration configuration)
        {
            _configuration = configuration;
            _hotelBookingDAL = new HotelBookingDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _clientDAL = new ClientDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _hotelESRepository = new HotelESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
        }
        public  List<HotelBooking> GetListByOrderId(long OrderId)
        {
            try
            {
                return  _hotelBookingDAL.GetListByOrderId(OrderId);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingByID - HotelBookingRepositories. " + ex);
                return null;
            }
        }
        public async Task<HotelBooking> GetHotelBookingByID(long id)
        {
            try
            {
                return await _hotelBookingDAL.GetHotelBookingByID(id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingByID - HotelBookingRepositories. " + ex);
                return null;
            }
        }
        public async Task<List<HotelBookingDetailViewModel>> GetHotelBookingById(long HotelBookingId)
        {
            var model = new List<HotelBookingDetailViewModel>();
            try
            {
                DataTable dt = await _hotelBookingDAL.GetHotelBookingById(HotelBookingId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    model = dt.ToList<HotelBookingDetailViewModel>();

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingById - HotelBookingRepository: " + ex);
            }
            return model;
        }
        public async Task<ServiceDeclinesViewModel> GetServiceDeclinesByServiceId(string ServiceId, int type)
        {
            try
            {
                DataTable dt = await _hotelBookingDAL.GetServiceDeclinesByServiceId(ServiceId, type);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var model = dt.ToList<ServiceDeclinesViewModel>();
                    return model[0];
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailHotelBookingByID - HotelBookingDAL: " + ex);
            }
            return null;
        }
        public async Task<List<HotelBookingViewModel>> GetDetailHotelBookingByID(long HotelBookingId)
        {

            try
            {
                DataTable dt = await _hotelBookingDAL.GetDetailHotelBookingByID(HotelBookingId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<HotelBookingViewModel>();
                    return data;
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailHotelBookingByID - HotelBookingRepository: " + ex);

            }
            return null;
        }    
        public async Task<List< HotelBookingRooms>> GetHotelBookingRoomsByID(long id)
        {
            
            try
            {
                    return await _hotelBookingDAL.GetHotelBookingRoomsByID(id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingRoomsByID - HotelBookingRepository: " + ex);

            }
            return null;
        }
        public int CreateHotelBooking(HotelBooking booking)
        {
            try
            {
                return  _hotelBookingDAL.CreateHotelBooking(booking);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateHotelBooking - HotelBookingRepository: " + ex);

            }
            return -1;
        }
        public int CreateHotelBookingRooms(HotelBookingRooms booking)
        {
            try
            {
                return _hotelBookingDAL.CreateHotelBookingRooms(booking);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateHotelBooking - HotelBookingRepository: " + ex);

            }
            return -1;
        }
        public int CreateHotelBookingRoomRates(HotelBookingRoomRates booking)
        {
            try
            {
                return _hotelBookingDAL.CreateHotelBookingRoomRates(booking);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateHotelBooking - HotelBookingRepository: " + ex);

            }
            return -1;
        }

        public async Task<List<HotelBookingRoomExtraPackages>> GetListHotelBookingRoomsExtraPackageByBookingId(long HotelBookingId)
        {
            var model = new List<HotelBookingRoomExtraPackages>();
            try
            {
                DataTable dt = await _hotelBookingDAL.GetListHotelBookingRoomsExtraPackageByBookingId(HotelBookingId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    model = dt.ToList<HotelBookingRoomExtraPackages>();

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingById - HotelBookingRepository: " + ex);
            }
            return model;
        }
        public async Task<List<HotelBookingRooms>> GetHotelBookingRoomsByHotelBookingID(long HotelBookingId)
        {
            try
            {
                return await _hotelBookingDAL.GetHotelBookingRoomsByHotelBookingID(HotelBookingId);
              
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingById - HotelBookingRepository: " + ex);
            }
            return null;
        }
        public async Task<List<HotelBookingRoomRates>> GetHotelBookingRoomRatesByBookingRoomsRateByHotelBookingID(long HotelBookingId)
        {
          
            try
            {
                return await _hotelBookingDAL.GetHotelBookingRoomRatesByBookingRoomsRateByHotelBookingID(HotelBookingId);
       
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingRoomRatesByBookingRoomsRateByHotelBookingID - HotelBookingRepository: " + ex);
            }
            return null;
        }
        public async Task<List<HotelBookingsRoomOptionalViewModel>> GetHotelBookingOptionalListByHotelBookingId(long hotelBookingId)
        {
            var model = new List<HotelBookingsRoomOptionalViewModel>();
            try
            {

                DataTable dt = await _hotelBookingDAL.GetHotelBookingRoomsOptionalByBookingId(hotelBookingId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    model = dt.ToList<HotelBookingsRoomOptionalViewModel>();
                    return model;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingOptionalListByHotelBookingId - HotelBookingRepository: " + ex);
                return null;
            }
        }
    }
}