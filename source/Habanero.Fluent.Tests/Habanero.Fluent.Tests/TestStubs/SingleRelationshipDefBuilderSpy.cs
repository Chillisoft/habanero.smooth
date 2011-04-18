using Habanero.BO;
using Habanero.Smooth.Test;

namespace Habanero.Fluent.Tests.TestStubs
{
    class SingleRelationshipDefBuilderSpy<T, TRelatedType> : SingleRelationshipDefBuilder<T, TRelatedType>
        where T : BusinessObject
        where TRelatedType : BusinessObject
    {
        public SingleRelationshipDefBuilderSpy()
            : base(new RelationshipsBuilderStub<T>(), GetRandomRelName())
        {
            /*                var relKeyDefBuilder = new SingleRelKeyDefBuilder<T, TRelatedType>(singleRelationshipDefBuilder);
                            this.SingleRelKeyDefBuilder = */
        }

        private static string GetRandomRelName()
        {
            return RandomValueGenerator.GetRandomString();
        }

        /*
                public SingleRelationshipDefBuilderSpy(RelationshipsBuilder<T> RelationshipsBuilder, Expression<Func<T, TRelatedType>> relationshipExpression) : base(RelationshipsBuilder, relationshipExpression)
                {
                }*/
    }


}