using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using Rhino.Mocks;

// ReSharper disable UnusedMember.Global

namespace AutoMappingHabanero.Test
{
    public static class RhinoExtensions
    {
        public static void ClearReturnValue<T>(this T fi)
        {
            // Switch back to record and then to replay - that
            // clears all behaviour and we can program new behavior.
            // Record/Replay do not occur otherwise in our tests, that another method of
            // using Rhino Mocks.
            fi.BackToRecord(BackToRecordOptions.All);
            fi.Replay();
        }
    }
    public class LamdaExpressionViews
    {
        public static string GetCodeString(Expression expression)
        {
            if (expression is LambdaExpression)
            {
                LambdaExpression lambdaExpression = (LambdaExpression)expression;
                return GetCodeString(lambdaExpression.Body);
            }
            if (expression is BinaryExpression)
            {
                BinaryExpression binaryExpression = (BinaryExpression)expression;
                string leftCodeString = GetCodeString(binaryExpression.Left);
                string rightCodeString = GetCodeString(binaryExpression.Right);
                string operatorCodeString = GetOperatorString(binaryExpression);
                return string.Format("{0} {1} {2}", leftCodeString, operatorCodeString, rightCodeString);
            }
            if (expression is MemberExpression)
            {
                MemberExpression memberExpression = (MemberExpression)expression;
                string expressionCodeString = GetCodeString(memberExpression.Expression);
                string memberCodeString = memberExpression.Member.Name;
                if (!String.IsNullOrEmpty(expressionCodeString)) expressionCodeString += ".";
                return string.Format("{0}{1}", expressionCodeString, memberCodeString);
            }
            if (expression is MethodCallExpression)
            {
                MethodCallExpression methodCallExpression = (MethodCallExpression)expression;

                string arguementsCodeString 
                    = string.Join(", ", 
                        methodCallExpression.Arguments
                        .Select(argument => GetCodeString(argument)).ToArray());
                string methodCodeString = methodCallExpression.Method.Name;
                return string.Format("{0}({1})", methodCodeString, arguementsCodeString);
            }
            return "";
        }

        private static string GetOperatorString(BinaryExpression binaryExpression)
        {
            switch (binaryExpression.NodeType)
            {
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "!=";
                case ExpressionType.And:
                    return "&";
                case ExpressionType.AndAlso:
                    return "&&";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.Or:
                    return "|";
                case ExpressionType.OrElse:
                    return "||";
                case ExpressionType.ExclusiveOr:
                    return "XOR";

                case ExpressionType.Negate:
                case ExpressionType.Not:

                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.ArrayIndex:
                case ExpressionType.Call:
                case ExpressionType.Coalesce:
                case ExpressionType.Conditional:
                case ExpressionType.Constant:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Divide:
                case ExpressionType.Invoke:
                case ExpressionType.Lambda:
                case ExpressionType.LeftShift:
                case ExpressionType.ListInit:
                case ExpressionType.MemberAccess:
                case ExpressionType.MemberInit:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.UnaryPlus:
                case ExpressionType.NegateChecked:
                case ExpressionType.New:
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                case ExpressionType.Parameter:
                case ExpressionType.Power:
                case ExpressionType.Quote:
                case ExpressionType.RightShift:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.TypeAs:
                case ExpressionType.TypeIs:
                default:
                    return binaryExpression.ToString();
            }
        }
    }
    public delegate void MethodThatThrows();
    public static class GeneralTestingExtensions
    {
        public static void ShouldBeFalse(this bool condition)
        {
            Assert.IsFalse(condition);
        }

        public static void ShouldBeFalse(this bool condition, string message)
        {
            Assert.IsFalse(condition, message);
        }

        public static void ShouldBeTrue(this bool condition)
        {
            Assert.IsTrue(condition);
        }

        public static object ShouldEqual(this object actual, object expected)
        {
            Assert.AreEqual(expected, actual);
            return expected;
        }
        public static object ShouldEqual(this object actual, object expected, string message)
        {
            Assert.AreEqual(expected, actual, message);
            return expected;
        }

        public static object ShouldNotEqual(this object actual, object expected)
        {
            Assert.AreNotEqual(expected, actual);
            return expected;
        }

        public static object ShouldNotEqual(this object actual, object expected, string message)
        {
            Assert.AreNotEqual(expected, actual, message);
            return expected;
        }

        public static void ShouldBeNull(this object anObject)
        {
            Assert.IsNull(anObject);
        }

