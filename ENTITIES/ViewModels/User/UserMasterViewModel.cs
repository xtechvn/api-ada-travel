using System;

namespace ENTITIES.ViewModels.User
{
    public  class UserMasterViewModel
    {
        public int Id { get; set; }        
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string ResetPassword { get; set; }
        public string Phone { get; set; }
        public DateTime? BirthDay { get; set; }
        public int? Gender { get; set; }
        public string Email { get; set; }
        public string Avata { get; set; }
        public string Address { get; set; }
        public int Status { get; set; }
        public string Note { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string CompanyType { get; set; }
        public bool IsActive2Fa { get; set; }
        public string CompanyDeactiveType { get; set; }        
    }
}
