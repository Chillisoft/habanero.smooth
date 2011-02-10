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
    public class OldClassDefBuilder<T> where T:BusinessObject
    {
        private PropDefCol _propDefCol;

        private IList<ISingleRelDefBuilder> _singleRelationshipDefBuilders = new List<ISingleRelDefBuilder>();
        private IList<IMultipleRelDefBuilder> _multipleRelationshipDefBuilders = new List<IMultipleRelDefBuilder>();


        private RelationshipDefCol _relationshipDefCol;
        //private IList<MultipleRelationshipDefBuilder<T>> _multipleRelationshipDefBuilders = new List<MultipleRelationshipDefBuilder<T>>();
        private List<string> _primaryKeyPropNames;
        private PrimaryKeyDef _primaryKeyDef;
        private IList<OldKeyDefBuilder<T>> _keyDefBuilders = new List<OldKeyDefBuilder<T>>();
        private KeyDefCol _keyDefCol = new KeyDefCol();
        private OldSuperClassDefBuilder<T> _oldSuperClassDefBuilder;

        private IList<OldPropDefBuilder<T>> PropDefBuilders { get; set; }

        public OldClassDefBuilder()
        {
            PropDefBuilders = new List<OldPropDefBuilder<T>>();
//             _builders
            _propDefCol = new PropDefCol();
            _relationshipDefCol = new RelationshipDefCol();
            _primaryKeyPropNames = new List<string>();
            _primaryKeyDef = new PrimaryKeyDef();
        }

        public IClassDef Build()
        {
            Type type = typeof(T);
            SetupPropDefCol();
            SetupRelationshipDefCol();
            SetupPrimaryKey();
            SetupKeysCol();
            var classDef = new ClassDef(type.Namespace, type.Name, _primaryKeyDef, _propDefCol, _keyDefCol, _relationshipDefCol, new UIDefCol());
            if (_oldSuperClassDefBuilder != null)
            {
                classDef.SuperClassDef = _oldSuperClassDefBuilder.Build();
            }
            return classDef;
        }

        private void SetupKeysCol()
        {
            foreach (var keyDefBuilder in _keyDefBuilders)
            {
                var keyDef = keyDefBuilder.Build(_propDefCol);
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
                var propDef = _propDefCol[propName];
                _primaryKeyDef.Add(propDef);
                if (_primaryKeyPropNames.Count == 1)
                {
                    _primaryKeyDef.IsGuidObjectID = propDef.PropertyType == typeof (Guid);

                }
            }
        }

        private void SetupPropDefCol()
        {
            foreach(var propDefBuilder in PropDefBuilders)
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

        public OldPropDefBuilder<T> WithProperty(string propertyName)
        {
            var propDefBuilder = GetPropDefBuilder(propertyName);
            PropDefBuilders.Add(propDefBuilder);
            return propDefBuilder;
        }
        public OldPropDefBuilder<T> WithProperty<TReturnType>(string propertyName)
        {
            var propDefBuilder = GetPropDefBuilder(propertyName);
            propDefBuilder.WithType<TReturnType>();
            PropDefBuilders.Add(propDefBuilder);
            return propDefBuilder;
        }

        public OldPropDefBuilder<T> WithProperty<TReturnType>(Expression<Func<T, TReturnType>> propExpression)
        {
            var propDefBuilder = new OldPropDefBuilder<T>(this);
            propDefBuilder.WithProperty(propExpression);
            PropDefBuilders.Add(propDefBuilder);
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

/*        /// <summary>
        /// Creates a valid value for the property identified by the lambda expression <paramref name="propExpression"/>.
        /// </summary>
        /// <param name="propExpression"></param>
        /// <returns></returns>
        public TReturn GetValidPropValue<TReturn>(Expression<Func<T, TReturn>> propExpression)
        {
            return this.GetValidPropValue(this.BusinessObject, propExpression);
        }*/
        private OldPropDefBuilder<T> GetPropDefBuilder(string propertyName)
        {
            var propDefBuilder = new OldPropDefBuilder<T>(this);
            propDefBuilder.WithPropertyName(propertyName);
            return propDefBuilder;
        }

        public OldSingleRelationshipDefBuilder<T, TRelatedType> WithSingleRelationship<TRelatedType>(string relationshipName) where TRelatedType : BusinessObject
        {
            var singleRelationshipDefBuilder = new OldSingleRelationshipDefBuilder<T, TRelatedType>(this, relationshipName);
            _singleRelationshipDefBuilders.Add(singleRelationshipDefBuilder);
            //return new RelKeyBuilder<T, TRelatedType>(singleRelationshipDefBuilder);
            return singleRelationshipDefBuilder;
        }

        public OldSingleRelationshipDefBuilder<T, TRelatedType> WithSingleRelationship<TRelatedType>(Expression<Func<T, TRelatedType>> relationshipExpression) where TRelatedType : BusinessObject
        {
            OldSingleRelationshipDefBuilder<T, TRelatedType> oldSingleRelationshipDefBuilder = new OldSingleRelationshipDefBuilder<T, TRelatedType>(this, relationshipExpression);
            _singleRelationshipDefBuilders.Add(oldSingleRelationshipDefBuilder);
            return oldSingleRelationshipDefBuilder;

        }
        public OldRelKeyDefBuilder<T, TRelatedType> WithNewSingleRelationship<TRelatedType>(Expression<Func<T, TRelatedType>> relationshipExpression) where TRelatedType : BusinessObject
        {
            OldSingleRelationshipDefBuilder<T, TRelatedType> oldSingleRelationshipDefBuilder = new OldSingleRelationshipDefBuilder<T, TRelatedType>(this, relationshipExpression);
            _singleRelationshipDefBuilders.Add(oldSingleRelationshipDefBuilder);
            var relKeyDefBuilder = new OldRelKeyDefBuilder<T, TRelatedType>(oldSingleRelationshipDefBuilder);
            oldSingleRelationshipDefBuilder.OldRelKeyDefBuilder = relKeyDefBuilder;
            return relKeyDefBuilder;
        }

        public OldMultipleRelationshipDefBuilder<T, TRelatedType> WithMultipleRelationship<TRelatedType>(string relationshipName) where TRelatedType : IBusinessObject
        {
            OldMultipleRelationshipDefBuilder<T, TRelatedType> oldMultipleRelationshipDefBuilder = new OldMultipleRelationshipDefBuilder<T, TRelatedType>(this)
                .WithRelationshipName(relationshipName);
                //.WithRelatedType<TRelatedType>();
            _multipleRelationshipDefBuilders.Add(oldMultipleRelationshipDefBuilder);
            return oldMultipleRelationshipDefBuilder;
                
        }

        public OldMultipleRelationshipDefBuilder<T, TBusinessObject> WithMultipleRelationship<TBusinessObject>(Expression<Func<T, BusinessObjectCollection<TBusinessObject>>> relationshipExpression)
            where TBusinessObject : class, IBusinessObject, new()
        {
            string relationshipName = GetPropertyName(relationshipExpression);
            OldMultipleRelationshipDefBuilder<T, TBusinessObject> oldMultipleRelationshipDefBuilder = new OldMultipleRelationshipDefBuilder<T, TBusinessObject>(this)
                .WithRelationshipName(relationshipName);
            _multipleRelationshipDefBuilders.Add(oldMultipleRelationshipDefBuilder);
            return oldMultipleRelationshipDefBuilder;
                
        }

        public OldClassDefBuilder<T> WithPrimaryKeyProp(string propertyName)
        {
            _primaryKeyPropNames.Add(propertyName);
            return this;
        }
        public OldClassDefBuilder<T> WithPrimaryKeyProp<TReturn>(Expression<Func<T, TReturn>> propExpression)
        {
            string propertyName = GetPropertyName(propExpression);
            _primaryKeyPropNames.Add(propertyName);
            return this;
        }

        public OldKeyDefBuilder<T> WithUniqueConstraint(string keyName = "")
        {
            OldKeyDefBuilder<T> oldKeyDefBuilder = new OldKeyDefBuilder<T>(this, keyName);
            _keyDefBuilders.Add(oldKeyDefBuilder);
            return oldKeyDefBuilder;
        }

        public OldSuperClassDefBuilder<T> WithSuperClass()
        {
            _oldSuperClassDefBuilder = new OldSuperClassDefBuilder<T>(this);
            return _oldSuperClassDefBuilder;
        }


    }

}