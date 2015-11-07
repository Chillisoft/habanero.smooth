// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2010 Chillisoft Solutions
//  
//  This file is part of the Habanero framework.
//  
//      Habanero is a free framework: you can redistribute it and/or modify
//      it under the terms of the GNU Lesser General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      The Habanero framework is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU Lesser General Public License for more details.
//  
//      You should have received a copy of the GNU Lesser General Public License
//      along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
// ---------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Habanero.Base;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth.ReflectionWrappers;

namespace Habanero.Smooth
{
    /// <summary>
    /// Extension methods used so that a more smooth syntax can be used when
    /// coding and using Habanero.Smooth.
    /// </summary>
    public static class AllClassesAutoMapperExtensions
    {
        /// <summary>
        /// Maps All classes in the Given Assembly to a <see cref="ClassDefCol"/>
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static ClassDefCol MapClasses(this Assembly assembly)
        {
            AllClassesAutoMapper.ClassDefCol = null;
            AssemblyTypeSource typeSource = new AssemblyTypeSource(assembly);
            AllClassesAutoMapper autoMapper = new AllClassesAutoMapper(typeSource);
            return autoMapper.Map();
        }

        /// <summary>
        /// Maps all classes in the Assembly for the given type <see cref="ClassDefCol"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ClassDefCol MapClasses(this Type type)
        {
            return type.Assembly.MapClasses();
        }

        /// <summary>
        /// Maps all classes in the Assembly for the given type <see cref="ClassDefCol"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="where">a valid where clause used to limit the Classes being mapped</param>
        /// <returns></returns>
        public static ClassDefCol MapClasses(this Type type, Func<Type, bool> where)
        {
            return type.Assembly.MapClasses(where);
        }

        private static ClassDefCol MapClasses(this Assembly assembly, Func<Type, bool> where)
        {
            AllClassesAutoMapper.ClassDefCol = null;
            AssemblyTypeSource typeSource = new AssemblyTypeSource(assembly, where);
            AllClassesAutoMapper autoMapper = new AllClassesAutoMapper(typeSource);
            return autoMapper.Map();
        }


/*        /// <summary>
        /// Maps all classes in the Assembly 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ClassDefCol MapClasses<T>()
        {
            return (typeof(T)).MapClasses();
        }*/

    }
    /// <summary>
    /// Maps all Business Objects in an ITypeSource to a <see cref="IClassDef"/>
    /// </summary>
    public class AllClassesAutoMapper
    {
        private ITypeSource Source { get; set; }
        /// <summary>
        /// Constructs the AllClassesAutoMapper with a specified Source.
        /// </summary>
        /// <param name="source"></param>
        public AllClassesAutoMapper(ITypeSource source)
        {
            if (source == null) throw new ArgumentNullException("source");
            Source = source;
        }
        /// <summary>
        /// Maps the <see cref="IBusinessObject"/> classes in the <see cref="ITypeSource"/>.
        /// </summary>
        /// <returns></returns>
        /// 
        public ClassDefCol Map()
        {
            var typesToBeMapped = Source.GetTypes().Where(type => type.MustBeMapped());
            var classDefsMapped = typesToBeMapped.Select(MapAndStoreClassDefFor).ToList();
            MapAllReverseRelationships(classDefsMapped);
            return ClassDefCol;
        }

        private static IClassDef MapAndStoreClassDefFor(TypeWrapper type)
        {
            var classDef = type.MapClass();
            MergeClassDefs(classDef);
            return classDef;
        }

        private static void MergeClassDefs(IClassDef classDef)
        {
            if (classDef.SuperClassDef != null)
            {
                //You always want the classDef that has been
                // Mapped via its subClass
                var superClassClassDef = classDef.SuperClassDef.SuperClassClassDef;
                MergeClassDefs(superClassClassDef);
            }
            if (!ClassDefCol.Contains(classDef.ClassType))
            {
                ClassDefCol.Add(classDef);
            }
        }

        private static void MapAllReverseRelationships(IEnumerable<IClassDef> classDefsMapped)
        {
            foreach (var classDef in classDefsMapped)
            {
                CreateReverseRelationshipDefs(ClassDefCol, classDef);
            }
        }

