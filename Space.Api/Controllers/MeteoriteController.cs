using Microsoft.AspNetCore.Mvc;
using Space.Application.Interfaces;
using Space.Domain.DTO;
using Space.Domain.Responses;

namespace Space.Api.Controllers;

/// <summary>
/// Методы для работы с метеоритами.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class MeteoriteController : ControllerBase
{
    private readonly IMeteoriteService _meteoriteService;
    private readonly IRecClassService _recClassService;
    private readonly ILogger<MeteoriteController> _logger;

    public MeteoriteController(IMeteoriteService meteoriteService, IRecClassService recClassService, ILogger<MeteoriteController> logger)
    {
        _meteoriteService = meteoriteService;
        _recClassService = recClassService;
        _logger = logger;
    }

    /// <summary>
    /// Возвращает список всех классов метеоритов.
    /// </summary>
    /// <remarks>
    /// Используется для фильтрации данных на фронтенде.
    /// </remarks>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Список доступных классов метеоритов.</returns>
    [HttpGet("classes")]
    [ProducesResponseType(typeof(List<RecClassDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<RecClassDto>>> GetClasses(CancellationToken cancellationToken)
    {
        var classes = await _recClassService.GetRecClassesAsync(cancellationToken);
        return Ok(classes);
    }

    /// <summary>
    /// Возвращает сгруппированные по годам данные о метеоритах.
    /// </summary>
    /// <remarks>
    /// Позволяет фильтровать данные по:
    /// - диапазону лет
    /// - классу метеорита
    /// - вхождению части названия
    ///
    /// Группировка: по году.
    /// Сортировка: по году, количеству и суммарной массе.
    /// </remarks>
    /// <param name="query">Параметры фильтрации и пагинации.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Список сводных данных по годам.</returns>
    [HttpGet("summary")]
    public async Task<ActionResult<PaginatedResponse<MeteoriteSummaryDto>>> GetSummary([FromQuery] MeteoriteQueryParams query, CancellationToken cancellationToken)
    {
        var result = await _meteoriteService.GetSummaryAsync(query, cancellationToken);
        return Ok(result);
    }


    /// <summary>
    /// Возвращает максимальный и минимальных год для фильтрации в таблице.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    [HttpGet("year-range")]
    public async Task<ActionResult<YearRangeDto>> GetYearRange(CancellationToken cancellationToken)
    {
        var range = await _meteoriteService.GetYearRangeAsync(cancellationToken);
        return Ok(range);
    }
}
