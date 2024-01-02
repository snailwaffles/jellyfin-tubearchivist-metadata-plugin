using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.TubeArchivistMetadata.Configuration;

/// <summary>
/// Plugin configuration.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
    /// </summary>
    public PluginConfiguration()
    {
        // set default options here
        URL = string.Empty;
        Token = string.Empty;
    }

    /// <summary>
    /// Gets or sets the token setting.
    /// </summary>
    public string URL { get; set; }

    /// <summary>
    /// Gets or sets the token setting.
    /// </summary>
    public string Token { get; set; }
}
