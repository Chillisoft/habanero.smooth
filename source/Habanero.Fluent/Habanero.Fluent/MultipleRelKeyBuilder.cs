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