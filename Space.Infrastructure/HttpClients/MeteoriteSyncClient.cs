using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Space.Application.Interfaces;
using Space.Domain.Models;
using Space.Infrastructure.Options;
using System.Runtime.CompilerServices;
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

    public async IAsyncEnumerable<MeteoriteDto> GetMeteoritesStreamAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("MeteoriteClient");

        _logger.LogInformation("Requesting meteorite data from {Url}", _options.SourceUrl);

        using var response = await client.GetAsync(_options.SourceUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        _logger.LogInformation("Starting meteorite data streaming...");

        await foreach (var dto in JsonSerializer.DeserializeAsyncEnumerable<MeteoriteDto>(stream, options, cancellationToken))
        {
            if (dto != null)
                yield return dto;
        }

        _logger.LogInformation("Meteorite data stream completed.");

    }
}
