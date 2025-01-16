using ego_auto.BFF.Application.Contracts.Persistence;
using ego_auto.BFF.Domain.Common;
using ego_auto.BFF.Domain.Entities;
using ego_auto.BFF.Domain.ExceptionTypes;
using ego_auto.BFF.Domain.Requests.Vehicle;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ego_auto.BFF.Persistence.Repositories;

public sealed class VehicleRepository(AppDbContext _context) : IVehicleRepository
{
    public async Task UpsertVehicleAsync(VehicleUpsertRequest model)
    {
        NpgsqlParameter vehicleIdParam = new("p_vehicle_id", model.VehicleId.HasValue ? model.VehicleId.Value : (object)DBNull.Value)
        {
            NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer 
        };

        NpgsqlParameter makeParam = new("p_make", model.Make ?? (object)DBNull.Value)
        {
            NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Varchar
        };

        NpgsqlParameter modelParam = new("p_model", model.Model ?? (object)DBNull.Value)
        {
            NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Varchar
        };

        NpgsqlParameter yearParam = new("p_year", model.Year.HasValue ? model.Year.Value : (object)DBNull.Value)
        {
            NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer
        };

        NpgsqlParameter pricePerDayParam = new("p_price_per_day", model.PricePerDay.HasValue ? (object)model.PricePerDay.Value : (object)DBNull.Value)
        {
            NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Numeric
        };

        NpgsqlParameter descriptionParam = new("p_description", model.Description ?? (object)DBNull.Value)
        {
            NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text
        };

        await _context.Database.ExecuteSqlRawAsync(
            "CALL public.upsert_vehicle(@p_vehicle_id, @p_make, @p_model, @p_year, @p_price_per_day, @p_description);",
            vehicleIdParam,
            makeParam,
            modelParam,
            yearParam,
            pricePerDayParam,
            descriptionParam
        );
    }


    public async Task<PaginatedResult<Vehicle>> GetVehiclesAsync(GetVehiclesRequest request)
    {
        var query = _context.Vehicles.AsQueryable();

        query = query
            .Where(v => string.IsNullOrEmpty(request.MakeFilter) || v.Make.Contains(request.MakeFilter))
            .Where(v => string.IsNullOrEmpty(request.ModelFilter) || v.Model.Contains(request.ModelFilter))
            .Where(v => !request.PricePerDayFilter.HasValue || v.PricePerDay == request.PricePerDayFilter.Value)
            .Where(v => string.IsNullOrEmpty(request.StatusFilter) || v.Status == request.StatusFilter)
            .Where(v => string.IsNullOrEmpty(request.DescriptionFilter) || v.Description!.Contains(request.DescriptionFilter));

        var totalPages = (int)Math.Ceiling
            (
                await query.CountAsync() / (double)request.PageSize
            );

        var vehicles = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PaginatedResult<Vehicle>
        {
            TotalCount = totalPages,
            Items = vehicles
        };
    }

    public async Task<Vehicle?> GetVehicleByIdAsync(int vehicleId) => await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == vehicleId);

    public async Task DeleteVehicleAsync(int vehicleId)
    {
        var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == vehicleId);

        if (vehicle is null)
        {
            throw new CustomNotFound($"Vehicle with ID {vehicleId} not found.");
        }

        _context.Vehicles.Remove(vehicle);

        await _context.SaveChangesAsync();
    }
}
