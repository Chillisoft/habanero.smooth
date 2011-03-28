
using System.Collections.Generic;
using Habanero.BO;
using Habanero.Smooth.Test;

namespace Habanero.Fluent.Tests.TestStubs
{
    public class NewMultipleRelationshipDefBuilderSpy<T, TRelatedType> : NewMultipleRelationshipDefBuilder<T, TRelatedType>
        where T : BusinessObject
        where TRelatedType : BusinessObject
    {
        public NewMultipleRelationshipDefBuilderSpy(string relationshipName)
            : base(new NewRelationshipsBuilderStub<T>(), relationshipName)
        {
        }

        private static string GetRandomRelName()
        {
            return RandomValueGenerator.GetRandomString();
        }

    }
}
