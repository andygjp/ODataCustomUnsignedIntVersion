namespace UnsignedIntVersion.VersionFeature;

using System;
using Microsoft.AspNetCore.OData.Formatter.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;

public sealed class VersionSerializerProvider : ODataSerializerProvider
{
    private readonly IServiceProvider serviceProvider;

    public VersionSerializerProvider(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public override IODataEdmTypeSerializer GetEdmTypeSerializer(IEdmTypeReference edmType)
    {
        if (Feature.IsVersion(edmType))
        {
            return serviceProvider.GetRequiredService<VersionTypeSerializer>();
        }

        return base.GetEdmTypeSerializer(edmType);
    }
}