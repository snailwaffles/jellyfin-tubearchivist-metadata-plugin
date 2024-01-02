namespace Jellyfin.Plugin.TubeArchivistMetadata.Response;

/// <summary>
/// The response from the video endpoint.
/// </summary>
public class Video
{
    /// <summary>
    /// Gets or sets the video data.
    /// </summary>
    public VideoData Data { get; set; } = new VideoData();
}
