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
using System.Reflection;
using Habanero.Base;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth.ReflectionWrappers;
using Habanero.Util;

namespace Habanero.Smooth
{
    /// <summary>
    /// Extension methods developed for syntactic sugar to allow you to do Identity Mapping easier
    /// </summary>
    public static class IdentityAutoMapperExtensions
    {
        /// <summary>
        /// gets the Primary Key Def for a ClassDef
        /// </summary>
        /// <param name="classDef"></param>
        /// <returns></returns>
        public static IPrimaryKeyDef MapIdentity(this IClassDef classDef)
        {
            if (classDef == null) return null;
            var autoMapper = new IdentityAutoMapper(classDef);

            return autoMapper.MapIdentity();
        }
    }

    /// <summary>
    /// Maps the Identity 
    /// </summary>
    public class IdentityAutoMapper
    {
        private readonly TypeWrapper _classType;
        private IClassDef ClassDef { get; set; }

        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="classDef"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public IdentityAutoMapper(IClassDef classDef)
        {
            ClassDef = classDef;
            if (classDef == null) throw new ArgumentNullException("classDef");
            _classType = this.ClassDef.ClassType.ToTypeWrapper();
        }

        /// <summary>
        /// Map the Identity for the given ClassDef
        /// </summary>
        /// <returns></returns>
        public IPrimaryKeyDef MapIdentity()
        {
            var classDef = this.ClassDef;

            var primaryKeyDef = GetPrimaryKeyDef(classDef);
            if (primaryKeyDef == null)
            {
                var propDef = GetPrimaryKeyPropDef();
                if (propDef == null) return null;
                var keyDef = new PrimaryKeyDef {propDef};

                keyDef.IsGuidObjectID = IsGuidObjectID(propDef);
                classDef.PrimaryKeyDef = keyDef;
            }

            return classDef.PrimaryKeyDef;
        }

        private static bool IsGuidObjectID(IPropDef propDef)
        {
            return propDef.PropertyType == typeof (Guid);
        }

        /// <summary>
        /// Gets the Primary Key def from this ClassDef or one of its Super Class Defs
        /// </summary>
        /// <param name="classDef"></param>
        /// <returns></returns>
        private static IPrimaryKeyDef GetPrimaryKeyDef(IClassDef classDef)
        {
            var primaryKeyDef = classDef.PrimaryKeyDef;
            if (primaryKeyDef == null && classDef.SuperClassDef != null)
            {
                primaryKeyDef = GetPrimaryKeyDef(classDef.SuperClassDef.SuperClassClassDef);
            }
            return primaryKeyDef;
        }


        private IPropDef GetPrimaryKeyPropDef()
        {
            var propDef = FindExistingPKPropDef()
                          ?? CreatePrimaryKeyProp();

            propDef.ReadWriteRule = PropReadWriteRule.WriteNew;
            propDef.Compulsory = true;
            return propDef;
        }

        private IPropDef FindExistingPKPropDef()
        {
            var pkPropName = _classType.GetPKPropName();
            return this.ClassDef.GetPropDef(pkPropName, false);
        }

        private IPropDef CreatePrimaryKeyProp()
        {
            var propertyName = PropNamingConvention.GetIDPropertyName(_classType);
            IPropDef propDef = new PropDef(propertyName, typeof (Guid), PropReadWriteRule.WriteNew, null);
            this.ClassDef.PropDefcol.Add(propDef);
            return propDef;
        }

        /// <summary>
        /// The Naming convention being used for AutoMapping
        /// </summary>
        public static INamingConventions PropNamingConvention
        {
            get { return ClassAutoMapper.PropNamingConvention; }
        }
    }
}