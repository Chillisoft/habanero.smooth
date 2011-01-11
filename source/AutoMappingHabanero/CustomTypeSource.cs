using System;
using System.Collections.Generic;
using AutoMappingHabanero.ReflectionWrappers;

// ReSharper disable MemberCanBePrivate.Global
namespace AutoMappingHabanero
{
    public class CustomTypeSource : ITypeSource
    {
        private readonly IList<TypeWrapper> _types = new List<TypeWrapper>();

        public CustomTypeSource()
        {
        }

        public CustomTypeSource(IEnumerable<Type> types)
        {
            Add(types);
        }

        protected IList<TypeWrapper> Types
        {
            get { return _types; }
        }

        public IEnumerable<TypeWrapper> GetTypes()
        {
            return Types;
        }

        public void Add(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                Add(type);
            }
        }

        public void Add(Type type)
        {
            Types.Add(type.ToTypeWrapper());
        }

        public void Add<T>()
        {
            Types.Add(typeof (T).ToTypeWrapper());
        }
    }
    // ReSharper restore MemberCanBePrivate.Global
}