        private static void CreateReverseRelationshipDefs(ClassDefCol classDefCol, IClassDef classDef)
        {
            foreach (var relationship in classDef.RelationshipDefCol)
            {
                CreateReverseRelationship(classDefCol, classDef, relationship);
            }
        }

        ///<summary>
        /// Creates a Reverse Relationship when required.
        ///</summary>
        ///<param name="classDefCol"></param>
        ///<param name="classDef"></param>
        ///<param name="relationship"></param>
        ///<returns></returns>
        public static IRelationshipDef CreateReverseRelationship(ClassDefCol classDefCol, IClassDef classDef, IRelationshipDef relationship)
        {
            IRelationshipDef rel = relationship;
            if (!ContainsRelatedClass(relationship, classDefCol)) return null;

            IClassDef relatedClassDef = RelatedObjectClassDef(classDefCol, relationship);
            bool foundReverseRelationship = relatedClassDef.RelationshipDefCol.Any(
                def => def.RelationshipName == rel.ReverseRelationshipName);

            if (foundReverseRelationship) return null;

            IRelationshipDef newReverseRelDef = CreateReverseRelDef(rel, classDef);

            
            relatedClassDef.RelationshipDefCol.Add(newReverseRelDef);
            IRelPropDef relPropDef = relationship.RelKeyDef.FirstOrDefault();
            if (relPropDef != null)
            {
                var reverseRelPropDef = new RelPropDef(relPropDef.RelatedClassPropName, relPropDef.OwnerPropertyName);
                newReverseRelDef.RelKeyDef.Add(reverseRelPropDef);
                bool hasPropDef = relatedClassDef.PropDefColIncludingInheritance.Any(
                    propDef => propDef.PropertyName == reverseRelPropDef.OwnerPropertyName);
                if (!hasPropDef)
                {
                    var fkPropDef = new PropDef(reverseRelPropDef.OwnerPropertyName, typeof (Guid), PropReadWriteRule.ReadWrite, null);
                    relatedClassDef.PropDefcol.Add(fkPropDef);
                }
            }
            return newReverseRelDef;
        }

        private static IRelationshipDef CreateReverseRelDef(IRelationshipDef rel, IClassDef classDef)
        {
            IRelationshipDef newReverseRelDef;
            if (rel.IsManyToOne)
            {
                newReverseRelDef = new MultipleRelationshipDef(rel.ReverseRelationshipName
                                                               , classDef.ClassType, new RelKeyDef(), true, ""
                                                               , DeleteParentAction.Prevent);
            }else
            {
                newReverseRelDef = new SingleRelationshipDef(rel.ReverseRelationshipName
                                                             , classDef.ClassType, new RelKeyDef(), true
                                                             , DeleteParentAction.DoNothing);
            }
            newReverseRelDef.ReverseRelationshipName = rel.RelationshipName;
            return newReverseRelDef;
        }

        private static bool ContainsRelatedClass(IRelationshipDef relationship, ClassDefCol classDefCol)
        {
            return classDefCol.Contains(relationship.RelatedObjectAssemblyName, relationship.RelatedObjectClassName);
        }

        /// <summary>
        /// The <see cref="ClassDef"/> for the related object.
        /// </summary>
        private static IClassDef RelatedObjectClassDef(ClassDefCol classDefCol, IRelationshipDef relationshipDef)
        {
            return classDefCol[relationshipDef.RelatedObjectAssemblyName, relationshipDef.RelatedObjectClassNameWithTypeParameter];
           
        }
        private static INamingConventions _propNameConvention;
        /// <summary>
        /// Returns the PropNaming Convention that is being used for this Mapping.
        /// </summary>
        public static INamingConventions PropNamingConvention
        {
            get
            {
                return _propNameConvention ??
                       (_propNameConvention = new DefaultPropNamingConventions());
            }
            set { _propNameConvention = value; }
        }

        private static ClassDefCol _classDefCol;

        /// <summary>
        /// The preixisting ClassDef that is being used for this mapping.
        /// </summary>
        public static ClassDefCol ClassDefCol
        {
            get { return _classDefCol ?? (_classDefCol = new ClassDefCol()); }
            set { _classDefCol = value; }
        }
    }
}