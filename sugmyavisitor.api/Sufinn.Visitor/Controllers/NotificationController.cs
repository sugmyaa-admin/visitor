using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Threading.Tasks;
using Sufinn.Visitor.Services;
using Sufinn.Visitor.Core.Model;
using Sufinn.Visitor.Core.Common;
using Sufinn.Visitor.Core.Entity;
using System;
using System.IO;
using RestSharp;
using System.Collections.Generic;
using Sufinn.Visitor.Controllers;
using static System.Net.WebRequestMethods;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sufinn.Visitor.Repository.Context;

namespace Sufinn.Visitor.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly TxnController _txnController;
        private readonly IConfiguration _configuration;
        private readonly string _notificationTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Template", "Notification.html");
        private readonly string _empNotificationTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Template", "EmpNotification.html");

        public NotificationController(AppDBContext context, IConfiguration configuration, TxnController txnController)
        {
            _context = context;
            _configuration = configuration;
            _txnController = txnController;
        }
        //Send wait time to visitor 
        [HttpPost("SendWaitTime")]
        public async Task<Result<string>> SendWaitTime([FromQuery] string waitingTime, [FromQuery] string mobileNumber)
        {
            var receptionMail = _configuration.GetSection("TestMailAdmin").Value;
            string ReceptionistName = "Vishal Singh";
            var visitorData = await _txnController.GetVisitorDetails(mobileNumber);
            if (visitorData.Data == null || visitorData.Data.Count == 0)
            {
                return Common<string>.getResponse(false, "visitor does not exist", "");
            }
            var visitorEntities = visitorData.Data;
            if (waitingTime == "10" || waitingTime == "15" || waitingTime == "30")
            {
                var employeeName = visitorData.Data[0].employee_name;
                SendWaitNotification(ReceptionistName, receptionMail, visitorEntities, (waitingTime));
                await SendSms(employeeName, mobileNumber, TimeSpan.FromMinutes(Convert.ToInt32(waitingTime)));
            }
            else if (waitingTime == "getthemin" || waitingTime == "imoutofoffice" || waitingTime == "idontknowthisperson")
            {
                SendWaitNotification(ReceptionistName, receptionMail, visitorEntities, (waitingTime));
            }
            else
            {
                return Common<string>.getResponse(true, "Undefined Messege", null);
            }
            return Common<string>.getResponse(true, "Wait time email sent successfully", "");
        }
        //For Sending Mail 
        [NonAction]
        public async void SendWaitNotification(string name, string mail, List<VisitorEntity> visitorDetail, string waitTime)
        {
            string smtp = _configuration["MAIL:SMTP"];
            string port = _configuration["MAIL:PORT"];
            string from = _configuration["MAIL:FROM"];
            string password = _configuration["MAIL:PASSWORD"];

            var emailMessage = await CreateWaitEmailMessage(name, mail, from, visitorDetail, waitTime);
            await SendWaitEmailAsync(smtp, Convert.ToInt32(port), from, password, emailMessage);
        }

        [NonAction]
        private async Task<MimeMessage> CreateWaitEmailMessage(string name, string to, string from, List<VisitorEntity> visitorDetail, string waitTime)
        {
            string htmlBody = "";
            string subject = "";

            if (waitTime == "getthemin" || waitTime == "imoutofoffice" || waitTime == "idontknowthisperson")
            {
                // Load template for different predefined messages
                htmlBody = await System.IO.File.ReadAllTextAsync(_empNotificationTemplatePath);
                htmlBody = htmlBody.Replace("{employeeName}", visitorDetail[0].employee_name);
                if (waitTime == "getthemin")
                {

                    htmlBody = htmlBody.Replace("{message}", "The visitor has been approved for entry. Please welcome them into the premises. We appreciate your cooperation!");
                }
                else if (waitTime == "imoutofoffice")
                {
                    htmlBody = htmlBody.Replace("{message}", "The Employee is currently unavailable as they are out of the office. Kindly inform the visitor and offer assistance in scheduling an alternative appointment if needed.");
                }
                else if (waitTime == "idontknowthisperson")
                {
                    htmlBody = htmlBody.Replace("{message}", $"The Employee does not recognize this visitor. Please verify the visitor’s details or direct them to the appropriate contact for further assistance.");
                }
                subject = $"Message for the Reception From {visitorDetail[0].employee_name}";
            }
            else if (waitTime == "10" || waitTime == "15" || waitTime == "30")
            {
                // Custom waiting time template for specific wait times
                htmlBody = await System.IO.File.ReadAllTextAsync(_notificationTemplatePath);
                htmlBody = htmlBody.Replace("{visitorName}", visitorDetail?.FirstOrDefault()?.name ?? "Visitor")
                                   .Replace("{waitTime}", waitTime);
                subject = $"Message from the {visitorDetail?.FirstOrDefault()?.employee_name}";
            }

            var emailMessage = new MimeMessage
            {
                From = { new MailboxAddress("System Admin", from) },
                To = { new MailboxAddress(name, to) },
                Subject = subject,
                Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody()
            };

            return emailMessage;
        }

        private async Task SendWaitEmailAsync(string smtpServer, int port, string username, string password, MimeMessage emailMessage)
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(smtpServer, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(username, password);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
        //Function for Sending Sms
        [NonAction]
        public async Task SendSms(string employeeName, string visitorPhoneNumber, TimeSpan waitTime)
        {

            string key = _configuration["SMSAPI:KEY"];
            string from = _configuration["SMSAPI:FROM"];
            string entityId = _configuration["SMSAPI:ENTITY_ID"];
            string msg = $"Dear Visitor, thank you for your patience. {employeeName} has requested that you kindly wait for approximately {waitTime.TotalMinutes + " Minutes"}. We appreciate your understanding. Best regards, Sugmya Finance Pvt. Ltd.";
            string tempId = "1007730751729510869";
            string url = $"{_configuration["SMSAPI:URL"]}?key={key}&to={visitorPhoneNumber}&from={from}&body={msg}&entityid={entityId}&templateid={tempId}";
            var options = new RestClientOptions(url) { MaxTimeout = -1 };
            var client = new RestClient(options);
            var request = new RestRequest(url, Method.Get);
            var response = await client.ExecuteAsync(request);
            if (!response.IsSuccessful)
            {
                throw new Exception($"SMS sending failed: {response.Content}");
            }

        }


    }
}
























