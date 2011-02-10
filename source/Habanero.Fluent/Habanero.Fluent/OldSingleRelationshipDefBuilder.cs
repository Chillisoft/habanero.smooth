using System;
using System.Linq.Expressions;
using System.Reflection;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Util;

namespace Habanero.Fluent
{
    public class OldSingleRelationshipDefBuilder<T, TRelatedType> : ISingleRelDefBuilder where T : BusinessObject
                                                                                      where TRelatedType :
                                                                                          BusinessObject
    {
        private IRelKeyDef _relKeyDef;
        private string _relatedObjectAssemblyName;
        private string _relatedClassName;
        private OldClassDefBuilder<T> _oldClassDefBuilder;
        //private RelKeyBuilder<T, TRelatedType> _relKeyBuilder;
        private string RelationshipName { get; set; }
        private static DeleteParentAction DeleteAction { get; set; }
        private bool KeepReference { get; set; }
        private InsertParentAction InsertAction { get; set; }
        private RelationshipType RelType { get; set; }

        public OldSingleRelationshipDefBuilder(Expression<Func<T, TRelatedType>> relationshipExpression)
        {
            SetupDefaultValues(GetPropertyName(relationshipExpression));
        }

        public OldSingleRelationshipDefBuilder(string relationshipName)
        {
            if (string.IsNullOrEmpty(relationshipName)) throw new ArgumentNullException("relationshipName");
            SetupDefaultValues(relationshipName);
        }

        public OldSingleRelationshipDefBuilder(OldClassDefBuilder<T> oldClassDefBuilder, string relationshipName)
        {
            _oldClassDefBuilder = oldClassDefBuilder;
            SetupDefaultValues(relationshipName);
        }

        public OldSingleRelationshipDefBuilder(OldClassDefBuilder<T> oldClassDefBuilder,
                                            Expression<Func<T, TRelatedType>> relationshipExpression)
        {
            _oldClassDefBuilder = oldClassDefBuilder;
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

        public OldClassDefBuilder<T> Return()
        {
            return _oldClassDefBuilder;
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

        public OldSingleRelationshipDefBuilder<T, TRelatedType> WithInsertParentAction(
            InsertParentAction insertParentAction)
        {
            InsertAction = insertParentAction;
            return this;
        }

        public OldSingleRelationshipDefBuilder<T, TRelatedType> WithRelationshipType(RelationshipType relationshipType)
        {
            RelType = relationshipType;
            return this;
        }


        public ISingleRelationshipDef Build()
        {
            //_relKeyDef = _relKeyBuilder.Build();
            _relKeyDef = this.OldRelKeyDefBuilder.Build();
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

        internal OldRelKeyDefBuilder<T, TRelatedType> OldRelKeyDefBuilder { set; get; }
    }
}