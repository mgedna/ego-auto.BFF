using ego_auto.BFF.Application.Contracts.Persistence;
using ego_auto.BFF.Domain.Requests.Authentication;
using ego_auto.BFF.Domain.Responses;

namespace ego_auto.BFF.Application.Contracts.Application;

public interface IUserService
{
    Task<AuthenticationResponse> LogIn(LogInRequest request);
    Task<AuthenticationResponse> SignUp(SignUpRequest request, string? userId);
    Task SetSessionUser(string? userId);
    Task ResetSessionUser();
}
