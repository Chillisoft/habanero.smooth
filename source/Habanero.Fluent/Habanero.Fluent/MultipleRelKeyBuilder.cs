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
using System.Linq.Expressions;
using System.Reflection;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Util;

namespace Habanero.Fluent
{
    public class MultipleRelKeyBuilder<T, TRelatedType> : IRelDefBuilder where T : BusinessObject 
    {
        private readonly MultipleRelationshipDefBuilder<T, TRelatedType> _multipleRelationshipDefBuilder;
        private readonly RelKeyDef _relKeyDef;
//        private readonly MultipleRelationshipDefBuilder<T, TRelatedType> _relBuilder;

        public MultipleRelKeyBuilder(MultipleRelationshipDefBuilder<T, TRelatedType> multipleRelationshipDefBuilder)
        {
            _multipleRelationshipDefBuilder = multipleRelationshipDefBuilder;
            _relKeyDef = new RelKeyDef();
        }

        public MultipleRelKeyBuilder<T, TRelatedType> WithRelProp(string ownerClassPropertyName, string relatedPropName) 
        {
            this._relKeyDef.Add(new RelPropDef(ownerClassPropertyName, relatedPropName));
            return this;
        }
        public MultipleRelKeyBuilder<T, TRelatedType> WithRelProp<TReturn>(Expression<Func<T, TReturn>> ownerPropExpression, Expression<Func<TRelatedType, TReturn>> relatedPropExpression) 
        {
            string ownerClassPropertyName = GetPropertyName(ownerPropExpression);
            string relatedPropName = GetRelatedPropertyName(relatedPropExpression);
            return this.WithRelProp(ownerClassPropertyName, relatedPropName);
        }

        public IRelKeyDef Build()
        {
            return _relKeyDef;
        }
        private static string GetRelatedPropertyName<TReturn>(Expression<Func<TRelatedType, TReturn>> propExpression)
        {
            return GetPropertyInfo(propExpression).Name;
        }
        private static string GetPropertyName<TReturn>(Expression<Func<T, TReturn>> propExpression)
        {
            return GetPropertyInfo(propExpression).Name;
        }

        private static PropertyInfo GetPropertyInfo<TModel, TReturn>(Expression<Func<TModel, TReturn>> expression)
        {
            return ReflectionUtilities.GetPropertyInfo(expression);
        }
        public MultipleRelationshipDefBuilder<T, TRelatedType> EndCompositeRelationshipKey()
        {
            return this._multipleRelationshipDefBuilder;
        }
    }
}