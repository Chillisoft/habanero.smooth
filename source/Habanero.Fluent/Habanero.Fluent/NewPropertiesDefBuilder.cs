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
        private PropDefCol _propDefCol;
        private IList<NewPropDefBuilder<T>> PropDefBuilders { get; set; }

        public NewPropertiesDefBuilder(NewClassDefBuilder2<T> classDefBuilder)
        {
            _classDefBuilder = classDefBuilder;
            PropDefBuilders = new List<NewPropDefBuilder<T>>();

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
            propDefBuilder.WithProperty(propExpression);
            PropDefBuilders.Add(propDefBuilder);
            return propDefBuilder;
        }

        //public NewPropDefBuilder<T> WithProperty<TReturnType>(Expression<Func<T, TReturnType>> propExpression)
        //{
        //    PropertyInfo propertyInfo = GetPropertyInfo(propExpression);
        //    PropertyName = propertyInfo.Name;
        //    Type propertyType = ReflectionUtilities.GetUndelyingPropertType(propertyInfo);
        //    WithAssemblyName(propertyType.Namespace);
        //    WithTypeName(propertyType.Name);
        //    return this;
        //}

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
            _classDefBuilder.SetupPropDefCol(SetupPropDefCol());
            return _classDefBuilder;
        }

        private IPropDefCol SetupPropDefCol()
        {
            foreach (var propDefBuilder in PropDefBuilders)
            {
                var propDef = propDefBuilder.Build();
                _propDefCol.Add(propDef);
            }
            return _propDefCol;
        }
    }
}
