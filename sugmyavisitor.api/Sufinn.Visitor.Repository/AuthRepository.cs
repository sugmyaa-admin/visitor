using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Npgsql;
using Sufinn.Visitor.Core.Common;
using Sufinn.Visitor.Core.Entity;
using Sufinn.Visitor.Repository.Context;
using Sufinn.Visitor.Repository.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Sufinn.Visitor.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDBContext _context;
        private readonly IJWTManagerRepository _jWTManager;
        public AuthRepository(AppDBContext context, IJWTManagerRepository jWTManager)
        {
            _context = context;
            _jWTManager = jWTManager;
        }
        public async Task<Result<string>> authenticate(int employeeCode, string password)
        {

            // Define the SQL query to call the PostgreSQL function
            string sqlQuery = "SELECT flag, msg FROM pr_visitor_login(@log_in_id, @pass_word,@pi_oper_flag,@old_password);";

            var loginIdParam = new NpgsqlParameter("@log_in_id", employeeCode);
            var passwordParam = new NpgsqlParameter("@pass_word", EncryptionHelper.Encrypt(password));
            var oprFlagParam = new NpgsqlParameter("@pi_oper_flag", "L");
            var oldPasswordParam = new NpgsqlParameter("@old_password", "");

            // Execute the raw SQL query and retrieve the result as a list
            var result = await _context.DbRespEntitys
                                       .FromSqlRaw(sqlQuery, loginIdParam, passwordParam, oprFlagParam, oldPasswordParam)
                                       .ToListAsync();


            char flag = result[0].flag;
            string msg = result[0].msg;

            if (flag.Equals('S'))
            {
                var authClaims = new List<Claim>
                    {
                        new Claim("LoginId", employeeCode.ToString()),
                    };
                var token = _jWTManager.CreateToken(authClaims);
                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                return Common<string>.getResponse(true, msg, tokenString);


            }
            return Common<string>.getResponse(false, msg, "");
        }


        public async Task<Result<string>> changeInPassword(int employeeCode, string password, char operationFlag, string oldPassword = null)
        {
            // Define the SQL query to call the PostgreSQL function
            string sqlQuery = "SELECT flag, msg FROM pr_visitor_login(@log_in_id, @pass_word, @pi_oper_flag,@old_password);";

            var loginIdParam = new NpgsqlParameter("@log_in_id", employeeCode);
            var passwordParam = new NpgsqlParameter("@pass_word", EncryptionHelper.Encrypt(password));
            var operationFlagParam = new NpgsqlParameter("@pi_oper_flag", operationFlag);

            var oldPasswordParam = new NpgsqlParameter("@old_password", oldPassword != null ? EncryptionHelper.Encrypt(oldPassword) : null);

            // Execute the raw SQL query and retrieve the result
            var result = await _context.DbRespEntitys
                                       .FromSqlRaw(sqlQuery, loginIdParam, passwordParam, operationFlagParam, oldPasswordParam)
                                       .ToListAsync();

            char flag = result[0].flag;
            string msg = result[0].msg;

            if (flag.Equals('S'))
            {
                return Common<string>.getResponse(true, msg, "");
            }

            return Common<string>.getResponse(false, msg, "");
        }


        public UserEntity findById(int employeeCode)
        {
            throw new NotImplementedException();
        }

        public void logout(int employeeCode)
        {
            throw new NotImplementedException();
        }
    }
}








