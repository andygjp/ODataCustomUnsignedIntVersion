namespace UnsignedIntVersion.VersionFeature;

using System;
using Microsoft.AspNetCore.OData.Edm;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

public sealed class VersionTypeMapper : DefaultODataTypeMapper
{
    public override Type GetClrType(IEdmModel edmModel, IEdmType edmType, bool nullable,
        IAssemblyResolver assembliesResolver)
    {
        if (Feature.IsVersion(edmType))
        {
            return typeof(ulong);
        }

        return base.GetClrType(edmModel, edmType, nullable, assembliesResolver);
    }
}