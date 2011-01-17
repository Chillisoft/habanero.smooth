using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Util;

namespace Habanero.Fluent
{
    public class NewPropertiesDefBuilder<T> where T : BusinessObject
    {
        private NewClassDefBuilder2<T> _classDefBuilder;
        private IList<NewPropDefBuilder<T>> PropDefBuilders { get; set; }

        public NewPropertiesDefBuilder(NewClassDefBuilder2<T> classDefBuilder, IList<NewPropDefBuilder<T>> propDefBuilders)
        {
            _classDefBuilder = classDefBuilder;
            PropDefBuilders = propDefBuilders;
        }

        public NewPropDefBuilder<T> Property(string propertyName)
        {
            var propDefBuilder = GetPropDefBuilder(propertyName);
            PropDefBuilders.Add(propDefBuilder);
            return propDefBuilder;
        }
        public NewPropDefBuilder<T> Property<TReturnType>(string propertyName)
        {
            var propDefBuilder = GetPropDefBuilder(propertyName);
            propDefBuilder.WithType<TReturnType>();
            PropDefBuilders.Add(propDefBuilder);
            return propDefBuilder;
        }

        public NewPropDefBuilder<T> Property<TReturnType>(Expression<Func<T, TReturnType>> propExpression)
        {
            var propDefBuilder = new NewPropDefBuilder<T>(this);
            PropertyInfo propertyInfo = GetPropertyInfo(propExpression);
            propDefBuilder.WithPropertyName(propertyInfo.Name);
            Type propertyType = ReflectionUtilities.GetUndelyingPropertType(propertyInfo);
            propDefBuilder.WithAssemblyName(propertyType.Namespace);
            propDefBuilder.WithTypeName(propertyType.Name);
            PropDefBuilders.Add(propDefBuilder);
            return propDefBuilder;
        }

        private NewPropDefBuilder<T> GetPropDefBuilder(string propertyName)
        {
            var propDefBuilder = new NewPropDefBuilder<T>(this);
            propDefBuilder.WithPropertyName(propertyName);
            return propDefBuilder;
        }

        private static string GetPropertyName<TReturn>(Expression<Func<T, TReturn>> propExpression)
        {
            return GetPropertyInfo(propExpression).Name;
        }

        private static PropertyInfo GetPropertyInfo<TModel, TReturn>(Expression<Func<TModel, TReturn>> expression)
        {
            return ReflectionUtilities.GetPropertyInfo(expression);
        }

        public NewClassDefBuilder2<T> Return()
        {
            return _classDefBuilder;
        }


    }
}
