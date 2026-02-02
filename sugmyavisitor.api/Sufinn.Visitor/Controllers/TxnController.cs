using MailKit;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Npgsql;
using Sufinn.Visitor.Core.Common;
using Sufinn.Visitor.Core.Entity;
using Sufinn.Visitor.Core.Model;
using Sufinn.Visitor.Repository.Context;
using Sufinn.Visitor.Repository.Interface;
using Sufinn.Visitor.Services.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace Sufinn.Visitor.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TxnController : ControllerBase
    {
        private readonly IBaseService _service;
        private readonly AppDBContext _context;
        private readonly IPostgreRepository _repo;
        private readonly string _templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Template", "CheckIn.html");
        private readonly string _checkOutTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Template", "CheckOut.html");
        public IConfiguration Configuration { get; }
        public TxnController(IPostgreRepository repo, IBaseService service, AppDBContext context, IConfiguration configuration)
        {
            _repo = repo;
            _service = service;
            _context = context;
            Configuration = configuration;  
        }
        [HttpPost("saveVisitorDetails")]
        public async Task<Result<string>> SaveVisitorDetails(IFormFile file, [FromForm] TxnVisitorDetail visitor)
        {
            try
            {
                var savePath = Configuration.GetSection("Paths:DownloadedPath").Value;
                var extension = Path.GetExtension(file.FileName).ToLower();
                Random rnd = new Random();
                int randomNum = rnd.Next(100000, 999999);
                var fileName = $"Sufinn_{randomNum}{extension}";

                using (var memoryStream = new MemoryStream())
                {
                    if (file != null || file.Length > 0)
                    {
                        await file.CopyToAsync(memoryStream);
                    }
                    if (!Directory.Exists(savePath))
                    {
                        Directory.CreateDirectory(savePath);
                    }

                    var filePath = Path.Combine(savePath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    visitor.CheckIn = DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss");
                    visitor.Picture = fileName;
                    var parameters = new Dictionary<string, object>
                    {
                        { "v_name", visitor.Name ?? (object)DBNull.Value },
                        { "v_email", visitor.Email?? (object)DBNull.Value },
                        { "v_number",Convert.ToInt64(visitor.Number) },
                        { "v_purpose", visitor.Purpose },
                        { "v_whom_to_meet", visitor.WhomToMeet?? (object)DBNull.Value },
                        { "v_company", visitor.Company?? (object)DBNull.Value },
                        { "v_number_of_person", visitor.NumberOfPerson?? (object)DBNull.Value },
                        { "v_check_in", visitor.CheckIn?? (object)DBNull.Value },
                        { "v_check_out", visitor.CheckOut?? (object)DBNull.Value },
                        { "v_force_check_out", visitor.ForceCheckOut?? (object)DBNull.Value },
                        { "v_mode", visitor.Mode?? (object)DBNull.Value },
                        { "v_picture", visitor.Picture ?? (object)DBNull.Value },
                        { "v_last_otp", visitor.LastLoginOtp ?? (object)DBNull.Value },
                    };
                    var resp = await _repo.Save("pr_insert_visitor_info", parameters);

                    if (resp.Status.Success)
                    {
                        var testMode = Convert.ToBoolean(Configuration.GetSection("TestMode").Value);
                        var testMail = Configuration.GetSection("TestMail").Value;
                        var employee = _context.MstPeopleDetails.FirstOrDefault(x => x.EmployeeId == visitor.WhomToMeet.ToString());
                        SendNotiication(employee.EmployeeName, testMode ? testMail : employee.MailId, visitor);
                    }
                    return resp;
                }

            }
            catch (Exception ex)
            {
                return Common<string>.getResponse(false, ex.Message, "");
            }
        }

        [HttpGet("getVisitorDetails/{mobileNumber}")]
        public async Task<Result<List<VisitorEntity>>> GetVisitorDetails(string mobileNumber)
        {
            try
            {
                var picPath = Configuration.GetSection("Paths:DownloadedPath").Value;
                var parameters = new Dictionary<string, object>
                {
                    { "mobile_num",Convert.ToInt64(mobileNumber) }
                };

                var data = await _service.GetRespository<VisitorEntity>().ExecuteStoredProcedureAsync("pr_get_visitor_detail", parameters, true);


                if (data.Count() > 0)
                {
                    var filePath = Path.Combine(picPath, data.FirstOrDefault()?.picture);
                    var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                    string base64String = Convert.ToBase64String(fileBytes);
                    data.FirstOrDefault().picture = base64String;
                    return Common<List<VisitorEntity>>.getResponse(true, "Data found.", data.ToList());
                }

                return Common<List<VisitorEntity>>.getResponse(false, "The user does not exist or check-in may not be completed yet.", null);
            }
            catch (Exception ex)
            {
                return Common<List<VisitorEntity>>.getResponse(false, ex.Message, null);
            }
        }


        [HttpGet("getVisitorDetails")]
        [Authorize]
        public async Task<Result<List<VisitorEntity>>> GetVisitorDetails()
        {
            try
            {
                var virtualPicPath = Configuration.GetSection("Paths:VirtualPath").Value;
                var parameters = new Dictionary<string, object>
                {
                    { "mobile_num", 0}
                };
                var data = await _service.GetRespository<VisitorEntity>().ExecuteStoredProcedureAsync("pr_get_visitor_detail", parameters, true);
                if (data.Count() > 0)
                {
                    data.Where(p => true).ToList().ForEach(p => p.picture = $"{virtualPicPath}{p.picture}");
                    return Common<List<VisitorEntity>>.getResponse(true, "Data found.", data.ToList());
                }
                return Common<List<VisitorEntity>>.getResponse(false, "No data found.", null);
            }
            catch (Exception ex)
            {
                return Common<List<VisitorEntity>>.getResponse(false, ex.Message, null);
            }
        }
        [HttpGet("checkout/{mobileNumber}")]
        public async Task<Result<string>> Checkout(string mobileNumber, int isForceCheckOut = 0, string txnId = "")
        {
            try
            {

                var testMode = Convert.ToBoolean(Configuration.GetSection("TestMode").Value);
                var testMail = Configuration.GetSection("TestMail").Value;
                string checkout = DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss");
                var parameters = new Dictionary<string, object>
                {
                        { "mobile_num",Convert.ToInt64(mobileNumber)},
                        { "check_out_date", checkout },
                        {"is_force_c_out",isForceCheckOut },
                        { "id",txnId != "" ? Convert.ToInt32(txnId) : 0 }

                };
                if (txnId == "")
                {
                    var visitorData = _service.GetRespository<TxnVisitorDetail>().Filter(x => x.Number == Convert.ToInt64(mobileNumber) && x.CheckOut == null).FirstOrDefault();
                    var employeeData = _context.MstPeopleDetails.FirstOrDefault(x => x.EmployeeId == visitorData.WhomToMeet.ToString());
                    visitorData.CheckOut = checkout;
                    SendCheckOutNotiication(visitorData, testMode?testMail: employeeData.MailId);
                }
                else
                {
                    var visitorData = _service.GetRespository<TxnVisitorDetail>().Filter(x => x.TxnId == Convert.ToInt32(txnId)).FirstOrDefault();
                    var employeeData = _context.MstPeopleDetails.FirstOrDefault(x => x.EmployeeId == visitorData.WhomToMeet.ToString());
                    visitorData.CheckOut = checkout;
                    SendCheckOutNotiication(visitorData, testMode ? testMail : employeeData.MailId);
                }
                var resp = await _repo.Save("pr_checkout_visitor", parameters);
                return resp;
            }
            catch (Exception ex)
            {
                return Common<string>.getResponse(false, ex.Message, null);
            }
        }
        [NonAction]
        public async Task<Result<string>> AutoCheckout()
        {
            try
            {
                var uncheckedVisitors = _service.GetRespository<TxnVisitorDetail>()
                                                .Filter(x => x.CheckOut == null).ToList();

                foreach (var visitor in uncheckedVisitors)
                {
                    var parameters = new Dictionary<string, object>
                    {
                       { "mobile_num", visitor.Number },
                       { "check_out_date", DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss") },
                       { "is_force_c_out", 0 },
                       { "id", visitor.TxnId }
                    };

                    await _repo.Save("pr_checkout_visitor", parameters);
                }

                return Common<string>.getResponse(true, "Auto checkout completed", null);
            }
            catch (Exception ex)
            {
                return Common<string>.getResponse(false, ex.Message, null);
            }
        }
        //Send mail
        [NonAction]
        public async void SendNotiication(string name, string mail, TxnVisitorDetail visitor)
        {

            string smtp = Configuration["MAIL:SMTP"];
            string port = Configuration["MAIL:PORT"];
            string from = Configuration["MAIL:FROM"];
            string password = Configuration["MAIL:PASSWORD"];

            var emailMessage = await CreateEmailMessage(name, mail, from, visitor);
            await SendEmailAsync(smtp, Convert.ToInt32(port), from, password, emailMessage);
        }
        [NonAction]
        private async Task<MimeMessage> CreateEmailMessage(string name, string to, string from, TxnVisitorDetail visitorDetail)
        {
            var picPath = Configuration.GetSection("Paths:DownloadedPath").Value;
            string filePath = Path.Combine(picPath, visitorDetail.Picture);
            string userImage;

            if (!System.IO.File.Exists(filePath))
            {
                userImage = "Image not found.";
            }
            else
            {
                byte[] imageBytes = System.IO.File.ReadAllBytes(filePath);
                userImage = "data:image/png;base64, " + Convert.ToBase64String(imageBytes);
            }
            string appUrl = Configuration["MAIL:RESPONSEURL"];
            string htmlBody = await System.IO.File.ReadAllTextAsync(_templatePath);
            htmlBody = htmlBody.Replace("{VisitorName}", visitorDetail.Name)
                           .Replace("{userImage}", userImage)
                           .Replace("{CheckInTime}", visitorDetail.CheckIn)
                           .Replace("{VisitorNumber}", visitorDetail.Number.ToString())
                           .Replace("{VisitorEmail}", visitorDetail.Email)
                           .Replace("{VisitorCompany}", visitorDetail.Company)
                           .Replace("{VisitorNumberOfPerson}", visitorDetail.NumberOfPerson.ToString())
                           .Replace("{appUrl}", appUrl)
                           .Replace("{mobNum}", visitorDetail.Number.ToString());
            var emailMessage = new MimeMessage
            {
                From = { new MailboxAddress("System Admin", from) },
                To = { new MailboxAddress(name, to) },
                Subject = "Hi! A visitor just checked in to meet you",
                Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody()
            };

            return emailMessage;
        }

        private async Task SendEmailAsync(string smtpServer, int port, string username, string password, MimeMessage emailMessage)
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(smtpServer, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(username, password);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }

        [NonAction]
        public async void SendCheckOutNotiication(TxnVisitorDetail visitorData, string mail)
        {
            string smtp = Configuration["TEMPMAIL:SMTP"];
            string port = Configuration["TEMPMAIL:PORT"];
            string from = Configuration["TEMPMAIL:FROM"];
            string password = Configuration["TEMPMAIL:PASSWORD"];

            var emailMessage = await CreateCheckOutEmailMessage(visitorData, mail, from);
            await SendEmailAsync(smtp, Convert.ToInt32(port), from, password, emailMessage);
        }
        [NonAction]
        private async Task<MimeMessage> CreateCheckOutEmailMessage(TxnVisitorDetail visitorData, string to, string from)
        {
            var picPath = Configuration.GetSection("Paths:DownloadedPath").Value;
            string filePath = Path.Combine(picPath, visitorData.Picture);
            string userImage;
            if (!System.IO.File.Exists(filePath))
            {
                userImage = "Image not found.";
            }
            else
            {
                byte[] imageBytes = System.IO.File.ReadAllBytes(filePath);
                userImage = "data:image/png;base64, " + Convert.ToBase64String(imageBytes);
            }
            string htmlBody = await System.IO.File.ReadAllTextAsync(_checkOutTemplatePath);
            var visitorName = visitorData.Name;
            htmlBody = htmlBody.Replace("{VisitorName}", visitorName)
                       .Replace("{userImage}", (userImage))
                       .Replace("{CheckInTime}", visitorData.CheckIn)
                       .Replace("{CheckOutTime}", visitorData.CheckOut);



            var emailMessage = new MimeMessage
            {
                From = { new MailboxAddress("System Admin", from) },
                To = { new MailboxAddress(visitorData.Name, to) },
                Subject = $"Hi! {visitorName} just checked out after meeting you",
                Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody()
            };

            return emailMessage;
        }

    }
}
