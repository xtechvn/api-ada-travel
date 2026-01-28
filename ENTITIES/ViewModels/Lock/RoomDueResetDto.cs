using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.Lock
{
    public class RoomDueResetDto
    {
        public int RoomId { get; set; }
        public long HotelId { get; set; }
        public long LockId { get; set; }
        public string RoomName { get; set; }
        public string RoomCode { get; set; }
        public DateTime LockResetCheckoutAt { get; set; }
    }
    public class AutoCheckoutRunItem
    {
        public int RoomId { get; set; }
        public long HotelId { get; set; }
        public long LockId { get; set; }
        public string RoomCode { get; set; }
        public string RoomName { get; set; }
        public string Status { get; set; }   // OK / SKIP / FAIL
        public string Message { get; set; }
        public int HistoryId { get; set; }
        public string Password { get; set; } // plaintext để n8n gửi mail
    }


}
