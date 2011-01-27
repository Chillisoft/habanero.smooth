using Habanero.Base;
using Habanero.BO;
using Habanero.Fluent;

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