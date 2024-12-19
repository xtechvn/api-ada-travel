using ENTITIES.Models;
using System;
using System.IO;
using static Utilities.Contants.CommonConstant;

namespace API_CORE.Service.Mail
{
    public class MailService
    {
        public string GetValueOrderTemplate(Order orderInfo, Client client,
            FlyBookingDetail flyBookingDetail, ContactClient contactClient, FlightSegment flightSegment)
        {

            string workingDirectory = Environment.CurrentDirectory;
            var currentDirectory = Directory.GetParent(workingDirectory);
            var template = currentDirectory + @"\Utilities\MailTemplate\B2C\OrderMailTemplate.html";
            var subject = File.ReadAllText(template);
            //thông tin order
            subject = subject.Replace("{{orderNo}}", orderInfo.OrderNo);
            var subtractDate = DateTime.Now - flyBookingDetail.ExpiryDate;
            subject = subject.Replace("{{keepTicketTime}}", subtractDate.Hours + "h" + subtractDate.Minutes + "m");
            subject = subject.Replace("{{orderDate}}", orderInfo.CreateTime.Value.ToString("dd/MM/yyyy HH:mm:ss"));

            //thông tin khách hàng
            subject = subject.Replace("{{customerName}}", client?.ClientName);
            subject = subject.Replace("{{phone}}", contactClient?.Mobile);
            subject = subject.Replace("{{email}}", contactClient?.Email);
            subject = subject.Replace("{{gender}}", client?.Gender == (int)CommonGender.FEMALE ? "Nữ" : "Nam");

            //thông tin chuyến bay đi
            subject = subject.Replace("{{handBaggage}}", flightSegment.HandBaggage + "kg");//hành lý chiều đi
            subject = subject.Replace("{{allowanceBaggage}}", flightSegment.AllowanceBaggage + "kg");//hành lý chiều về

            subject = subject.Replace("{{flyOrderNo}}", flyBookingDetail.BookingCode);
            subject = subject.Replace("{{flyCode}}", flightSegment.FlightNumber);//chưa mapping

            subject = subject.Replace("{{flyName}}", flyBookingDetail.Airline);
            subject = subject.Replace("{{dayGo}}", flightSegment.StartTime != null ?
                GetDay(flightSegment.StartTime.DayOfWeek) : "");
            subject = subject.Replace("{{dateGo}}", flightSegment.StartTime != null ?
                flightSegment.StartTime.ToString("dd/MM/yyyy") : "");
            subject = subject.Replace("{{addressFrom}}", flightSegment.StartPoint);//HAN - Hà Nội
            subject = subject.Replace("{{addressTo}}", flightSegment.EndPoint);// PQC - Phú Quốc
            subject = subject.Replace("{{flyTicketClass}}", flightSegment.Class);
            subject = subject.Replace("{{timeFromGo}}", flightSegment.StartTime.ToString("HH:mm"));//10:40
            subject = subject.Replace("{{timeToGo}}", flightSegment.EndTime.ToString("HH:mm"));//22:45
            //thông tin chuyến bay về
            subject = subject.Replace("{{dayBack}}", flightSegment.EndTime != null ?
                GetDay(flightSegment.EndTime.DayOfWeek) : "");
            subject = subject.Replace("{{dateBack}}", flightSegment.EndTime != null ?
                flightSegment.EndTime.ToString("dd/MM/yyyy") : "");
            subject = subject.Replace("{{timeFromBack}}", flightSegment.StartTime.ToString("HH:mm"));//10:40
            subject = subject.Replace("{{timeToBack}}", flightSegment.EndTime.ToString("HH:mm"));//22:45

            //số tiền thanh toán
            subject = subject.Replace("{{total}}", flyBookingDetail.Amount.ToString());
            //số tiền cần thanh toán
            subject = subject.Replace("{{amount}}", flyBookingDetail.Amount.ToString());
            //link thanh toán
            subject = subject.Replace("{{payLink}}", "");
            //link thanh toán xong
            subject = subject.Replace("{{payLinkDone}}", "");

            return subject;
        }

        public string GetDay(DayOfWeek day)
        {
            var dayStr = String.Empty;
            if (day == DayOfWeek.Monday)
            {
                dayStr = "Thứ 2";
            }
            if (day == DayOfWeek.Tuesday)
            {
                dayStr = "Thứ 3";
            }
            if (day == DayOfWeek.Wednesday)
            {
                dayStr = "Thứ 4";
            }
            if (day == DayOfWeek.Thursday)
            {
                dayStr = "Thứ 5";
            }
            if (day == DayOfWeek.Friday)
            {
                dayStr = "Thứ 6";
            }
            if (day == DayOfWeek.Saturday)
            {
                dayStr = "Thứ 7";
            }
            if (day == DayOfWeek.Sunday)
            {
                dayStr = "Chủ nhật";
            }
            return dayStr;
        }
    }
}
