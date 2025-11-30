using Microsoft.AspNetCore.Mvc;
using Space.Application.Interfaces;

namespace Space.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DebugController : ControllerBase
{
    private readonly IMeteoriteSyncClient _client;
    private readonly IMeteoriteService _meteoriteService;

    public DebugController(IMeteoriteSyncClient client, IMeteoriteService meteoriteService)
    {
        _client = client;
        _meteoriteService = meteoriteService;
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

    [HttpGet("test-full-sync")]
    public async Task<IActionResult> TestFullSync()
    {

        await _meteoriteService.SyncAsync();

        return Ok();
    }
}
