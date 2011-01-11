using Habanero.BO;

namespace Habanero.Naked.Tests
{
    // ReSharper disable UnusedMember.Global
    //These are fake classes and their props are used via reflection.
    public class FakeBo : BusinessObject
    {

        public string FakeBoName { get; set; }

    }
    public class FakeBoW2Props : BusinessObject
    {
        public string FakeBoName { get; set; }
        public bool FakeBoName2 { get; set; }
    }
    // ReSharper restore UnusedMember.Global
}
