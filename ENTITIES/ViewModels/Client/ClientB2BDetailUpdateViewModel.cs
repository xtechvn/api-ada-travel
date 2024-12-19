using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Client
{
    public class ClientB2BDetailUpdateViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public int client_type { get; set; }
        public string client_type_name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string indentifer_no { get; set; }
        public string country { get; set; }
        public string provinced_id { get; set; }
        public string district_id { get; set; }
        public string ward_id { get; set; }
        public string address { get; set; }
        public string account_number { get; set; }
        public string account_name { get; set; }
        public string bank_name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int GroupPermission { get; set; }
        public int Status { get; set; }
        public int ParentId { get; set; }
 
    }
    public class AccountClientB2BViewModel
    {
        public int id { get; set; }
        public string ClientName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PasswordBackup { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public int ClientType { get; set; }
        public int AccountId { get; set; }
    }
}
