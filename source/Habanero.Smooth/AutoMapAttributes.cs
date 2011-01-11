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
    /// <summary>
    /// You can mark any property or class as ignore and in these cases Habanero.Smooth
    /// will totally ignore these.
    /// </summary>
    public class AutoMapIgnoreAttribute : Attribute { }
    /// <summary>
    /// Attribute to mark the mapped TableName on a class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoMapTableNameAttribute : Attribute
    {
        public AutoMapTableNameAttribute()
        {

        }

        public AutoMapTableNameAttribute(string tableName)
        {
            TableName = tableName;
        }

        public string TableName { get; set; }
    }
    /// <summary>
    /// Attribute used for marking a specific property as the Primary Key.
    /// This is only required in cases where the convention is not followed.
    /// The standard convention is
    /// the PrimaryKeyProp is the ClassName + "ID" and the PrimaryKeyProp is a Guid
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoMapPrimaryKeyAttribute : Attribute { }
    /// <summary>
    /// Base class for the <see cref="AutoMapOneToManyAttribute"/>, <see cref="AutoMapManyToOneAttribute"/> and <see cref="AutoMapOneToOneAttribute"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class AutoMapRelationshipAttribute : Attribute
    {
        protected AutoMapRelationshipAttribute()
        {
            this.RelationshipType = Base.RelationshipType.Association;
        }

        protected AutoMapRelationshipAttribute(string reverseRelationshipName)
        {
            this.ReverseRelationshipName = reverseRelationshipName;
            this.RelationshipType = Base.RelationshipType.Association;
        }
        protected AutoMapRelationshipAttribute(RelationshipType relationshipType)
        {
            this.RelationshipType = relationshipType;
        }
        protected AutoMapRelationshipAttribute(string reverseRelationshipName, RelationshipType relationshipType)
        {
            this.ReverseRelationshipName = reverseRelationshipName;
            this.RelationshipType = relationshipType;
        }
        /// <summary>
        /// The specified Reverse RelationshipName
        /// </summary>
        public string ReverseRelationshipName { get; set; }

        /// <summary>
        /// The RelationshipType that is specified.
        /// </summary>
        public RelationshipType RelationshipType { get; set; }

    }
    /// <summary>
    /// Automap a relationship between two Business Objects as a One to One.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoMapOneToOneAttribute : AutoMapRelationshipAttribute {
        public AutoMapOneToOneAttribute()
        {
        }

        public AutoMapOneToOneAttribute(string reverseRelationshipName) : base(reverseRelationshipName)
        {
        }

        public AutoMapOneToOneAttribute(RelationshipType relationshipType) : base(relationshipType)
        {
        }

        public AutoMapOneToOneAttribute(string reverseRelationshipName, RelationshipType relationshipType) : base(reverseRelationshipName, relationshipType)
        {
        }
    }
    /// <summary>
    /// Automap a Property as Compulsory.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoMapCompulsoryAttribute : Attribute
    {
    }
    /// <summary>
    /// AutoMap a One to Many Relationship between two Business Objects.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoMapOneToManyAttribute : AutoMapRelationshipAttribute {
        public AutoMapOneToManyAttribute()
        {
        }

        public AutoMapOneToManyAttribute(string reverseRelationshipName)
            : base(reverseRelationshipName)
        {
        }

        public AutoMapOneToManyAttribute(RelationshipType relationshipType) : base(relationshipType)
        {
        }

        public AutoMapOneToManyAttribute(string reverseRelationshipName, RelationshipType relationshipType) : base(reverseRelationshipName, relationshipType)
        {
        }
    }

    /// <summary>
    /// Attribute to automap a Many to one Relationship between two BusinessObjects
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoMapManyToOneAttribute : AutoMapRelationshipAttribute
    {
        public AutoMapManyToOneAttribute()
        {
        }

        public AutoMapManyToOneAttribute(string reverseRelationshipName) : base(reverseRelationshipName)
        {
        }

        public AutoMapManyToOneAttribute(RelationshipType relationshipType) : base(relationshipType)
        {
        }

        public AutoMapManyToOneAttribute(string reverseRelationshipName, RelationshipType relationshipType) : base(reverseRelationshipName, relationshipType)
        {
        }
    }

    /// <summary>
    /// Property to add a unique constraint to a property.
    /// Note_ To handle composite unique constraints add this attribute to two
    /// properties where each property has the same UniqueConstraintName.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoMapUniqueConstraintAttribute : Attribute
    {
        public AutoMapUniqueConstraintAttribute(string uniqueConstraintName)
        {
            UniqueConstraintName = uniqueConstraintName;
        }

        public string UniqueConstraintName { get; set; }
    }

    /// <summary>
    /// Attribute to add a default value to a property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoMapDefaultAttribute : Attribute
    {
        public AutoMapDefaultAttribute(string defaultValue)
        {
            this.DefaultValue = defaultValue;
        }

        public string DefaultValue { get; private set; }
    }

    /// <summary>
    /// Attribute to mark the ReadWriteRules on a Property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoMapReadWriteRuleAttribute : Attribute
    {
        public AutoMapReadWriteRuleAttribute(PropReadWriteRule readWriteRule)
        {
            ReadWriteRule = readWriteRule;
        }

        public PropReadWriteRule ReadWriteRule { get; private set; }
    }
    /// <summary>
    /// Marks a property as autoincrementing
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoMapAutoIncrementingAttribute : Attribute
    {
    }

    
}