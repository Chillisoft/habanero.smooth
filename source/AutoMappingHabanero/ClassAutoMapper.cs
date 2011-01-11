using System;
using System.Collections.Generic;
using System.Linq;
using AutoMappingHabanero.ReflectionWrappers;
using Habanero.Base;
using Habanero.BO.ClassDefinition;
using Habanero.Util;

namespace AutoMappingHabanero
{
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
                var underlyingType = this.TypeWrapper.GetUnderlyingType();
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
            Type type = this.TypeWrapper.GetUnderlyingType();
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
            var classType = this.TypeWrapper.GetUnderlyingType();
            return classType.GetProperties().Select(info => info.MapProperty()).Where(propDef => propDef != null);
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
