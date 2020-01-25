using System;
using System.Runtime.Serialization;

namespace App.Data.Exceptions
{
    public class UnitOfWorkCompletedException : Exception
    {
        public UnitOfWorkCompletedException()
        {
        }

        public UnitOfWorkCompletedException(string message) : base(message)
        {
        }

        public UnitOfWorkCompletedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnitOfWorkCompletedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}