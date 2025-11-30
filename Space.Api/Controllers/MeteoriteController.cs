using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        var (isValid, error) = Validate(query);
        if (!isValid) return BadRequest(error);

        try
        {
            var result = await _meteoriteService.GetSummaryAsync(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting meteorite summary");
            return StatusCode(500, "Internal server error");
        }
    }


    /// <summary>
    /// Возвращает максимальный и минимальных год для фильтрации в таблице.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    [HttpGet("year-range")]
    public async Task<ActionResult<YearRangeDto>> GetYearRange(CancellationToken cancellationToken)
    {
        try
        {
            var range = await _meteoriteService.GetYearRangeAsync(cancellationToken);
            return Ok(range);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting year range");
            return StatusCode(500, "Internal server error");
        }
    }

    private (bool IsValid, string? Error) Validate(MeteoriteQueryParams q)
    {
        if (q.Page < 1) return (false, "Page must be >= 1");
        if (q.PageSize < 1 || q.PageSize > 500) return (false, "PageSize must be between 1 and 500");
        if (q.YearFrom.HasValue && q.YearTo.HasValue && q.YearFrom > q.YearTo) return (false, "YearFrom must be <= YearTo");

        var allowedFields = new[] { "year", "count", "totalMass" };
        var allowedOrders = new[] { "asc", "desc" };

        if (!string.IsNullOrEmpty(q.SortField) && !allowedFields.Contains(q.SortField.ToLower()))
            return (false, "SortField must be one of: year, count, totalMass");

        if (!string.IsNullOrEmpty(q.SortOrder) && !allowedOrders.Contains(q.SortOrder.ToLower()))
            return (false, "SortOrder must be asc or desc");

        return (true, null);
    }
}
