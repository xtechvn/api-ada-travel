﻿using ENTITIES.ViewModels.Notify;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL.MongoDB.Notify
{
    public class NotifyMongoDAL
    {
        private IMongoCollection<MessageViewModel> messageCollection;
        private IMongoCollection<ReceiverMessageViewModel> receiverCollection;
        //Gọi phương thức GetDatabase() trên MongoClient và chỉ định tên cơ sở dữ liệu để kết nối(FirstDatabase trong trường hợp này)
        //GetDatabase trả về đối tượng .IMongoDatabase.Tiếp theo bookingCollection collection được truy suất sử dụng phương thức GetCollection của IMongoDatabase.
        public NotifyMongoDAL(IConfiguration configuration)
        {
            try
            {
                //mongodb://adavigolog_writer:adavigolog_2022@103.163.216.42:27017/?authSource=HoanBds
                string url = "mongodb://" + configuration["DataBaseConfig:MongoServer:user"] +
                    ":" + configuration["DataBaseConfig:MongoServer:pwd"] +
                    "@" + configuration["DataBaseConfig:MongoServer:Host"] +
                    ":" + configuration["DataBaseConfig:MongoServer:Port"] +
                    "/?authSource=" + configuration["DataBaseConfig:MongoServer:catalog_core"] + "";

                var client = new MongoClient(url);
                IMongoDatabase db = client.GetDatabase(configuration["DataBaseConfig:MongoServer:catalog_core"]);
                messageCollection = db.GetCollection<MessageViewModel>("MessageNotify");
                receiverCollection = db.GetCollection<ReceiverMessageViewModel>("ReceiverNotify");
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DAL.MongoDB.Notify - NotifyDAL: " + ex);
            }
        }

        public async Task<string> pushMessage(MessageViewModel data)
        {
            try
            {
                data.GenID();
                await messageCollection.InsertOneAsync(data);
                return data._id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("NotifyDAL - pushMessage: " + ex);
                return null;
            }
        }

        public async Task<string> pushReceiverReadMessage(ReceiverMessageViewModel data)
        {
            try
            {
                data.GenID();
                await receiverCollection.InsertOneAsync(data);
                return data._id.ToString();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("NotifyDAL - pushReceiverReadMessage: " + ex);
                return null;
            }
        }

        /// <summary>
        ///  trạng thái xem notify 0: chua xem, 1: da xem tong quan, 2 da xem detail
        ///  List<int> idsToUpdate = new List<int> { 1, 2, 3 };
        /// </summary>
        /// <param name="id"></param>
        /// <param name="seen_status"></param>
        /// <returns></returns>
        public async Task<bool> updateSeenNotify(List<string> notify_id, int seen_status, int user_seen_id)
        {
            try
            {

                var filter1 = Builders<ReceiverMessageViewModel>.Filter.In("notify_id", new BsonArray(notify_id));
                var filter2 = Builders<ReceiverMessageViewModel>.Filter.Eq("user_receiver_id", user_seen_id);

                var combinedFilter = Builders<ReceiverMessageViewModel>.Filter.And(filter1, filter2);

                // Khi sử dụng toán tử And, cả hai điều kiện filter1 và filter2 phải đồng thời đúng để tài liệu phù hợp.

                // Tiếp theo, bạn có thể sử dụng combinedFilter trong truy vấn cập nhật (UpdateOne, UpdateMany, ...):
                var update_def = Builders<ReceiverMessageViewModel>.Update.Set("seen_status", seen_status);

                // Ví dụ: Cập nhật một document phù hợp với hai điều kiện filter1 và filter2
                var result = await receiverCollection.UpdateManyAsync(combinedFilter, update_def);


                //var update_def = Builders<ReceiverMessageViewModel>.Update.Set("seen_status", seen_status);
                //var result = await receiverCollection.UpdateManyAsync(filter, update_def);
                if (result.IsAcknowledged)
                {
                    return true;
                }
                else
                {
                    Utilities.LogHelper.InsertLogTelegram("NotifyDAL - updateSeenNotify with id  " + notify_id + " update error ");
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("NotifyDAL - updateSeenNotify: " + ex);
                return false;
            }
        }

        public async Task<NotifySummeryViewModel> getListNotify(int user_id_send)
        {
            try
            {
                var filter = Builders<ReceiverMessageViewModel>.Filter.Where(x =>x.company_type == 0 && x.user_receiver_id == user_id_send && (x.seen_status == (Int16)SeenType.NOT_SEEN || x.seen_status == (Int16)SeenType.SEEN_ALL));
                var lst_noti = await receiverCollection.Find(filter).ToListAsync();

                // Lọc ra những tin chưa dc đọc lần nào
                var lst_not_seen = lst_noti.FindAll(x => x.seen_status == (Int16)SeenType.NOT_SEEN);

                // Lấy ra id nhóm tin chưa đọc
                var id_list_not_seen = lst_not_seen.Select(x => x.notify_id).ToList();

                var noti = new NotifySummeryViewModel
                {
                    total_not_seen = id_list_not_seen.Count, // Tổng số tin chưa xem
                    lst_id_not_seen = string.Join(",", id_list_not_seen), //danh sách id tin chưa xem lần nào
                    lst_not_seen_detail = lst_noti.OrderByDescending(x => x.seen_date).Skip(0).Take(50).ToList()  // ds tin chưa xem lần nào và nhưng tin đã xem
                };
                return noti;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("NotifyDAL - getListNotify: " + ex);
                return null;
            }
        }

        public async Task<NotifySummeryViewModel> getListNotifyPage(int user_id_send, int pageIndex, int pageSize)
        {
            try
            {
                var filter = Builders<ReceiverMessageViewModel>.Filter.Where(x => x.company_type == 0 && x.user_receiver_id == user_id_send && (x.seen_status == (Int16)SeenType.NOT_SEEN || x.seen_status == (Int16)SeenType.SEEN_ALL));
                var lst_noti = await receiverCollection.Find(filter).Sort(Builders<ReceiverMessageViewModel >.Sort.Descending("seen_date")).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync();

                // Lọc ra những tin chưa dc đọc lần nào
                var lst_not_seen = lst_noti.FindAll(x => x.seen_status == (Int16)SeenType.NOT_SEEN);

                // Lấy ra id nhóm tin chưa đọc
                var id_list_not_seen = lst_not_seen.Select(x => x.notify_id).ToList();

                var noti = new NotifySummeryViewModel
                {
                    total_not_seen = id_list_not_seen.Count, // Tổng số tin chưa xem
                    lst_id_not_seen = string.Join(",", id_list_not_seen), //danh sách id tin chưa xem lần nào
                    //lst_not_seen_detail = lst_noti.OrderByDescending(x => x.seen_date).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()  // ds tin chưa xem lần nào và nhưng tin đã xem
                    lst_not_seen_detail = lst_noti.ToList()  // ds tin chưa xem lần nào và nhưng tin đã xem
                };
                return noti;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("NotifyDAL - getListNotifyPage: " + ex);
                return null;
            }
        }

    }
}
