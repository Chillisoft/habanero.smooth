﻿// ---------------------------------------------------------------------------------
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

    public interface INamingConventions
    {
        string GetIDPropertyName<T>();
        string GetIDPropertyName(TypeWrapper t);
        string GetSingleRelOwningPropName(string relationshipName);
    }

    public class DefaultPropNamingConventions : INamingConventions
    {
        public string GetIDPropertyName<T>()
        {
            var type = typeof (T).ToTypeWrapper();
            return GetIDPropertyName(type);
        }

        public string GetIDPropertyName(TypeWrapper t)
        {
            return t.Name + "ID";
        }
        public string GetSingleRelOwningPropName(string relationshipName)
        {
            return relationshipName + "ID";
        }
    }
}