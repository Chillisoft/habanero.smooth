using System.Collections.Generic;
using Habanero.BO;

namespace Habanero.Fluent.Tests.TestStubs
{
    class NewRelationshipsBuilderStub<T> : NewRelationshipsBuilder<T> where T : BusinessObject
    {
        public NewRelationshipsBuilderStub()
            : base(new ClassDefBuilder2Stub<T>(), new List<ISingleRelDefBuilder>(), new List<IMultipleRelDefBuilder>())
        {
        }
    }
}