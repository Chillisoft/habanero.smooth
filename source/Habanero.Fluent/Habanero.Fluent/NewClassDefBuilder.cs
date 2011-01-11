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
    public class NewClassDefBuilder<T> where T : BusinessObject
    {
        private List<string> _primaryKeyPropNames;
        private NewSuperClassDefBuilder<T> _superClassDefBuilder;
        private NewClassDefBuilder2<T> _newClassDefBuilder2;

        public NewClassDefBuilder()
        {
            _newClassDefBuilder2 = new NewClassDefBuilder2<T>(this);
        }

        public NewClassDefBuilder2<T> WithPrimaryKey(string propertyName)
        {
            _primaryKeyPropNames.Add(propertyName);
            return new NewClassDefBuilder2<T>(this);
        }

        public NewPrimaryKeyDefBuilder<T> WithCompositePrimaryKey()
        {
            return new NewPrimaryKeyDefBuilder<T>(new NewClassDefBuilder2<T>(this));
        }


        public NewClassDefBuilder2<T> WithPrimaryKey<TReturn>(Expression<Func<T, TReturn>> propExpression)
        {
            string propertyName = GetPropertyName(propExpression);
            _primaryKeyPropNames.Add(propertyName);
            return new NewClassDefBuilder2<T>(this);
        }

        public NewSuperClassDefBuilder<T> WithSuperClass()
        {
            _superClassDefBuilder = new NewSuperClassDefBuilder<T>(_newClassDefBuilder2);
            return _superClassDefBuilder;
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
