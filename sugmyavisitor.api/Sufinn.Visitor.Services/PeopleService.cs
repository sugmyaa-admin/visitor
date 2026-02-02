using System;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using Sufinn.Visitor.Repository.Context;
using Microsoft.EntityFrameworkCore;

public class PeopleService
{
    private readonly AppDBContext _context;

    public PeopleService(AppDBContext context)
    {
        _context = context;
    }

    public DataTable GetPeopleDetails()
    {
        using (var command = _context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "pr_get_people_detail";
            command.CommandType = CommandType.StoredProcedure;

            // Open the connection if it's not open
            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            using (var reader = command.ExecuteReader())
            {
                var dataTable = new DataTable();
                dataTable.Load(reader);
                return dataTable;
            }
        }
    }
    
}