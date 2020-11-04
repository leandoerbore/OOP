using System;

namespace Lab3.Exceptions
{
    public class ExceptionDistance : Exception
    {
        public ExceptionDistance(string message) : base(message) {}
    }

    public class ExceptionTime : Exception
    {
        public ExceptionTime(string message) : base(message) {}
    }
    
    public class ExceptionSpeed : Exception
    {
        public ExceptionSpeed(string message) : base(message) {}
    }
    
}