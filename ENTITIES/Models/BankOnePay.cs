using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class BankOnePay
    {
        public int Id { get; set; }
        public string BankName { get; set; }
        public string Code { get; set; }
        public byte Type { get; set; }
        public string Logo { get; set; }
        public byte? Status { get; set; }
        public string FullnameEn { get; set; }
        public string FullnameVi { get; set; }
    }
}
