using System.ComponentModel;

namespace Utilities.Contants
{
    // Trạng thái đơn
    public enum OrderStatus
    {
        [Description("Tạo mới")]
        CREATED_ORDER = 0,

        [Description("Nhận triển khai")]
        CONFIRMED_SALE = 1,

        [Description("Chờ điều hành duyệt")]
        WAITING_FOR_OPERATOR = 2,

        [Description("Điều hành từ chối")]
        OPERATOR_DECLINE = 3,

        [Description("Chờ kế toán duyệt")]
        WAITING_FOR_ACCOUNTANT = 4,

        [Description("Kế toán từ chối")]
        ACCOUNTANT_DECLINE = 5,

        [Description("Hoàn thành")]
        FINISHED = 6,

        [Description("Hủy")]
        CANCEL = 7,
    }
}
