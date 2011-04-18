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
        private UniqueContraintsBuilder<T> _uniqueContraintsBuilder;
        private readonly string _keyName;

        public KeyDefBuilder(UniqueContraintsBuilder<T> uniqueContraintsBuilder, string keyName)
        {
            _propNames = new List<string>();
            _uniqueContraintsBuilder = uniqueContraintsBuilder;
            _keyName = keyName;
        }

        public KeyDefBuilder<T> AddProperty(string propertyName)
        {
            _propNames.Add(propertyName);
            return this;
        }

        public KeyDefBuilder<T> AddProperty<TReturnType>(Expression<Func<T, TReturnType>> propExpression)
        {
            _propNames.Add(propExpression.GetPropertyName());
            return this;
        }

        public UniqueContraintsBuilder<T> EndUniqueConstraint()
        {
            return _uniqueContraintsBuilder;
        }

        public IKeyDef Build(IPropDefCol propDefCol)
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
    public static class ExpressionExtensions
    {

        public static string GetPropertyName<TModel, TReturn>(this Expression<Func<TModel, TReturn>> expression)
        {
            return ReflectionUtilities.GetPropertyName(expression);
        }
    }
}