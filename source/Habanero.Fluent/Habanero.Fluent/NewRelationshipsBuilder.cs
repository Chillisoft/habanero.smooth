using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Habanero.Base;
using Habanero.BO;
using Habanero.Util;

namespace Habanero.Fluent
{
    public class NewRelationshipsBuilder<T>  where T : BusinessObject
    {
        private readonly ClassDefBuilder2<T> _classDefBuilder;
        private readonly IList<ISingleRelDefBuilder> _singleRelationshipDefBuilders;
        private readonly IList<IMultipleRelDefBuilder> _multipleRelationshipDefBuilders;

        public NewRelationshipsBuilder(ClassDefBuilder2<T> classDefBuilder, IList<ISingleRelDefBuilder> singleRelationshipDefBuilders, IList<IMultipleRelDefBuilder> multipleRelationshipDefBuilders)
        {
            _classDefBuilder = classDefBuilder;
            _singleRelationshipDefBuilders = singleRelationshipDefBuilders;
            _multipleRelationshipDefBuilders = multipleRelationshipDefBuilders;
        }

        public NewSingleRelKeyDefBuilder<T, TRelatedType> WithNewSingleRelationship<TRelatedType>(string relationshipName) where TRelatedType : BusinessObject
        {
            NewSingleRelationshipDefBuilder<T, TRelatedType> singleRelationshipDefBuilder = new NewSingleRelationshipDefBuilder<T, TRelatedType>(this, relationshipName);
            _singleRelationshipDefBuilders.Add(singleRelationshipDefBuilder);
            var relKeyDefBuilder = new NewSingleRelKeyDefBuilder<T, TRelatedType>(singleRelationshipDefBuilder);
            singleRelationshipDefBuilder.SingleRelKeyDefBuilder = relKeyDefBuilder;
            return relKeyDefBuilder;
        }

        public NewSingleRelKeyDefBuilder<T, TRelatedType> WithNewSingleRelationship<TRelatedType>(Expression<Func<T, TRelatedType>> relationshipExpression) where TRelatedType : BusinessObject
        {
            NewSingleRelationshipDefBuilder<T, TRelatedType> singleRelationshipDefBuilder = new NewSingleRelationshipDefBuilder<T, TRelatedType>(this, relationshipExpression);
            _singleRelationshipDefBuilders.Add(singleRelationshipDefBuilder);
            var relKeyDefBuilder = new NewSingleRelKeyDefBuilder<T, TRelatedType>(singleRelationshipDefBuilder);
            singleRelationshipDefBuilder.SingleRelKeyDefBuilder = relKeyDefBuilder;
            return relKeyDefBuilder;
        }

        public NewMultipleRelKeyDefBuilder<T, TRelatedType> WithNewMultipleRelationship<TRelatedType>(string relationshipName) where TRelatedType : BusinessObject
        {
            NewMultipleRelationshipDefBuilder<T, TRelatedType> multipleRelationshipDefBuilder = new NewMultipleRelationshipDefBuilder<T, TRelatedType>(this);
            multipleRelationshipDefBuilder.WithRelationshipName(relationshipName);
            _multipleRelationshipDefBuilders.Add(multipleRelationshipDefBuilder);
            var relKeyDefBuilder = new NewMultipleRelKeyDefBuilder<T, TRelatedType>(multipleRelationshipDefBuilder);
            multipleRelationshipDefBuilder.MultipleRelKeyDefBuilder = relKeyDefBuilder;
            return relKeyDefBuilder;

        }


        public NewMultipleRelKeyDefBuilder<T, TBusinessObject> WithNewMultipleRelationship<TBusinessObject>(Expression<Func<T, BusinessObjectCollection<TBusinessObject>>> relationshipExpression)
            where TBusinessObject : class, IBusinessObject, new()
        {
            string relationshipName = GetPropertyName(relationshipExpression);
            NewMultipleRelationshipDefBuilder<T, TBusinessObject> multipleRelationshipDefBuilder = new NewMultipleRelationshipDefBuilder<T, TBusinessObject>(this);
            multipleRelationshipDefBuilder.WithRelationshipName(relationshipName);
            _multipleRelationshipDefBuilders.Add(multipleRelationshipDefBuilder);
            var relKeyDefBuilder = new NewMultipleRelKeyDefBuilder<T, TBusinessObject>(multipleRelationshipDefBuilder);
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