        public static T ShouldNotBeNull<T>(this T anObject, string message)
        {
            Assert.IsNotNull(anObject, message);

            return anObject;
        }
        public static T ShouldNotBeNull<T>(this T anObject)
        {
            Assert.IsNotNull(anObject);

            return anObject;
        }

        public static object ShouldBeTheSameAs(this object actual, object expected)
        {
            Assert.AreSame(expected, actual);
            return expected;
        }

        public static object ShouldNotBeTheSameAs(this object actual, object expected)
        {
            Assert.AreNotSame(expected, actual);
            return expected;
        }

        public static void ShouldBeOfType(this object actual, Type expected)
        {
            Assert.IsInstanceOf(expected, actual);
        }

        public static void ShouldBeOfType<T>(this object actual)
        {
            actual.ShouldBeOfType(typeof(T));
        }

        public static void ShouldNotBeOfType(this object actual, Type expected)
        {
            Assert.IsInstanceOf(expected, actual);
        }

        public static void ShouldNotBeOfType<T>(this object actual)
        {
            actual.ShouldNotBeOfType(typeof(T));
        }

        public static void ShouldImplementType<T>(this object actual)
        {
            typeof(T).IsAssignableFrom(actual.GetType()).ShouldBeTrue();
        }

        public static void ShouldContain(this IList actual, object expected)
        {
            Assert.Contains(expected, actual);
        }

        public static void ShouldContain<T>(this IEnumerable<T> actual, T expected, string message)
        {
            ShouldContain(actual, x => x.Equals(expected), message);
        }
        public static void ShouldContain<T>(this IEnumerable<T> actual, IEnumerable<T> expected, string message)
        {
            foreach (var expectedItem in expected)
            {
                T item = expectedItem;
                ShouldContain(actual, x => x.Equals(item), message);
            }
        }
        public static void ShouldContain<T>(this IEnumerable<T> actual, T expected)
        {
            ShouldContain(actual, x => x.Equals(expected));
        }

        public static void ShouldContain<T>(this IEnumerable<T> actual, Func<T, bool> expected, string message)
        {
            actual.FirstOrDefault(expected).ShouldNotEqual(default(T), message);
        }
        public static void ShouldContain<T>(this IEnumerable<T> actual, Func<T, bool> expected)
        {
            actual.FirstOrDefault(expected).ShouldNotEqual(default(T), "Should contain item");
        }

        public static void ShouldContain(this IDictionary actual, string key, string value)
        {
            Assert.That(actual.Contains(key));
            actual[key].ShouldEqual(value);
        }

        public static IDictionary<KEY, VALUE> ShouldContain<KEY, VALUE>(this IDictionary<KEY, VALUE> actual, KEY key, VALUE value)
        {
            actual.Keys.Contains(key).ShouldBeTrue();
            actual[key].ShouldEqual(value);
            return actual;
        }

        public static void ShouldNotContain(this IList actual, object expected)
        {
            Assert.That(actual, Has.None.Member(expected));
        }

        public static void ShouldNotContain<T>(this IEnumerable<T> actual, T expected, string message)
        {
            actual.Contains(expected).ShouldBeFalse(message);
        }
        public static void ShouldNotContain<T>(this IEnumerable<T> actual, T expected)
        {
            actual.Contains(expected).ShouldBeFalse("Should not contain '" + expected + "'");
        }

        public static void ShouldNotContain<T>(this IEnumerable<T> actual, Func<T, bool> expected)
        {
            actual.ShouldNotContain(expected, "Should not contain item");
        }

        public static void ShouldNotContain<T>(this IEnumerable<T> actual, Func<T, bool> expected, string message)
        {
            foreach (var t in actual)
            {
                expected(t).ShouldBeFalse(message);
            }
        }

        public static void ShouldNotContain(this IDictionary actual, string key, string value)
        {
            Assert.That(actual.Contains(key), Is.False);
        }

        public static void ShouldNotContain<TKey, TValue>(this IDictionary<TKey, TValue> actual, TKey key, TValue value)
        {
            actual.Keys.Contains(key).ShouldBeFalse();
        }

        public static void ShouldContain<T>(this T[] actual, T expected)
        {
            ShouldContain((IList)actual, expected);
        }

