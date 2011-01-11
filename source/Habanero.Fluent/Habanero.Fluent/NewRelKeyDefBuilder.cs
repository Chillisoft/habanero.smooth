using Habanero.Base;
using Habanero.BO;
using Habanero.Fluent;

public class NewRelKeyDefBuilder<TBo, TRelatedType>
    where TBo : BusinessObject
    where TRelatedType : BusinessObject
{
    private NewSingleRelationshipDefBuilder<TBo, TRelatedType> _singleRelationshipDefBuilder;
    private NewRelKeyBuilder<TBo, TRelatedType> _relKeyBuilder;

    public NewRelKeyDefBuilder(NewSingleRelationshipDefBuilder<TBo, TRelatedType> singleRelationshipDefBuilder)
    {
        _singleRelationshipDefBuilder = singleRelationshipDefBuilder;
    }

    public NewSingleRelationshipDefBuilder<TBo, TRelatedType> WithRelProp(string ownerPropName, string relatedPropName)
    {
        _relKeyBuilder = new NewRelKeyBuilder<TBo, TRelatedType>(_singleRelationshipDefBuilder);
        _relKeyBuilder.WithRelProp(ownerPropName, relatedPropName);
        return _singleRelationshipDefBuilder;
    }

    public NewRelKeyBuilder<TBo, TRelatedType> WithCompositeRelationshipKey()
    {
        _relKeyBuilder = new NewRelKeyBuilder<TBo, TRelatedType>(_singleRelationshipDefBuilder);
        return _relKeyBuilder;
    }

    public IRelKeyDef Build()
    {
        return _relKeyBuilder.Build();
    }

}