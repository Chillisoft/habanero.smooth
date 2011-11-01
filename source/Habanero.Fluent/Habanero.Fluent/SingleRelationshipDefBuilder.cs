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
    public class SingleRelationshipDefBuilder<T, TRelatedType> : ISingleRelDefBuilder where T : BusinessObject
                                                                                         where TRelatedType :
                                                                                             BusinessObject
    {
        private readonly RelationshipsBuilder<T> _relationshipsBuilder;
        private IRelKeyDef _relKeyDef;
        private string _relatedObjectAssemblyName;
        private string _relatedClassName;
        //private RelKeyBuilder<T, TRelatedType> _relKeyBuilder;
        private string RelationshipName { get; set; }
        private static DeleteParentAction DeleteAction { get; set; }
        private bool KeepReference { get; set; }
        private InsertParentAction InsertAction { get; set; }
        private RelationshipType RelType { get; set; }

        //public SingleRelationshipDefBuilder(Expression<Func<T, TRelatedType>> relationshipExpression)
        //{
        //    SetupDefaultValues(GetPropertyName(relationshipExpression));
        //}

        //public SingleRelationshipDefBuilder(string relationshipName)
        //{
        //    if (string.IsNullOrEmpty(relationshipName)) throw new ArgumentNullException("relationshipName");
        //    SetupDefaultValues(relationshipName);
        //}

        public SingleRelationshipDefBuilder(RelationshipsBuilder<T> relationshipsBuilder, string relationshipName)
        {
            if (string.IsNullOrEmpty(relationshipName)) throw new ArgumentNullException("relationshipName");
            _relationshipsBuilder = relationshipsBuilder;
            SetupDefaultValues(relationshipName);
        }

        public SingleRelationshipDefBuilder(RelationshipsBuilder<T> relationshipsBuilder, Expression<Func<T, TRelatedType>> relationshipExpression)
        {
            _relationshipsBuilder = relationshipsBuilder;
            SetupDefaultValues(GetPropertyName(relationshipExpression));
        }

        private void SetupDefaultValues(string relationshipName)
        {
            DeleteAction = DeleteParentAction.DoNothing;
            InsertAction = InsertParentAction.InsertRelationship;
            RelType = RelationshipType.Association;
            KeepReference = true;
            Type type = typeof (TRelatedType);
            _relatedObjectAssemblyName = type.Namespace;
            _relatedClassName = type.Name;
            RelationshipName = relationshipName;
            //_relKeyBuilder = CreateRelKeyBuilder();
        }

        //protected virtual RelKeyBuilder<T, TRelatedType> CreateRelKeyBuilder()
        //{
        //    return new RelKeyBuilder<T, TRelatedType>(this);
        //}

        public RelationshipsBuilder<T> EndSingleRelationship()
        {
            return _relationshipsBuilder;
        }

/*
        public SingleRelationshipDefBuilder<T, TRelatedType> WithRelProp( string ownerClassPropertyName, string relatedPropName)
        {
            
            _relKeyBuilder.WithRelProp(ownerClassPropertyName, relatedPropName);
            return this;
        }

        public SingleRelationshipDefBuilder<T, TRelatedType> WithRelProp<TReturn>( Expression<Func<T, TReturn>> ownerPropExpression, Expression<Func<TRelatedType, TReturn>> relatedPropExpression)
        {
            _relKeyBuilder.WithRelProp(ownerPropExpression, relatedPropExpression);
            return this;
        }*/

        public SingleRelationshipDefBuilder<T, TRelatedType> WithInsertParentAction(
            InsertParentAction insertParentAction)
        {
            InsertAction = insertParentAction;
            return this;
        }

        public SingleRelationshipDefBuilder<T, TRelatedType> WithRelationshipType(RelationshipType relationshipType)
        {
            RelType = relationshipType;
            return this;
        }


        public ISingleRelationshipDef Build()
        {
            //_relKeyDef = _relKeyBuilder.Build();
            if(this.SingleRelKeyDefBuilder == null) this.SingleRelKeyDefBuilder = new SingleRelKeyDefBuilder<T, TRelatedType>(this);
            _relKeyDef = this.SingleRelKeyDefBuilder.Build();
            return new SingleRelationshipDef(RelationshipName, _relatedObjectAssemblyName, _relatedClassName, _relKeyDef,
                                             KeepReference, DeleteAction, InsertAction, RelType);
        }

/*        public SingleRelationshipDefBuilder<T, TRelatedType> WithRelationshipName<TReturnType>(Expression<Func<T, TReturnType>> propExpression)
        {
            RelationshipName = GetPropertyName(propExpression);
            return this;
        }*/
/*
        private static string GetRelatedPropertyName<TReturn>(Expression<Func<TRelatedType, TReturn>> propExpression)
        {
            return GetPropertyInfo(propExpression).Name;
        }*/

        private static string GetPropertyName<TReturn>(Expression<Func<T, TReturn>> propExpression)
        {
            return GetPropertyInfo(propExpression).Name;
        }

        private static PropertyInfo GetPropertyInfo<TModel, TReturn>(Expression<Func<TModel, TReturn>> expression)
        {
            return ReflectionUtilities.GetPropertyInfo(expression);
        }

        internal protected SingleRelKeyDefBuilder<T, TRelatedType> SingleRelKeyDefBuilder { set; get; }
    }

}