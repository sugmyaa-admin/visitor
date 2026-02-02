using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sufinn.Visitor.Core.Entity;
using Sufinn.Visitor.Core.Model;
using Sufinn.Visitor.Repository.Context.Interface;
using Sufinn.Visitor.Repository.Interface;
using System;

namespace Sufinn.Visitor.Repository.Context
{
    public partial class AppDBContext : DbContext, IDbContext
    {
        public AppDBContext()
        {
        }

        private readonly string _connectionString;
        public AppDBContext(DbContextOptions<AppDBContext> options, ITenantProvider tenantProvider, IConfiguration config)
        : base(options)
        {
            var tenant = tenantProvider.GetTenant();
            var template = config.GetConnectionString("TenantDbTemplate") ?? "";
            _connectionString = template.Replace("{tenant}", tenant);
        }
        public virtual DbSet<MstLoginVisitorDetail> MstLoginVisitorDetails { get; set; }
        public virtual DbSet<MstPeopleDetail> MstPeopleDetails { get; set; }
        public virtual DbSet<MstPurpose> MstPurposes { get; set; }
        public virtual DbSet<TxnVisitorDetail> TxnVisitorDetails { get; set; }
        public virtual DbSet<DbRespEntity> DbRespEntitys { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
           .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
           .AddJsonFile("appsettings.json")
           .Build();
                optionsBuilder.UseNpgsql(configuration.GetConnectionString("AppDbConnection"));
                //optionsBuilder.UseSqlServer("Server=34.131.248.73;Database=shavi_inventory;User Id=sqlserver;Password=Vishal@123;");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "English_United States.1252");
            modelBuilder.Entity<DbRespEntity>().HasNoKey();
            modelBuilder.Entity<MstLoginVisitorDetail>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("mst_login_visitor_details");

                entity.HasIndex(e => e.EmailId, "mst_login_visitor_details_email_id_key")
                    .IsUnique();

                entity.Property(e => e.AccountLock)
                    .HasMaxLength(10)
                    .HasColumnName("account_lock")
                    .HasDefaultValueSql("'N'::bpchar")
                    .IsFixedLength(true);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(20)
                    .HasColumnName("created_by")
                    .HasDefaultValueSql("'Admin'::character varying");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.EmailId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("email_id");

                entity.Property(e => e.LastLogin).HasColumnName("last_login");

                entity.Property(e => e.LoginId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("login_id");

                entity.Property(e => e.MobileNum)
                    .HasMaxLength(20)
                    .HasColumnName("mobile_num");

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(20)
                    .HasColumnName("modified_by");

                entity.Property(e => e.ModifiedDate).HasColumnName("modified_date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .HasMaxLength(30)
                    .HasColumnName("password")
                    .HasDefaultValueSql("'A0n+FiGOV1bM/Khu3hmT/Q=='::character varying");

                entity.Property(e => e.PasswordResetDate).HasColumnName("password_reset_date");

                entity.Property(e => e.RetryCount)
                    .HasColumnName("retry_count")
                    .HasDefaultValueSql("0");
            });

            modelBuilder.Entity<MstPeopleDetail>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("mst_people_details");

                entity.Property(e => e.Active)
                    .HasMaxLength(50)
                    .HasColumnName("active");

                entity.Property(e => e.AreaId)
                    .HasMaxLength(50)
                    .HasColumnName("area_id");

                entity.Property(e => e.BloodGroup)
                    .HasMaxLength(50)
                    .HasColumnName("blood_group");

                entity.Property(e => e.BranchId)
                    .HasMaxLength(50)
                    .HasColumnName("branch_id");

                entity.Property(e => e.BranchJoiningDate)
                    .HasMaxLength(50)
                    .HasColumnName("branch_joining_date");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasMaxLength(50)
                    .HasColumnName("created_on");

                entity.Property(e => e.CurrentBranch)
                    .HasMaxLength(50)
                    .HasColumnName("current_branch");

                entity.Property(e => e.CurrentBranchId)
                    .HasMaxLength(50)
                    .HasColumnName("current_branch_id");

                entity.Property(e => e.DateOfBirth)
                    .HasMaxLength(50)
                    .HasColumnName("date_of_birth");

                entity.Property(e => e.DateOfJoining)
                    .HasMaxLength(50)
                    .HasColumnName("date_of_joining");

                entity.Property(e => e.Department)
                    .HasMaxLength(50)
                    .HasColumnName("department");

                entity.Property(e => e.DepartmentName)
                    .HasMaxLength(50)
                    .HasColumnName("department_name");

                entity.Property(e => e.DesigId)
                    .HasMaxLength(50)
                    .HasColumnName("desig_id");

                entity.Property(e => e.DesignationId)
                    .HasMaxLength(50)
                    .HasColumnName("designation_id");

                entity.Property(e => e.DesignationName)
                    .HasMaxLength(50)
                    .HasColumnName("designation_name");

                entity.Property(e => e.DesignationType)
                    .HasMaxLength(50)
                    .HasColumnName("designation_type");

                entity.Property(e => e.EmployeeId)
                    .HasMaxLength(50)
                    .HasColumnName("employee_id");

                entity.Property(e => e.EmployeeName)
                    .HasMaxLength(50)
                    .HasColumnName("employee_name");

                entity.Property(e => e.ExtEmployeeId)
                    .HasMaxLength(50)
                    .HasColumnName("ext_employee_id");

                entity.Property(e => e.FatherName)
                    .HasMaxLength(50)
                    .HasColumnName("father_name");

                entity.Property(e => e.FrozenBookFlag)
                    .HasMaxLength(50)
                    .HasColumnName("frozen_book_flag");

                entity.Property(e => e.FunctionalTitle)
                    .HasMaxLength(50)
                    .HasColumnName("functional_title");

                entity.Property(e => e.Gender)
                    .HasMaxLength(50)
                    .HasColumnName("gender");

                entity.Property(e => e.GradeId)
                    .HasMaxLength(50)
                    .HasColumnName("grade_id");

                entity.Property(e => e.GradeName)
                    .HasMaxLength(50)
                    .HasColumnName("grade_name");

                entity.Property(e => e.GroupId)
                    .HasMaxLength(50)
                    .HasColumnName("group_id");

                entity.Property(e => e.HighestQualification)
                    .HasMaxLength(50)
                    .HasColumnName("highest_qualification");

                entity.Property(e => e.JobExpInYrs)
                    .HasMaxLength(50)
                    .HasColumnName("job_exp_in_yrs");

                entity.Property(e => e.MailId)
                    .HasMaxLength(50)
                    .HasColumnName("mail_id");

                entity.Property(e => e.MobileNumber)
                    .HasMaxLength(50)
                    .HasColumnName("mobile_number");

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(50)
                    .HasColumnName("modified_by");

                entity.Property(e => e.ModifiedOn)
                    .HasMaxLength(50)
                    .HasColumnName("modified_on");

                entity.Property(e => e.MotherName)
                    .HasMaxLength(50)
                    .HasColumnName("mother_name");

                entity.Property(e => e.Source)
                    .HasMaxLength(50)
                    .HasColumnName("source");

                entity.Property(e => e.SourceName)
                    .HasMaxLength(50)
                    .HasColumnName("source_name");

                entity.Property(e => e.SpouseName)
                    .HasMaxLength(50)
                    .HasColumnName("spouse_name");

                entity.Property(e => e.Uan)
                    .HasMaxLength(50)
                    .HasColumnName("uan");

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .HasColumnName("user_id");
            });

