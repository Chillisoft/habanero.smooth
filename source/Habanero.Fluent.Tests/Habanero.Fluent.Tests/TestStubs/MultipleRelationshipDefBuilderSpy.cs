
using System.Collections.Generic;
using Habanero.BO;
using Habanero.Smooth.Test;

namespace Habanero.Fluent.Tests.TestStubs
{
    public class MultipleRelationshipDefBuilderSpy<T, TRelatedType> : MultipleRelationshipDefBuilder<T, TRelatedType>
        where T : BusinessObject
        where TRelatedType : BusinessObject
    {
        public MultipleRelationshipDefBuilderSpy(string relationshipName)
            : base(new RelationshipsBuilderStub<T>(), relationshipName)
        {
        }

        private static string GetRandomRelName()
        {
            return RandomValueGenerator.GetRandomString();
        }

    }
}
