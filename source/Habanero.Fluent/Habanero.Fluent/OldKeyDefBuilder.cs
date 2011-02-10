using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Util;

namespace Habanero.Fluent
{
    public class OldKeyDefBuilder<T> where T : BusinessObject
    {
        private List<string> _propNames;
        private OldClassDefBuilder<T> _oldClassDefBuilder;
        private readonly string _keyName;

        public OldKeyDefBuilder(OldClassDefBuilder<T> oldClassDefBuilder, string keyName)
        {
            _propNames = new List<string>();
            _oldClassDefBuilder = oldClassDefBuilder;
            _keyName = keyName;
        }

        public OldKeyDefBuilder<T> AddProperty(string propertyName)
        {
            _propNames.Add(propertyName);
            return this;
        }

        public OldKeyDefBuilder<T> AddProperty<TReturnType>(Expression<Func<T, TReturnType>> propExpression)
        {
            _propNames.Add(GetPropertyName(propExpression));
            return this;
        }

        private static string GetPropertyName<TModel, TReturn>(Expression<Func<TModel, TReturn>> expression)
        {
            return ReflectionUtilities.GetPropertyName(expression);
        }
        public OldClassDefBuilder<T> Return()
        {
            return _oldClassDefBuilder;
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