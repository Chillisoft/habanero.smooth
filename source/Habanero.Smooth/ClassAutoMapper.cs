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
using Habanero.Base;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth.ReflectionWrappers;
using Habanero.Util;

namespace Habanero.Smooth
{
    /// <summary>
    /// Extension method so that the code for automappin
    /// an individual class can b more fluent.
    /// </summary>
    public static class ClassAutoMapperExtensions
    {
        public static IClassDef MapClass(this Type type)
        {
            return type == null ? null : type.ToTypeWrapper().MapClass();
        }

        public static IClassDef MapClass(this TypeWrapper typeWrapper)
        {
            if (typeWrapper.IsNull()) return null;
            ClassAutoMapper autoMapper = new ClassAutoMapper(typeWrapper);
            return autoMapper.Map();
        }
        public static bool MustBeMapped(this TypeWrapper typeWrapper)
        {
            if (typeWrapper.IsNull()) return false;
            ClassAutoMapper autoMapper = new ClassAutoMapper(typeWrapper);
            return autoMapper.MustBeMapped();
        }
        public static bool MustBeMapped(this Type type)
        {
            return type != null && type.ToTypeWrapper().MustBeMapped();
        }
    }
    public class ClassAutoMapper
    {
        public ClassAutoMapper(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            TypeWrapper = type.ToTypeWrapper();
        }
        public ClassAutoMapper(TypeWrapper typeWrapper)
        {
            if (typeWrapper.IsNull()) throw new ArgumentNullException("typeWrapper");
            TypeWrapper = typeWrapper;
        }
        private TypeWrapper TypeWrapper { get; set; }

        public IClassDef Map()
        {
            if (MustBeMapped())
            {
                var classDefCol = AllClassesAutoMapper.ClassDefCol;
                var underlyingType = this.TypeWrapper.UnderlyingType;
                if(classDefCol.Contains(underlyingType))
                {
                    return classDefCol[underlyingType];
                }
                ClassDef = CreateClassDef();
                MapSuperClassHierarchy();
                MapProperties();
                ClassDef.MapIndentity();

                MapManyToOneRelationships();
                MapOneToManyRelationships();
                MapOneToOneRelationships();
                return ClassDef;
            }
            return null;
        }
/*        /// <summary>
        /// This does automapping where only the properties are mapped.
        /// </summary>
        /// <returns></returns>
        public IClassDef MapPropertiesOnly()
        {
            if (MustBeMapped())
            {
                ClassDef = CreateClassDef();
                MapProperties();
                ClassDef.MapIndentity();
                return ClassDef;
            }
            return null;
        }*/
        private void MapSuperClassHierarchy()
        {
            var inheritanceRelationship = this.TypeWrapper.MapInheritance();
            this.ClassDef.SuperClassDef = inheritanceRelationship;
        }

        internal bool MustBeMapped()
        {
            return this.TypeWrapper.IsBusinessObject 
                    && !this.TypeWrapper.HasIgnoreAttribute 
                    && this.TypeWrapper.IsRealClass;
        }

        private IClassDef CreateClassDef()
        {
            Type type = this.TypeWrapper.UnderlyingType;
            this.ClassDef = new ClassDef(type, null, new PropDefCol(), new KeyDefCol(), new RelationshipDefCol(), new UIDefCol());
            return this.ClassDef;
        }

        private bool HasPropDef(string propertyName)
        {
            var propDef = this.ClassDef.GetPropDef(propertyName, false);
            return (propDef != null);
        }

        private void MapProperties()
        {
            IClassDef classDef = this.ClassDef;
            IEnumerable<IPropDef> propDefs = MapPropDefs();
            classDef.AddPropDefs(propDefs);
        }

        private  void CreateFKKeyProp(IRelationshipDef relationshipDef)
        {
            var propertyName = PropNamingConvention.GetSingleRelOwningPropName(relationshipDef.RelationshipName);
            IPropDef propDef = new PropDef(propertyName, typeof(Guid?), PropReadWriteRule.ReadWrite, null)
                                   {Compulsory = relationshipDef.IsCompulsory};
            this.ClassDef.PropDefcol.Add(propDef);
        }

        private void MapManyToOneRelationships()
        {
            MapRelationships(info => info.MapManyToOne());
        }

        private void MapOneToManyRelationships()
        {
            MapRelationships(info => info.MapOneToMany());
        }
        private void MapOneToOneRelationships()
        {
            MapRelationships(info => info.MapOneToOne());
        }

        private void MapRelationships(Func<PropertyWrapper, IRelationshipDef> mappingExpression)
        {
            IEnumerable<IRelationshipDef> relDefs = GetRelDefs(mappingExpression);
            CreateOwningPropIfRequired(relDefs);
            this.ClassDef.AddRelationshipDefs(relDefs);
        }

        private void CreateOwningPropIfRequired(IEnumerable<IRelationshipDef> relDefs)
        {
            var relationshipDefs = from relDef in relDefs
                       let relKeyDef = relDef.RelKeyDef
                       from relPropDef in relKeyDef.Where(relPropDef => !HasPropDef(relPropDef.OwnerPropertyName))
                       select relDef;
            foreach (var relDef in relationshipDefs)
            {
                CreateFKKeyProp(relDef);
            }
        }

        private  IEnumerable<IRelationshipDef> GetRelDefs(Func<PropertyWrapper, IRelationshipDef> mapSelector)
        {
            IEnumerable<PropertyWrapper> properties = this.TypeWrapper.GetProperties();
            var mapManyToOneRelDefs = properties.Select(mapSelector).Where(relDef => relDef != null);
            return mapManyToOneRelDefs;
        }

//        private void SetRelationshipsOwningClassDef(IEnumerable<IRelationshipDef> mapManyToOneRelDefs)
//        {
//            foreach (RelationshipDef relationshipDef in mapManyToOneRelDefs)
//            {
//                ReflectionUtilities.SetPropertyValue(relationshipDef, "OwningClassDef", this.ClassDef);
//            }
//        }

        private  IEnumerable<IPropDef> MapPropDefs()
        {
            var classType = this.TypeWrapper.UnderlyingType;
            return classType.GetProperties()
                .Select(info => info.MapProperty())
                .Where(propDef => propDef != null);
        }



        private IClassDef ClassDef { get; set; }

        public static INamingConventions PropNamingConvention
        {
            get
            {
                return AllClassesAutoMapper.PropNamingConvention;
            }
        }
    }

    public static class ClassDefExtensions
    {
        public static void AddPropDefs(this IClassDef classDef, IEnumerable<IPropDef> propDefs)
        {
            foreach (var propDef in propDefs)
            {
                classDef.PropDefcol.Add(propDef);
                propDef.ClassDef = classDef;
            }
        }

        public static void AddRelationshipDefs(this IClassDef classDef, IEnumerable<IRelationshipDef> relDefs)
        {
            foreach (var relDef in relDefs.Where(propDef => propDef != null))
            {
                relDef.OwningClassDef = classDef;
                classDef.RelationshipDefCol.Add(relDef);
            }
        }
    }
}
