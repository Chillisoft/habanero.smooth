using System;
using System.Linq.Expressions;
using Habanero.Base;
using Habanero.BO;
using Habanero.Fluent;
using Habanero.Util;

public class NewSingleRelKeyDefBuilder<TBo, TRelatedType>
    where TBo : BusinessObject
    where TRelatedType : BusinessObject
{
    private NewSingleRelationshipDefBuilder<TBo, TRelatedType> _singleRelationshipDefBuilder;
    private NewSingleRelKeyBuilder<TBo, TRelatedType> _singleRelKeyBuilder;

    public NewSingleRelKeyDefBuilder(NewSingleRelationshipDefBuilder<TBo, TRelatedType> singleRelationshipDefBuilder)
    {
        _singleRelationshipDefBuilder = singleRelationshipDefBuilder;
    }

    public NewSingleRelationshipDefBuilder<TBo, TRelatedType> WithRelProp(string ownerPropName, string relatedPropName)
    {
        _singleRelKeyBuilder = new NewSingleRelKeyBuilder<TBo, TRelatedType>(_singleRelationshipDefBuilder);
        _singleRelKeyBuilder.WithRelProp(ownerPropName, relatedPropName);
        return _singleRelationshipDefBuilder;
    }

    public NewSingleRelationshipDefBuilder<TBo, TRelatedType> WithRelProp<TReturn>(Expression<Func<TBo, TReturn>> ownerPropExpression, Expression<Func<TRelatedType, TReturn>> relatedPropExpression)
    {
        string ownerClassPropertyName = ReflectionUtilities.GetPropertyName(ownerPropExpression);
        string relatedPropName = ReflectionUtilities.GetPropertyName(relatedPropExpression);  // note the differente expressions
        return this.WithRelProp(ownerClassPropertyName, relatedPropName);
    }

    public NewSingleRelKeyBuilder<TBo, TRelatedType> WithCompositeRelationshipKey()
    {
        _singleRelKeyBuilder = new NewSingleRelKeyBuilder<TBo, TRelatedType>(_singleRelationshipDefBuilder);
        return _singleRelKeyBuilder;
    }

    public IRelKeyDef Build()
    {
        return _singleRelKeyBuilder.Build();
    }

}