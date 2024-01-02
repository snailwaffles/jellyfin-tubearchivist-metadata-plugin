namespace Jellyfin.Plugin.TubeArchivistMetadata.Response;

/// <summary>
/// The internal part of the video JSON response that contains the data.
/// </summary>
public class VideoData
{
    /// <summary>
    /// Gets or sets the title of the video.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the video.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date the video was published.
    /// </summary>
    public string Published { get; set; } = string.Empty;
}