            modelBuilder.Entity<MstPurpose>(entity =>
            {
                entity.ToTable("mst_purpose");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .HasColumnName("created_by")
                    .HasDefaultValueSql("'Admin'::character varying");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.ModifyBy)
                    .HasMaxLength(50)
                    .HasColumnName("modify_by")
                    .HasDefaultValueSql("'Admin'::character varying");

                entity.Property(e => e.ModifyDate)
                    .HasColumnName("modify_date")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("title");
            });

            modelBuilder.Entity<TxnVisitorDetail>(entity =>
            {
                entity.ToTable("txn_visitor_details");

                entity.Property(e => e.TxnId).HasColumnName("txn_id");

                entity.Property(e => e.CheckIn)
                    .HasColumnName("check_in")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CheckOut)
                    .HasMaxLength(100)
                    .HasColumnName("check_out");

                entity.Property(e => e.Company)
                    .HasMaxLength(100)
                    .HasColumnName("company");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");

                entity.Property(e => e.ForceCheckOut)
                    .HasColumnName("force_check_out")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.LastLoginOtp).HasColumnName("last_login_otp");

                entity.Property(e => e.Mode)
                    .HasMaxLength(10)
                    .HasColumnName("mode")
                    .HasDefaultValueSql("'D'::character varying");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Number)
                    .IsRequired()
                    .HasMaxLength(15)
                    .HasColumnName("number");

                entity.Property(e => e.NumberOfPerson)
                    .HasColumnName("number_of_person")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.Picture).HasColumnName("picture");

                entity.Property(e => e.Purpose).HasColumnName("purpose");

                entity.Property(e => e.WhomToMeet)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("whom_to_meet");
            });

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
