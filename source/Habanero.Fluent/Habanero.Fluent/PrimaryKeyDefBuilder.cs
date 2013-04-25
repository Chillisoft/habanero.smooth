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
        private readonly PropertiesDefSelector<T> _propertiesDefSelector;
        private readonly IList<string> _primaryKeyPropNames;


        public PrimaryKeyDefBuilder(PropertiesDefSelector<T> propertiesDefSelector, IList<string> primaryKeyPropNames)
        {
            _propertiesDefSelector = propertiesDefSelector;
            _primaryKeyPropNames = primaryKeyPropNames;
        }

        public PrimaryKeyDefBuilder<T> WithPrimaryKeyProperty<TReturn>(Expression<Func<T, TReturn>> propExpression)
        {
            var propertyName = GetPropertyName(propExpression);
            _primaryKeyPropNames.Add(propertyName);
            return this;
        }

        public PrimaryKeyDefBuilder<T> WithPrimaryKeyProperty(string propertyName)
        {
            _primaryKeyPropNames.Add(propertyName);
            return this;
        }

        public PropertiesDefSelector<T> EndCompositePrimaryKey()
        {
            return _propertiesDefSelector;
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
