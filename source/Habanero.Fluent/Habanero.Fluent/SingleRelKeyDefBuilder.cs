using System;
using System.Linq.Expressions;
using Habanero.Base;
using Habanero.BO;
using Habanero.Fluent;
using Habanero.Util;

public class SingleRelKeyDefBuilder<TBo, TRelatedType>
    where TBo : BusinessObject
    where TRelatedType : BusinessObject
{
    private SingleRelationshipDefBuilder<TBo, TRelatedType> _singleRelationshipDefBuilder;
    private SingleRelKeyBuilder<TBo, TRelatedType> _singleRelKeyBuilder;

    public SingleRelKeyDefBuilder(SingleRelationshipDefBuilder<TBo, TRelatedType> singleRelationshipDefBuilder)
    {
        _singleRelationshipDefBuilder = singleRelationshipDefBuilder;
        _singleRelKeyBuilder = new SingleRelKeyBuilder<TBo, TRelatedType>(_singleRelationshipDefBuilder);
    }

    public SingleRelationshipDefBuilder<TBo, TRelatedType> WithRelProp(string ownerPropName, string relatedPropName)
    {
        //_singleRelKeyBuilder = new SingleRelKeyBuilder<TBo, TRelatedType>(_singleRelationshipDefBuilder);
        _singleRelKeyBuilder.WithRelProp(ownerPropName, relatedPropName);
        return _singleRelationshipDefBuilder;
    }

    public SingleRelationshipDefBuilder<TBo, TRelatedType> WithRelProp<TReturn>(Expression<Func<TBo, TReturn>> ownerPropExpression, Expression<Func<TRelatedType, TReturn>> relatedPropExpression)
    {
        string ownerClassPropertyName = ReflectionUtilities.GetPropertyName(ownerPropExpression);
        string relatedPropName = ReflectionUtilities.GetPropertyName(relatedPropExpression);  // note the differente expressions
        return this.WithRelProp(ownerClassPropertyName, relatedPropName);
    }

    public SingleRelKeyBuilder<TBo, TRelatedType> WithCompositeRelationshipKey()
    {
        //_singleRelKeyBuilder = new SingleRelKeyBuilder<TBo, TRelatedType>(_singleRelationshipDefBuilder);
        return _singleRelKeyBuilder;
    }

    public IRelKeyDef Build()
    {
        return _singleRelKeyBuilder.Build();
    }

}