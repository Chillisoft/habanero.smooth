using System;
using System.Collections.Generic;
using System.Linq;
using Habanero.Base;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth.ReflectionWrappers;

namespace Habanero.Smooth
{
    ///<summary>
    /// Extension methods to make the syntax of Automapping unique costraints easier
    ///</summary>
    public static class UniqueConstraintAutoMapperExtensions
    {
        /// <summary>
        /// Automaps all the unique constrainst for the ClassType defined by the <paramref name="classDef"/> using reflection
        /// </summary>
        /// <param name="classDef"></param>
        /// <returns></returns>
        public static IList<IKeyDef> MapUniqueConstraints(this IClassDef classDef)
        {
            if (classDef == null) return new List<IKeyDef>();
            UniqueConstraintAutoMapper autoMapper = new UniqueConstraintAutoMapper(classDef);

            return autoMapper.MapUniqueConstraints();
        }
    }
    /// <summary>
    /// Automatically maps all unique constraints defined on a class using a the appropriate attributes
    /// (e.g. <see cref="AutoMapUniqueConstraintAttribute"/>) via reflection.
    /// </summary>
    public class UniqueConstraintAutoMapper
    {
        private IClassDef ClassDef { get; set; }
        private TypeWrapper _classType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classDef"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UniqueConstraintAutoMapper(IClassDef classDef)
        {
            if (classDef == null) throw new ArgumentNullException("classDef");
            ClassDef = classDef;
            _classType = this.ClassDef.ClassType.ToTypeWrapper();
        }
        /// <summary>
        /// Maps all the Unique Constraints for the Class.
        /// </summary>
        /// <returns></returns>
        public IList<IKeyDef> MapUniqueConstraints()
        {
            var ucNames = from propWrapper in _classType.GetProperties()
                          where propWrapper.HasUniqueConstraintAttribute && !propWrapper.IsInherited
                          select propWrapper.GetAttribute<AutoMapUniqueConstraintAttribute>().UniqueConstraintName;

            //var keyDefsLinq = from propWrapper in _classType.GetProperties()
            //              where propWrapper.HasAttribute<AutoMapUniqueConstraintAttribute>()
            //              select (IKeyDef) new KeyDef(propWrapper.GetAttribute<AutoMapUniqueConstraintAttribute>().UniqueConstraintName);

            var keyDefs = ucNames.Distinct().ToList().ConvertAll(s => (IKeyDef)new KeyDef(s));

            keyDefs.ForEach(keyDef =>
                                {
                                    var propNames =
                                        from propWrapper in _classType.GetProperties()
                                        where propWrapper.HasUniqueConstraintAttribute &&
                                              propWrapper.GetAttribute<AutoMapUniqueConstraintAttribute>().UniqueConstraintName == keyDef.KeyName
                                        select propWrapper.Name;

                                    var p = propNames.ToList();

                                    var propDefs =
                                        from propDef in ClassDef.PropDefcol
                                        where propNames.Contains(propDef.PropertyName)
                                        select propDef;

                                    propDefs.ToList().ForEach(keyDef.Add);

                                    var col = ClassDef.RelationshipDefCol;
                                    var rels = col.Where(def => propNames.Contains(def.RelationshipName));
                                    var props = rels.SelectMany(def => def.RelKeyDef.Select(propDef => ClassDef.PropDefcol[propDef.OwnerPropertyName]));
                                    props.ForEach(keyDef.Add);
                                });

            return keyDefs;
        }


        private static IPrimaryKeyDef GetPrimaryKeyDef(IClassDef classDef)
        {
            var primaryKeyDef = classDef.PrimaryKeyDef;
            if (primaryKeyDef == null && classDef.SuperClassDef != null)
            {
                primaryKeyDef = GetPrimaryKeyDef(classDef.SuperClassDef.SuperClassClassDef);
            }
            return primaryKeyDef;
        }
    }
}