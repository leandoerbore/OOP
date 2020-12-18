using System;

namespace Banks
{
    public class ExceptionNotEnoughMoney : Exception
    {
        public ExceptionNotEnoughMoney(string message) : base(message) {}
    }
    
    public class ExceptionBankDoesNotExist : Exception
    {
        public ExceptionBankDoesNotExist(string message) : base(message) {}
    }

    public class ExceptionCreditLimitExceeded : Exception
    {
        public ExceptionCreditLimitExceeded(string message) : base(message) {}
    }

    public class ExceptionDepositTime : Exception
    {
        public ExceptionDepositTime(string message) : base(message) {}
    }

    public class ExceptionClientDoesNotExist : Exception
    {
        public ExceptionClientDoesNotExist(string message) : base(message) {}
    }

    public class ExceptionAccountDoesNotExist : Exception
    {
        public ExceptionAccountDoesNotExist(string message) : base(message) {}
    }

    public class ExceptionWrongTransactionNumber : Exception
    {
        public ExceptionWrongTransactionNumber(string message) : base(message) {}
    }

    public class ExceptionLimit : Exception
    {
        public ExceptionLimit(string message) : base(message){}
    }
    
    
}