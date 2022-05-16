namespace UnsignedIntVersion.VersionFeature;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OData.Formatter.Serialization;
using Microsoft.OData;
using Microsoft.OData.Edm;

public sealed class VersionTypeSerializer : ODataEdmTypeSerializer
{
    public VersionTypeSerializer() : base(ODataPayloadKind.Property)
    {
    }

    public override Task WriteObjectInlineAsync(object graph, IEdmTypeReference expectedType, ODataWriter writer,
        ODataSerializerContext writeContext)
    {
        throw new NotImplementedException("Didn't expect this function would be required.");
    }

    public override ODataValue CreateODataValue(object graph, IEdmTypeReference expectedType,
        ODataSerializerContext writeContext)
    {
        return new ODataPrimitiveValue(Convert.ToDecimal((ulong)graph));
    }

    public override Task WriteObjectAsync(object graph, Type type, ODataMessageWriter messageWriter,
        ODataSerializerContext writeContext)
    {
        throw new NotImplementedException("Didn't expect this function would be required.");
    }
}