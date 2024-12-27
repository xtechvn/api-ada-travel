using DAL;
using Entities.ConfigModels;
using ENTITIES.Models;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Utilities;

namespace Repositories.Repositories
{
    public class HotelBookingRoomExtraPackageRepository : IHotelBookingRoomExtraPackageRepository
    {
        
        private readonly HotelBookingRoomExtraPackagesDAL _hotelBookingRoomExtraPackagesDAL;

        public HotelBookingRoomExtraPackageRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {

            _hotelBookingRoomExtraPackagesDAL = new HotelBookingRoomExtraPackagesDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }

        public int CreateHotelBookingRoomExtraPackages(HotelBookingRoomExtraPackages packages)
        {
            return _hotelBookingRoomExtraPackagesDAL.CreateHotelBookingRoomExtraPackages(packages);
        }
        public int UpdateHotelBookingExtraPackagesSP(HotelBookingRoomExtraPackages packages)
        {
            return _hotelBookingRoomExtraPackagesDAL.UpdateHotelBookingExtraPackagesSP(packages);
        }
      
    }
}
