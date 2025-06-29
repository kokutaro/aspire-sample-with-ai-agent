using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyAspireApp.Domain.Common;

namespace MyAspireApp.Infrastructure.Converters;

public class StronglyTypedIdConverter<TStronglyTypedId>(ConverterMappingHints? mappingHints = null) : ValueConverter<TStronglyTypedId, Guid>(
        id => id.Value,
        value => (TStronglyTypedId)Activator.CreateInstance(typeof(TStronglyTypedId), value)!,
        mappingHints)
    where TStronglyTypedId : StronglyTypedId<Guid>
{
}
