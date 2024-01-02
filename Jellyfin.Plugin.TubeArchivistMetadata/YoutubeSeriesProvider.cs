using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.TubeArchivistMetadata;

/// <summary>
/// The metadata provider.
/// </summary>
public class YoutubeSeriesProvider : IRemoteMetadataProvider<Series, SeriesInfo>
{
    /// <summary>
    /// The _logger.
    /// </summary>
    private readonly ILogger<YoutubeSeriesProvider> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="YoutubeSeriesProvider"/> class.
    /// </summary>
    /// <param name="appPaths">The app paths.</param>
    /// <param name="logger">The logger.</param>
    public YoutubeSeriesProvider(IApplicationPaths appPaths, ILogger<YoutubeSeriesProvider> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public string Name => "TubeArchivist";

    /// <inheritdoc />
    public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
    {
        Console.WriteLine($"get image for {url}");
        _logger.LogDebug("GetImages: GetImageResponse ", url);
        var httpClient = Plugin.Instance!.GetHttpClient();
        return await httpClient.GetAsync(new Uri(url), cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<MetadataResult<Series>> GetMetadata(SeriesInfo info, CancellationToken cancellationToken)
    {
        var metadataResult = new MetadataResult<Series>();

        // var tmdbId = info.GetProviderId(MetadataProvider.Tmdb);

        string channelId = Path.GetFileNameWithoutExtension(info.Path);

        var httpClient = Plugin.Instance!.GetHttpClient();
        using HttpResponseMessage response = await httpClient.GetAsync($"/api/channel/{channelId}/", cancellationToken).ConfigureAwait(false);
        using Stream content = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = new SnakeCaseNamingPolicy()
        };

        Response.Channel? result = await JsonSerializer.DeserializeAsync<Response.Channel>(content, jsonOptions, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (result == null)
        {
            return metadataResult;
        }

        metadataResult.HasMetadata = true;
        metadataResult.QueriedById = true;

        var series = new Series
        {
            Name = result.Data.ChannelName,
        };

        // series.SetProviderId(MetadataProvider.Tmdb, tvShow.Id.ToString());
        series.Overview = result.Data.ChannelDescription;

        // series.HomePageUrl = ....;

        // All youtube channels are continuing
        series.Status = SeriesStatus.Continuing;

        metadataResult.Item = series;

        Console.WriteLine($"sending channel result {series}");
        Console.WriteLine($"sending channel result name {series.Name}");
        return metadataResult;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(SeriesInfo searchInfo, CancellationToken cancellationToken)
    {
        var metadataResult = await GetMetadata(searchInfo, cancellationToken).ConfigureAwait(false);

        var item = metadataResult.Item;

        return new[]
        {
            new RemoteSearchResult
            {
                IndexNumber = item.IndexNumber,
                Name = item.Name,
                ParentIndexNumber = item.ParentIndexNumber,
                PremiereDate = item.PremiereDate,
                ProductionYear = item.ProductionYear,
                ProviderIds = item.ProviderIds,
                SearchProviderName = Name
            }
        };
    }
}
