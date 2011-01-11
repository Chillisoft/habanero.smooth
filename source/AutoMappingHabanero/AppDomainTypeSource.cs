using System;
using System.Collections.Generic;
using System.Linq;
using AutoMappingHabanero.ReflectionWrappers;
using Habanero.Base;

namespace AutoMappingHabanero
{
    public class AppDomainTypeSource : ITypeSource
    {
        private Func<TypeWrapper, bool> Where { get; set; }

        public AppDomainTypeSource(Func<TypeWrapper, bool> where)
        {
            Where = where;
        }

        public AppDomainTypeSource()
        {
        }

        public IEnumerable<TypeWrapper> GetTypes()
        {
            return this.Where == null 
                    ? TypesImplementing() 
                    : TypesImplementing().Where(this.Where);
        }

        private static IEnumerable<TypeWrapper> TypesImplementing()
        {
            return AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Select(type1 => type1.ToTypeWrapper())
                .Where(type => type.IsBusinessObject && type.IsRealClass);
        }
    }
}