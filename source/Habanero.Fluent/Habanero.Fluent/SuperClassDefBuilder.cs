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
using Habanero.Base;
using Habanero.BO;
using Habanero.Smooth;
using Habanero.Smooth.ReflectionWrappers;
using Habanero.Util;

namespace Habanero.Fluent
{
    public class SuperClassDefBuilder<T> where T : BusinessObject
    {
        private readonly ClassDefBuilder<T> _classDefBuilder;

        public SuperClassDefBuilder(ClassDefBuilder<T> classDefBuilder)
        {
            _classDefBuilder = classDefBuilder;
        }

        private string Discriminator { get; set; }

        public ISuperClassDef Build()
        {
            var typeWrapper = new TypeWrapper(typeof (T));
            // single table inheritance
            var superClassDef = typeWrapper.MapInheritance();
            if (Discriminator != null) superClassDef.Discriminator = Discriminator;
            return superClassDef;
        }

        public SuperClassDefBuilder<T> WithDiscriminator(string discriminator)
        {
            this.Discriminator = discriminator;
            return this;
        }

        public SuperClassDefBuilder<T> WithDiscriminator<TReturnType>(Expression<Func<T, TReturnType>> propExpression)
        {
            PropertyInfo propertyInfo = GetPropertyInfo(propExpression);
            this.Discriminator = propertyInfo.Name;
            return this;
        }

        private static PropertyInfo GetPropertyInfo<TModel, TReturn>(Expression<Func<TModel, TReturn>> expression)
        {
            return ReflectionUtilities.GetPropertyInfo(expression);
        }


        public PropertiesDefSelector<T> EndSuperClass()
        {
            return new PropertiesDefSelector<T>(_classDefBuilder, this );
        }
    }
}