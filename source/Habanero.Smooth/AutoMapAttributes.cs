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
using Habanero.Base;

namespace Habanero.Smooth
{

    public class AutoMapIgnoreAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public class AutoMapPrimaryKeyAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class AutoMapRelationshipAttribute : Attribute
    {
        protected AutoMapRelationshipAttribute()
        {
        }

        protected AutoMapRelationshipAttribute(string reverseRelationshipName)
        {
            this.ReverseRelationshipName = reverseRelationshipName;
        }

        public string ReverseRelationshipName { get; set; }

    }
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoMapOneToOne : AutoMapRelationshipAttribute {
        public AutoMapOneToOne()
        {
        }

        public AutoMapOneToOne(string reverseRelationshipName) : base(reverseRelationshipName)
        {
        }
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoMapCompulsoryAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class AutoMapOneToMany : AutoMapRelationshipAttribute {
        public AutoMapOneToMany()
        {
        }

        public AutoMapOneToMany(string reverseRelationshipName)
            : base(reverseRelationshipName)
        {
        }
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoMapDefaultAttribute : Attribute
    {
        public AutoMapDefaultAttribute(string defaultValue)
        {
            this.DefaultValue = defaultValue;
        }

        public string DefaultValue { get; private set; }
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoMapManyToOneAttribute : AutoMapRelationshipAttribute
    {
        public AutoMapManyToOneAttribute()
        {
        }

        public AutoMapManyToOneAttribute(string reverseRelationshipName) : base(reverseRelationshipName)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class AutoMapReadWriteRuleAttribute : Attribute
    {
        public AutoMapReadWriteRuleAttribute(PropReadWriteRule readWriteRule)
        {
            ReadWriteRule = readWriteRule;
        }

        public PropReadWriteRule ReadWriteRule { get; private set; }
    }
}