using System;
using System.Linq.Expressions;
using System.Reflection;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Util;

namespace Habanero.Fluent
{
    public class NewMultipleRelationshipDefBuilder<T, TRelatedType> : IMultipleRelDefBuilder where T : BusinessObject
    {
        private IRelKeyDef _relKeyDef;
        private int _timeOut;
        private string _orderBy;
        private string _relationshipName;
        private static DeleteParentAction _deleteAction;
        private bool _keepReference;
        private InsertParentAction _insertAction;
        private RelationshipType _relationshipType;
        private NewClassDefBuilder2<T> _classDefBuilder;
        private string _relatedObjectAssemblyName;
        private string _relatedClassName;
        private NewRelationshipsBuilder<T> _newRelationshipsBuilder;

        //public NewMultipleRelationshipDefBuilder()
        //{
        //    SetupDefaultValues();
        //}

        //public NewMultipleRelationshipDefBuilder(NewClassDefBuilder2<T> classDefBuilder)
        //{
        //    _classDefBuilder = classDefBuilder;
        //    SetupDefaultValues();
        //}

        public NewMultipleRelationshipDefBuilder(NewRelationshipsBuilder<T> newRelationshipsBuilder, string relationshipName)
        {
            _newRelationshipsBuilder = newRelationshipsBuilder;
            WithRelationshipName(relationshipName); 
            SetupDefaultValues();
        }

        public NewMultipleRelationshipDefBuilder(NewRelationshipsBuilder<T> newRelationshipsBuilder)
        {
            _newRelationshipsBuilder = newRelationshipsBuilder;
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

        public NewRelationshipsBuilder<T> EndMultipleRelationship()
        {
            return _newRelationshipsBuilder;
        }

        public NewMultipleRelationshipDefBuilder<T, TRelatedType> WithRelationshipName(string relationshipName)
        {
            _relationshipName = relationshipName;
            return this;
        }


        public NewMultipleRelationshipDefBuilder<T, TRelatedType> WithInsertParentAction(
            InsertParentAction insertParentAction)
        {
            _insertAction = insertParentAction;
            return this;
        }

        public NewMultipleRelationshipDefBuilder<T, TRelatedType> WithRelationshipType(RelationshipType relationshipType)
        {
            _relationshipType = relationshipType;
            return this;
        }

        public IRelationshipDef Build()
        {
            if (this.MultipleRelKeyDefBuilder == null) this.MultipleRelKeyDefBuilder = new NewMultipleRelKeyDefBuilder<T, TRelatedType>(this);
            _relKeyDef = this.MultipleRelKeyDefBuilder.Build();
            return new MultipleRelationshipDef(_relationshipName, _relatedObjectAssemblyName, _relatedClassName,
                                               _relKeyDef, _keepReference, _orderBy, _deleteAction, _insertAction,
                                               _relationshipType, _timeOut);
        }


        public NewMultipleRelationshipDefBuilder<T, TRelatedType> WithOrderBy(string orderBy)
        {
            _orderBy = orderBy;
            return this;
        }

        public NewMultipleRelationshipDefBuilder<T, TRelatedType> WithTimeout(int timeout)
        {
            _timeOut = timeout;
            return this;
        }


        public NewMultipleRelationshipDefBuilder<T, TRelatedType> WithRelationshipName<TReturnType>(
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

        internal NewMultipleRelKeyDefBuilder<T, TRelatedType> MultipleRelKeyDefBuilder { set; get; }
    }
}