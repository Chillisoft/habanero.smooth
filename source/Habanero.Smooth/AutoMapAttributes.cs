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
using Habanero.BO;

namespace Habanero.Smooth
{
    /// <summary>
    /// You can mark any property or class as ignore and in these cases Habanero.Smooth
    /// will totally ignore these.
    /// </summary>
    public class AutoMapIgnoreAttribute : Attribute { }
    /// <summary>
    /// Attribute to mark the mapped Database TableName on a class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoMapTableNameAttribute : Attribute
    {
        /// <summary>
        /// Constructs Attribute
        /// </summary>
        public AutoMapTableNameAttribute()
        {
        }

        /// <summary>
        /// Constructs Attribute with tableName
        /// </summary>
        /// <param name="tableName"></param>
        public AutoMapTableNameAttribute(string tableName)
        {
            TableName = tableName;
        }

        /// <summary>
        /// The database table name that htis class is persisted to
        /// </summary>
        public string TableName { get; set; }
    }

    /// <summary>
    /// Attribute to mark the mapped Database FieldName on a Property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoMapFieldNameAttribute: Attribute
    {
        /// <summary>
        /// The Name of the Field in the Database
        /// </summary>
        public string FieldName { get; private set; }
        /// <summary>
        /// Constructs the attribute with teh field name
        /// </summary>
        /// <param name="fieldName"></param>
        public AutoMapFieldNameAttribute(string fieldName)
        {
            FieldName = fieldName;
        }
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
        /// <summary>
        /// 
        /// </summary>
        protected AutoMapRelationshipAttribute()
        {
            this.RelationshipType = Base.RelationshipType.Association;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reverseRelationshipName"></param>
        protected AutoMapRelationshipAttribute(string reverseRelationshipName)
        {
            this.ReverseRelationshipName = reverseRelationshipName;
            this.RelationshipType = Base.RelationshipType.Association;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="relationshipType"></param>
        protected AutoMapRelationshipAttribute(RelationshipType relationshipType)
        {
            this.RelationshipType = relationshipType;
        }        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reverseRelationshipName"></param>
        /// <param name="relationshipType"></param>
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
        /// <summary>
        /// Construct the Attribute
        /// </summary>
        public AutoMapOneToOneAttribute()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reverseRelationshipName"></param>
        public AutoMapOneToOneAttribute(string reverseRelationshipName) : base(reverseRelationshipName)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relationshipType"></param>
        public AutoMapOneToOneAttribute(RelationshipType relationshipType) : base(relationshipType)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="reverseRelationshipName"></param>
        /// <param name="relationshipType"></param>
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
    public class AutoMapOneToManyAttribute : AutoMapRelationshipAttribute 
    {
                /// <summary>
        /// Defines the action that should be taken when the Parent of the relationship is deleted
        /// </summary>
        public DeleteParentAction DeleteParentAction { get; set; }
        /// <summary>
        /// Default constructor for OneToMany
        /// </summary>
        public AutoMapOneToManyAttribute()
        {
        }
        /// <summary>
        /// </summary>
        public AutoMapOneToManyAttribute(string reverseRelationshipName)
            : base(reverseRelationshipName)
        {
            this.DeleteParentAction = DeleteParentAction.Prevent;
        }
        /// <summary>
        /// </summary>
        public AutoMapOneToManyAttribute(RelationshipType relationshipType) : base(relationshipType)
        {
            this.DeleteParentAction = DeleteParentAction.Prevent;
        }
        /// <summary>
        /// </summary>
        public AutoMapOneToManyAttribute(string reverseRelationshipName, RelationshipType relationshipType, DeleteParentAction deleteParentAction) : base(reverseRelationshipName, relationshipType)
        {
            this.DeleteParentAction = deleteParentAction;
        }
/*
        protected AutoMapOneToManyAttribute(DeleteParentAction deleteParentAction)
        {
            this.DeleteParentAction = deleteParentAction;
        }*/
    }

    /// <summary>
    /// Attribute to automap a Many to one Relationship between two BusinessObjects
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoMapManyToOneAttribute : AutoMapRelationshipAttribute
    {
        /// <summary>
        /// </summary>
        public AutoMapManyToOneAttribute()
        {
        }
        /// <summary>
        /// </summary>
        public AutoMapManyToOneAttribute(string reverseRelationshipName) : base(reverseRelationshipName)
        {
        }
        /// <summary>
        /// </summary>
        public AutoMapManyToOneAttribute(RelationshipType relationshipType) : base(relationshipType)
        {
        }
        /// <summary>
        /// </summary>
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
        /// <summary>
        /// </summary>
        public AutoMapUniqueConstraintAttribute(string uniqueConstraintName)
        {
            UniqueConstraintName = uniqueConstraintName;
        }
        /// <summary>
        /// </summary>
        public string UniqueConstraintName { get; set; }
    }
    /// <summary>
    /// the Display name Attribute is used to add a more meaningful description/display name to a Property or relationship.
    /// Note_ The description will be used in error messages and labels in forms generated by faces etc.
    /// For more info see the ClassDef.PropDef displayName
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoMapDisplayNameAttribute : Attribute
    {        
        /// <summary>
        /// Construct the attribute with a displayName
        /// </summary>
        /// <param name="displayName"></param>
        public AutoMapDisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
        /// <summary>
        /// </summary>
        public string DisplayName { get; set; }
    }

    /// <summary>
    /// Attribute to add a default value to a property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoMapDefaultAttribute : Attribute
    {
        /// <summary>
        /// Construct the attribute with a default value
        /// </summary>
        /// <param name="defaultValue"></param>
        public AutoMapDefaultAttribute(string defaultValue)
        {
            this.DefaultValue = defaultValue;
        }
        /// <summary>
        /// </summary>
        public string DefaultValue { get; private set; }
    }

    /// <summary>
    /// Attribute to mark the ReadWriteRules on a Property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoMapReadWriteRuleAttribute : Attribute
    {        
        /// <summary>
        /// </summary>
        public AutoMapReadWriteRuleAttribute(PropReadWriteRule readWriteRule)
        {
            ReadWriteRule = readWriteRule;
        }
        /// <summary>
        /// </summary>
        public PropReadWriteRule ReadWriteRule { get; private set; }
    }
    /// <summary>
    /// Marks a property as autoincrementing
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoMapAutoIncrementingAttribute : Attribute
    {
    }

     /// <summary>
    /// Marks a property with KeepValuePrivate = true. This is for password fields etc.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoMapKeepValuePrivate : Attribute
     {
         
     }

    /// <summary>
    /// Marks an Integer range property rule.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoMapIntPropRuleAttribute : Attribute
    {
        /// <summary>
        /// </summary>
        public AutoMapIntPropRuleAttribute()
            : this(int.MinValue, int.MaxValue)
        {
        }
        /// <summary>
        /// </summary>
        public AutoMapIntPropRuleAttribute(int min, int max)
        {
            Min = min;
            Max = max;
        }
        /// <summary>
        /// </summary>
        public int Max { get; private set; }
        /// <summary>
        /// </summary>
        public int Min { get; private set; }
    }
    /// <summary>
    /// Marks an Short range property rule.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoMapShortPropRuleAttribute : Attribute
    {
        /// <summary>
        /// </summary>
        public AutoMapShortPropRuleAttribute()
            : this(short.MinValue, short.MaxValue)
        {
        }
        /// <summary>
        /// </summary>
        public AutoMapShortPropRuleAttribute(short min, short max)
        {
            Min = min;
            Max = max;
        }
        /// <summary>
        /// </summary>
        public short Max { get; private set; }
        /// <summary>
        /// </summary>
        public short Min { get; private set; }
    }

    /// <summary>
    /// Marks a String length property rule
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoMapStringLengthPropRuleAttribute : Attribute
    {

        /// <summary>
        /// </summary>
        public int MinLength { get; private set; }
        /// <summary>
        /// </summary>
        public int MaxLength { get; private set; }

        /// <summary>
        /// </summary>
        public AutoMapStringLengthPropRuleAttribute()
            : this(0, 255)
        {
        }

        /// <summary>
        /// </summary>
        public AutoMapStringLengthPropRuleAttribute(int minLength, int maxLength)
        {
            MinLength = minLength;
            MaxLength = maxLength;
        }


    }
    /// <summary>
    /// Marks a String Pattern Match property rule. <see cref="PropRuleString"/>
    /// </summary>
    public class AutoMapStringPatternMatchPropRuleAttribute : Attribute
    {
        ///<summary>
        /// The Regex pattern that will be used for validating the Property
        ///</summary>
        public string Pattern { get; private set; }
        /// <summary>
        /// The Error message that will be returned by the BO in the case where
        /// the Pattern Match does not work.
        /// </summary>
        public string Message { get; private set; }

        ///<summary>
        /// Construct the Attribute with a specific RegEx pattern
        ///</summary>
        ///<param name="pattern"></param>
        public AutoMapStringPatternMatchPropRuleAttribute(string pattern)
        {
            Pattern = pattern;
        }
        //patternMatchMessage
        ///<summary>
        /// Construct the Attribute with a specific RegEx pattern and a <see cref="Message"/> 
        ///</summary>
        ///<param name="pattern"><see cref="Pattern"/></param>
        ///<param name="message"><see cref="Message"/></param>
        public AutoMapStringPatternMatchPropRuleAttribute(string pattern, string message):this(pattern)
        {
            this.Message = message;
        }
    }

    /// <summary>
    /// Marks a Date Range property rule
    /// </summary>
    public class AutoMapDateTimePropRuleAttribute : Attribute
    {
        /// <summary>
        /// </summary>
        public DateTime StartDate { get; private set; }
        /// <summary>
        /// </summary>
        public DateTime EndDate { get; private set; }
        /// <summary>
        /// </summary>
        public string StartDateString { get; private set; }
        /// <summary>
        /// </summary>
        public string EndDateString { get; private set; }

        /// <summary>
        /// </summary>
        public AutoMapDateTimePropRuleAttribute(DateTime startDateValue, DateTime endDateValue)
        {
            StartDate = startDateValue;
            EndDate = endDateValue;
        }

        /// <summary>
        /// </summary>
        public AutoMapDateTimePropRuleAttribute(string startDateValue, string endDateValue)
        {
            StartDateString = startDateValue;
            EndDateString = endDateValue;
        }
    }

}