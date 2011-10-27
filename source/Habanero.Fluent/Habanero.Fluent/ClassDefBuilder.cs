#region Licensing Header
// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2011 Chillisoft Solutions
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
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Habanero.BO;
using Habanero.Util;

namespace Habanero.Fluent
{
    public class ClassDefBuilder<T> where T : BusinessObject
    {
        private readonly IList<string> _primaryKeyPropNames;
        private SuperClassDefBuilder<T> _superClassDefBuilder;
        private ClassDefBuilder2<T> _classDefBuilder2;

        public ClassDefBuilder()
        {
            _primaryKeyPropNames = new List<string>();
        }

        public PropertiesDefSelector<T> WithPrimaryKey(string propertyName)
        {
            _primaryKeyPropNames.Add(propertyName);
            return new PropertiesDefSelector<T>(this, _primaryKeyPropNames);
        }

        public PropertiesDefSelector<T> WithPrimaryKey<TReturn>(Expression<Func<T, TReturn>> propExpression)
        {
            string propertyName = GetPropertyName(propExpression);
            _primaryKeyPropNames.Add(propertyName);
            return new PropertiesDefSelector<T>(this, _primaryKeyPropNames);
        }

        public PrimaryKeyDefBuilder<T> WithCompositePrimaryKey()
        {
            return new PrimaryKeyDefBuilder<T>(new PropertiesDefSelector<T>(this,_primaryKeyPropNames), _primaryKeyPropNames);
        }

        public SuperClassDefBuilder<T> WithSuperClass()
        {
            _superClassDefBuilder = new SuperClassDefBuilder<T>(this);
            return _superClassDefBuilder;
        }

        private static string GetPropertyName<TReturn>(Expression<Func<T, TReturn>> propExpression)
        {
            var propertyInfo = GetPropertyInfo(propExpression);
            return propertyInfo.Name;
        }

        private static PropertyInfo GetPropertyInfo<TModel, TReturn>(Expression<Func<TModel, TReturn>> expression)
        {
            return ReflectionUtilities.GetPropertyInfo(expression);
        }
    }

}
