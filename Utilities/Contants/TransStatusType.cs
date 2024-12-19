namespace Utilities.Contants
{
    public struct TransStatusType
    {
        public const int CREATE_NEW_TRANS = 0; // tao moi trans
        public const int WAITING_VERIFY_PAYMENT = 1; // cho thanh toan
        public const int WAITING_VERIFY_ACCOUNTANT = 2; // cho ke toan duyet
        public const int ACCEPT_VERIFY_ACCOUNTANT = 3; // ke toan duyet
        public const int REJECT_VERIFY_ACCOUNTANT = 4; // ke toan tu choi
    }
}
