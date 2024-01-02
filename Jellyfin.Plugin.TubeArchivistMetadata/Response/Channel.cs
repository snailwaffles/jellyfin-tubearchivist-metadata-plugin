namespace Jellyfin.Plugin.TubeArchivistMetadata.Response;

/// <summary>
/// The response from the channel endpoint.
/// </summary>
public class Channel
{
    /// <summary>
    /// Gets or sets the channel data.
    /// </summary>
    public ChannelData Data { get; set; } = new ChannelData();
}
