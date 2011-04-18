using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Habanero.BO;
using Habanero.Util;

namespace Habanero.Fluent
{
    public class ClassDefBuilder<T> where T : BusinessObject
    {
        private IList<string> _primaryKeyPropNames;
        private SuperClassDefBuilder<T> _superClassDefBuilder;
        private ClassDefBuilder2<T> _classDefBuilder2;

        public ClassDefBuilder()
        {
            _primaryKeyPropNames = new List<string>();
            _classDefBuilder2 = new ClassDefBuilder2<T>(this, _primaryKeyPropNames);
        }

        public ClassDefBuilder2<T> WithPrimaryKey(string propertyName)
        {
            _primaryKeyPropNames.Add(propertyName);
            return new ClassDefBuilder2<T>(this, _primaryKeyPropNames);
        }

        public ClassDefBuilder2<T> WithPrimaryKey<TReturn>(Expression<Func<T, TReturn>> propExpression)
        {
            string propertyName = GetPropertyName(propExpression);
            _primaryKeyPropNames.Add(propertyName);
            return new ClassDefBuilder2<T>(this, _primaryKeyPropNames);
        }

        public PrimaryKeyDefBuilder<T> WithCompositePrimaryKey()
        {
            return new PrimaryKeyDefBuilder<T>(new ClassDefBuilder2<T>(this, _primaryKeyPropNames), _primaryKeyPropNames);
        }

        public SuperClassDefBuilder<T> WithSuperClass()
        {
            _superClassDefBuilder = new SuperClassDefBuilder<T>(this);
            return _superClassDefBuilder;
        }

        private static string GetPropertyName<TReturn>(Expression<Func<T, TReturn>> propExpression)
        {
            var propertyInfo = GetPropertyInfo(propExpression);
            return propertyInfo.Name;
        }

        private static PropertyInfo GetPropertyInfo<TModel, TReturn>(Expression<Func<TModel, TReturn>> expression)
        {
            return ReflectionUtilities.GetPropertyInfo(expression);
        }
    }

}
