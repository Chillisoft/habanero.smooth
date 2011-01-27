using System;
using System.Linq.Expressions;
using System.Reflection;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Util;

namespace Habanero.Fluent
{
    public class NewMultipleRelKeyBuilder<T, TRelatedType> : IRelDefBuilder where T : BusinessObject 
    {
        private readonly NewMultipleRelationshipDefBuilder<T, TRelatedType> _multipleRelationshipDefBuilder;
        private readonly RelKeyDef _relKeyDef;
//        private readonly MultipleRelationshipDefBuilder<T, TRelatedType> _relBuilder;

        public NewMultipleRelKeyBuilder(NewMultipleRelationshipDefBuilder<T, TRelatedType> multipleRelationshipDefBuilder)
        {
            _multipleRelationshipDefBuilder = multipleRelationshipDefBuilder;
            _relKeyDef = new RelKeyDef();
        }

        public NewMultipleRelKeyBuilder<T, TRelatedType> WithRelProp(string ownerClassPropertyName, string relatedPropName) 
        {
            this._relKeyDef.Add(new RelPropDef(ownerClassPropertyName, relatedPropName));
            return this;
        }
        public NewMultipleRelKeyBuilder<T, TRelatedType> WithRelProp<TReturn>(Expression<Func<T, TReturn>> ownerPropExpression, Expression<Func<TRelatedType, TReturn>> relatedPropExpression) 
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
        public NewMultipleRelationshipDefBuilder<T, TRelatedType> EndRelProps()
        {
            return this._multipleRelationshipDefBuilder;
        }
    }
}