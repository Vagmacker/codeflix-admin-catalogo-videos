using Newtonsoft.Json.Serialization;

namespace FC.Codeflix.Catalog.Api.Configuration.Extension;

public static class StringSnakeCaseExtension
{
    private static readonly NamingStrategy SnakeCaseNamingStrategy = new SnakeCaseNamingStrategy();

    public static string ToSnakeCase(this string stringToConvert)
    {
        ArgumentNullException.ThrowIfNull(stringToConvert, nameof(stringToConvert));
        return SnakeCaseNamingStrategy.GetPropertyName(stringToConvert, false);
    }
}