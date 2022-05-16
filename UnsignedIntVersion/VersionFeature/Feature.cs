namespace UnsignedIntVersion.VersionFeature;

using System.Linq;
using Microsoft.AspNetCore.OData.Edm;
using Microsoft.OData.Edm;

public static class Feature
{
    public static void AddVersionSupport(EdmModel edmModel)
    {
        var reference = VersionReference(edmModel);

        foreach (var type in edmModel.SchemaElements.OfType<EdmEntityType>())
        {
            var versionProperty = type.AddStructuralProperty(nameof(CustomCustomer.Version), reference);

            if (edmModel.EntityContainer.Elements.OfType<EdmEntitySet>()
                    .FirstOrDefault(x => x.EntityType().IsEquivalentTo(type)) is { } entitySet)
            {
                edmModel.SetOptimisticConcurrencyAnnotation(entitySet, new[] { versionProperty });
            }

            foreach (var derivedType in edmModel.FindDirectlyDerivedTypes(type).OfType<EdmEntityType>())
            {
                if (edmModel.EntityContainer.EntitySets()
                        .FirstOrDefault(x => x.EntityType().IsEquivalentTo(derivedType)) is not
                    { } derivedEntitySet) continue;

                edmModel.SetOptimisticConcurrencyAnnotation(derivedEntitySet, new[] { versionProperty });
            }
        }

        edmModel.SetTypeMapper(new VersionTypeMapper());
    }

    private static EdmTypeDefinitionReference VersionReference(EdmModel edmModel)
    {
        if (edmModel.FindDeclaredType($"Custom.{nameof(CustomCustomer.Version)}") is not IEdmTypeDefinition type)
        {
            type = new EdmTypeDefinition("Custom", nameof(CustomCustomer.Version),
                EdmPrimitiveTypeKind.Decimal);

            edmModel.AddElement(type);
        }

        return new EdmTypeDefinitionReference(type, 
            isNullable: false, 
            isUnbounded: false,
            maxLength: null,
            isUnicode: null, 
            precision: null, 
            scale: 0,
            spatialReferenceIdentifier: null);
    }

    internal static bool IsVersion(IEdmTypeReference edmType)
    {
        return IsVersion(edmType.Definition);
    }

    internal static bool IsVersion(IEdmType edmType)
    {
        return edmType.FullTypeName() == $"Custom.{nameof(CustomCustomer.Version)}";
    }
}