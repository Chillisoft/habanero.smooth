using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Util;

namespace Habanero.Fluent
{
    public class ClassDefBuilder<T> where T:BusinessObject
    {
        private PropDefCol _propDefCol;
        private IList<PropDefBuilder<T>> _propDefBuilders = new List<PropDefBuilder<T>>();
        private IList<SingleRelationshipDefBuilder<T>> _singleRelationshipDefBuilders = new List<SingleRelationshipDefBuilder<T>>();
        private RelationshipDefCol _relationshipDefCol;
        private IList<MultipleRelationshipDefBuilder<T>> _multipleRelationshipDefBuilders = new List<MultipleRelationshipDefBuilder<T>>();
        private List<string> _primaryKeyPropNames;
        private PrimaryKeyDef _primaryKeyDef;
        private IList<KeyDefBuilder<T>> _keyDefBuilders = new List<KeyDefBuilder<T>>();
        private KeyDefCol _keyDefCol = new KeyDefCol();


        public ClassDefBuilder()
        {
//             _builders
            _propDefCol = new PropDefCol();
            _relationshipDefCol = new RelationshipDefCol();
            _primaryKeyPropNames = new List<string>();
            _primaryKeyDef = new PrimaryKeyDef();
        }

        public ClassDef Build()
        {
            Type type = typeof(T);
            SetupPropDefCol();
            SetupRelationshipDefCol();
            SetupPrimaryKey();
            SetupKeysCol();
            return new ClassDef(type.Namespace, type.Name, _primaryKeyDef, _propDefCol, _keyDefCol, _relationshipDefCol, new UIDefCol());
        }

        private void SetupKeysCol()
        {
            foreach (var keyDefBuilder in _keyDefBuilders)
            {
                KeyDef keyDef = keyDefBuilder.Build(_propDefCol);
                _keyDefCol.Add(keyDef);
            }
        }

        private void SetupPrimaryKey()
        {
            if (_primaryKeyPropNames.Count > 1)
            {
                _primaryKeyDef.IsGuidObjectID = false;
            }
            foreach (var propName in _primaryKeyPropNames)
            {
                IPropDef propDef = _propDefCol[propName];
                _primaryKeyDef.Add(propDef);
                if (_primaryKeyPropNames.Count == 1)
                {
                    _primaryKeyDef.IsGuidObjectID = propDef.PropertyType == typeof (Guid);

                }
            }
        }

        private void SetupPropDefCol()
        {
            foreach(var propDefBuilder in _propDefBuilders)
            {
                var propDef = propDefBuilder.Build();
                _propDefCol.Add(propDef);
            }
        }

        private void SetupRelationshipDefCol()
        {
            foreach (var singleRelationshipDefBuilder in _singleRelationshipDefBuilders)
            {
                var singleRelationshipDef = singleRelationshipDefBuilder.Build();
                _relationshipDefCol.Add(singleRelationshipDef);
            }
            foreach (var multipleRelationshipDefBuilder in _multipleRelationshipDefBuilders)
            {
                var multipleRelationshipDef = multipleRelationshipDefBuilder.Build();
                _relationshipDefCol.Add(multipleRelationshipDef);
            }
        }

        public PropDefBuilder<T> WithProperty(string propertyName)
        {
            PropDefBuilder<T> propDefBuilder = GetPropDefBuilder(propertyName);
            _propDefBuilders.Add(propDefBuilder);
            return propDefBuilder;
        }
        public PropDefBuilder<T> WithProperty<TReturnType>(string propertyName)
        {
            PropDefBuilder<T> propDefBuilder = GetPropDefBuilder(propertyName);
            propDefBuilder.WithType<TReturnType>();
            _propDefBuilders.Add(propDefBuilder);
            return propDefBuilder;
        }

        public PropDefBuilder<T> WithProperty<TReturnType>(Expression<Func<T, TReturnType>> propExpression)
        {
            string propertyName = GetPropertyName(propExpression);
            return this.WithProperty(propertyName);
        }

