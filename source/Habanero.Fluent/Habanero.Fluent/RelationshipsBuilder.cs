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