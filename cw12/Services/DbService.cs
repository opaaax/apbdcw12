using cw12.DTOs;
using Microsoft.EntityFrameworkCore;

namespace cw12.Services;

public class DbService : IDbService
{
    private readonly MasterContext _context;

    public DbService(MasterContext context)
    {
        _context = context;
    }
    
    public async Task<List<GetTripsDTO>> GetTripsAsync(int page = 1, int pageSize = 10)
    {
        var countTrips = await _context.Trips.CountAsync();
        var allPages = (int) Math.Ceiling(countTrips / (double)pageSize);
        
        var trips = await _context.Trips
            .Skip((page-1)*pageSize)
            .Take(pageSize)
            .Select(trip => new GetTripsDTO()
        {
            PageNum = page,
            PageSize = pageSize,
            AllPages = allPages,
            Name = trip.Name,
            Description = trip.Description,
            DateFrom = trip.DateFrom,
            DateTo = trip.DateTo,
            MaxPeople = trip.MaxPeople,
            Countries = _context.Countries.Where(country => trip.IdCountries.Contains(country)).Select(country => new GetCountriesDTO()
            {
                Name = country.Name
            }).ToList(),
            Clients = _context.Clients
                .Where(client => _context.ClientTrips
                    .Where(c => c.IdTrip == trip.IdTrip)
                    .Select(e => e.IdClient)
                    .ToList()
                    .Contains(client.IdClient))
                .Select(client => new GetClientDTO()
            {
                FirstName = client.FirstName,
                LastName = client.LastName,
            }).ToList()
        }).OrderByDescending(trip => trip.DateFrom).ToListAsync();
        return trips;
    }

    public async Task DeleteClientAsync(int id)
    {
        var noOfClientTrips = _context.ClientTrips.Count(clientTrip => clientTrip.IdClient == id);
        if (noOfClientTrips > 0)
        {
            throw new Exception("Can't delete, client has assigned trips");
        }
        else
        {
            _context.Clients.Remove(await _context.Clients.FindAsync(id) ?? throw new InvalidOperationException("Client not found"));
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddClientToTripAsync(int idTrip, PostClientDTO client)
    {
        var doesExist = await _context.Clients.Select(e => e.Pesel).ContainsAsync(client.Pesel);
        if (doesExist)
        {
            throw new Exception("Client with this id already exists");
        }

        var isAlreadySignedUp = await _context.Clients
            .Where(cl => _context.ClientTrips.
                Where(c => c.IdTrip == client.IdTrip)
                .Select(e => e.IdClient).ToList()
                .Contains(cl.IdClient))
            .Select(e => e.Pesel)
            .ContainsAsync(client.Pesel);
        
        if (isAlreadySignedUp)
        {
            throw new Exception("Client with this pesel is already signed up");
        }
        
        var doesTripExist = await _context.Trips.AnyAsync(trip => trip.IdTrip == client.IdTrip);
        var isDateInTheFuture = await _context.Trips.AnyAsync(trip => trip.DateFrom > DateTime.Now);

        if (!doesTripExist || !isDateInTheFuture)
        {
            throw new Exception("Trip conditions aren't met");
        }

        Client addedClient = new Client()
        {
            FirstName = client.FirstName,
            LastName = client.LastName,
            Pesel = client.Pesel,
            Email = client.Email,
            Telephone = client.Telephone
        };

        await _context.Clients.AddAsync(addedClient);
        await _context.SaveChangesAsync();
        
        ClientTrip clientTrip = new ClientTrip()
        {
            IdClient = await _context.Clients.Where(e => e.Pesel == client.Pesel).Select(e=> e.IdClient).FirstOrDefaultAsync(),
            IdTrip = client.IdTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = client.PaymentDate,
        };


        await _context.ClientTrips.AddAsync(clientTrip);
        await _context.SaveChangesAsync();
    }
}