        public static void ShouldBeEmpty<T>(this IEnumerable<T> actual)
        {
            const string message = "Should Be Empty";
            actual.ShouldNotBeNull(message);
            actual.Count().ShouldEqual(0, message);
        }
        public static void ShouldNotBeEmpty<T>(this IEnumerable<T> actual)
        {
            const string message = "Should Not be Empty";
            actual.ShouldNotBeNull(message);
            actual.Count().ShouldNotEqual(0, message);
        }

        public static void ItemsShouldBeEqual<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
        {
            const string message = "Items Should Be Equal";
            actual.Count().ShouldEqual(expected.Count(), message);

            var index = 0;

            foreach (var item in actual)
            {
                var expectedItem = expected.ElementAt(index);

                item.ShouldEqual(expectedItem, message);
                index++;
            }
        }

        public static IEnumerable<T> ShouldHaveCount<T>(this IEnumerable<T> actual, int expected)
        {
            return actual.ShouldHaveCount(expected, "Should Have Count");
        }
        public static IEnumerable<T> ShouldHaveCount<T>(this IEnumerable<T> actual, int expected, string message)
        {
            actual.Count().ShouldEqual(expected, message);
            return actual;
        }

        public static IComparable ShouldBeGreaterThan(this IComparable arg1, IComparable arg2)
        {
            Assert.Greater(arg1, arg2);
            return arg2;
        }

        public static IComparable ShouldBeLessThan(this IComparable arg1, IComparable arg2)
        {
            Assert.Less(arg1, arg2);
            return arg2;
        }

        public static void ShouldBeEmpty(this ICollection collection)
        {
            Assert.IsEmpty(collection);
        }

        public static void ShouldBeEmpty(this string aString)
        {
            Assert.IsEmpty(aString);
        }

        public static void ShouldNotBeEmpty(this ICollection collection)
        {
            Assert.IsNotEmpty(collection);
        }

        public static void ShouldNotBeEmpty(this string aString)
        {
            Assert.IsNotEmpty(aString);
        }

        public static void ShouldNotContain(this string actual, string expected)
        {
            StringAssert.DoesNotContain(expected, actual);
        }

        public static void ShouldContain(this string actual, string expected)
        {
            StringAssert.Contains(expected, actual);
        }

        public static string ShouldBeEqualIgnoringCase(this string actual, string expected)
        {
            StringAssert.AreEqualIgnoringCase(expected, actual);
            return expected;
        }

        public static void ShouldEndWith(this string actual, string expected)
        {
            StringAssert.EndsWith(expected, actual);
        }

        public static void ShouldStartWith(this string actual, string expected)
        {
            StringAssert.StartsWith(expected, actual);
        }

        public static void ShouldNotStartWith(this string actual, string expected)
        {
            StringAssert.DoesNotStartWith(expected, actual);
        }

        public static void ShouldContainErrorMessage(this Exception exception, string expected)
        {
            StringAssert.Contains(expected, exception.Message);
        }


        public static Exception ShouldBeThrownBy(this Type exceptionType, MethodThatThrows method)
        {
            Exception exception = null;

            try
            {
                method();
            }
            catch (Exception e)
            {
                Assert.AreEqual(exceptionType, e.GetType());
                exception = e;
            }

            if (exception == null)
            {
                Assert.Fail(String.Format("Expected {0} to be thrown.", exceptionType.FullName));
            }

            return exception;
        }
/*
            public static void ShouldEqualSqlDate(this DateTime actual, DateTime expected)
            {
                TimeSpan timeSpan = actual - expected;
                Assert.Less(Math.Abs(timeSpan.TotalMilliseconds), 3);
            }

            public static object AttributeShouldEqual(this XmlElement element, string attributeName, object expected)
            {
                Assert.IsNotNull(element, "The Element is null");
                string actual = element.GetAttribute(attributeName);
                Assert.AreEqual(expected, actual);
                return expected;
            }

            public static void ChildNodeCountShouldEqual(this XmlElement element, int expected)
            {
                Assert.AreEqual(expected, element.ChildNodes.Count);
            }

            public static XmlElement ShouldHaveChild(this XmlElement element, string xpath)
            {
                XmlElement child = element.SelectSingleNode(xpath) as XmlElement;
                Assert.IsNotNull(child, "Should have a child element matching " + xpath);

                return child;
            }

            public static XmlElement DoesNotHaveAttribute(this XmlElement element, string attributeName)
            {
                Assert.IsNotNull(element, "The Element is null");
                Assert.IsFalse(element.HasAttribute(attributeName), "Element should not have an attribute named " + attributeName);

                return element;
            }*/
    }
    // ReSharper restore UnusedMember.Global

}