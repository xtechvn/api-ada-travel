using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.ViewModels.Order
{
    public class GetListOrdersResponseModel
    {
        /*
         o.OrderId ,o.OrderNo,  o.StartDate, o.EndDate, pm.Amount as Payment, o.Amount,  
	ac.Description as StatusName, ac.CodeValue as StatusCode, o.CreateTime as CreatedDate, acl2.FullName as CreateName, acl.FullName as  UpdateName,
	isnull(ca2.value,'') OperatorIdName,o.UpdateLast,
	count(*) over()  AS  TotalRow
         
         */
        public long OrderId { get; set; }
        public string OrderNo { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? Payment { get; set; }
        public double? Amount { get; set; }
        public string StatusName { get; set; }
        public string CreateName { get; set; }
        public string UpdateName { get; set; }
        public string OperatorIdName { get; set; }
        public int? StatusCode { get; set; }
        public int? PaymentStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdateLast { get; set; }
        public int TotalRow { get; set; }

    }
}