//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using MimeKit;
//using MailKit.Net.Smtp;
//using MailKit.Security;
//using System.Threading.Tasks;
//using Sufinn.Visitor.Services;
//using Sufinn.Visitor.Core.Model;
//using Sufinn.Visitor.Core.Common;
//using Sufinn.Visitor.Core.Entity;
//using System;
//using System.IO;
//using RestSharp;
//using System.Collections.Generic;
//using Sufinn.Visitor.Controllers;
//using static System.Net.WebRequestMethods;
//using System.Linq;
//using Microsoft.EntityFrameworkCore;
//using Sufinn.Visitor.Repository.Context;

//namespace Sufinn.Visitor.API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class NotificationController : ControllerBase
//    {
//        private readonly AppDBContext _context;
//        private readonly TxnController _txnController;
//        private readonly IConfiguration _configuration;
//        private readonly string _notificationTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Template", "Notification.html");
//        private readonly string _empNotificationTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Template", "EmpNotification.html");

//        public NotificationController(AppDBContext context, IConfiguration configuration, TxnController txnController)
//        {
//            _context = context;
//            _configuration = configuration;
//            _txnController = txnController;
//        }
//        //Send wait time to visitor 
//        [HttpPost("SendWaitTime")]
//        public async Task<Result<string>> SendWaitTime([FromQuery] string waitingTime, [FromQuery] string mobileNumber)
//        {
//            var receptionMail = _configuration.GetSection("TestMailAdmin").Value;
//            string ReceptionistName = "Vishal Singh";
//            if (waitingTime == "10" || waitingTime == "15" || waitingTime == "30")
//            {
//                var visitorData = await _txnController.GetVisitorDetails(mobileNumber);
//                if (visitorData.Data == null || visitorData.Data.Count == 0)
//                {
//                    return Common<string>.getResponse(false, "visitor does not exist", "");
//                }
//                var visitorEntities = visitorData.Data;
//                var employeeName = visitorData.Data[0].employee_name;

