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
using System.Linq.Expressions;
using System.Reflection;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Util;

namespace Habanero.Fluent
{
    public class MultipleRelationshipDefBuilder<T, TRelatedType> : IMultipleRelDefBuilder where T : BusinessObject
    {
        private IRelKeyDef _relKeyDef;
        private int _timeOut;
        private string _orderBy;
        private string _relationshipName;
        private static DeleteParentAction _deleteAction;
        private bool _keepReference;
        private InsertParentAction _insertAction;
        private RelationshipType _relationshipType;
        private ClassDefBuilder2<T> _classDefBuilder;
        private string _relatedObjectAssemblyName;
        private string _relatedClassName;
        private RelationshipsBuilder<T> _relationshipsBuilder;

        //public MultipleRelationshipDefBuilder()
        //{
        //    SetupDefaultValues();
        //}

        //public MultipleRelationshipDefBuilder(ClassDefBuilder2<T> classDefBuilder)
        //{
        //    _classDefBuilder = classDefBuilder;
        //    SetupDefaultValues();
        //}

        public MultipleRelationshipDefBuilder(RelationshipsBuilder<T> relationshipsBuilder, string relationshipName)
        {
            _relationshipsBuilder = relationshipsBuilder;
            WithRelationshipName(relationshipName); 
            SetupDefaultValues();
        }

        public MultipleRelationshipDefBuilder(RelationshipsBuilder<T> relationshipsBuilder)
        {
            _relationshipsBuilder = relationshipsBuilder;
            //WithRelationshipName(relationshipExpression); 
            SetupDefaultValues();
        }

        private void SetupDefaultValues()
        {
            _deleteAction = DeleteParentAction.DoNothing;
            _insertAction = InsertParentAction.InsertRelationship;
            _relationshipType = RelationshipType.Association;
            _keepReference = true;
            _orderBy = "";
            Type type = typeof (TRelatedType);
            _relatedObjectAssemblyName = type.Namespace;
            _relatedClassName = type.Name;
            _relKeyDef = new RelKeyDef();
            _timeOut = 0;
        }

        public RelationshipsBuilder<T> EndMultipleRelationship()
        {
            return _relationshipsBuilder;
        }

        public MultipleRelationshipDefBuilder<T, TRelatedType> WithRelationshipName(string relationshipName)
        {
            _relationshipName = relationshipName;
            return this;
        }


        public MultipleRelationshipDefBuilder<T, TRelatedType> WithInsertParentAction(
            InsertParentAction insertParentAction)
        {
            _insertAction = insertParentAction;
            return this;
        }

        public MultipleRelationshipDefBuilder<T, TRelatedType> WithDeleteParentAction(DeleteParentAction deleteParentAction)
        {
            _deleteAction = deleteParentAction;
            return this;
        }

        public MultipleRelationshipDefBuilder<T, TRelatedType> WithRelationshipType(RelationshipType relationshipType)
        {
            _relationshipType = relationshipType;
            return this;
        }

        public IRelationshipDef Build()
        {
            if (this.MultipleRelKeyDefBuilder == null) this.MultipleRelKeyDefBuilder = new MultipleRelKeyDefBuilder<T, TRelatedType>(this);
            _relKeyDef = this.MultipleRelKeyDefBuilder.Build();
            return new MultipleRelationshipDef(_relationshipName, _relatedObjectAssemblyName, _relatedClassName,
                                               _relKeyDef, _keepReference, _orderBy, _deleteAction, _insertAction,
                                               _relationshipType, _timeOut);
        }


        public MultipleRelationshipDefBuilder<T, TRelatedType> WithOrderBy(string orderBy)
        {
            _orderBy = orderBy;
            return this;
        }

        public MultipleRelationshipDefBuilder<T, TRelatedType> WithTimeout(int timeout)
        {
            _timeOut = timeout;
            return this;
        }


        public MultipleRelationshipDefBuilder<T, TRelatedType> WithRelationshipName<TReturnType>(
            Expression<Func<T, TReturnType>> propExpression)
        {
            _relationshipName = GetPropertyName(propExpression);
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

        private static string GetRelatedPropertyName<TReturn>(Expression<Func<TRelatedType, TReturn>> propExpression)
        {
            return GetPropertyInfo(propExpression).Name;
        }

        internal MultipleRelKeyDefBuilder<T, TRelatedType> MultipleRelKeyDefBuilder { set; get; }
    }
}