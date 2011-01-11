using Habanero.Base;
using Habanero.BO;
using Habanero.Fluent;

public class RelKeyDefBuilder<TBo, TRelatedType>
    where TBo : BusinessObject
    where TRelatedType : BusinessObject
{
    private SingleRelationshipDefBuilder<TBo, TRelatedType> _singleRelationshipDefBuilder;
    private RelKeyBuilder<TBo, TRelatedType> _relKeyBuilder;

    public RelKeyDefBuilder(SingleRelationshipDefBuilder<TBo, TRelatedType> singleRelationshipDefBuilder)
    {
        _singleRelationshipDefBuilder = singleRelationshipDefBuilder;
    }

    public SingleRelationshipDefBuilder<TBo, TRelatedType> WithRelProp(string ownerPropName, string relatedPropName)
    {
        _relKeyBuilder = new RelKeyBuilder<TBo, TRelatedType>(_singleRelationshipDefBuilder);
        _relKeyBuilder.WithRelProp(ownerPropName, relatedPropName);
        return _singleRelationshipDefBuilder;
    }

    public RelKeyBuilder<TBo, TRelatedType> WithCompositeRelationshipKey()
    {
        _relKeyBuilder = new RelKeyBuilder<TBo, TRelatedType>(_singleRelationshipDefBuilder);
        return _relKeyBuilder;
    }

    public IRelKeyDef Build()
    {
        return _relKeyBuilder.Build();
    }

}