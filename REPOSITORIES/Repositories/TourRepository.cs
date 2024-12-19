using DAL;
using DAL.MongoDB;
using DAL.MongoDB.Hotel;
using Entities.ConfigModels;
using ENTITIES.Models;
using ENTITIES.ViewModels;
using ENTITIES.ViewModels.Hotel;
using ENTITIES.ViewModels.Tour;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Utilities;

namespace Repositories.Repositories
{

    public class TourRepository : ITourRepository
    {
        private readonly TourDAL tourDAL;
        private readonly TourProductDAL tourProductDAL;
        private readonly BookingTourDAL bookingTourDAL;

        private readonly string _UrlStaticImage;

        public TourRepository(IOptions<DataBaseConfig> dataBaseConfig, IOptions<DomainConfig> domainConfig)
        {
            tourDAL = new TourDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            tourProductDAL = new TourProductDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            bookingTourDAL = new BookingTourDAL(dataBaseConfig.Value.MongoServer.connection_string, dataBaseConfig.Value.MongoServer.catalog_core);

            _UrlStaticImage = domainConfig.Value.ImageStatic;
        }

        public async Task<Tour> GetTourById(long tour_id)
        {
            return await tourDAL.GetTourById(tour_id);
        }
        public async Task<TourViewModel> GetDetailTourByID(long TourId)
        {
            try
            {
                DataTable dt = await tourDAL.GetDetailTourByID(TourId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<TourViewModel>();
                    return data[0];
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailTourByID - TourRepository: " + ex.ToString());
                return null;
            }

        }
        public async Task<TourDtailFeViewModel> GetDetailTourFeByID(long TourId)
        {
            try
            {
                DataTable dt = await tourDAL.GetDetailTourByID(TourId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<TourDtailFeViewModel>();
                    return data[0];
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailTourByID - TourRepository: " + ex.ToString());
                return null;
            }

        }
        public async Task<TourProductDetailModel> GetTourProductById(long id)
        {
            try
            {
                return await tourProductDAL.GetTourProductById(id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTourProductById - TourRepository: " + ex);
                return null;
            }
        }
        public async Task<List<ListTourProductViewModel>> GetListTourProduct(string TourType, long pagesize, long pageindex, string StartPoint, string Endpoint,string transportation="")
        {
            try
            {
                return await tourProductDAL.GetListTourProduct( TourType, pagesize  , pageindex, StartPoint, Endpoint, transportation);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListTourProduct - TourRepository: " + ex);
                return null;
            }
        }
        public async Task<List<ListTourProductViewModel>> GetListTourProductPosition(string TourType, long pagesize, long pageindex, string StartPoint, string Endpoint,long PositionType)
        {
            try
            {
                return await tourProductDAL.GetListTourProductPosition(TourType, pagesize, pageindex, StartPoint, Endpoint, PositionType);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListTourProduct - TourRepository: " + ex);
                return null;
            }
        }
        public async Task<List<TourLocationViewModel>> GetLocationById(int tour_type, string s_start_point, string s_end_point)
        {
            try
            {
                return await tourProductDAL.GetLocationById( tour_type,  s_start_point,  s_end_point);
            }
            catch (Exception ex)
            {

                LogHelper.InsertLogTelegram("GetLocationById - TourRepository: " + ex);
                return null;
            }
        }
        public async Task<List<ListTourProductViewModel>> GetListFavoriteTourProduct(int PageIndex, int PageSize)
        {
            try
            {
                return await tourProductDAL.GetListFavoriteTourProduct(PageIndex, PageSize);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListFavoriteTourProduct - TourRepository: " + ex);
                return null;
            }
        }
        public async Task<List<OrderListTourViewModel>> GetListTourByAccountId(long TourId)
        {
            try
            {
                DataTable dt = await tourDAL.GetListTourByAccountId(TourId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<OrderListTourViewModel>();
                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListTourByAccountId - TourRepository: " + ex.ToString());
                return null;
            }

        }
        public async Task<List<TourProgramPackages>> GetListTourProgramPackagesByTourProductId(long id, string client_types = null)
        {
            try
            {
                var dt = await tourProductDAL.GetListTourProgramPackagesByTourProductId(id,client_types);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<TourProgramPackages>();
                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListTourProgramPackagesByTourProductId - TourRepository: " + ex);
                return null;
            }
        }
        public async Task<string> saveBooking(BookingTourMongoViewModel data, string booking_id)
        {
            try
            {
                if (booking_id != null && booking_id.Trim() != "")
                {
                    var exists = bookingTourDAL.GetBookingById(booking_id);
                    if (exists != null && exists._id != null && exists._id.Trim() != "")
                    {
                        return await bookingTourDAL.UpdateBooking(data, booking_id);
                    }
                }
                return await bookingTourDAL.InsertBooking(data);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("saveBooking - TourRepository: " + ex);
            }
            return null;
        }
        public async Task<List<BookingTourMongoViewModel>> getBookingByID(string[] booking_id)
        {
            try
            {
                List<BookingTourMongoViewModel> data = new List<BookingTourMongoViewModel>();

                foreach (var item in booking_id)
                {
                    var a = bookingTourDAL.GetBookingById(item);
                    data.Add(a);
                }
                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<TourProduct> GetDetailTourProductById(long id)
        {
            try
            {
                return await tourProductDAL.FindAsync(id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTourProductById - TourRepository: " + ex);
                return null;
            }
        }
    }
    
}