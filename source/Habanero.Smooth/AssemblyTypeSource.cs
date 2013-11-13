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
using Habanero.Smooth.ReflectionWrappers;

namespace Habanero.Smooth
{
    /// <summary>
    /// 
    /// </summary>
    public class AssemblyTypeSource : ITypeSource
    {
        private readonly Func<Type, bool> _whereClause = _defaultWhereClause;
        private static readonly Func<Type, bool> _defaultWhereClause = type => true;
        private Assembly Assembly { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public AssemblyTypeSource(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");
            Assembly = assembly;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="whereClause"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public AssemblyTypeSource(Assembly assembly, Func<Type, bool> whereClause)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");
            _whereClause = whereClause;
            if (_whereClause == null) _whereClause = _defaultWhereClause;
            Assembly = assembly;
        }
        /// <summary>
        /// sets the assembly to be the assembly that the Type type belongs to.
        /// </summary>
        /// <param name="type"></param>
        public AssemblyTypeSource(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            Assembly = type.Assembly;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TypeWrapper> GetTypes()
        {
            var desiredType = typeof (IBusinessObject);
            return Assembly.GetTypes()
                .Where(_whereClause)
                .Select(type1 => type1.ToTypeWrapper())
                .Where(type => desiredType.IsAssignableFrom(type.UnderlyingType) && type.IsRealClass);
        }
    }
}