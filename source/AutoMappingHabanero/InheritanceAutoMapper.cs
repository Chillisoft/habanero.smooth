using System;
using System.Collections.Generic;
using System.Linq;
using AutoMappingHabanero.ReflectionWrappers;
using Habanero.Base;
using Habanero.BO.ClassDefinition;

namespace AutoMappingHabanero
{
    public static class InheritanceAutoMapperExtensions
    {
        public static ISuperClassDef MapInheritance(this Type type)
        {
            return type == null ? null : type.ToTypeWrapper().MapInheritance();
        }

        public static ISuperClassDef MapInheritance(this TypeWrapper typeWrapper)
        {
            if (typeWrapper.IsNull()) return null;
            InheritanceAutoMapper autoMapper = new InheritanceAutoMapper(typeWrapper);
            return autoMapper.Map();
        }
    }
    public class InheritanceAutoMapper
    {
        public InheritanceAutoMapper(TypeWrapper typeWrapper)
        {
            if (typeWrapper.IsNull()) throw new ArgumentNullException("typeWrapper");
            TypeWrapper = typeWrapper;
        }
        private TypeWrapper TypeWrapper { get; set; }

        public ISuperClassDef Map()
        {
            if (MustBeMapped())
            {
                var baseType = this.TypeWrapper.BaseType;

                var superClassInheritanceRel = baseType.MapInheritance();
                var superClassDef = baseType.MapClass();
                ISuperClassDef inheritanceDef;
                if (superClassDef != null)
                {
                    if (superClassInheritanceRel != null)
                    {
                        superClassDef.SuperClassDef = superClassInheritanceRel;
                    }
                    inheritanceDef = new SuperClassDef(superClassDef, ORMapping.SingleTableInheritance);

                    //If this is the Most Base Type i.e. it 
                    // does not have another Business object as its super class
                    // then you should create the discriminator Property
                    if (superClassInheritanceRel == null)
                    {
                        inheritanceDef.Discriminator = superClassDef.ClassName + "Type";
                        CreateDiscriminatorProp(inheritanceDef);
                    }else
                    {
                        inheritanceDef.Discriminator = superClassInheritanceRel.Discriminator;
                    }
                    return inheritanceDef;
                }

            }
            return null;
        }

        private static void CreateDiscriminatorProp(ISuperClassDef inheritanceClassDef)
        {
            IClassDef superClassClassDef = inheritanceClassDef.SuperClassClassDef;
            IPropDef foundPropDef = superClassClassDef.GetPropDef(inheritanceClassDef.Discriminator, false);
            if (foundPropDef != null) return;

            IPropDef propDef = new PropDef(inheritanceClassDef.Discriminator, typeof (String),
                                           PropReadWriteRule.WriteNew, null);
            superClassClassDef.PropDefcol.Add(propDef);
        }

        private bool MustBeMapped()
        {
            return this.TypeWrapper.MustBeMapped() 
                    && this.TypeWrapper.HasBaseType 
                    && !this.TypeWrapper.IsBaseTypeLayerSuperType ;
        }

//
//        private static INamingConventions PropNamingConvention
//        {
//            get
//            {
//                return AllClassesAutoMapper.PropNamingConvention;
//            }
//        }
    }
}