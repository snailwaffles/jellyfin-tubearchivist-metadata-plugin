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
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.TubeArchivistMetadata;

/// <summary>
/// The metadata provider.
/// </summary>
public class YoutubeEpisodeProvider : IRemoteMetadataProvider<Episode, EpisodeInfo>
{
    /// <summary>
    /// The _logger.
    /// </summary>
    private readonly ILogger<YoutubeEpisodeProvider> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="YoutubeEpisodeProvider"/> class.
    /// </summary>
    /// <param name="appPaths">The app paths.</param>
    /// <param name="logger">The logger.</param>
    public YoutubeEpisodeProvider(IApplicationPaths appPaths, ILogger<YoutubeEpisodeProvider> logger)
    {
        this._logger = logger;
    }

    /// <inheritdoc />
    public string Name => "TubeArchivist";

    /// <inheritdoc />
    public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
    {
        _logger.LogDebug("GetImages: GetImageResponse ", url);
        var httpClient = Plugin.Instance!.GetHttpClient();
        return await httpClient.GetAsync(new Uri(url), cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<MetadataResult<Episode>> GetMetadata(EpisodeInfo info, CancellationToken cancellationToken)
    {
        var metadataResult = new MetadataResult<Episode>();

        if (info.IsMissingEpisode)
        {
            return metadataResult;
        }

        string videoId = Path.GetFileNameWithoutExtension(info.Path);

        var httpClient = Plugin.Instance!.GetHttpClient();
        using HttpResponseMessage response = await httpClient.GetAsync($"/api/video/{videoId}/", cancellationToken).ConfigureAwait(false);
        using Stream content = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        Response.Video? result = await JsonSerializer.DeserializeAsync<Response.Video>(content, jsonOptions, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (result == null)
        {
            return metadataResult;
        }

        metadataResult.HasMetadata = true;
        metadataResult.QueriedById = true;

        var item = new Episode
        {
            Name = info.Name,
            IndexNumber = info.IndexNumber,
            ParentIndexNumber = info.ParentIndexNumber,
            IndexNumberEnd = info.IndexNumberEnd
        };

        if (!string.IsNullOrEmpty(result.Data.Title))
        {
            item.Name = result.Data.Title;
        }

        if (!string.IsNullOrEmpty(result.Data.Description))
        {
            item.Overview = result.Data.Description;
        }

        if (!string.IsNullOrEmpty(result.Data.Published))
        {
            string format = "yyyy-MM-dd";
            item.PremiereDate = DateTime.ParseExact(result.Data.Published, format, CultureInfo.InvariantCulture);
        }

        metadataResult.Item = item;
        Console.WriteLine($"sending result {item}");

        return metadataResult;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(EpisodeInfo searchInfo, CancellationToken cancellationToken)
    {
        if (!searchInfo.IndexNumber.HasValue || !searchInfo.ParentIndexNumber.HasValue)
        {
            return Enumerable.Empty<RemoteSearchResult>();
        }

        var metadataResult = await GetMetadata(searchInfo, cancellationToken).ConfigureAwait(false);

        if (!metadataResult.HasMetadata)
        {
            return Enumerable.Empty<RemoteSearchResult>();
        }

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
                SearchProviderName = Name,
                IndexNumberEnd = item.IndexNumberEnd
            }
        };
    }
}
