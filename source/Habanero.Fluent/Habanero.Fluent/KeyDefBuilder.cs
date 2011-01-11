using System.Collections.Generic;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;

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

        public ClassDefBuilder<T> Return()
        {
            return _classDefBuilder;
        }

        public KeyDef Build(PropDefCol propDefCol)
        {
            var keyDef = new KeyDef(_keyName);
            foreach (var propName in _propNames)
            {
                IPropDef propDef = propDefCol[propName];
                keyDef.Add(propDef);
            }
            return keyDef;
        }
    }
}