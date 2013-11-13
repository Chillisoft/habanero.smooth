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
using Habanero.Smooth.ReflectionWrappers;

namespace Habanero.Smooth
{
    /// <summary>
    /// This is an interface that for stubbing out standard naming conventions
    /// This can be used later to allow the user of this library to
    /// implement their own naming conventions.
    /// </summary>
    public interface INamingConventions
    {
        /// <summary>
        /// Based on the Type T return an IDPropName based on convention.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        string GetIDPropertyName<T>();
        /// <summary>
        /// Based on the Type wrapped by t return an IDPropName based on convention.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        string GetIDPropertyName(TypeWrapper t);
        /// <summary>
        /// Based on the relationshipName create a FKPropName i.e. the FKProp on the single side of the relationship.
        /// </summary>
        /// <param name="relationshipName"></param>
        /// <returns></returns>
        string GetSingleRelOwningPropName(string relationshipName);
    }

    ///<summary>
    /// The default naming conventions used by Chillisoft.
    ///</summary>
    public class DefaultPropNamingConventions : INamingConventions
    {    
        /// <summary>
        /// 
        /// </summary>
        public string GetIDPropertyName<T>()
        {
            var type = typeof (T).ToTypeWrapper();
            return GetIDPropertyName(type);
        }
        /// <summary>
        /// 
        /// </summary>
        public string GetIDPropertyName(TypeWrapper t)
        {

            return t.Name + "ID";
        }
        /// <summary>
        /// 
        /// </summary>
        public string GetSingleRelOwningPropName(string relationshipName)
        {
            return relationshipName + "ID";
        }
    }
}