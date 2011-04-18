using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Habanero.BO;
using Habanero.Util;

namespace Habanero.Fluent
{
    public class PrimaryKeyDefBuilder<T> where T : BusinessObject
    {
        private ClassDefBuilder2<T> _classDefBuilder;
        private IList<string> _primaryKeyPropNames;


        public PrimaryKeyDefBuilder(ClassDefBuilder2<T> classDefBuilder, IList<string> primaryKeyPropNames)
        {
            _classDefBuilder = classDefBuilder;
            _primaryKeyPropNames = primaryKeyPropNames;
        }

        public PrimaryKeyDefBuilder<T> WithPrimaryKeyProperty<TReturn>(Expression<Func<T, TReturn>> propExpression)
        {
            string propertyName = GetPropertyName(propExpression);
            _primaryKeyPropNames.Add(propertyName);
            return this;
        }

        public PrimaryKeyDefBuilder<T> WithPrimaryKeyProperty(string propertyName)
        {
            _primaryKeyPropNames.Add(propertyName);
            return this;
        }

        public ClassDefBuilder2<T> EndCompositePrimaryKey()
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