        private static string GetPropertyName<TReturn>(Expression<Func<T, TReturn>> propExpression)
        {
            return GetPropertyInfo(propExpression).Name;
        }

        private static PropertyInfo GetPropertyInfo<TModel, TReturn>(Expression<Func<TModel, TReturn>> expression)
        {
            return ReflectionUtilities.GetPropertyInfo(expression);
        }

/*        /// <summary>
        /// Creates a valid value for the property identified by the lambda expression <paramref name="propExpression"/>.
        /// </summary>
        /// <param name="propExpression"></param>
        /// <returns></returns>
        public TReturn GetValidPropValue<TReturn>(Expression<Func<T, TReturn>> propExpression)
        {
            return this.GetValidPropValue(this.BusinessObject, propExpression);
        }*/
        private PropDefBuilder<T> GetPropDefBuilder(string propertyName)
        {
            var propDefBuilder = new PropDefBuilder<T>(this);
            propDefBuilder.WithPropertyName(propertyName);
            return propDefBuilder;
        }

        public SingleRelationshipDefBuilder<T> WithSingleRelationship<TRelatedType>(string relationshipName) where TRelatedType : BusinessObject
        {
            SingleRelationshipDefBuilder<T> singleRelationshipDefBuilder = new SingleRelationshipDefBuilder<T>(this)
                .WithRelationshipName(relationshipName)
                .WithRelatedType<TRelatedType>();
            _singleRelationshipDefBuilders.Add(singleRelationshipDefBuilder);
            return singleRelationshipDefBuilder;

        }


        public SingleRelationshipDefBuilder<T> WithSingleRelationship<TRelatedType>(Expression<Func<T, TRelatedType>> propExpression)
        {
            string relationshipName = GetPropertyName(propExpression);
            SingleRelationshipDefBuilder<T> singleRelationshipDefBuilder = new SingleRelationshipDefBuilder<T>(this)
                .WithRelationshipName(relationshipName)
                .WithRelatedType<TRelatedType>();
            _singleRelationshipDefBuilders.Add(singleRelationshipDefBuilder);
            return singleRelationshipDefBuilder;

        }

        public MultipleRelationshipDefBuilder<T> WithMultipleRelationship<TRelatedType>(string relationshipName) where TRelatedType : BusinessObject
        {
            MultipleRelationshipDefBuilder<T> multipleRelationshipDefBuilder = new MultipleRelationshipDefBuilder<T>(this)
                .WithRelationshipName(relationshipName)
                .WithRelatedType<TRelatedType>();
            _multipleRelationshipDefBuilders.Add(multipleRelationshipDefBuilder);
            return multipleRelationshipDefBuilder;
                
        }

        public MultipleRelationshipDefBuilder<T> WithMultipleRelationship<TReturn>(Expression<Func<T, TReturn>> propExpression, Type relatedType)
        {
            string relationshipName = GetPropertyName(propExpression);
            MultipleRelationshipDefBuilder<T> multipleRelationshipDefBuilder = new MultipleRelationshipDefBuilder<T>(this)
                .WithRelationshipName(relationshipName)
                .WithRelatedType(relatedType);
            _multipleRelationshipDefBuilders.Add(multipleRelationshipDefBuilder);
            return multipleRelationshipDefBuilder;
                
        }

        public ClassDefBuilder<T> WithPrimaryKeyProp(string propertyName)
        {
            _primaryKeyPropNames.Add(propertyName);
            return this;
        }

        public KeyDefBuilder<T> WithUniqueConstraint()
        {
            return WithUniqueConstraint("");
        }
        public KeyDefBuilder<T> WithUniqueConstraint(string keyName)
        {
            KeyDefBuilder<T> keyDefBuilder = new KeyDefBuilder<T>(this, keyName);
            _keyDefBuilders.Add(keyDefBuilder);
            return keyDefBuilder;
        }
    }
}