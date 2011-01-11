using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Habanero.BO;
using Habanero.Util;

namespace Habanero.Fluent
{
    public class NewPrimaryKeyDefBuilder<T> where T : BusinessObject
    {
        private NewClassDefBuilder2<T> _classDefBuilder;
        private List<string> _primaryKeyPropNames;

        public NewPrimaryKeyDefBuilder(NewClassDefBuilder2<T> classDefBuilder)
        {
            _classDefBuilder = classDefBuilder;
        }

        public NewPrimaryKeyDefBuilder<T> WithPrimaryKeyProperty<TReturn>(Expression<Func<T, TReturn>> propExpression)
        {
            string propertyName = GetPropertyName(propExpression);
            _primaryKeyPropNames.Add(propertyName);
            return this;
        }

        public NewPrimaryKeyDefBuilder<T> WithPrimaryKeyProperty(string propertyName)
        {
            _primaryKeyPropNames.Add(propertyName);
            return this;
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
