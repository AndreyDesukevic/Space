using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Space.Application.Interfaces;
using Space.Domain.Models;
using Space.Infrastructure.Options;
using System.Text.Json;

namespace Space.Infrastructure.HttpClients;

public class MeteoriteSyncClient : IMeteoriteSyncClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly MeteorDataOptions _options;
    private readonly ILogger<MeteoriteSyncClient> _logger;

    public MeteoriteSyncClient(
        IHttpClientFactory httpClientFactory,
        IOptions<MeteorDataOptions> options,
        ILogger<MeteoriteSyncClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IEnumerable<MeteoriteDto>> GetMeteoritesAsync(CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("MeteoriteClient");

        try
        {
            _logger.LogInformation("Запрос данных метеоритов с {Url}", _options.SourceUrl);

            using var response = await client.GetAsync(_options.SourceUrl, cancellationToken);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var dtos = await JsonSerializer.DeserializeAsync<IEnumerable<MeteoriteDto>>(stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                cancellationToken);

            _logger.LogInformation("Получено {Count} объектов", dtos?.Count() ?? 0);
            return dtos ?? Enumerable.Empty<MeteoriteDto>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Ошибка HTTP при получении метеоритов");
            return Enumerable.Empty<MeteoriteDto>();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Ошибка при десериализации JSON");
            return Enumerable.Empty<MeteoriteDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Неизвестная ошибка при получении метеоритов");
            return Enumerable.Empty<MeteoriteDto>();
        }
    }
}
