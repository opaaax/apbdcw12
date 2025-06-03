using cw12.DTOs;
using cw12.Services;
using Microsoft.AspNetCore.Mvc;

namespace cw12.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly IDbService _dbService;

    public TripsController(IDbService dbService)
    {
        _dbService = dbService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTripsAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var trips = await _dbService.GetTripsAsync(page, pageSize);
            return Ok(trips);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{idTrip}/clients")]
    public async Task<IActionResult> AddToTripAsync(int idTrip, [FromBody] PostClientDTO client)
    {
        try
        {
            await _dbService.AddClientToTripAsync(idTrip, client);
            return Created("", client);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

}