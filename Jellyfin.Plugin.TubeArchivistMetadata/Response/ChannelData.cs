using System.Text.Json;

namespace Jellyfin.Plugin.TubeArchivistMetadata.Response;

/// <summary>
/// The internal part of the video JSON response that contains the data.
/// </summary>
public class ChannelData
{
    /// <summary>
    /// Gets or sets the title of the channel.
    /// </summary>
    public string ChannelName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the channel.
    /// </summary>
    public string ChannelDescription { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the channel banner URL of the channel.
    /// </summary>
    public string ChannelBannerUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the channel thumbnail URL of the channel.
    /// </summary>
    public string ChannelThumbUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the TV art URL of the channel.
    /// </summary>
    public string ChannelTvartUrl { get; set; } = string.Empty;
}
