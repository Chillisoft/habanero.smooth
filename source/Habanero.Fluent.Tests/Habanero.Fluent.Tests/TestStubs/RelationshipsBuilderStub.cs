using System.Collections.Generic;
using Habanero.BO;

namespace Habanero.Fluent.Tests.TestStubs
{
    class RelationshipsBuilderStub<T> : RelationshipsBuilder<T> where T : BusinessObject
    {
        public RelationshipsBuilderStub()
            : base(new ClassDefBuilder2Stub<T>(), new List<ISingleRelDefBuilder>(), new List<IMultipleRelDefBuilder>())
        {
        }
    }
}