using ego_auto.BFF.Application.Contracts.Application;
using ego_auto.BFF.Domain.Requests.Vehicle;
using ego_auto.BFF.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ego_auto.BFF.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VehicleController(IVehicleService _service) : BaseController
{
    [Authorize(Roles = "Admin, Support, Renter, Guest")]
    [HttpGet]
    public async Task<IActionResult> GetVehiclesAsync([FromQuery] GetVehiclesRequest request)     
    => Ok(await _service.GetVehiclesAsync(request));

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> UpsertVehicleAsync([FromBody] VehicleUpsertRequest request)
    => Ok(await _service.UpsertVehicleAsync(request));

    [Authorize(Roles = "Admin, Renter, Guest")]
    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetVehicleAsync(int id)
    => Ok(await _service.GetVehicleAsync(id));

    [Authorize(Roles = "Admin")]
    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteVehicleAsync(int id)
    => Ok(await _service.DeleteVehicleAsync(id));
}
