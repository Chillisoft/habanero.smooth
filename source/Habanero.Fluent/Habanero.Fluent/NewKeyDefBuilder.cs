using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Util;

namespace Habanero.Fluent
{
    public class NewKeyDefBuilder<T> where T : BusinessObject
    {
        private List<string> _propNames;
        private NewUniqueContraintsBuilder<T> _uniqueContraintsBuilder;
        private readonly string _keyName;

        public NewKeyDefBuilder(NewUniqueContraintsBuilder<T> uniqueContraintsBuilder, string keyName)
        {
            _propNames = new List<string>();
            _uniqueContraintsBuilder = uniqueContraintsBuilder;
            _keyName = keyName;
        }

        public NewKeyDefBuilder<T> AddProperty(string propertyName)
        {
            _propNames.Add(propertyName);
            return this;
        }

        public NewKeyDefBuilder<T> AddProperty<TReturnType>(Expression<Func<T, TReturnType>> propExpression)
        {
            _propNames.Add(GetPropertyName(propExpression));
            return this;
        }

        private static string GetPropertyName<TModel, TReturn>(Expression<Func<TModel, TReturn>> expression)
        {
            return ReflectionUtilities.GetPropertyName(expression);
        }
        public NewUniqueContraintsBuilder<T> EndUniqueConstraint()
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
}