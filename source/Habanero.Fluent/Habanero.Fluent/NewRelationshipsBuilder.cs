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
        private readonly NewClassDefBuilder2<T> _classDefBuilder;
        private readonly IList<ISingleRelDefBuilder> _singleRelationshipDefBuilders;
        private readonly IList<IMulipleRelDefBuilder> _multipleRelationshipDefBuilders;

        public NewRelationshipsBuilder(NewClassDefBuilder2<T> classDefBuilder, IList<ISingleRelDefBuilder> singleRelationshipDefBuilders, IList<IMulipleRelDefBuilder> multipleRelationshipDefBuilders)
        {
            _classDefBuilder = classDefBuilder;
            _singleRelationshipDefBuilders = singleRelationshipDefBuilders;
            _multipleRelationshipDefBuilders = multipleRelationshipDefBuilders;
        }


        public NewRelKeyDefBuilder<T, TRelatedType> WithNewSingleRelationship<TRelatedType>(string relationshipName) where TRelatedType : BusinessObject
        {
            NewSingleRelationshipDefBuilder<T, TRelatedType> singleRelationshipDefBuilder = new NewSingleRelationshipDefBuilder<T, TRelatedType>(this, relationshipName);
            _singleRelationshipDefBuilders.Add(singleRelationshipDefBuilder);
            var relKeyDefBuilder = new NewRelKeyDefBuilder<T, TRelatedType>(singleRelationshipDefBuilder);
            singleRelationshipDefBuilder.RelKeyDefBuilder = relKeyDefBuilder;
            return relKeyDefBuilder;
        }

        public NewRelKeyDefBuilder<T, TRelatedType> WithNewSingleRelationship<TRelatedType>(Expression<Func<T, TRelatedType>> relationshipExpression) where TRelatedType : BusinessObject
        {
            NewSingleRelationshipDefBuilder<T, TRelatedType> singleRelationshipDefBuilder = new NewSingleRelationshipDefBuilder<T, TRelatedType>(this, relationshipExpression);
            _singleRelationshipDefBuilders.Add(singleRelationshipDefBuilder);
            var relKeyDefBuilder = new NewRelKeyDefBuilder<T, TRelatedType>(singleRelationshipDefBuilder);
            singleRelationshipDefBuilder.RelKeyDefBuilder = relKeyDefBuilder;
            return relKeyDefBuilder;
        }

        public NewMultipleRelationshipDefBuilder<T, TRelatedType> WithMultipleRelationship<TRelatedType>(string relationshipName) where TRelatedType : IBusinessObject
        {
            NewMultipleRelationshipDefBuilder<T, TRelatedType> multipleRelationshipDefBuilder = new NewMultipleRelationshipDefBuilder<T, TRelatedType>(_classDefBuilder)
                .WithRelationshipName(relationshipName);
            //.WithRelatedType<TRelatedType>();
            _multipleRelationshipDefBuilders.Add(multipleRelationshipDefBuilder);
            return multipleRelationshipDefBuilder;

        }

        public NewMultipleRelationshipDefBuilder<T, TBusinessObject> WithMultipleRelationship<TBusinessObject>(Expression<Func<T, BusinessObjectCollection<TBusinessObject>>> relationshipExpression)
            where TBusinessObject : class, IBusinessObject, new()
        {
            string relationshipName = GetPropertyName(relationshipExpression);
            NewMultipleRelationshipDefBuilder<T, TBusinessObject> multipleRelationshipDefBuilder = new NewMultipleRelationshipDefBuilder<T, TBusinessObject>(_classDefBuilder)
                .WithRelationshipName(relationshipName);
            _multipleRelationshipDefBuilders.Add(multipleRelationshipDefBuilder);
            return multipleRelationshipDefBuilder;
        }


        public NewClassDefBuilder2<T> Return()
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