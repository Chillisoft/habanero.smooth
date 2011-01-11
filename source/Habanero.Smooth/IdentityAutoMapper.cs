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
    public static class IdentityAutoMapperExtensions
    {
        public static IPrimaryKeyDef MapIdentity(this IClassDef classDef)
        {
            if (classDef == null) return null;
            IdentityAutoMapper autoMapper = new IdentityAutoMapper(classDef);

            return autoMapper.MapIdentity();
        }
    }
    public class IdentityAutoMapper
    {
        private TypeWrapper _classType;
        private IClassDef ClassDef { get; set; }

        public IdentityAutoMapper(IClassDef classDef)
        {
            ClassDef = classDef;
            if (classDef == null) throw new ArgumentNullException("classDef");
            _classType = this.ClassDef.ClassType.ToTypeWrapper();
        }

        public IPrimaryKeyDef MapIdentity()
        {
            IClassDef classDef = this.ClassDef;

            var primaryKeyDef = GetPrimaryKeyDef(classDef);
            if (primaryKeyDef == null)
            {
                IPropDef propDef = GetPrimaryKeyPropDef();
                if (propDef == null) return null;
                classDef.PrimaryKeyDef = new PrimaryKeyDef();
                classDef.PrimaryKeyDef.Add(propDef);
            }
            
            return classDef.PrimaryKeyDef;
        }

        private static IPrimaryKeyDef GetPrimaryKeyDef(IClassDef classDef)
        {
            var primaryKeyDef = classDef.PrimaryKeyDef;
            if(primaryKeyDef == null && classDef.SuperClassDef !=null)
            {
                primaryKeyDef = GetPrimaryKeyDef(classDef.SuperClassDef.SuperClassClassDef);
            }
            return primaryKeyDef;
        }


        private IPropDef GetPrimaryKeyPropDef()
        {
            IPropDef propDef = FindExistingPKPropDef() 
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

        public static INamingConventions PropNamingConvention
        {
            get
            {
                return ClassAutoMapper.PropNamingConvention;
            }

        }

    }

  
}

