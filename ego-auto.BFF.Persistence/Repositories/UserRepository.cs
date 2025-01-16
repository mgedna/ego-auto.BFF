using ego_auto.BFF.Application.Contracts.Persistence;
using ego_auto.BFF.Domain.Entities;
using ego_auto.BFF.Domain.ExceptionTypes;
using ego_auto.BFF.Domain.Requests.Authentication;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace ego_auto.BFF.Persistence.Repositories;

public sealed class UserRepository(AppDbContext _context) : IUserRepository
{
    public async Task<User> GetUser(string email) => await _context.Users.FirstOrDefaultAsync(ef => ef.Email == email);

    public async Task<int> GetUserIdByEmail(string email)
    => await _context.Users
           .Where(u => u.Email == email)
           .Select(u => u.Id)
           .FirstOrDefaultAsync();

    public async Task UpsertUser(SignUpRequest request, string? userId)
    {
        await SetSessionUser(userId);

        using (var command = _context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "SHOW myapp.user_id;";
            _context.Database.OpenConnection();
            using (var result = await command.ExecuteReaderAsync())
            {
                if (result.Read())
                {
                    var userIdd = result.GetString(0);  // Assuming the value is in the first column
                                                       // Convert or use the userId as needed
                }
            }
        }
        NpgsqlParameter accountNameParam = new("p_account_name", request.AccountName ?? (object)DBNull.Value)
        {
            Direction = ParameterDirection.Input
        };
        NpgsqlParameter emailParam = new("p_email", request.Email ?? (object)DBNull.Value)
        {
            Direction = ParameterDirection.Input
        };
        NpgsqlParameter passwordParam = new("p_password", request.Password ?? (object)DBNull.Value)
        {
            Direction = ParameterDirection.Input
        };
        NpgsqlParameter roleParam = new("p_role", request.Role ?? "Renter")
        {
            Direction = ParameterDirection.Input
        };

        await _context.Database.ExecuteSqlRawAsync(
            "CALL public.upsert_user(@p_account_name, @p_email, @p_password, @p_role);",
            accountNameParam,
            emailParam,
            passwordParam,
            roleParam
        );
    }

    public async Task SetSessionUser(string? userId)
     {
        string sql;

        if (!string.IsNullOrEmpty(userId))
            sql = $"SET myapp.user_id = '{userId}';";
        else
            sql = "RESET myapp.user_id;";

        await _context.Database.ExecuteSqlRawAsync(sql);
    }
}
