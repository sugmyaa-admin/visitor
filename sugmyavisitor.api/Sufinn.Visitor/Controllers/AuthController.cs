using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sufinn.Visitor.Core.Common;
using Sufinn.Visitor.Core.Entity;
using Sufinn.Visitor.Repository;
using Sufinn.Visitor.Repository.Context;
using Sufinn.Visitor.Repository.Interface;
using Sufinn.Visitor.Services;
using Sufinn.Visitor.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufinn.Visitor.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly OtpService _otpService;
        private readonly AppDBContext _context;
        private readonly IAuthRepository _authRepository;
        private readonly IBaseService _service;
        public AuthController(IBaseService service, IAuthRepository authRepository, OtpService otpService, AppDBContext context)
        {
            _authRepository = authRepository;
            _otpService = otpService;
            _context = context;
            _service = service;
        }
        [HttpPost("login")]
        public async Task<Result<string>> Login(LoginEntity auth)
        {
            try
            {
                return await _authRepository.authenticate(auth.loginId, auth.password);
            }
            catch (Exception ex)
            {
                return Common<string>.getResponse(false, ex.Message, "");
            }
        }

        [HttpPost("changePassword")]
        [Authorize]
        public async Task<Result<string>> ChangePassord(LoginEntity auth)
        {
            try
            {
                string loggedInUserId = User.FindFirst("LoginId").Value;
                auth.loginId = Convert.ToInt32(loggedInUserId);
                char operFlag = Convert.ToChar(auth.oprFlag);
                string oldPassword = auth.oldPassword;
                return await _authRepository.changeInPassword(auth.loginId, auth.password, operFlag, oldPassword);
            }
            catch (Exception ex)
            {
                return Common<string>.getResponse(false, ex.Message, "");
            }
        }
        [HttpPost("getOtp")]
        public async Task<Result<string>> GetOtp(AuthEntity auth)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "mobile_num",Convert.ToInt64(auth.mobileNumber) }
                };
                var data = await _service.GetRespository<VisitorEntity>().ExecuteStoredProcedureAsync("pr_get_visitor_detail", parameters, true);
                if (data?.Count() == 0)
                {
                    _otpService.GenerateOtp(auth.mobileNumber);
                    return Common<string>.getResponse(true, "OTP has been sent successfully.", "");
                }
                else
                {
                    return Common<string>.getResponse(false, "The user is already checked in our system. Please complete the checkout process first.", "");
                }
            }
            catch (Exception ex)
            {
                return Common<string>.getResponse(false, ex.Message, "");
            }
        }
        [HttpPost("verify")]
        public Result<string> VerifyOtp([FromBody] AuthEntity request)
        {
            try
            {
                var isValid = _otpService.VerifyOtp(request.mobileNumber, request.otp);
                if (isValid)
                {
                    return Common<string>.getResponse(true, "OTP verified successfully.", "");
                }
                return Common<string>.getResponse(false, "Invalid OTP.", "");
            }
            catch (Exception ex)
            {
                return Common<string>.getResponse(false, ex.Message, "");
            }
        }
        [HttpGet("getEmployeeDetail")]
        [Authorize]
        public async Task<Result<List<LoginData>>> GetEmployeeDetails()
        {
            var loginId = User.Claims.FirstOrDefault(x => x.Type == "LoginId")?.Value;

            var parameters = new Dictionary<string, object>
                {
                    { "pi_login_id", Convert.ToInt32(loginId)}
                };
            var data = await _service.GetRespository<LoginData>().ExecuteStoredProcedureAsync("pr_get_login_detail", parameters, true);
            data.FirstOrDefault().loginId = Convert.ToInt32(loginId);
            return Common<List<LoginData>>.getResponse(true, "Data found.", data.ToList());
        }
        [HttpGet("getSuffinPayQrCode")]
        public async Task<IActionResult> GetSuffinPayQrCode([FromQuery] string loanId)
        {
            try
            {
                return Ok(await Utils.GetSufinnPayQrCode(loanId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}



