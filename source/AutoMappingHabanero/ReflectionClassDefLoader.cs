using System;
using Habanero.Base;
using Habanero.BO.Loaders;

namespace AutoMappingHabanero
{
    public class ReflectionClassDefLoader : IClassDefsLoader
    {
        private ITypeSource Source { get; set; }

        public ReflectionClassDefLoader(ITypeSource source)
        {
            if (source == null) throw new ArgumentNullException("source");
            Source = source;
        }

        public ClassDefCol LoadClassDefs(string classDefsXml)
        {
            throw new NotImplementedException();
        }

        public ClassDefCol LoadClassDefs()
        {
            AllClassesAutoMapper allClassesAutoMapper = new AllClassesAutoMapper(Source);
            return allClassesAutoMapper.Map();
        }
    }
}