using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.APPModels.PushHotel;
using ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{
    public class RoomFunDAL : GenericService<RoomFun>
    {
        private static DbWorker _DbWorker;
        public RoomFunDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public async Task<int> CreateOrUpdateRoomFun(HotelContract detail)
        {
            int result = (int)ResponseType.FAILED;
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var exists = _DbContext.RoomFun.Where(x => x.AllotmentId == detail.contract.AllotmentId && x.HotelId == detail.contract.HotelId).FirstOrDefault();
                    if(exists!=null && exists.Id > 0)
                    {
                        detail.contract.Id = exists.Id;
                    }
                    else
                    {
                        _DbContext.RoomFun.Add(detail.contract);
                        _DbContext.SaveChanges();
                    }
                    if (detail.contract.Id > 0 && detail.packages_list !=null && detail.packages_list.Count>0)
                    {
                        var remove_packages_list = _DbContext.RoomPackage.Where(x => x.RoomFunId == detail.contract.Id).ToList();

                        foreach (var package in detail.packages_list)
                        {
                            var exists_package = _DbContext.RoomPackage.Where(x => x.PackageId == package.package.PackageId && x.Code == package.package.Code && x.RoomFunId == detail.contract.Id).FirstOrDefault();
                            if (exists_package != null && exists_package.Id > 0)
                            {
                                remove_packages_list.Remove(exists_package);
                                package.package.Id = exists_package.Id;
                            }
                            else
                            {
                                package.package.RoomFunId = detail.contract.Id;
                                var descripton_correct = package.package.Description;
                                package.package.Description = descripton_correct.Substring(0, descripton_correct.Length<=50? descripton_correct.Length: 50);
                                _DbContext.RoomPackage.Add(package.package);
                                _DbContext.SaveChanges();
                            }
                            if (package.package.Id > 0 && package.room_list != null && package.room_list.Count > 0)
                            {
                                var remove_price_rooms = _DbContext.ServicePiceRoom.Where(x => x.RoomPackageId == package.package.Id).ToList();
                                foreach (var room in package.room_list)
                                {
                                    room.RoomPackageId = package.package.Id;
                                    var exists_room = _DbContext.ServicePiceRoom.Where(x => x.RoomId==room.RoomId &&x.RoomPackageId==package.package.Id).FirstOrDefault();
                                    if (exists_room != null && exists_room.Id > 0)
                                    {
                                        remove_price_rooms.Remove(exists_room);
                                        exists_room.RoomName = room.RoomName;
                                        exists_room.RoomCode = room.RoomCode;
                                        exists_room.Price = room.Price>0? room.Price: exists_room.Price;
                                        exists_room.Discount = room.Discount;
                                        _DbContext.ServicePiceRoom.Update(exists_room);
                                    }
                                    else
                                    {
                                        room.RoomPackageId = package.package.Id;
                                        _DbContext.ServicePiceRoom.Add(room);
                                    }
                                }
                                _DbContext.SaveChanges();
                                /*
                                foreach (var removed_room in remove_price_rooms)
                                {
                                    _DbContext.ServicePiceRoom.Remove(removed_room);
                                }
                                _DbContext.SaveChanges();*/
                            }
                        }
                        /*
                        foreach(var removed in remove_packages_list)
                        {
                            var remove_price_rooms = _DbContext.ServicePiceRoom.Where(x =>x.RoomPackageId == removed.Id).ToList();
                            foreach (var removed_room in remove_price_rooms)
                            {
                                _DbContext.ServicePiceRoom.Remove(removed_room);
                            }
                            _DbContext.SaveChanges();
                            _DbContext.RoomPackage.Remove(removed);
                        }
                        _DbContext.SaveChanges();*/
                    }
                }
                result = (int)ResponseType.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateOrUpdateRoomFun - RoomFunDAL: " + ex);
                result = (int)ResponseType.ERROR;
            }
            return result;
        }
      
        public string GetNameByID(string allotment_id)
        {
            using (var _DbContext = new EntityDataContext(_connection))
            {
                var find = _DbContext.RoomFun.Where(x => x.AllotmentId == allotment_id).FirstOrDefault();
                if (find != null && find.Id > 0)
                {
                    return find.AllotmentName;
                }
                else return "";
            }
        }
        public async Task<int> CheckContractQuanity(HotelContract detail)
        {
            int result= (int)ResponseType.FAILED;
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    if (detail.contract.Id > 0 && detail.packages_list != null && detail.packages_list.Count > 0)
                    {
                        var remove_packages_list = _DbContext.RoomPackage.Where(x => x.RoomFunId == detail.contract.Id).ToList();
                        foreach (var package in detail.packages_list)
                        {
                            if (package.package.Id > 0 && package.room_list != null && package.room_list.Count > 0)
                            {
                                foreach (var room in package.room_list)
                                {

                                }
                            }
                        }
                    }
                }
                    
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateOrUpdateRoomFun - RoomFunDAL: " + ex);
                result = (int)ResponseType.ERROR;
            }
            return result;
        }
    }
}
