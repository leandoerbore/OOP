using System;

namespace Banks.Exceptions
{
    public class ExceptionBankDoesNotExist : Exception
    {
        public ExceptionBankDoesNotExist(string message) : base(message) {}
    }

    public class ExceptionCreditLimitExceeded : Exception
    {
        public ExceptionCreditLimitExceeded(string message) : base(message) {}
    }
    
}