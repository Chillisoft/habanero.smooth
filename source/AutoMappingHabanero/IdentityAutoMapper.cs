using System;
using System.Reflection;
using AutoMappingHabanero.ReflectionWrappers;
using Habanero.Base;
using Habanero.BO.ClassDefinition;
using Habanero.Util;

namespace AutoMappingHabanero
{
    public static class IdentiyAutoMapperExtensions
    {
        public static IPrimaryKeyDef MapIndentity(this IClassDef classDef)
        {
            if (classDef == null) return null;
            IdentiyAutoMapper autoMapper = new IdentiyAutoMapper(classDef);

            return autoMapper.MapIndentity();
        }
    }
    public class IdentiyAutoMapper
    {
        private TypeWrapper _classType;
        private IClassDef ClassDef { get; set; }

        public IdentiyAutoMapper(IClassDef classDef)
        {
            ClassDef = classDef;
            if (classDef == null) throw new ArgumentNullException("classDef");
            _classType = this.ClassDef.ClassType.ToTypeWrapper();
        }

        public IPrimaryKeyDef MapIndentity()
        {
            IClassDef classDef = this.ClassDef;

            var primaryKeyDef = GetPrimaryKeyDef(classDef);
            if (primaryKeyDef == null)
            {
                IPropDef propDef = GetPrimaryKeyPropDef();
                if (propDef == null) return null;
                classDef.PrimaryKeyDef = new PrimaryKeyDef();
                classDef.PrimaryKeyDef.Add(propDef);
            }
            
            return classDef.PrimaryKeyDef;
        }

        private static IPrimaryKeyDef GetPrimaryKeyDef(IClassDef classDef)
        {
            var primaryKeyDef = classDef.PrimaryKeyDef;
            if(primaryKeyDef == null && classDef.SuperClassDef !=null)
            {
                primaryKeyDef = GetPrimaryKeyDef(classDef.SuperClassDef.SuperClassClassDef);
            }
            return primaryKeyDef;
        }


        private IPropDef GetPrimaryKeyPropDef()
        {
            IPropDef propDef = FindExistingPKPropDef() 
                    ?? CreatePrimaryKeyProp();

            propDef.ReadWriteRule = PropReadWriteRule.WriteNew;
            propDef.Compulsory = true;
            return propDef;
        }

        private IPropDef FindExistingPKPropDef()
        {
            var pkPropName = _classType.GetPKPropName();
            return this.ClassDef.GetPropDef(pkPropName, false);
        }


        private IPropDef CreatePrimaryKeyProp()
        {
            var propertyName = PropNamingConvention.GetIDPropertyName(_classType);
            IPropDef propDef = new PropDef(propertyName, typeof (Guid), PropReadWriteRule.WriteNew, null);
            this.ClassDef.PropDefcol.Add(propDef);
            return propDef;
        }

        public static INamingConventions PropNamingConvention
        {
            get
            {
                return ClassAutoMapper.PropNamingConvention;
            }

        }

    }

}

