using System;

namespace Banks
{
    public class ExceptionNotEnoughMoney : Exception
    {
        public ExceptionNotEnoughMoney(string message) : base(message)
        {}
    }
    
    
}