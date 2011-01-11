using System;
using System.Reflection;
using Habanero.Base;
using NUnit.Framework;

namespace AutoMappingHabanero.Test
{
    [TestFixture]
    public class TestReflectionUtils
    {
        //TODO brett 30 Jan 2010: Must not Props that have private Get's
        [Test]
        public void Test_GetProperty_WithExpression_ShouldReturnProperty()
        {
            //---------------Set up test pack-------------------
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var property = ReflectionUtils.GetProperty<FakeBOWProps>(x => x.PublicGetGuidProp);
            //---------------Test Result -----------------------
            var propertyInfo = typeof(FakeBOWProps).GetProperty("PublicGetGuidProp");
            property.ShouldEqual(propertyInfo);
            property.PropertyType.ShouldEqual(propertyInfo.PropertyType);
        }
    }
}