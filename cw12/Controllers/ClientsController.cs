using cw12.DTOs;
using cw12.Services;
using Microsoft.AspNetCore.Mvc;

namespace cw12.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IDbService _dbService;

    public ClientsController(IDbService dbService)
    {
        _dbService = dbService;
    }

    
    [HttpDelete("{id:int}")]
    
    public async Task<IActionResult> DeleteClient(int id)
    {
        try
        {
            await _dbService.DeleteClientAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
}