using Microsoft.EntityFrameworkCore;
using Npgsql;
using Sufinn.Visitor.Core.Entity;
using Sufinn.Visitor.Repository.Context;
using Sufinn.Visitor.Repository.Context.Interface;
using Sufinn.Visitor.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Twilio.TwiML.Voice;
using static System.Net.Mime.MediaTypeNames;

namespace Sufinn.Visitor.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly IDbContext _context;
        private readonly AppDBContext _appDBContext;
        public BaseRepository(IDbContext context, AppDBContext appDBContext)
        {
            this._context = context;
            _appDBContext = appDBContext;
        }
        public void Delete(T obj) => _context.Entry(obj).State = EntityState.Deleted;

        public IEnumerable<T> Filter(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes)
        {
            var query = _context.Set<T>().Where(where);
            foreach (Expression<Func<T, object>> i in includes)
            {
                query = query.Include(i);
            }
            return query.ToList();
        }

        public IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includes)
        {
            var query = _context.Set<T>().AsQueryable();
            foreach (Expression<Func<T, object>> i in includes)
            {
                query = query.Include(i);
            }
            return query.ToList();
        }

        public async Task<IEnumerable<T>> GetAsync(bool isProcedure, string spName, params Expression<Func<T, object>>[] includes)
        {
            if (isProcedure)
            {
                var query = _context.Set<T>().FromSqlRaw("exec " + spName);
                foreach (Expression<Func<T, object>> i in includes)
                {
                    query = query.Include(i);
                }
                return query.ToList();
            }
            else
            {
                var query = _context.Set<T>().AsQueryable();
                foreach (Expression<Func<T, object>> i in includes)
                {
                    query = query.Include(i);
                }
                return query.ToList();
            }
        }

        public async Task<IEnumerable<T>> ExecuteStoredProcedureAsync(string storedProcedure, Dictionary<string, object> parameters, bool isCustomEntity)
        {
            var result = new List<T>();
            using var connection = (NpgsqlConnection)_appDBContext.Database.GetDbConnection();
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                using (var command = new NpgsqlCommand(storedProcedure, connection, transaction))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add input parameters
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }


                    // Define the cursor output parameter
                    var cursorParam = new NpgsqlParameter("out_cursor", NpgsqlTypes.NpgsqlDbType.Refcursor)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(cursorParam);

                    // Execute the function to open the cursor
                    await command.ExecuteNonQueryAsync();

                    // Get the actual cursor name
                    var actualCursorName = (string)cursorParam.Value;

                    // Fetch data from the cursor
                    using (var fetchCommand = new NpgsqlCommand($"FETCH ALL IN \"{actualCursorName}\";", connection, transaction))
                    {
                        using (var reader = await fetchCommand.ExecuteReaderAsync())
                        {
                            // Step 1: Get the column names from the reader
                            var columnNames = Enumerable.Range(0, reader.FieldCount)
                                                        .Select(reader.GetName)
                                                        .ToList();

                            while (await reader.ReadAsync())
                            {
                                // Map columns to entity properties (manual mapping)
                                var properties = typeof(T).GetProperties();

                                var entity = (T)Activator.CreateInstance(typeof(T));


                                foreach (var property in properties)
                                {
                                    // Check if the property name matches any of the column names
                                    if (columnNames.Contains(property.Name.ToLower()))
                                    {
                                        var columnIndex = reader.GetOrdinal(property.Name);
                                        if (!reader.IsDBNull(columnIndex))
                                        {
                                            if (isCustomEntity)
                                            {
                                                // Set the value from the database to the property
                                                //if (property.Name == "picture")
                                                //{
                                                //    property.SetValue(entity, Convert.ToBase64String((byte[])reader.GetValue(columnIndex)));
                                                //}
                                                //else
                                                //{
                                                    property.SetValue(entity, reader.GetValue(columnIndex).ToString());
                                                //}
                                            }
                                            else
                                            {
                                                property.SetValue(entity, reader.GetValue(columnIndex));
                                            }

                                        }
                                    }
                                }

                                result.Add(entity);
                            }
                        }
                    }
                }
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error executing stored procedure: {ex.Message}");
            }
            finally { connection.Dispose(); transaction.Dispose(); }
            return result;
        }

        public bool IsExist(Expression<Func<T, bool>> where)
        {
            var query = _context.Set<T>().Where(where);
            return query.Count() > 0;
        }

        public void Save(T obj) => _context.Set<T>().Add(obj);

        public int SaveChanges() => _context.SaveChanges();

        public void Update(T obj) => _context.Entry(obj).State = EntityState.Modified;

    }
}
