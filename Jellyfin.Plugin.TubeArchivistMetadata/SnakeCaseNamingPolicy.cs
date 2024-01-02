using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Jellyfin.Plugin.TubeArchivistMetadata;

/// <summary>
/// Snake case naming for json.
/// </summary>
public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    /// <summary>
    /// Snake case naming for json.
    /// </summary>
    /// <param name="name">the string to be converted.</param>
    /// <returns>pascal case string.</returns>
    public override string ConvertName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return name;
        }

        var builder = new StringBuilder();

        for (int i = 0; i < name.Length; i++)
        {
            char ch = name[i];

            if (i == 0 && char.IsUpper(ch))
            {
                builder.Append(char.ToLower(ch, CultureInfo.InvariantCulture));
                continue;
            }

            if (char.IsUpper(ch) && name[i - 1] != '_')
            {
                builder.Append('_');
                builder.Append(char.ToLower(ch, CultureInfo.InvariantCulture));
            }
            else
            {
                builder.Append(ch);
            }
        }

        return builder.ToString();
    }
}
