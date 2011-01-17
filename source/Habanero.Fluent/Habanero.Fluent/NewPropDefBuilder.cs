using System;
using System.Linq.Expressions;
using System.Reflection;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Util;

namespace Habanero.Fluent
{
    public class NewPropDefBuilder<T> where T : BusinessObject
    {
        private readonly NewPropertiesDefBuilder<T> _propertiesDefBuilder;
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

        public NewPropDefBuilder()
        {
            SetupBuilderDefaults();
        }

        public NewPropDefBuilder(NewPropertiesDefBuilder<T> propertiesDefBuilder)
        {
            _propertiesDefBuilder = propertiesDefBuilder;
            SetupBuilderDefaults();
        }

        private void SetupBuilderDefaults()
        {
            PropertyTypeAssemblyName = "System";
            PropertyTypeName = "String";
            _isCompulsory = false;
        }

        public NewPropertiesDefBuilder<T> Return()
        {
            return _propertiesDefBuilder;
        }

        public NewPropDefBuilder<T> WithPropertyName(string propertyName)
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

        public NewPropDefBuilder<T> IsCompulsory()
        {
            _isCompulsory = true;
            return this;
        }

        public NewPropDefBuilder<T> WithAssemblyName(string assemblyName)
        {
            PropertyTypeAssemblyName = assemblyName;
            return this;
        }

        public NewPropDefBuilder<T> WithTypeName(string typeName)
        {
            PropertyTypeName = typeName;
            return this;
        }

        public NewPropDefBuilder<T> WithType<TReturnType>()
        {
            var type = typeof (TReturnType);
            return WithType(type);
        }

        public NewPropDefBuilder<T> WithType(Type type)
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

        public NewPropDefBuilder<T> WithReadWriteRule(PropReadWriteRule propReadWriteRule)
        {
            ReadWriteRule = propReadWriteRule;
            return this;
        }

        public NewPropDefBuilder<T> WithDatabaseFieldName(string fieldName)
        {
            DatabaseFieldName = fieldName;
            return this;
        }

        public NewPropDefBuilder<T> WithDefaultValue(string defaultValue)
        {
            DefaultValueString = defaultValue;
            return this;
        }

        public NewPropDefBuilder<T> IsAutoIncrementing()
        {
            _isAutoIncrementing = true;
            return this;

        }

        public NewPropDefBuilder<T> WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public NewPropDefBuilder<T>  KeepValuePrivate()
        {
            _keepValuePrivate = true;
            return this;
        }

        public NewPropDefBuilder<T> WithDisplayName(string displayName)
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

        // Moved to NewPropertiesDefBuilder
        //public NewPropDefBuilder<T> WithProperty<TReturnType>(Expression<Func<T, TReturnType>> propExpression)
        //{
        //    PropertyInfo propertyInfo = GetPropertyInfo(propExpression);
        //    PropertyName = propertyInfo.Name;
        //    Type propertyType = ReflectionUtilities.GetUndelyingPropertType(propertyInfo);
        //    WithAssemblyName(propertyType.Namespace);
        //    WithTypeName(propertyType.Name);
        //    return this;
        //}


    }
}