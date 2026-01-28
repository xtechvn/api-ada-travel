using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.Lock
{
    public class TTLockListLockResponse
    {
        public List<TTLockLockItem> list { get; set; } = new List<TTLockLockItem>();
    }

    public class TTLockLockItem
    {
        public long lockId { get; set; }
        public int electricQuantity { get; set; }
        public int rssi { get; set; }
        public string lockAlias { get; set; }
        public long updateDate { get; set; }
        public string lockMac { get; set; }
        public string lockName { get; set; }
    }
    public class TTLockSimpleResponse
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public string description { get; set; }
    }

    public class DeviceHierarchyViewModel
    {
        // Root node (nếu bạn muốn show)
        public string RootName { get; set; } = "Main Server Node";
        public string RootIp { get; set; } = "192.168.1.100";

        // Gateway hiện tại
        public long GatewayId { get; set; }
        public string GatewayName { get; set; } = "";
        public string GatewayMac { get; set; } = "";
        public string GatewayStatus { get; set; } = "Online";

        // Locks
        public List<TTLockLockItem> Locks { get; set; } = new List<TTLockLockItem>();

        // Footer stats (optional)
        public int TotalNodes => 1 /*root*/ + 1 /*gateway*/ + (Locks?.Count ?? 0);
        public int ActiveAlerts => Locks?.Count(x => x.electricQuantity <= 20) ?? 0;

        public int AvgSignal
        {
            get
            {
                if (Locks == null || Locks.Count == 0) return 0;
                return (int)Math.Round(Locks.Average(x => x.rssi));
            }
        }
    }

 }
