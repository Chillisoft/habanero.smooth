#region Licensing Header
// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2011 Chillisoft Solutions
//  
//  This file is part of the Habanero framework.
//  
//      Habanero is a free framework: you can redistribute it and/or modify
//      it under the terms of the GNU Lesser General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      The Habanero framework is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU Lesser General Public License for more details.
//  
//      You should have received a copy of the GNU Lesser General Public License
//      along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
// ---------------------------------------------------------------------------------
#endregion
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
    public class ClassDefBuilder2<T> where T : BusinessObject
    {
        private ClassDefBuilder<T> _classDefBuilder;
        private PropertiesDefBuilder<T> _propertiesDefBuilder;
        private IPropDefCol _propDefCol;
        private IList<ISingleRelDefBuilder> _singleRelationshipDefBuilders = new List<ISingleRelDefBuilder>();
        private IList<IMultipleRelDefBuilder> _multipleRelationshipDefBuilders = new List<IMultipleRelDefBuilder>();
        private RelationshipDefCol _relationshipDefCol;
        private IList<string> _primaryKeyPropNames;
        private PrimaryKeyDef _primaryKeyDef;
        private IList<KeyDefBuilder<T>> _keyDefBuilders = new List<KeyDefBuilder<T>>();
        private KeyDefCol _keyDefCol = new KeyDefCol();
        private SuperClassDefBuilder<T> _superClassDefBuilder;
        private PropDefBuilder<T> _propDefBuilder;
        private IList<PropDefBuilder<T>> PropDefBuilders { get; set; }

        public ClassDefBuilder2(ClassDefBuilder<T> classDefBuilder, List<PropDefBuilder<T>> propDefBuilders, IList<string> primaryKeyPropNames)
        {
            _classDefBuilder = classDefBuilder;
            _primaryKeyPropNames = primaryKeyPropNames;
            PropDefBuilders = propDefBuilders;
            Initialise();
        }


        public ClassDefBuilder2(ClassDefBuilder<T> classDefBuilder, List<PropDefBuilder<T>> propDefBuilders, IList<string> primaryKeyPropNames, SuperClassDefBuilder<T> superClassDefBuilder)
            :this(classDefBuilder,propDefBuilders,primaryKeyPropNames )
        {   
            _superClassDefBuilder = superClassDefBuilder;
        }

        private void Initialise()
        {
            _propDefCol = new PropDefCol();
            //PropDefBuilders = new List<PropDefBuilder<T>>();
            _propertiesDefBuilder = new PropertiesDefBuilder<T>(this, PropDefBuilders);
            _relationshipDefCol = new RelationshipDefCol();
            _primaryKeyDef = new PrimaryKeyDef();
        }

        public IClassDef Build()
        {
            Type type = typeof(T);
            SetupPropDefCol();
            SetupPrimaryKey();
            SetupRelationshipDefCol();
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
            if (_primaryKeyPropNames == null) return;
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
                    if (_primaryKeyDef.IsGuidObjectID) propDef.Compulsory = true;
                }
            }
        }

        private void UpdatePropDefCol(string propName, bool isPrimaryKeyProp)
        {
            if (!_propDefCol.Contains(propName))
            {
                //set up this property
                var newPropDefBuilder = new PropDefBuilder<T>();
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

        public RelationshipsBuilder<T> WithRelationships()
        {
            return new RelationshipsBuilder<T>(this, _singleRelationshipDefBuilders, _multipleRelationshipDefBuilders);
        }

        public UniqueContraintsBuilder<T> WithUniqueConstraints()
        {
            return new UniqueContraintsBuilder<T>(this, _keyDefBuilders);
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

