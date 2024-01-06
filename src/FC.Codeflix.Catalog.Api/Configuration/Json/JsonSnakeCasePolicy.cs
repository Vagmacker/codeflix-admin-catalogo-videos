using System.Text.Json;
using FC.Codeflix.Catalog.Api.Configuration.Extension;

namespace FC.Codeflix.Catalog.Api.Configuration.Json;

public class JsonSnakeCasePolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
        => name.ToSnakeCase();
}