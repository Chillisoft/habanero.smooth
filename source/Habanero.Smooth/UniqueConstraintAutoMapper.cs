using System;
using System.Collections.Generic;
using System.Linq;
using Habanero.Base;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth.ReflectionWrappers;

namespace Habanero.Smooth
{
    public static class UniqueConstraintAutoMapperExtensions
    {
        public static IList<IKeyDef> MapUniqueConstraints(this IClassDef classDef)
        {
            if (classDef == null) return new List<IKeyDef>();
            UniqueConstraintAutoMapper autoMapper = new UniqueConstraintAutoMapper(classDef);

            return autoMapper.MapUniqueConstraints();
        }
    }

    public class UniqueConstraintAutoMapper
    {
        private IClassDef ClassDef { get; set; }
        private TypeWrapper _classType;

        public UniqueConstraintAutoMapper(IClassDef classDef)
        {
            if (classDef == null) throw new ArgumentNullException("classDef");
            ClassDef = classDef;
            _classType = this.ClassDef.ClassType.ToTypeWrapper();
        }

        public IList<IKeyDef> MapUniqueConstraints()
        {
            var ucNames = from propWrapper in _classType.GetProperties()
                          where propWrapper.HasAttribute<AutoMapUniqueConstraint>()
                          select propWrapper.GetAttribute<AutoMapUniqueConstraint>().UniqueConstraintName;

            //var keyDefsLinq = from propWrapper in _classType.GetProperties()
            //              where propWrapper.HasAttribute<AutoMapUniqueConstraint>()
            //              select (IKeyDef) new KeyDef(propWrapper.GetAttribute<AutoMapUniqueConstraint>().UniqueConstraintName);

            var keyDefs = ucNames.Distinct().ToList().ConvertAll(s => (IKeyDef)new KeyDef(s));

            keyDefs.ForEach(keyDef =>
                                {
                                    var propNames =
                                        from propWrapper in _classType.GetProperties()
                                        where propWrapper.HasAttribute<AutoMapUniqueConstraint>() &&
                                              propWrapper.GetAttribute<AutoMapUniqueConstraint>().UniqueConstraintName == keyDef.KeyName
                                        select propWrapper.Name;

                                    var propDefs =
                                        from propDef in ClassDef.PropDefcol
                                        where propNames.Contains(propDef.PropertyName)
                                        select propDef;

                                    propDefs.ToList().ForEach(keyDef.Add);
                                });

            return keyDefs;
        }
    }
}