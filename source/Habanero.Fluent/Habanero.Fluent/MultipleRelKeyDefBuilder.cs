using System;
using System.Linq.Expressions;
using Habanero.Base;
using Habanero.BO;
using Habanero.Fluent;
using Habanero.Util;

public class MultipleRelKeyDefBuilder<TBo, TRelatedType>
    where TBo : BusinessObject
{
    private MultipleRelationshipDefBuilder<TBo, TRelatedType> _multipleRelationshipDefBuilder;
    private MultipleRelKeyBuilder<TBo, TRelatedType> _multipleRelKeyBuilder;

    public MultipleRelKeyDefBuilder(MultipleRelationshipDefBuilder<TBo, TRelatedType> multipleRelationshipDefBuilder)
    {
        _multipleRelationshipDefBuilder = multipleRelationshipDefBuilder;
        _multipleRelKeyBuilder = new MultipleRelKeyBuilder<TBo, TRelatedType>(_multipleRelationshipDefBuilder);
    }

    public MultipleRelationshipDefBuilder<TBo, TRelatedType> WithRelProp(string ownerPropName, string relatedPropName)
    {
        _multipleRelKeyBuilder.WithRelProp(ownerPropName, relatedPropName);
        return _multipleRelationshipDefBuilder;
    }

    public MultipleRelationshipDefBuilder<TBo, TRelatedType> WithRelProp<TReturn>(Expression<Func<TBo, TReturn>> ownerPropExpression, Expression<Func<TRelatedType, TReturn>> relatedPropExpression)
    {
        string ownerClassPropertyName = ReflectionUtilities.GetPropertyName(ownerPropExpression);
        string relatedPropName = ReflectionUtilities.GetPropertyName(relatedPropExpression);  // note the different expression
        return this.WithRelProp(ownerClassPropertyName, relatedPropName);
    }

    public MultipleRelKeyBuilder<TBo, TRelatedType> WithCompositeRelationshipKey()
    {
        return _multipleRelKeyBuilder;
    }

    public IRelKeyDef Build()
    {
        return _multipleRelKeyBuilder.Build();
    }

}