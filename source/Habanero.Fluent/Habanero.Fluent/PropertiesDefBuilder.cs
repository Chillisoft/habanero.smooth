#region Licensing Header
// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2011 Chillisoft Solutions
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
#endregion
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Habanero.BO;
using Habanero.Util;

namespace Habanero.Fluent
{
    public class PropertiesDefBuilder<T> where T : BusinessObject
    {
        protected ClassDefBuilder2<T> _classDefBuilder;
        protected IList<PropDefBuilder<T>> PropDefBuilders { get; set; }

        private PropertiesDefBuilder2<T> _propertiesDefBuilder2;

        public PropertiesDefBuilder(ClassDefBuilder2<T> classDefBuilder, IList<PropDefBuilder<T>> propDefBuilders)
        {
            _classDefBuilder = classDefBuilder;
            PropDefBuilders = propDefBuilders;
        }

        public PropDefBuilder<T> Property(string propertyName)
        {
            var propDefBuilder = GetPropDefBuilder(propertyName);
            PropDefBuilders.Add(propDefBuilder);
            return propDefBuilder;
        }
        public PropDefBuilder<T> Property<TReturnType>(string propertyName)
        {
            var propDefBuilder = GetPropDefBuilder(propertyName);
            propDefBuilder.WithType<TReturnType>();
            PropDefBuilders.Add(propDefBuilder);
            return propDefBuilder;
        }

        public PropDefBuilder<T> Property<TReturnType>(Expression<Func<T, TReturnType>> propExpression)
        {
            var propDefBuilder = new PropDefBuilder<T>(CreatePropertiesDefBuilder2());
            PropertyInfo propertyInfo = GetPropertyInfo(propExpression);
            propDefBuilder.WithPropertyName(propertyInfo.Name);
            Type propertyType = ReflectionUtilities.GetUndelyingPropertType(propertyInfo);
            propDefBuilder.WithAssemblyName(propertyType.Namespace);
            propDefBuilder.WithTypeName(propertyType.Name);
            PropDefBuilders.Add(propDefBuilder);
            return propDefBuilder;
        }

        private PropDefBuilder<T> GetPropDefBuilder(string propertyName)
        {
            var propDefBuilder = new PropDefBuilder<T>(CreatePropertiesDefBuilder2());
            propDefBuilder.WithPropertyName(propertyName);
            return propDefBuilder;
        }

        private static string GetPropertyName<TReturn>(Expression<Func<T, TReturn>> propExpression)
        {
            return GetPropertyInfo(propExpression).Name;
        }

        private static PropertyInfo GetPropertyInfo<TModel, TReturn>(Expression<Func<TModel, TReturn>> expression)
        {
            return ReflectionUtilities.GetPropertyInfo(expression);
        }

        private PropertiesDefBuilder2<T> CreatePropertiesDefBuilder2()
        {
            return new PropertiesDefBuilder2<T>(_classDefBuilder, PropDefBuilders);
        }

    }

    public class PropertiesDefBuilder2<T> : PropertiesDefBuilder<T> where T : BusinessObject
    {
        public PropertiesDefBuilder2(ClassDefBuilder2<T> propertiesDefBuilder, IList<PropDefBuilder<T>> propDefBuilders)
            : base(propertiesDefBuilder, propDefBuilders)
        {

        }

        public ClassDefBuilder2<T> EndProperties()
        {
            return _classDefBuilder;
        }

    }
}
