using Microsoft.AspNetCore.Mvc;
using Space.Application.Interfaces;

namespace Space.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DebugController : ControllerBase
{
    private readonly IMeteoriteSyncClient _client;

    public DebugController(IMeteoriteSyncClient client)
    {
        _client = client;
    }

    [HttpGet("test-sync")]
    public async Task<IActionResult> TestSync()
    {
        var dtos = await _client.GetMeteoritesAsync();
        return Ok(new
        {
            Count = dtos.Count(),
            Sample = dtos.Take(5)
        });
    }
}
