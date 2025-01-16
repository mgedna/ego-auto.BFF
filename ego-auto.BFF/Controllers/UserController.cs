using ego_auto.BFF.Application.Contracts.Application;
using ego_auto.BFF.Application.Utilities;
using ego_auto.BFF.Domain.Requests.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ego_auto.BFF.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(IUserService _service) : ControllerBase
{
    [HttpPost]
    [Route("log-in")]
    public async Task<IActionResult> LogIn(LogInRequest request)
    => Ok(await _service.LogIn(request));

    [HttpPost]
    [Route("sign-up")]
    public async Task<IActionResult> SignUp(SignUpRequest request)
    {
        var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

        var response = await _service.SignUp(request, AuthHelper.GetUserIdFromToken(token));
        return Ok(response);
    }
}
