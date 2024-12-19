using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Contants
{
    public enum PriceServiceType
    {
        ROOM_VIN = 1, // cách tính giá tiền phòng cho bên VIN
        ROOM_MANUAL = 2, // cách tính giá tiền cho phòng với đối tác phải nhập tay lên hệ thống
        FLYING_TICKET = 3,
        VEHICLE_RENT=4,
        VINWONDER=5,
        SAFARI=6,
        TOURIST=7
    }
}
