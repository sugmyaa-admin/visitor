using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Sufinn.Visitor.Controllers;
using Sufinn.Visitor.Core.Entity;
using Sufinn.Visitor.Core.Model;
using Sufinn.Visitor.Middleware;
using Sufinn.Visitor.Repository;
using Sufinn.Visitor.Repository.Context;
using Sufinn.Visitor.Repository.Interface;
using Sufinn.Visitor.Services;
using Sufinn.Visitor.Services.Interface;
using System;
using System.Linq;
using System.Text;

namespace Sufinn.Visitor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private static TimeZoneInfo GetIndiaTimeZone()
        {
            var timeZones = TimeZoneInfo.GetSystemTimeZones();

            if (timeZones.Any(tz => tz.Id == "Asia/Kolkata"))
                return TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata");

            if (timeZones.Any(tz => tz.Id == "India Standard Time"))
                return TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

            return TimeZoneInfo.Utc;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string[] origins = Configuration.GetSection("AllowedOrigins").Value.Split(",");
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder =>
                    {
                        builder.WithOrigins(origins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                    });
            });
            services.AddDbContext<AppDBContext>();
            services.AddSingleton<IJWTManagerRepository, JWTManagerRepository>();
            services.AddTransient<IBaseService, BaseService<AppDBContext>>();
            services.AddTransient<IAuthRepository, AuthRepository>();
            services.AddTransient<IPostgreRepository, PostgreRepository>();
            services.AddScoped<OtpService>();
            services.AddScoped<PeopleService>();
            services.AddScoped<TxnController>();
            services.AddScoped<AutoCheckoutService>();
            services.AddHttpContextAccessor();
            services.AddScoped<ITenantProvider, TenantProvider>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API Documentation",
                    Contact = new OpenApiContact { Name = "Support", Email = "vishal.singh@sufinn.com" },
                    Version = "v1"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:ValidIssuer"],
                    ValidAudience = Configuration["Jwt:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });
            services.AddControllers();
            services.AddRazorPages();
            services.AddMvcCore();
            services.AddMemoryCache();
            services.AddHangfire(configuration =>
            configuration.UseMemoryStorage());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDBContext dbContext, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //dbContext.Database.EnsureCreated();
            app.UseMiddleware<TenantMiddleware>();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UsePathBase("/visitor-api");
            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
            //app.UseHangfireDashboard();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[]
                {
                    new HangfireAuthorization(Configuration.GetSection("HangfireUserName").Value, Configuration.GetSection("HangfirePassword").Value)
                }
            });
            app.UseHangfireServer();
            // Hangfire Job Setup
            using (var scope = serviceProvider.CreateScope())
            {
                var jobRepository = scope.ServiceProvider.GetRequiredService<AutoCheckoutService>();
                RecurringJob.AddOrUpdate<AutoCheckoutService>(
    "auto-cheked-out-job",job => job.AutoCheckout(),Configuration.GetSection("AutoCheckoutJobTime").Value,GetIndiaTimeZone()
);
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/visitor-api/swagger/v1/swagger.json", "My API v1");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
