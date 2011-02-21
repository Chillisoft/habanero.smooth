using Habanero.BO;

namespace Habanero.Fluent.Tests.TestStubs
{
    class NewClassDefBuilderStub<T> : NewClassDefBuilder<T> where T : BusinessObject
    {
    }
}