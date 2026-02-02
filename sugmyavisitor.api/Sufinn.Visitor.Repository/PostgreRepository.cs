using Microsoft.EntityFrameworkCore;
using Sufinn.Visitor.Core.Common;
using Sufinn.Visitor.Core.Entity;
using Sufinn.Visitor.Repository.Context;
using Sufinn.Visitor.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufinn.Visitor.Repository
{
    public class PostgreRepository : IPostgreRepository
    {
        private readonly AppDBContext _context;
        public PostgreRepository(AppDBContext context)
        {
            _context = context;
        }
        public async Task<Result<string>> Save(string storedProcedure, Dictionary<string, object> parameters)
        {
            Result<string> objResult = new Result<string>();
            try
            {
                // Build the SQL query dynamically based on the function name
                string sqlQuery = $"SELECT * FROM {storedProcedure}(" + string.Join(", ", parameters.Keys.Select(k => $"@{k}")) + ");";

                // Convert the dictionary into an array of NpgsqlParameter
                var npgsqlParams = Utils.ConvertDictionaryToNpgsqlParameters(parameters);

                // Execute the raw SQL query and retrieve the result
                var result = await _context.DbRespEntitys
                                           .FromSqlRaw(sqlQuery, npgsqlParams)
                                           .ToListAsync();


                objResult.Status = new Result
                {
                    Success = result[0].flag.ToString() == "F" ? false : true,
                    Message = result[0].msg
                };
                return objResult;
            }
            catch (Exception ex)
            {
                objResult.Status = new Result
                {
                    Success = false,
                    Message = ex.Message
                };
                return objResult;
            }

        }
    }
}
