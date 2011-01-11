using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMappingHabanero.ReflectionWrappers;
using Habanero.Base;

namespace AutoMappingHabanero
{
    public class AssemblyTypeSource : ITypeSource
    {
        private readonly Func<Type, bool> _whereClause = _defaultWhereClause;
        private static readonly Func<Type, bool> _defaultWhereClause = type => true;
        private Assembly Assembly { get; set; }

        public AssemblyTypeSource(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");
            Assembly = assembly;
        }
        public AssemblyTypeSource(Assembly assembly, Func<Type, bool> whereClause)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");
            _whereClause = whereClause;
            if (_whereClause == null) _whereClause = _defaultWhereClause;
            Assembly = assembly;
        }
        /// <summary>
        /// sets the assembly to be the assembly that the Type type belongs to.
        /// </summary>
        /// <param name="type"></param>
        public AssemblyTypeSource(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            Assembly = type.Assembly;
        }

        public IEnumerable<TypeWrapper> GetTypes()
        {
            var desiredType = typeof (IBusinessObject);
            return Assembly.GetTypes()
                .Where(_whereClause)
                .Select(type1 => type1.ToTypeWrapper())
                .Where(type => desiredType.IsAssignableFrom(type.GetUnderlyingType()) && type.IsRealClass);
        }
    }
}