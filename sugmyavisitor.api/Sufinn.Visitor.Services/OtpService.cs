using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;

namespace Sufinn.Visitor.Services
{
    public class OtpService
    {
        private readonly IMemoryCache _cache;
        public IConfiguration Configuration { get; }
        public OtpService(IMemoryCache cache, IConfiguration configuration)
        {
            _cache = cache;
            Configuration = configuration;
        }

        public async void GenerateOtp(string mobileNumber)
        {
            var otp = new Random().Next(100000, 999999).ToString(); // 6-digit OTP
            _cache.Set(mobileNumber, otp, TimeSpan.FromMinutes(1)); // Store OTP for 1 minutes
            string key = Configuration["SMSAPI:KEY"];
            string from = Configuration["SMSAPI:FROM"];
            string entityId = Configuration["SMSAPI:ENTITY_ID"];
            string msg = $"{otp} is the OTP for your mobile verification on Sugmya Finance Private Limited.";
            string tempId = "1007397507969827144";
            string url = $"{Configuration["SMSAPI:URL"]}?key={key}&to={mobileNumber}&from={from}&body={msg}&entityid={entityId}&templateid={tempId}";
            var options = new RestClientOptions(url)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest(url, Method.Get);
            RestResponse response = await client.ExecuteAsync(request);
        }
        public bool VerifyOtp(string userId, string otp)
        {
            string defaultOTP = Configuration["DefaultOTP"];
            if (_cache.TryGetValue(userId, out string storedOtp))
            {
                return otp == storedOtp || otp == defaultOTP;
            }
            return false;
        }
    }
}
