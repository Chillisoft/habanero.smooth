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
using Habanero.Smooth.ReflectionWrappers;

// ReSharper disable MemberCanBePrivate.Global
namespace Habanero.Smooth
{
    public class CustomTypeSource : ITypeSource
    {
        private readonly IList<TypeWrapper> _types = new List<TypeWrapper>();

        public CustomTypeSource()
        {
        }

        public CustomTypeSource(IEnumerable<Type> types)
        {
            Add(types);
        }

        protected IList<TypeWrapper> Types
        {
            get { return _types; }
        }

        public IEnumerable<TypeWrapper> GetTypes()
        {
            return Types;
        }

        public void Add(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                Add(type);
            }
        }

        public void Add(Type type)
        {
            Types.Add(type.ToTypeWrapper());
        }

        public void Add<T>()
        {
            Types.Add(typeof (T).ToTypeWrapper());
        }
    }
    // ReSharper restore MemberCanBePrivate.Global
}