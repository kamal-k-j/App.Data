using System;

namespace App.Data.Conventions
{
    [AttributeUsage(AttributeTargets.Property)]
    public class VarBinaryLengthAttribute : Attribute
    {
        public VarBinaryLengthAttribute(int length)
        {
            Length = length;
        }
        public int Length { get; }
    }
}