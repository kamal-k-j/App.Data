using System;

namespace App.Data.Conventions
{
    [AttributeUsage(AttributeTargets.Property)]
    public class StringLengthAttribute : Attribute
    {
        public StringLengthAttribute(int length)
        {
            Length = length;
        }
        public int Length { get; }
    }
}