using Habanero.Base;
using Habanero.BO;
using Habanero.Fluent;

public class NewMultipleRelKeyDefBuilder<TBo, TRelatedType>
    where TBo : BusinessObject
{
    private NewMultipleRelationshipDefBuilder<TBo, TRelatedType> _multipleRelationshipDefBuilder;
    private NewMultipleRelKeyBuilder<TBo, TRelatedType> _multipleRelKeyBuilder;

    public NewMultipleRelKeyDefBuilder(NewMultipleRelationshipDefBuilder<TBo, TRelatedType> multipleRelationshipDefBuilder)
    {
        _multipleRelationshipDefBuilder = multipleRelationshipDefBuilder;
    }

    public NewMultipleRelationshipDefBuilder<TBo, TRelatedType> WithRelProp(string ownerPropName, string relatedPropName)
    {
        _multipleRelKeyBuilder = new NewMultipleRelKeyBuilder<TBo, TRelatedType>(_multipleRelationshipDefBuilder);
        _multipleRelKeyBuilder.WithRelProp(ownerPropName, relatedPropName);
        return _multipleRelationshipDefBuilder;
    }

    public NewMultipleRelKeyBuilder<TBo, TRelatedType> WithCompositeRelationshipKey()
    {
        _multipleRelKeyBuilder = new NewMultipleRelKeyBuilder<TBo, TRelatedType>(_multipleRelationshipDefBuilder);
        return _multipleRelKeyBuilder;
    }

    public IRelKeyDef Build()
    {
        return _multipleRelKeyBuilder.Build();
    }

}