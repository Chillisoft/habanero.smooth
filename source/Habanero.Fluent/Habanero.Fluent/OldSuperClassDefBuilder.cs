using System;
using System.Linq.Expressions;
using System.Reflection;
using Habanero.Base;
using Habanero.BO;
using Habanero.Smooth;
using Habanero.Smooth.ReflectionWrappers;
using Habanero.Util;

namespace Habanero.Fluent
{
    public class OldSuperClassDefBuilder<T> where T : BusinessObject
    {
        private readonly OldClassDefBuilder<T> _oldClassDefBuilder;

        public OldSuperClassDefBuilder(OldClassDefBuilder<T> oldClassDefBuilder)
        {
            _oldClassDefBuilder = oldClassDefBuilder;
        }

        public OldSuperClassDefBuilder()
        {
        }

        private string Discriminator { get; set; }

        public ISuperClassDef Build()
        {
            var typeWrapper = new TypeWrapper(typeof (T));
            var superClassDef = typeWrapper.MapInheritance();
            if (Discriminator != null) superClassDef.Discriminator = Discriminator;
            return superClassDef;
        }

        public OldSuperClassDefBuilder<T> WithDiscriminator(string discriminator)
        {
            this.Discriminator = discriminator;
            return this;
        }

        public OldSuperClassDefBuilder<T> WithDiscriminator<TReturnType>(Expression<Func<T, TReturnType>> propExpression)
        {
            PropertyInfo propertyInfo = GetPropertyInfo(propExpression);
            this.Discriminator = propertyInfo.Name;
            return this;
        }

        private static PropertyInfo GetPropertyInfo<TModel, TReturn>(Expression<Func<TModel, TReturn>> expression)
        {
            return ReflectionUtilities.GetPropertyInfo(expression);
        }


        public OldClassDefBuilder<T> Return()
        {
            return _oldClassDefBuilder;
        }
    }
}