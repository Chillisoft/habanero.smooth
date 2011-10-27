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
using System.Reflection;
using Habanero.Base;
using Habanero.BO;
using Habanero.Util;

namespace Habanero.Fluent
{
    public class RelationshipsBuilder<T>  where T : BusinessObject
    {
        private readonly ClassDefBuilder2<T> _classDefBuilder;
        private readonly IList<ISingleRelDefBuilder> _singleRelationshipDefBuilders;
        private readonly IList<IMultipleRelDefBuilder> _multipleRelationshipDefBuilders;

        public RelationshipsBuilder(ClassDefBuilder2<T> classDefBuilder, IList<ISingleRelDefBuilder> singleRelationshipDefBuilders, IList<IMultipleRelDefBuilder> multipleRelationshipDefBuilders)
        {
            _classDefBuilder = classDefBuilder;
            _singleRelationshipDefBuilders = singleRelationshipDefBuilders;
            _multipleRelationshipDefBuilders = multipleRelationshipDefBuilders;
        }

        public SingleRelKeyDefBuilder<T, TRelatedType> WithSingleRelationship<TRelatedType>(string relationshipName) where TRelatedType : BusinessObject
        {
            SingleRelationshipDefBuilder<T, TRelatedType> singleRelationshipDefBuilder = new SingleRelationshipDefBuilder<T, TRelatedType>(this, relationshipName);
            _singleRelationshipDefBuilders.Add(singleRelationshipDefBuilder);
            var relKeyDefBuilder = new SingleRelKeyDefBuilder<T, TRelatedType>(singleRelationshipDefBuilder);
            singleRelationshipDefBuilder.SingleRelKeyDefBuilder = relKeyDefBuilder;
            return relKeyDefBuilder;
        }

        public SingleRelKeyDefBuilder<T, TRelatedType> WithSingleRelationship<TRelatedType>(Expression<Func<T, TRelatedType>> relationshipExpression) where TRelatedType : BusinessObject
        {
            SingleRelationshipDefBuilder<T, TRelatedType> singleRelationshipDefBuilder = new SingleRelationshipDefBuilder<T, TRelatedType>(this, relationshipExpression);
            _singleRelationshipDefBuilders.Add(singleRelationshipDefBuilder);
            var relKeyDefBuilder = new SingleRelKeyDefBuilder<T, TRelatedType>(singleRelationshipDefBuilder);
            singleRelationshipDefBuilder.SingleRelKeyDefBuilder = relKeyDefBuilder;
            return relKeyDefBuilder;
        }

        public MultipleRelKeyDefBuilder<T, TRelatedType> WithMultipleRelationship<TRelatedType>(string relationshipName) where TRelatedType : BusinessObject
        {
            MultipleRelationshipDefBuilder<T, TRelatedType> multipleRelationshipDefBuilder = new MultipleRelationshipDefBuilder<T, TRelatedType>(this);
            multipleRelationshipDefBuilder.WithRelationshipName(relationshipName);
            _multipleRelationshipDefBuilders.Add(multipleRelationshipDefBuilder);
            var relKeyDefBuilder = new MultipleRelKeyDefBuilder<T, TRelatedType>(multipleRelationshipDefBuilder);
            multipleRelationshipDefBuilder.MultipleRelKeyDefBuilder = relKeyDefBuilder;
            return relKeyDefBuilder;

        }


        public MultipleRelKeyDefBuilder<T, TBusinessObject> WithMultipleRelationship<TBusinessObject>(Expression<Func<T, BusinessObjectCollection<TBusinessObject>>> relationshipExpression)
            where TBusinessObject : class, IBusinessObject, new()
        {
            string relationshipName = GetPropertyName(relationshipExpression);
            MultipleRelationshipDefBuilder<T, TBusinessObject> multipleRelationshipDefBuilder = new MultipleRelationshipDefBuilder<T, TBusinessObject>(this);
            multipleRelationshipDefBuilder.WithRelationshipName(relationshipName);
            _multipleRelationshipDefBuilders.Add(multipleRelationshipDefBuilder);
            var relKeyDefBuilder = new MultipleRelKeyDefBuilder<T, TBusinessObject>(multipleRelationshipDefBuilder);
            multipleRelationshipDefBuilder.MultipleRelKeyDefBuilder = relKeyDefBuilder;
            return relKeyDefBuilder;
        }


        public ClassDefBuilder2<T> EndRelationships()
        {
            return _classDefBuilder;
        }

        private static string GetPropertyName<TReturn>(Expression<Func<T, TReturn>> propExpression)
        {
            return GetPropertyInfo(propExpression).Name;
        }

        private static PropertyInfo GetPropertyInfo<TModel, TReturn>(Expression<Func<TModel, TReturn>> expression)
        {
            return ReflectionUtilities.GetPropertyInfo(expression);
        }

    }

}