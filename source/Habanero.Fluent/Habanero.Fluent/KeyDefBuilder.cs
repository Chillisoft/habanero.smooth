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
using System.Linq.Expressions;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Util;

namespace Habanero.Fluent
{
    public class KeyDefBuilder<T> where T : BusinessObject
    {
        private List<string> _propNames;
        private UniqueContraintsBuilder<T> _uniqueContraintsBuilder;
        private readonly string _keyName;

        public KeyDefBuilder(UniqueContraintsBuilder<T> uniqueContraintsBuilder, string keyName)
        {
            _propNames = new List<string>();
            _uniqueContraintsBuilder = uniqueContraintsBuilder;
            _keyName = keyName;
        }

        public KeyDefBuilder<T> AddProperty(string propertyName)
        {
            _propNames.Add(propertyName);
            return this;
        }

        public KeyDefBuilder<T> AddProperty<TReturnType>(Expression<Func<T, TReturnType>> propExpression)
        {
            _propNames.Add(propExpression.GetPropertyName());
            return this;
        }

        public UniqueContraintsBuilder<T> EndUniqueConstraint()
        {
            return _uniqueContraintsBuilder;
        }

        public IKeyDef Build(IPropDefCol propDefCol)
        {
            var keyDef = new KeyDef(_keyName);
            foreach (var propName in _propNames)
            {
                var propDef = propDefCol[propName];
                keyDef.Add(propDef);
            }
            return keyDef;
        }
    }
    public static class ExpressionExtensions
    {

        public static string GetPropertyName<TModel, TReturn>(this Expression<Func<TModel, TReturn>> expression)
        {
            return ReflectionUtilities.GetPropertyName(expression);
        }
    }
}