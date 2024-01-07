using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.MediaInfo;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.TubeArchivistMetadata;

/// <summary>
/// The image provider.
/// </summary>
public class YoutubeFolderImageProvider : IDynamicImageProvider
{
    /// <summary>
    /// The _logger.
    /// </summary>
    private readonly ILogger<YoutubeFolderProvider> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="YoutubeFolderImageProvider"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public YoutubeFolderImageProvider(ILogger<YoutubeFolderProvider> logger)
    {
        this._logger = logger;
    }

    /// <inheritdoc />
    public string Name => "TubeArchivist";

    /// <inheritdoc />
    public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
    {
        return new[]
        {
            ImageType.Primary,
        };
    }

    /// <inheritdoc />
    public async Task<DynamicImageResponse> GetImage(BaseItem item, ImageType type, CancellationToken cancellationToken)
    {
        _logger.LogDebug("[TubeArchivist] Fetching image for {0}", item.Path);

        string channelId = Path.GetFileNameWithoutExtension(item.Path);

        var httpClient = Plugin.Instance!.GetHttpClient();
        using HttpResponseMessage response = await httpClient.GetAsync($"/api/channel/{channelId}/", cancellationToken).ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return new DynamicImageResponse { HasImage = false };
        }
        else if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("[TubeArchivist] Error getting channel info for {0}: {1}", item.Path, response.StatusCode);
            return new DynamicImageResponse { HasImage = false };
        }

        using Stream content = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = new SnakeCaseNamingPolicy()
        };

        Response.Channel? result = await JsonSerializer.DeserializeAsync<Response.Channel>(content, jsonOptions, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (result == null || string.IsNullOrEmpty(result.Data.ChannelThumbUrl))
        {
            return new DynamicImageResponse { HasImage = false };
        }

        using HttpResponseMessage imageResponse = await httpClient.GetAsync(result.Data.ChannelThumbUrl, cancellationToken).ConfigureAwait(false);
        using Stream contentStream = await imageResponse.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        var extension = Path.GetExtension(result.Data.ChannelThumbUrl);
        ImageFormat format = extension switch
            {
                ".bmp" => ImageFormat.Bmp,
                ".gif" => ImageFormat.Gif,
                ".png" => ImageFormat.Png,
                ".webp" => ImageFormat.Webp,
                _ => ImageFormat.Jpg
            };

        var memoryStream = new MemoryStream();
        await contentStream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
        memoryStream.Position = 0;

        return new DynamicImageResponse
        {
            Format = format,
            HasImage = true,
            Stream = memoryStream
        };
    }

    /// <inheritdoc />
    public bool Supports(BaseItem item)
    {
        return item is Folder;
    }
}
