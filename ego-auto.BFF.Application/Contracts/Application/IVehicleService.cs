﻿using ego_auto.BFF.Domain.Common;
using ego_auto.BFF.Domain.Entities;
using ego_auto.BFF.Domain.Requests.Vehicle;
using ego_auto.BFF.Domain.Responses;

namespace ego_auto.BFF.Application.Contracts.Application;

public interface IVehicleService
{
    Task<CustomResponse<PaginatedResult<Vehicle>>> GetVehiclesAsync(GetVehiclesRequest request);
    Task<CustomResponse<Vehicle>> GetVehicleAsync(int id);
    Task<CustomResponse> UpsertVehicleAsync(VehicleUpsertRequest request);
    Task<CustomResponse> DeleteVehicleAsync(int id);
}
