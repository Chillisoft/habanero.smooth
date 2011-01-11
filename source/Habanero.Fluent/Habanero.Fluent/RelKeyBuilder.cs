using System;
using System.Linq.Expressions;
using System.Reflection;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Util;

namespace Habanero.Fluent
{
    public class RelKeyBuilder<T, TRelatedType> : IRelDefBuilder where T : BusinessObject where TRelatedType : BusinessObject
    {
        private readonly SingleRelationshipDefBuilder<T, TRelatedType> _singleRelationshipDefBuilder;
        private readonly RelKeyDef _relKeyDef;
//        private readonly SingleRelationshipDefBuilder<T, TRelatedType> _relBuilder;

        public RelKeyBuilder(SingleRelationshipDefBuilder<T, TRelatedType> singleRelationshipDefBuilder)
        {
            _singleRelationshipDefBuilder = singleRelationshipDefBuilder;
            _relKeyDef = new RelKeyDef();
        }

        public RelKeyBuilder<T, TRelatedType> WithRelProp(string ownerClassPropertyName, string relatedPropName) 
        {
            this._relKeyDef.Add(new RelPropDef(ownerClassPropertyName, relatedPropName));
            return this;
        }
        public RelKeyBuilder<T, TRelatedType> WithRelProp<TReturn>(Expression<Func<T, TReturn>> ownerPropExpression, Expression<Func<TRelatedType, TReturn>> relatedPropExpression) 
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
        public SingleRelationshipDefBuilder<T, TRelatedType> Return()
        {
            return this._singleRelationshipDefBuilder;
        }
    }

    public interface IRelDefBuilder 
    {
    }
    //public static class SingleRelationshipDefBuilderExtensions
    //{
    //    public static RelKeyBuilder<T, TRelatedType> WithRelProp<T, TRelatedType>(this RelKeyBuilder<T, TRelatedType> relKeyBuilder, string ownerClassPropertyName, string relatedPropName) where T : BusinessObject
    //    {
    //        return relKeyBuilder.Add(new RelPropDef(ownerClassPropertyName, relatedPropName));
    //        //return relKeyBuilder;
    //        //return relBuilder.
    //    }
    //    public static RelKeyBuilder<T, TRelatedType> WithRelProp<T, TRelatedType, TReturn>(this RelKeyBuilder<T, TRelatedType> relKeyBuilder, Expression<Func<T, TReturn>> ownerPropExpression, Expression<Func<TRelatedType, TReturn>> relatedPropExpression) where T : BusinessObject
    //    {
    //        string ownerClassPropertyName = GetPropertyName(ownerPropExpression);
    //        string relatedPropName = GetRelatedPropertyName(relatedPropExpression);
    //        return relKeyBuilder.WithRelProp(ownerClassPropertyName, relatedPropName);
    //    }




    //    private static string GetRelatedPropertyName<TRelatedType, TReturn>(Expression<Func<TRelatedType, TReturn>> propExpression)
    //    {
    //        return GetPropertyInfo(propExpression).Name;
    //    }
    //    private static string GetPropertyName<T, TReturn>(Expression<Func<T, TReturn>> propExpression)
    //    {
    //        return GetPropertyInfo(propExpression).Name;
    //    }

    //    private static PropertyInfo GetPropertyInfo<TModel, TReturn>(Expression<Func<TModel, TReturn>> expression)
    //    {
    //        return ReflectionUtilities.GetPropertyInfo(expression);
    //    }
    //}
}