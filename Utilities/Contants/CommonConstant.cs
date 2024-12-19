namespace Utilities.Contants
{
    public class CommonConstant
    {
        public enum CommonStatus
        {
            ACTIVE = 1,
            INACTIVE = 0
        }
        public enum CommonGender
        {
            MALE = 1,
            FEMALE = 0
        }
        public enum FlyBookingDetailType
        {
            GO = 0, // chiều đi
            BACK = 1 // chiều về
        }
        public const string PersonType_ADULT = "ADT";
        public const string PersonType_CHILDREN = "CHD";
        public const string PersonType_INFANT = "INF";
    }
}
