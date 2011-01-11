using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Util;

namespace Habanero.Fluent
{
    public class KeyDefBuilder<T> where T : BusinessObject
    {
        private List<string> _propNames;
        private ClassDefBuilder<T> _classDefBuilder;
        private readonly string _keyName;

        public KeyDefBuilder(ClassDefBuilder<T> classDefBuilder, string keyName)
        {
            _propNames = new List<string>();
            _classDefBuilder = classDefBuilder;
            _keyName = keyName;
        }

        public KeyDefBuilder<T> AddProperty(string propertyName)
        {
            _propNames.Add(propertyName);
            return this;
        }

        public KeyDefBuilder<T> AddProperty<TReturnType>(Expression<Func<T, TReturnType>> propExpression)
        {
            _propNames.Add(GetPropertyName(propExpression));
            return this;
        }

        private static string GetPropertyName<TModel, TReturn>(Expression<Func<TModel, TReturn>> expression)
        {
            return ReflectionUtilities.GetPropertyName(expression);
        }
        public ClassDefBuilder<T> Return()
        {
            return _classDefBuilder;
        }

        public IKeyDef Build(PropDefCol propDefCol)
        {
            var keyDef = new KeyDef(_keyName);
            foreach (var propName in _propNames)
            {
                var propDef = propDefCol[propName];
                keyDef.Add(propDef);
            }
            return keyDef;
        }
    }
}