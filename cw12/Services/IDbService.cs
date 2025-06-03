using cw12.DTOs;

namespace cw12.Services;

public interface IDbService
{
    Task<List<GetTripsDTO>> GetTripsAsync(int page = 1, int pageSize = 10);
    Task DeleteClientAsync(int id);
    Task AddClientToTripAsync(int clientId, PostClientDTO client);
}