using System;

namespace AutoMappingHabanero
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
    public class AutoMapManyToOneAttribute : AutoMapRelationshipAttribute
    {
        public AutoMapManyToOneAttribute()
        {
        }

        public AutoMapManyToOneAttribute(string reverseRelationshipName) : base(reverseRelationshipName)
        {
        }
    }
}