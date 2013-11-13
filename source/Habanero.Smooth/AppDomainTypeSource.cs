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
using Habanero.Smooth.ReflectionWrappers;

namespace Habanero.Smooth
{
    /// <summary>
    /// 
    /// </summary>
    public class AppDomainTypeSource : ITypeSource
    {
        private Func<TypeWrapper, bool> Where { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="where"></param>
        public AppDomainTypeSource(Func<TypeWrapper, bool> where)
        {
            Where = where;
        }

        /// <summary>
        /// 
        /// </summary>
        public AppDomainTypeSource()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TypeWrapper> GetTypes()
        {
            return this.Where == null 
                    ? TypesImplementing() 
                    : TypesImplementing().Where(this.Where);
        }

        private static IEnumerable<TypeWrapper> TypesImplementing()
        {
            return AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Select(type1 => type1.ToTypeWrapper())
                .Where(type => type.IsBusinessObject && type.IsRealClass);
        }
    }
}