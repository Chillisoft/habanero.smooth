using System;
using System.Linq.Expressions;
using System.Reflection;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Util;

namespace Habanero.Fluent
{
    public interface IMulipleRelDefBuilder
    {
        IRelationshipDef Build();
    }

    public class MultipleRelationshipDefBuilder<T, TRelatedType> : IMulipleRelDefBuilder where T : BusinessObject
    {
        private RelKeyDef _relKeyDef;
        private int _timeOut;
        private string _orderBy;
        private string _relationshipName;
        private static DeleteParentAction _deleteAction;
        private bool _keepReference;
        private InsertParentAction _insertAction;
        private RelationshipType _relationshipType;
        private ClassDefBuilder<T> _classDefBuilder;
        private string _relatedObjectAssemblyName;
        private string _relatedClassName;

        public MultipleRelationshipDefBuilder()
        {
            SetupDefaultValues();
        }

        public MultipleRelationshipDefBuilder(ClassDefBuilder<T> classDefBuilder)
        {
            _classDefBuilder = classDefBuilder;
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

        public ClassDefBuilder<T> Return()
        {
            return _classDefBuilder;
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

        public MultipleRelationshipDefBuilder<T, TRelatedType> WithRelationshipType(RelationshipType relationshipType)
        {
            _relationshipType = relationshipType;
            return this;
        }

        public MultipleRelationshipDefBuilder<T, TRelatedType> WithRelProp(string ownerClassPropertyName,
                                                                           string relatedPropName)
        {
            _relKeyDef.Add(new RelPropDef(ownerClassPropertyName, relatedPropName));
            return this;
        }

        public MultipleRelationshipDefBuilder<T, TRelatedType> WithRelProp<TReturn>(
            Expression<Func<T, TReturn>> ownerPropExpression,
            Expression<Func<TRelatedType, TReturn>> relatedPropExpression)
        {
            string ownerClassPropertyName = GetPropertyName(ownerPropExpression);
            string relatedPropName = GetRelatedPropertyName(relatedPropExpression);
            return WithRelProp(ownerClassPropertyName, relatedPropName);
        }

        public IRelationshipDef Build()
        {
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
    }
}