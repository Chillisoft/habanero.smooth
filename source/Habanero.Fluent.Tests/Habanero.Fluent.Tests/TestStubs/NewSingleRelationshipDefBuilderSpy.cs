using Habanero.BO;
using Habanero.Smooth.Test;

namespace Habanero.Fluent.Tests.TestStubs
{
    class NewSingleRelationshipDefBuilderSpy<T, TRelatedType> : NewSingleRelationshipDefBuilder<T, TRelatedType>
        where T : BusinessObject
        where TRelatedType : BusinessObject
    {
        public NewSingleRelationshipDefBuilderSpy()
            : base(new NewRelationshipsBuilderStub<T>(), GetRandomRelName())
        {
            /*                var relKeyDefBuilder = new NewSingleRelKeyDefBuilder<T, TRelatedType>(singleRelationshipDefBuilder);
                            this.SingleRelKeyDefBuilder = */
        }

        private static string GetRandomRelName()
        {
            return RandomValueGenerator.GetRandomString();
        }

        /*
                public NewSingleRelationshipDefBuilderSpy(NewRelationshipsBuilder<T> newRelationshipsBuilder, Expression<Func<T, TRelatedType>> relationshipExpression) : base(newRelationshipsBuilder, relationshipExpression)
                {
                }*/
    }


}