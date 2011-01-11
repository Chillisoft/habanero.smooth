using System.Collections.Generic;
using AutoMappingHabanero.ReflectionWrappers;

namespace AutoMappingHabanero
{
    /// <summary>
    /// A source for Type instances, acts as a facade for an Assembly or as an alternative Type provider.
    /// </summary>
    public interface ITypeSource
    {
        IEnumerable<TypeWrapper> GetTypes();
    }
}