using System.Collections.Generic;
using Habanero.BO;

namespace Habanero.Fluent.Tests.TestStubs
{
    class ClassDefBuilder2Stub<T> : ClassDefBuilder2<T> where T : BusinessObject
    {
        public ClassDefBuilder2Stub()
            : base(new ClassDefBuilderStub<T>(), new List<string>())
        {
        }

    }
}