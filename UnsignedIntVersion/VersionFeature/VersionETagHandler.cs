namespace UnsignedIntVersion.VersionFeature;

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.OData.Abstracts;
using Microsoft.Net.Http.Headers;

public sealed class VersionETagHandler : IETagHandler
{
    public IDictionary<string, object> ParseETag(EntityTagHeaderValue etagHeaderValue)
    {
        if (ParseVersion(etagHeaderValue) is { } version)
        {
            // An empty or whitespace will fail to parse
            var value = Convert.ToDecimal(version);
            return new Dictionary<string, object> { ["0"] = Convert.ToUInt64(value) };
        }

        return new Dictionary<string, object>();
    }

    private static string? ParseVersion(EntityTagHeaderValue etagHeaderValue)
    {
        var base64 = etagHeaderValue.Tag.ToString().Trim('\"');
        var bytes = Convert.FromBase64String(base64);
        var tag = Encoding.UTF8.GetString(bytes);
        return tag.StartsWith("version'") ? tag[8..^1] : null;
    }

    public EntityTagHeaderValue? CreateETag(IDictionary<string, object> properties, TimeZoneInfo? timeZoneInfo = null)
    {
        return properties switch
        {
            { Count: 0 } => null,
            { Count: 1 } when TryGetVersion(properties) is (true, { } x) => CreateVersionETag(x),
            _ => throw new ArgumentException(
                "Expected either no concurrency properties or a single property with the correct type.",
                nameof(properties))
        };
    }

    private static (bool, ulong?) TryGetVersion(IDictionary<string, object> properties)
    {
        return properties.TryGetValue(nameof(CustomCustomer.Version), out var v)
            ? (true, Convert.ToUInt64(v))
            : (false, null);
    }

    private static EntityTagHeaderValue CreateVersionETag(ulong value)
    {
        var tag = $"version'{Convert.ToDecimal(value)}'";
        var bytes = Encoding.UTF8.GetBytes(tag);
        var base64 = Convert.ToBase64String(bytes);
        return new EntityTagHeaderValue($"\"{base64}\"", isWeak: true);
    }
}