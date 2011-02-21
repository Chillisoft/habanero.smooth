using System.Collections.Generic;
using Habanero.BO;

namespace Habanero.Fluent.Tests.TestStubs
{
    class NewClassDefBuilder2Stub<T> : NewClassDefBuilder2<T> where T : BusinessObject
    {
        public NewClassDefBuilder2Stub()
            : base(new NewClassDefBuilderStub<T>(), new List<string>())
        {
        }

    }
}