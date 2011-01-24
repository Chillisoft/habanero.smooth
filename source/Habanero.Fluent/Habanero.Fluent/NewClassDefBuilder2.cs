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
    public class NewClassDefBuilder2<T> where T : BusinessObject
    {
        private NewClassDefBuilder<T> _classDefBuilder;
        private NewPropertiesDefBuilder<T> _propertiesDefBuilder;

        public NewClassDefBuilder2(NewClassDefBuilder<T> classDefBuilder, IList<string> primaryKeyPropNames)
        {
            _classDefBuilder = classDefBuilder;
            _primaryKeyPropNames = primaryKeyPropNames;
//             _builders
            _propDefCol = new PropDefCol();
            PropDefBuilders = new List<NewPropDefBuilder<T>>();
            _propertiesDefBuilder = new NewPropertiesDefBuilder<T>(this, PropDefBuilders);
            _relationshipDefCol = new RelationshipDefCol();
            _primaryKeyDef = new PrimaryKeyDef();
        }



        private IPropDefCol _propDefCol;

        private IList<ISingleRelDefBuilder> _singleRelationshipDefBuilders = new List<ISingleRelDefBuilder>();
        private IList<IMulipleRelDefBuilder> _multipleRelationshipDefBuilders = new List<IMulipleRelDefBuilder>();


        private RelationshipDefCol _relationshipDefCol;
        //private IList<MultipleRelationshipDefBuilder<T>> _multipleRelationshipDefBuilders = new List<MultipleRelationshipDefBuilder<T>>();
        private IList<string> _primaryKeyPropNames;
        private PrimaryKeyDef _primaryKeyDef;
        private IList<NewKeyDefBuilder<T>> _keyDefBuilders = new List<NewKeyDefBuilder<T>>();
        private KeyDefCol _keyDefCol = new KeyDefCol();
        private SuperClassDefBuilder<T> _superClassDefBuilder;
        private NewPropDefBuilder<T> _newPropDefBuilder;
        private IList<NewPropDefBuilder<T>> PropDefBuilders { get; set; }



        public IClassDef Build()
        {
            Type type = typeof(T);
            SetupPropDefCol();
            SetupRelationshipDefCol();
            SetupPrimaryKey();
            SetupKeysCol();
            var classDef = new ClassDef(type.Namespace, type.Name, _primaryKeyDef, _propDefCol, _keyDefCol, _relationshipDefCol, new UIDefCol());
            if (_superClassDefBuilder != null)
            {
                classDef.SuperClassDef = _superClassDefBuilder.Build();
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
                UpdatePropDefCol(propName, true);
                var propDef = _propDefCol[propName];
                _primaryKeyDef.Add(propDef);
                if (_primaryKeyPropNames.Count == 1)
                {
                    _primaryKeyDef.IsGuidObjectID = propDef.PropertyType == typeof (Guid);

                }
            }
        }

        private void UpdatePropDefCol(string propName, bool isPrimaryKeyProp)
        {
            if (!_propDefCol.Contains(propName))
            {
                //set up this property
                var newPropDefBuilder = new NewPropDefBuilder<T>();
                var propertyInfo = ReflectionUtilities.GetPropertyInfo(typeof (T), propName);
                if (propertyInfo == null)
                {
                    newPropDefBuilder.WithPropertyName(propName);
                    if (isPrimaryKeyProp)
                    {
                        newPropDefBuilder.WithType(typeof(Guid));
                    }
                }
                else
                {
                    Type propertyType = ReflectionUtilities.GetUndelyingPropertType(propertyInfo);
                    newPropDefBuilder.WithPropertyName(propertyInfo.Name);
                    newPropDefBuilder.WithType(propertyType);
                }
                if (isPrimaryKeyProp)
                {
                    newPropDefBuilder.WithReadWriteRule(PropReadWriteRule.WriteNew);
                }
                var pkPropDef = newPropDefBuilder.Build();
                _propDefCol.Add(pkPropDef);
            }
        }


        private void SetupRelationshipDefCol()
        {
            foreach (var singleRelationshipDefBuilder in _singleRelationshipDefBuilders)
            {
                var singleRelationshipDef = singleRelationshipDefBuilder.Build();
                CheckOwnerProps(singleRelationshipDef.RelKeyDef);
                _relationshipDefCol.Add(singleRelationshipDef);
            }
            foreach (var multipleRelationshipDefBuilder in _multipleRelationshipDefBuilders)
            {
                var multipleRelationshipDef = multipleRelationshipDefBuilder.Build();
                CheckOwnerProps(multipleRelationshipDef.RelKeyDef);
                _relationshipDefCol.Add(multipleRelationshipDef);
            }
        }

        private void CheckOwnerProps(IRelKeyDef relPropDefs)
        {
            foreach (var relPropDef in relPropDefs)
            {
                UpdatePropDefCol(relPropDef.OwnerPropertyName, false);

            }
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

        public NewPropertiesDefBuilder<T> WithProperties()
        {
            return _propertiesDefBuilder;
        }

        //public NewSingleRelationshipDefBuilder<T, TRelatedType> WithSingleRelationship<TRelatedType>(string relationshipName) where TRelatedType : BusinessObject
        //{
        //    var singleRelationshipDefBuilder = new NewSingleRelationshipDefBuilder<T, TRelatedType>(this, relationshipName);
        //    _singleRelationshipDefBuilders.Add(singleRelationshipDefBuilder);
        //    //return new RelKeyBuilder<T, TRelatedType>(singleRelationshipDefBuilder);
        //    return singleRelationshipDefBuilder;
        //}

        //public NewSingleRelationshipDefBuilder<T, TRelatedType> WithSingleRelationship<TRelatedType>(Expression<Func<T, TRelatedType>> relationshipExpression) where TRelatedType : BusinessObject
        //{
        //    NewSingleRelationshipDefBuilder<T, TRelatedType> singleRelationshipDefBuilder = new NewSingleRelationshipDefBuilder<T, TRelatedType>(this, relationshipExpression);
        //    _singleRelationshipDefBuilders.Add(singleRelationshipDefBuilder);
        //    return singleRelationshipDefBuilder;

        //}

        public NewRelationshipsBuilder<T> WithRelationships()
        {
            return new NewRelationshipsBuilder<T>(this, _singleRelationshipDefBuilders, _multipleRelationshipDefBuilders);
        }

        public NewKeyDefBuilder<T> WithUniqueConstraint(string keyName = "")
        {
            NewKeyDefBuilder<T> keyDefBuilder = new NewKeyDefBuilder<T>(this, keyName);
            _keyDefBuilders.Add(keyDefBuilder);
            return keyDefBuilder;
        }


        private IPropDefCol SetupPropDefCol()
        {
            foreach (var propDefBuilder in PropDefBuilders)
            {
                var propDef = propDefBuilder.Build();
                _propDefCol.Add(propDef);
            }
            return _propDefCol;
        }


    }
}

