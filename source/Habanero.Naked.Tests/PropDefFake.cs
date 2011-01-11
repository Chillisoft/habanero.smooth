using Habanero.Base;
using Habanero.BO.ClassDefinition;
using System;

namespace Habanero.Naked.Tests
{
    internal class PropDefFake : PropDef
    {
        public PropDefFake(Type propType)
            : base(GetRandomString(), propType, PropReadWriteRule.ReadWrite, null)
        {
        }
        public PropDefFake() : base(GetRandomString(), typeof(int), PropReadWriteRule.ReadWrite, null)
        {
        }
        private static string GetRandomString()
        {
            return ("A" + Guid.NewGuid().ToString().Replace("-", ""));
        }
    }
    

}

