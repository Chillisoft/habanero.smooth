using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMappingHabanero.ReflectionWrappers;
using Habanero.Base;
using Habanero.BO.ClassDefinition;
using Habanero.Util;
using AutoMappingHabanero;

namespace AutoMappingHabanero
{


    public static class AutoMapperExtensionMethods
    {
        public static bool IsOfType<T>(this Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }
    }
}