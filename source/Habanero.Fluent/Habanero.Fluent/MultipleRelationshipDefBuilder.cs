using System;
using System.Linq.Expressions;
using System.Reflection;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Util;

namespace Habanero.Fluent
{
    public class MultipleRelationshipDefBuilder<T> where T : BusinessObject
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
            _relKeyDef = new RelKeyDef();
            _timeOut = 0;
        }

        public ClassDefBuilder<T> Return()
        {
            return _classDefBuilder;
        }
        
        public MultipleRelationshipDefBuilder<T> WithRelationshipName(string relationshipName)
        {
            _relationshipName = relationshipName;
            return this;

        }

        public MultipleRelationshipDefBuilder<T> WithRelatedType<TRelatedType>()
        {
            Type type = typeof(TRelatedType);
            _relatedObjectAssemblyName = type.Namespace;
            _relatedClassName = type.Name;
            return this;
        }


        public MultipleRelationshipDefBuilder<T> WithInsertParentAction(InsertParentAction insertParentAction)
        {
            _insertAction = insertParentAction;
            return this;
        }

        public MultipleRelationshipDefBuilder<T> WithRelationshipType(RelationshipType relationshipType)
        {
            _relationshipType = relationshipType;
            return this;
        }

        public MultipleRelationshipDefBuilder<T> WithRelProp(string ownerClassPropertyName, string relatedPropName)
        {
            RelPropDef relPropDef = new RelPropDef(ownerClassPropertyName, relatedPropName);
            _relKeyDef.Add(relPropDef);
            return this;
        }

        public MultipleRelationshipDef Build()
        {
            return new MultipleRelationshipDef(_relationshipName, _relatedObjectAssemblyName, _relatedClassName, _relKeyDef, _keepReference, _orderBy, _deleteAction, _insertAction, _relationshipType, _timeOut); 
        }


        public MultipleRelationshipDefBuilder<T> WithOrderBy(string orderBy)
        {
            _orderBy = orderBy;
            return this;
        }

        public MultipleRelationshipDefBuilder<T> WithTimeout(int timeout)
        {
            _timeOut = timeout;
            return this;
        }


        public MultipleRelationshipDefBuilder<T> WithRelationshipName<TReturnType>(Expression<Func<T, TReturnType>> propExpression)
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

        public MultipleRelationshipDefBuilder<T> WithRelatedType(Type relatedType)
        {
            _relatedObjectAssemblyName = relatedType.Namespace;
            _relatedClassName = relatedType.Name;
            return this;

        }
    }
}