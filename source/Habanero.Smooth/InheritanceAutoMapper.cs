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
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth.ReflectionWrappers;

namespace Habanero.Smooth
{
    /// <summary>
    /// Simple Extension methods to make the code for 
    /// automapping extension methods more fluid.
    /// </summary>
    public static class InheritanceAutoMapperExtensions
    {
        /// <summary>
        /// Maps any Inhertiance Relationships for the <paramref name="type"/>.
        /// and returns the <see cref="ISuperClassDef"/> that is used to 
        /// map these in Habanero.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ISuperClassDef MapInheritance(this Type type)
        {
            return type == null ? null : type.ToTypeWrapper().MapInheritance();
        }
        /// <summary>
        /// Maps any Inhertiance Relationships for the <paramref name="typeWrapper"/>.
        /// and returns the <see cref="ISuperClassDef"/> that is used to 
        /// map these in Habanero.
        /// </summary>
        public static ISuperClassDef MapInheritance(this TypeWrapper typeWrapper)
        {
            if (typeWrapper.IsNull()) return null;
            InheritanceAutoMapper autoMapper = new InheritanceAutoMapper(typeWrapper);
            return autoMapper.Map();
        }
    }
    /// <summary>
    /// Automatically maps an inheritance Relationship.
    /// based on the heuristic of ChildClass: ParentClass
    /// where the ChildClass and ParentClass are both <see cref="BusinessObject"/>s.
    /// The Inhertiance Relationship is always taken as Single Table.
    /// 
    /// </summary>
    public class InheritanceAutoMapper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeWrapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public InheritanceAutoMapper(TypeWrapper typeWrapper)
        {
            if (typeWrapper.IsNull()) throw new ArgumentNullException("typeWrapper");
            TypeWrapper = typeWrapper;
        }
        private TypeWrapper TypeWrapper { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
                    && !this.TypeWrapper.IsBaseTypeBusinessObject ;
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