using ego_auto.BFF.Domain.Entities;
using ego_auto.BFF.Domain.Requests.Authentication;

namespace ego_auto.BFF.Application.Contracts.Persistence;

public interface IUserRepository
{
    Task<User> GetUser(string email);
    Task<int> GetUserIdByEmail(string email);
    Task UpsertUser(SignUpRequest request, string? userId);
    Task SetSessionUser(string? userId);
}
