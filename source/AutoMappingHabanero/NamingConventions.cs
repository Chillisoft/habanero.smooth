using System;
using AutoMappingHabanero.ReflectionWrappers;

namespace AutoMappingHabanero
{

    public interface INamingConventions
    {
        string GetIDPropertyName<T>();
        string GetIDPropertyName(TypeWrapper t);
        string GetSingleRelOwningPropName(string relationshipName);
    }

    public class DefaultPropNamingConventions : INamingConventions
    {
        public string GetIDPropertyName<T>()
        {
            var type = typeof (T).ToTypeWrapper();
            return GetIDPropertyName(type);
        }

        public string GetIDPropertyName(TypeWrapper t)
        {
            return t.Name + "ID";
        }
        public string GetSingleRelOwningPropName(string relationshipName)
        {
            return relationshipName + "ID";
        }
    }
}