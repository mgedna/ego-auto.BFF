using ego_auto.BFF.Application.Contracts.Application;
using ego_auto.BFF.Application.Contracts.Persistence;
using ego_auto.BFF.Application.Utilities;
using ego_auto.BFF.Domain.Common.Bindings;
using ego_auto.BFF.Domain.Entities;
using ego_auto.BFF.Domain.ExceptionTypes;
using ego_auto.BFF.Domain.Requests.Authentication;
using ego_auto.BFF.Domain.Responses;
using Microsoft.Extensions.Options;

namespace ego_auto.BFF.Application.Implementations;

public class UserService(IUserRepository _userRepository, IOptions<AppSettings> appSettings) : IUserService
{
    private readonly AppSettings _appSettings = appSettings.Value;

    public async Task<AuthenticationResponse> LogIn(LogInRequest request)
    {
        User currentUser = await _userRepository.GetUser(request.Email!) ?? throw new CustomNotFound("Given email address doesn't exist in our database!");

        if (!AuthHelper.VerifyPassword(request.Password!, currentUser.Password)) throw new CustomBadRequest("Given password is incorrect!");

        return new AuthenticationResponse(Token: AuthHelper.GenerateJwtToken
            (
                data: new()
                {
                    Email = currentUser.Email ?? null,
                    Role = currentUser.Role ?? "Guest",
                    AccountName = currentUser.AccountName ?? "Guest",
                    UserId = currentUser.Id is 0 ? -1 : currentUser.Id
                },
                jwtConfiguration: _appSettings.JwtConfiguration
            ));
    }

    public async Task<AuthenticationResponse> SignUp(SignUpRequest request, string? userId)
    {
        var existingUser = await _userRepository.GetUser(request.Email);

        if (existingUser is not null) throw new CustomBadRequest("Given email address exist in our database!");

        await _userRepository.UpsertUser(request, userId);

        int userIdFromDb = await _userRepository.GetUserIdByEmail(request.Email);

        return new AuthenticationResponse
            (
                Token: AuthHelper.GenerateJwtToken
                (
                    data: new()
                    {
                        Email = request.Email ?? null,
                        Role = request.Role ?? "Renter",
                        AccountName = request.AccountName ?? "RenterDefault",
                        UserId = userIdFromDb is 0 ? -1 : userIdFromDb
                    },
                    jwtConfiguration: _appSettings.JwtConfiguration
                )
            );
    }


    public async Task SetSessionUser(string? userId) => await _userRepository.SetSessionUser(userId);
    public async Task ResetSessionUser() => await _userRepository.SetSessionUser(null);
}
