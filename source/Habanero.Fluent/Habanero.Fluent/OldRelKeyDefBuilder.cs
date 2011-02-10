using Habanero.Base;
using Habanero.BO;
using Habanero.Fluent;

public class OldRelKeyDefBuilder<TBo, TRelatedType>
    where TBo : BusinessObject
    where TRelatedType : BusinessObject
{
    private OldSingleRelationshipDefBuilder<TBo, TRelatedType> _oldSingleRelationshipDefBuilder;
    private OldRelKeyBuilder<TBo, TRelatedType> _oldRelKeyBuilder;

    public OldRelKeyDefBuilder(OldSingleRelationshipDefBuilder<TBo, TRelatedType> oldSingleRelationshipDefBuilder)
    {
        _oldSingleRelationshipDefBuilder = oldSingleRelationshipDefBuilder;
    }

    public OldSingleRelationshipDefBuilder<TBo, TRelatedType> WithRelProp(string ownerPropName, string relatedPropName)
    {
        _oldRelKeyBuilder = new OldRelKeyBuilder<TBo, TRelatedType>(_oldSingleRelationshipDefBuilder);
        _oldRelKeyBuilder.WithRelProp(ownerPropName, relatedPropName);
        return _oldSingleRelationshipDefBuilder;
    }

    public OldRelKeyBuilder<TBo, TRelatedType> WithCompositeRelationshipKey()
    {
        _oldRelKeyBuilder = new OldRelKeyBuilder<TBo, TRelatedType>(_oldSingleRelationshipDefBuilder);
        return _oldRelKeyBuilder;
    }

    public IRelKeyDef Build()
    {
        return _oldRelKeyBuilder.Build();
    }

}