//                SendWaitNotification(ReceptionistName,receptionMail, visitorEntities, (waitingTime));
//                await SendSms(employeeName,mobileNumber, TimeSpan.FromMinutes(Convert.ToInt32(waitingTime)));
//            }
//            else
//            {
//                //var visitorEntity = new VisitorEntity();
//                SendWaitNotification(ReceptionistName, receptionMail, null, (waitingTime));
//            }
//            return Common<string>.getResponse(true, "Wait time email sent successfully", "");
//        }
//        //For Sending Mail 
//        [NonAction]
//        public async void SendWaitNotification(string name, string mail, List<VisitorEntity> visitorDetail, string waitTime)
//        {
//            string smtp = _configuration["MAIL:SMTP"];
//            string port = _configuration["MAIL:PORT"];
//            string from = _configuration["MAIL:FROM"];
//            string password = _configuration["MAIL:PASSWORD"];

//            var emailMessage = await CreateWaitEmailMessage(name, mail, from, visitorDetail, waitTime);
//            await SendWaitEmailAsync(smtp, Convert.ToInt32(port), from, password, emailMessage);
//        }

//        [NonAction]
//        private async Task<MimeMessage> CreateWaitEmailMessage(string name, string to, string from, List<VisitorEntity> visitorDetail, string waitTime)
//        {
//            string htmlBody="";
//            string subject="";

//            if (waitTime == "getthemin" || waitTime == "imoutofoffice" || waitTime == "idontknowthisperson")
//            {
//                // Load template for different predefined messages
//                htmlBody = await System.IO.File.ReadAllTextAsync(_empNotificationTemplatePath);

//                if (waitTime == "getthemin")
//                {
//                    htmlBody = htmlBody.Replace("{message}", "The visitor has been approved for entry. Please welcome them into the premises. We appreciate your cooperation!");
//                }
//                else if (waitTime == "imoutofoffice")
//                {
//                    htmlBody = htmlBody.Replace("{message}", "The Employee is currently unavailable as they are out of the office. Kindly inform the visitor and offer assistance in scheduling an alternative appointment if needed.");
//                }
//                else if (waitTime == "idontknowthisperson")
//                {
//                    htmlBody = htmlBody.Replace("{message}", $"The Employee does not recognize this visitor. Please verify the visitor’s details or direct them to the appropriate contact for further assistance.");
//                }
//                subject = $"Message for the Reception";
//            }
//            else if(waitTime == "10" || waitTime == "15" || waitTime == "30")
//            {
//                // Custom waiting time template for specific wait times
//                htmlBody = await System.IO.File.ReadAllTextAsync(_notificationTemplatePath);
//                htmlBody = htmlBody.Replace("{visitorName}", visitorDetail?.FirstOrDefault()?.name ?? "Visitor")
//                                   .Replace("{waitTime}", waitTime);
//                subject = $"Message from the {visitorDetail?.FirstOrDefault()?.employee_name}";
//            }

//            var emailMessage = new MimeMessage
//            {
//                From = { new MailboxAddress("System Admin", from) },
//                To = { new MailboxAddress(name, to) },
//                Subject = subject,
//                Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody()
//            };

//            return emailMessage;
//        }

//        private async Task SendWaitEmailAsync(string smtpServer, int port, string username, string password, MimeMessage emailMessage)
//        {
//            using var client = new SmtpClient();
//            await client.ConnectAsync(smtpServer, port, SecureSocketOptions.StartTls);
//            await client.AuthenticateAsync(username, password);
//            await client.SendAsync(emailMessage);
//            await client.DisconnectAsync(true);
//        }
//        //Function for Sending Sms
//        [NonAction]
//        public async Task SendSms(string employeeName,string visitorPhoneNumber, TimeSpan waitTime)
//        {

//            string key = _configuration["SMSAPI:KEY"];
//            string from = _configuration["SMSAPI:FROM"];
//            string entityId = _configuration["SMSAPI:ENTITY_ID"];
//            string msg = $"Dear Visitor, thank you for your patience. {employeeName} has requested that you kindly wait for approximately {waitTime.TotalMinutes+" Minutes"}. We appreciate your understanding. Best regards, Sugmya Finance Pvt. Ltd.";
//            string tempId = "1007730751729510869";
//            string url = $"{_configuration["SMSAPI:URL"]}?key={key}&to={visitorPhoneNumber}&from={from}&body={msg}&entityid={entityId}&templateid={tempId}";
//            var options = new RestClientOptions(url) { MaxTimeout = -1 };
//            var client = new RestClient(options);
//            var request = new RestRequest(url, Method.Get);
//            var response = await client.ExecuteAsync(request);
//            if (!response.IsSuccessful)
//            {
//                throw new Exception($"SMS sending failed: {response.Content}");
//            }

//        }


//    }
//}

