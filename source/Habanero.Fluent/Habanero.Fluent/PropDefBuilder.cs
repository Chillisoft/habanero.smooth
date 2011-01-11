using System;
using System.Linq.Expressions;
using System.Reflection;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Util;

namespace Habanero.Fluent
{
    public class PropDefBuilder<T> where T : BusinessObject
    {
        private readonly ClassDefBuilder<T> _classDefBuilder;
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

        public PropDefBuilder()
        {
            SetupBuilderDefaults();
        }

        public PropDefBuilder(ClassDefBuilder<T> classDefBuilder)
        {
            _classDefBuilder = classDefBuilder;
            SetupBuilderDefaults();
        }

        private void SetupBuilderDefaults()
        {
            PropertyTypeAssemblyName = "System";
            PropertyTypeName = "String";
            _isCompulsory = false;
        }

        public ClassDefBuilder<T> Return()
        {
            return _classDefBuilder;
        }

        public PropDefBuilder<T> WithPropertyName(string propertyName)
        {
            PropertyName = propertyName;
            return this;

        }

        //public static implicit operator PropDef(PropDefBuilder builder)
        //{
        //    return new PropDef(PropertyName, PropertyTypeAssemblyName, PropertyTypeName, ReadWriteRule, DatabaseFieldName, DefaultValueString, _isCompulsory, _isAutoIncrementing);
        //}

        public PropDef Build()
        {

            return new PropDef(PropertyName, PropertyTypeAssemblyName, PropertyTypeName
                , ReadWriteRule, DatabaseFieldName, DefaultValueString, _isCompulsory, _isAutoIncrementing, Int32.MaxValue , _displayName, _description, _keepValuePrivate );
        }

        public PropDefBuilder<T> IsCompulsory()
        {
            _isCompulsory = true;
            return this;
        }

        public PropDefBuilder<T> WithAssemblyName(string assemblyName)
        {
            PropertyTypeAssemblyName = assemblyName;
            return this;
        }

        public PropDefBuilder<T> WithTypeName(string typeName)
        {
            PropertyTypeName = typeName;
            return this;
        }

        public PropDefBuilder<T> WithType<TReturnType>()
        {
            var type = typeof (TReturnType);
            return WithType(type);
        }

        public PropDefBuilder<T> WithType(Type type)
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

        public PropDefBuilder<T> WithReadWriteRule(PropReadWriteRule propReadWriteRule)
        {
            ReadWriteRule = propReadWriteRule;
            return this;
        }

        public PropDefBuilder<T> WithDatabaseFieldName(string fieldName)
        {
            DatabaseFieldName = fieldName;
            return this;
        }

        public PropDefBuilder<T> WithDefaultValue(string defaultValue)
        {
            DefaultValueString = defaultValue;
            return this;
        }

        public PropDefBuilder<T> IsAutoIncrementing()
        {
            _isAutoIncrementing = true;
            return this;

        }

        public PropDefBuilder<T> WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public PropDefBuilder<T>  KeepValuePrivate()
        {
            _keepValuePrivate = true;
            return this;
        }

        public PropDefBuilder<T> WithDisplayName(string displayName)
        {
            _displayName = displayName;
            return this;
        }

        public PropDefBuilder<T> WithPropertyName<TReturnType>(Expression<Func<T, TReturnType>> propExpression)
        {
            PropertyName = GetPropertyName(propExpression);
            return this;
        }

        private static string GetPropertyName<TReturn>(Expression<Func<T, TReturn>> propExpression)
        {
            return GetPropertyInfo(propExpression).Name;
        }

        private static PropertyInfo GetPropertyInfo<TModel, TReturn>(Expression<Func<TModel, TReturn>> expression)
        {
            return ReflectionUtilities.GetPropertyInfo(expression);
        }

        //public PropDefBuilder<T> WithProperty<TReturnType>(Expression<Func<T, TReturnType>> propExpression)
        //{
        //    PropertyInfo propertyInfo = GetPropertyInfo(propExpression);
        //    PropertyName = propertyInfo.Name;
        //    Type propertyType = propertyInfo.PropertyType;
        //    WithAssemblyName(propertyType.Namespace);
        //    WithTypeName(propertyType.Name);
        //    return this;
        //}
    }
}