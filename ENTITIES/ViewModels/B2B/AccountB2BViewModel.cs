using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.B2B
{
   public class AccountB2BViewModel
    {
        public int id { get; set; }
        public string ClientName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PasswordBackup { get; set; }
        public string ClientCode { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public int ClientType { get; set; }
        public int AccountId { get; set; }
        public int ParentId { get; set; }
    }
    public class ListAccountClientModel
    {
        public int Id { get; set; }
        public string Avartar { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string ClientName { get; set; }
        public string StatusName { get; set; }
        public string UserName { get; set; }
        public string GroupPermissionName { get; set; }
        public int Status { get; set; }
        public int TotalRow { get; set; }
    }
}
