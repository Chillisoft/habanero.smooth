using System;
using System.Linq.Expressions;
using System.Reflection;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Util;

namespace Habanero.Fluent
{
    public class OldPropDefBuilder<T> where T : BusinessObject
    {
        private readonly OldClassDefBuilder<T> _oldClassDefBuilder;
        private  bool _isCompulsory;
        private  bool _isAutoIncrementing;
        private string _description;
        private bool _keepValuePrivate;
        private string _displayName;

        private  string PropertyName { get; set; }
        private  string PropertyTypeAssemblyName { get; set; }
        private  string PropertyTypeName { get; set; }
        private  PropReadWriteRule ReadWriteRule { get; set; }
        private  string DatabaseFieldName { get; set; }
        private  string DefaultValueString { get; set; }

        public OldPropDefBuilder()
        {
            SetupBuilderDefaults();
        }

        public OldPropDefBuilder(OldClassDefBuilder<T> oldClassDefBuilder)
        {
            _oldClassDefBuilder = oldClassDefBuilder;
            SetupBuilderDefaults();
        }

        private void SetupBuilderDefaults()
        {
            PropertyTypeAssemblyName = "System";
            PropertyTypeName = "String";
            _isCompulsory = false;
        }

        public OldClassDefBuilder<T> Return()
        {
            return _oldClassDefBuilder;
        }

        public OldPropDefBuilder<T> WithPropertyName(string propertyName)
        {
            PropertyName = propertyName;
            return this;

        }

        //public static implicit operator PropDef(PropDefBuilder builder)
        //{
        //    return new PropDef(PropertyName, PropertyTypeAssemblyName, PropertyTypeName, ReadWriteRule, DatabaseFieldName, DefaultValueString, _isCompulsory, _isAutoIncrementing);
        //}

        public IPropDef Build()
        {

            return new PropDef(PropertyName, PropertyTypeAssemblyName, PropertyTypeName
                , ReadWriteRule, DatabaseFieldName, DefaultValueString, _isCompulsory, _isAutoIncrementing, Int32.MaxValue , _displayName, _description, _keepValuePrivate );
        }

        public OldPropDefBuilder<T> IsCompulsory()
        {
            _isCompulsory = true;
            return this;
        }

        public OldPropDefBuilder<T> WithAssemblyName(string assemblyName)
        {
            PropertyTypeAssemblyName = assemblyName;
            return this;
        }

        public OldPropDefBuilder<T> WithTypeName(string typeName)
        {
            PropertyTypeName = typeName;
            return this;
        }

        public OldPropDefBuilder<T> WithType<TReturnType>()
        {
            var type = typeof (TReturnType);
            return WithType(type);
        }

        public OldPropDefBuilder<T> WithType(Type type)
        {
            string propTypeAssemblyName;
            string propTypeName;
            //TypeLoader.ClassTypeInfo(type, out propTypeAssemblyName, out propTypeName);
            propTypeAssemblyName = type.Namespace;
            propTypeName = type.Name;
            WithAssemblyName(propTypeAssemblyName);
            WithTypeName(propTypeName);
            return this;
        }

        public OldPropDefBuilder<T> WithReadWriteRule(PropReadWriteRule propReadWriteRule)
        {
            ReadWriteRule = propReadWriteRule;
            return this;
        }

        public OldPropDefBuilder<T> WithDatabaseFieldName(string fieldName)
        {
            DatabaseFieldName = fieldName;
            return this;
        }

        public OldPropDefBuilder<T> WithDefaultValue(string defaultValue)
        {
            DefaultValueString = defaultValue;
            return this;
        }

        public OldPropDefBuilder<T> IsAutoIncrementing()
        {
            _isAutoIncrementing = true;
            return this;

        }

        public OldPropDefBuilder<T> WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public OldPropDefBuilder<T>  KeepValuePrivate()
        {
            _keepValuePrivate = true;
            return this;
        }

        public OldPropDefBuilder<T> WithDisplayName(string displayName)
        {
            _displayName = displayName;
            return this;
        }

/*        public PropDefBuilder<T> WithPropertyName<TReturnType>(Expression<Func<T, TReturnType>> propExpression)
        {
            PropertyName = GetPropertyName(propExpression);
            return this;
        }*/

/*        private static string GetPropertyName<TReturn>(Expression<Func<T, TReturn>> propExpression)
        {
            return GetPropertyInfo(propExpression).Name;
        }*/

        private static PropertyInfo GetPropertyInfo<TModel, TReturn>(Expression<Func<TModel, TReturn>> expression)
        {
            return ReflectionUtilities.GetPropertyInfo(expression);
        }

        public OldPropDefBuilder<T> WithProperty<TReturnType>(Expression<Func<T, TReturnType>> propExpression)
        {
            PropertyInfo propertyInfo = GetPropertyInfo(propExpression);
            PropertyName = propertyInfo.Name;
            Type propertyType = ReflectionUtilities.GetUndelyingPropertType(propertyInfo);
            WithAssemblyName(propertyType.Namespace);
            WithTypeName(propertyType.Name);
            return this;
        }
    }
}