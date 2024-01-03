using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.TubeArchivistMetadata;

/// <summary>
/// The image provider.
/// </summary>
public class YoutubeEpisodeImageProvider : IDynamicImageProvider
{
    /// <summary>
    /// The _logger.
    /// </summary>
    private readonly ILogger<YoutubeEpisodeProvider> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="YoutubeEpisodeImageProvider"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public YoutubeEpisodeImageProvider(ILogger<YoutubeEpisodeProvider> logger)
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
        var video = (Video)item;

        // No support for these
        if (video.IsPlaceHolder || video.VideoType == VideoType.Dvd)
        {
            return new DynamicImageResponse { HasImage = false };
        }

        Console.WriteLine("replace image1");

        string videoId = Path.GetFileNameWithoutExtension(item.Path);

        var httpClient = Plugin.Instance!.GetHttpClient();
        using HttpResponseMessage response = await httpClient.GetAsync($"/api/video/{videoId}/", cancellationToken).ConfigureAwait(false);
        using Stream content = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = new SnakeCaseNamingPolicy()
        };

        Response.Video? result = await JsonSerializer.DeserializeAsync<Response.Video>(content, jsonOptions, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (result == null || string.IsNullOrEmpty(result.Data.VidThumbUrl))
        {
            return new DynamicImageResponse { HasImage = false };
        }

        using HttpResponseMessage imageResponse = await httpClient.GetAsync(result.Data.VidThumbUrl, cancellationToken).ConfigureAwait(false);
        using Stream contentStream = await imageResponse.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        var extension = Path.GetExtension(result.Data.VidThumbUrl);
        ImageFormat format = extension switch
            {
                ".bmp" => ImageFormat.Bmp,
                ".gif" => ImageFormat.Gif,
                ".png" => ImageFormat.Png,
                ".webp" => ImageFormat.Webp,
                _ => ImageFormat.Jpg
            };

        var imagePath = Path.ChangeExtension(item.Path, extension);
        using (var outputStream = new FileStream(imagePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 8192, useAsync: true))
        {
            await contentStream.CopyToAsync(outputStream, cancellationToken).ConfigureAwait(false);
        }

        return new DynamicImageResponse
        {
            Format = format,
            HasImage = true,
            Path = imagePath
        };
    }

    /// <inheritdoc />
    public bool Supports(BaseItem item)
    {
        return item is Episode;
    }
}
