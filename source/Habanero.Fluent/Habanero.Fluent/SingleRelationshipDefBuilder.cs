using System;
using System.Linq.Expressions;
using System.Reflection;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Util;

namespace Habanero.Fluent
{
    public class SingleRelationshipDefBuilder<T> where T:BusinessObject
    {
        private RelKeyDef _relKeyDef;
        private string _relatedObjectAssemblyName;
        private string _relatedClassName;
        private ClassDefBuilder<T> _classDefBuilder;
        private string RelationshipName { get; set; }
        private static DeleteParentAction DeleteAction { get; set; }
        private bool KeepReference { get; set; }
        private InsertParentAction InsertAction { get; set; }
        private RelationshipType RelType { get; set; }

        public SingleRelationshipDefBuilder()
        {
            SetupDefaultValues();
        }


        public SingleRelationshipDefBuilder(ClassDefBuilder<T> classDefBuilder)
        {
            _classDefBuilder = classDefBuilder;
            SetupDefaultValues();
        }

        private void SetupDefaultValues()
        {
            DeleteAction = DeleteParentAction.DoNothing;
            InsertAction = InsertParentAction.InsertRelationship;
            RelType = RelationshipType.Association;
            KeepReference = true;
            _relKeyDef = new RelKeyDef();
        }

        public ClassDefBuilder<T> Return()
        {
            return _classDefBuilder;
        }
        
        public SingleRelationshipDefBuilder<T> WithRelationshipName(string relationshipName)
        {
            RelationshipName = relationshipName;
            return this;

        }

        public SingleRelationshipDefBuilder<T> WithRelatedType<TRelatedType>()
        {
            Type type = typeof(TRelatedType);
            _relatedObjectAssemblyName = type.Namespace;
            _relatedClassName = type.Name;
            return this;
        }

        public SingleRelationshipDefBuilder<T> WithInsertParentAction(InsertParentAction insertParentAction)
        {
            InsertAction = insertParentAction;
            return this;
        }

        public SingleRelationshipDefBuilder<T> WithRelationshipType(RelationshipType relationshipType)
        {
            RelType = relationshipType;
            return this;
        }

        public SingleRelationshipDefBuilder<T> WithRelProp(string ownerClassPropertyName, string relatedPropName)
        {
            RelPropDef relPropDef = new RelPropDef(ownerClassPropertyName, relatedPropName);
            _relKeyDef.Add(relPropDef);
            return this;
        }

        public ISingleRelationshipDef Build()
        {
            return new SingleRelationshipDef(RelationshipName, _relatedObjectAssemblyName, _relatedClassName, _relKeyDef, KeepReference, DeleteAction, InsertAction, RelType);
        }

        public SingleRelationshipDefBuilder<T> WithRelationshipName<TReturnType>(Expression<Func<T, TReturnType>> propExpression)
        {
            RelationshipName = GetPropertyName(propExpression);
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

    }
}