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
        /// <summary>
        /// Maps the Class of <paramref name="type"/> to a ClassDef.
        /// Note_ this willl not create the reverse relationships on 
        /// the Related Class if they are required. If you require reverse relationships to be set up then please
        /// use the <see cref="AllClassesAutoMapper"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IClassDef MapClass(this Type type)
        {
            return type == null ? null : type.ToTypeWrapper().MapClass();
        }
        /// <summary>
        /// Maps the Class of <paramref name="typeWrapper"/> to a ClassDef.
        /// Note_ this willl not create the reverse relationships on 
        /// the Related Class if they are required. If you require reverse relationships to be set up then please
        /// use the <see cref="AllClassesAutoMapper"/>
        /// </summary>
        /// <param name="typeWrapper"></param>
        /// <returns></returns>
        public static IClassDef MapClass(this TypeWrapper typeWrapper)
        {
            if (typeWrapper.IsNull()) return null;
            ClassAutoMapper autoMapper = new ClassAutoMapper(typeWrapper);
            return autoMapper.Map();
        }
        /// <summary>
        /// Must the Class wrapped by the <paramref name="typeWrapper"/>
        /// be mapped.
        /// </summary>
        /// <param name="typeWrapper"></param>
        /// <returns></returns>
        public static bool MustBeMapped(this TypeWrapper typeWrapper)
        {
            if (typeWrapper.IsNull()) return false;
            ClassAutoMapper autoMapper = new ClassAutoMapper(typeWrapper);
            return autoMapper.MustBeMapped();
        }
        /// <summary>
        /// Must the Class wrapped by the <paramref name="type"/>
        /// be mapped.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool MustBeMapped(this Type type)
        {
            return type != null && type.ToTypeWrapper().MustBeMapped();
        }
    }
    /// <summary>
    /// Automatically Maps the Class identified by a <see cref="TypeWrapper"/>
    /// to a ClassDefinition <see cref="IClassDef"/>
    /// </summary>
    public class ClassAutoMapper
    {
        ///<summary>
        /// Constructs a ClassAutoMapper.
        ///</summary>
        ///<param name="type"></param>
        ///<exception cref="ArgumentNullException"></exception>
        public ClassAutoMapper(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            TypeWrapper = type.ToTypeWrapper();
        }
        ///<summary>
        /// Constructs a ClassAutoMapper.
        ///</summary>
        ///<param name="typeWrapper"></param>
        ///<exception cref="ArgumentNullException"></exception>
        public ClassAutoMapper(TypeWrapper typeWrapper)
        {
            if (typeWrapper.IsNull()) throw new ArgumentNullException("typeWrapper");
            this.TypeWrapper = typeWrapper;
        }
        private TypeWrapper TypeWrapper { get; set; }
        /// <summary>
        /// Maps the type wrapped by the <see cref="ReflectionWrappers.TypeWrapper"/>
        /// to a ClassDef.
        /// NNB: This only maps this Class it will not try to Create or map relationships from related classes.
        /// This method is primarily for testing etc normally you would be mapping using the <see cref="AllClassesAutoMapper"/>
        /// </summary>
        /// <returns></returns>
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
                this.ClassDef = CreateClassDef();
                MapSuperClassHierarchy();
                MapProperties();
                
                this.ClassDef.MapIdentity();

                MapManyToOneRelationships();
                MapOneToManyRelationships();
                MapOneToOneRelationships();

                MapUniqueConstraints();

                return this.ClassDef;
            }
            return null;
        }

        private void MapUniqueConstraints()
        {
            foreach (var keyDef in ClassDef.MapUniqueConstraints())
            {
                this.ClassDef.KeysCol.Add(keyDef);    
            }
        }

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
            var tableName = this.TypeWrapper.GetTableName();
            if (tableName != null) this.ClassDef.TableName = tableName;
            return this.ClassDef;
        }

        private bool HasPropDef(string propertyName)
        {
            var propDef = this.ClassDef.GetPropDef(propertyName, false);
            return (propDef != null);
        }

        private void MapProperties()
        {
            var classDef = this.ClassDef;
            var propDefs = MapPropDefs();
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

        // ReSharper disable UnusedMember.Local
        //used by tests. Pls do not delete
        private void MapRelDefs()
        {
            MapManyToOneRelationships();
            MapOneToManyRelationships();
            MapOneToOneRelationships();
        }
        // ReSharper restore UnusedMember.Local

        private void MapRelationships(Func<PropertyWrapper, IRelationshipDef> mappingExpression)
        {
            var relDefs = GetRelDefs(mappingExpression).ToList();
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
            var properties = this.TypeWrapper.GetProperties();
            var mapManyToOneRelDefs = properties.Select(mapSelector).Where(relDef => relDef != null);
            return mapManyToOneRelDefs;
        }

        private  IEnumerable<IPropDef> MapPropDefs()
        {
            var classType = this.TypeWrapper.UnderlyingType;
            return classType.GetProperties()
                .Select(info => info.MapProperty())
                .Where(propDef => propDef != null);
        }



        private IClassDef ClassDef { get; set; }
        /// <summary>
        /// Returns the PropNaming Convention that is being used for this Mapping.
        /// </summary>
        public static INamingConventions PropNamingConvention
        {
            get
            {
                return AllClassesAutoMapper.PropNamingConvention;
            }
        }
    }
    /// <summary>
    /// A set of extension methods that can be used to make it easier to add and edit A ClassDef.
    /// </summary>
    public static class ClassDefExtensions
    {
        /// <summary>
        /// Adds a set of <see cref="IPropDef"/>s to a <see cref="IClassDef"/>
        /// </summary>
        /// <param name="classDef"></param>
        /// <param name="propDefs"></param>
        public static void AddPropDefs(this IClassDef classDef, IEnumerable<IPropDef> propDefs)
        {
            foreach (var propDef in propDefs)
            {
                classDef.PropDefcol.Add(propDef);
                propDef.ClassDef = classDef;
            }
        }
        /// <summary>
        /// Adds a set of <see cref="IRelationshipDef"/> to an <see cref="IClassDef"/>
        /// </summary>
        /// <param name="classDef"></param>
        /// <param name="relDefs"></param>
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
