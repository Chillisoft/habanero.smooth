using System;
using System.Linq.Expressions;
using System.Reflection;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Util;

namespace Habanero.Fluent
{
    public class NewSingleRelKeyBuilder<T, TRelatedType> : IRelDefBuilder where T : BusinessObject where TRelatedType : BusinessObject
    {
        private readonly NewSingleRelationshipDefBuilder<T, TRelatedType> _singleRelationshipDefBuilder;
        private readonly RelKeyDef _relKeyDef;
//        private readonly SingleRelationshipDefBuilder<T, TRelatedType> _relBuilder;

        public NewSingleRelKeyBuilder(NewSingleRelationshipDefBuilder<T, TRelatedType> singleRelationshipDefBuilder)
        {
            _singleRelationshipDefBuilder = singleRelationshipDefBuilder;
            _relKeyDef = new RelKeyDef();
        }

        public NewSingleRelKeyBuilder<T, TRelatedType> WithRelProp(string ownerClassPropertyName, string relatedPropName) 
        {
            this._relKeyDef.Add(new RelPropDef(ownerClassPropertyName, relatedPropName));
            return this;
        }
        public NewSingleRelKeyBuilder<T, TRelatedType> WithRelProp<TReturn>(Expression<Func<T, TReturn>> ownerPropExpression, Expression<Func<TRelatedType, TReturn>> relatedPropExpression) 
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
        public NewSingleRelationshipDefBuilder<T, TRelatedType> EndCompositeRelationshipKey()
        {
            return this._singleRelationshipDefBuilder;
        }
    }
}