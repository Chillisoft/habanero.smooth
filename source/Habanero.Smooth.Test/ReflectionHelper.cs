// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2010 Chillisoft Solutions
//  
//  This file is part of the Habanero framework.
//  
//      Habanero is a free framework: you can redistribute it and/or modify
//      it under the terms of the GNU Lesser General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      The Habanero framework is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU Lesser General Public License for more details.
//  
//      You should have received a copy of the GNU Lesser General Public License
//      along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
// ---------------------------------------------------------------------------------
using System;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;

namespace Habanero.Smooth.Test
{
    /// <summary>
    /// This class is a Utility Class used for obtaining Information about a Class using 
    /// Lambda Expressions.
    /// This class is copied wholesale from FluentNHibernate (31/01/2010) thanks to
    /// the FluentNHibernate team.
    /// </summary>
    public static class ReflectionUtils
    {
        public static bool IsMethodExpression<TModel>(Expression<Func<TModel, object>> expression)
        {
            return IsMethodExpression<TModel, object>(expression);
        }

        public static bool IsMethodExpression<TModel, TResult>(Expression<Func<TModel, TResult>> expression)
        {
            return expression.Body is MethodCallExpression;
        }

        public static bool IsPropertyExpression<TModel>(Expression<Func<TModel, object>> expression)
        {
            return GetMemberExpression(expression, false) != null;
        }

        public static PropertyInfo GetProperty<TModel>(Expression<Func<TModel, object>> expression)
        {
            var isExpressionOfDynamicComponent = expression.ToString().Contains("get_Item");

            if (isExpressionOfDynamicComponent)
                return GetDynamicComponentProperty(expression);

            var memberExpression = GetMemberExpression(expression);

            return (PropertyInfo)memberExpression.Member;
        }

        private static PropertyInfo GetDynamicComponentProperty<TModel, T>(Expression<Func<TModel, T>> expression)
        {
            Type desiredConversionType = null;
            MethodCallExpression methodCallExpression = null;
            var nextOperand = expression.Body;

            while (nextOperand != null)
            {
                if (nextOperand.NodeType == ExpressionType.Call)
                {
                    methodCallExpression = nextOperand as MethodCallExpression;
                    desiredConversionType = desiredConversionType ?? methodCallExpression.Method.ReturnType;
                    break;
                }

                if (nextOperand.NodeType != ExpressionType.Convert)
                    throw new ArgumentException("Expression not supported", "expression");

                var unaryExpression = (UnaryExpression)nextOperand;
                desiredConversionType = unaryExpression.Type;
                nextOperand = unaryExpression.Operand;
            }

            var constExpression = methodCallExpression.Arguments[0] as ConstantExpression;

            return new DummyPropertyInfo((string)constExpression.Value, desiredConversionType);
        }

        public static PropertyInfo GetProperty<TModel, T>(Expression<Func<TModel, T>> expression)
        {
            var isExpressionOfDynamicComponent = expression.ToString().Contains("get_Item");

            if (isExpressionOfDynamicComponent)
                return GetDynamicComponentProperty(expression);

            MemberExpression memberExpression = GetMemberExpression(expression);

            return (PropertyInfo)memberExpression.Member;
        }

        private static MemberExpression GetMemberExpression<TModel, T>(Expression<Func<TModel, T>> expression)
        {
            return GetMemberExpression(expression, true);
        }

        private static MemberExpression GetMemberExpression<TModel, T>(Expression<Func<TModel, T>> expression, bool enforceCheck)
        {
            MemberExpression memberExpression = null;
            if (expression.Body.NodeType == ExpressionType.Convert)
            {
                var body = (UnaryExpression)expression.Body;
                memberExpression = body.Operand as MemberExpression;
            }
            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = expression.Body as MemberExpression;
            }

            if (enforceCheck && memberExpression == null)
            {
                throw new ArgumentException("Not a member access", "expression");
            }

            return memberExpression;
        }
//
//        public static Accessor GetAccessor<MODEL>(Expression<Func<MODEL, object>> expression)
//        {
//            MemberExpression memberExpression = GetMemberExpression(expression);
//
//            return getAccessor(memberExpression);
//        }
//
//        private static Accessor getAccessor(MemberExpression memberExpression)
//        {
//            var list = new List<PropertyInfo>();
//
//            while (memberExpression != null)
//            {
//                list.Add((PropertyInfo)memberExpression.Member);
//                memberExpression = memberExpression.Expression as MemberExpression;
//            }
//
//            if (list.Count == 1)
//            {
//                return new SingleProperty(list[0]);
//            }
//
//            list.Reverse();
//            return new PropertyChain(list.ToArray());
//        }
//
//        public static Accessor GetAccessor<MODEL, T>(Expression<Func<MODEL, T>> expression)
//        {
//            MemberExpression memberExpression = GetMemberExpression(expression);
//
//            return getAccessor(memberExpression);
//        }


        public static MethodInfo GetMethod<T>(Expression<Func<T, object>> expression)
        {
            MethodCallExpression methodCall = (MethodCallExpression)expression.Body;
            return methodCall.Method;
        }

        public static MethodInfo GetMethod<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            MethodCallExpression methodCall = (MethodCallExpression)expression.Body;
            return methodCall.Method;
        }

        public static MethodInfo GetMethod<T, U, V>(Expression<Func<T, U, V>> expression)
        {
            MethodCallExpression methodCall = (MethodCallExpression)expression.Body;
            return methodCall.Method;
        }

        public static MethodInfo GetMethod(Expression<Func<object>> expression)
        {
            MethodCallExpression methodCall = (MethodCallExpression)expression.Body;
            return methodCall.Method;
        }
    }
}