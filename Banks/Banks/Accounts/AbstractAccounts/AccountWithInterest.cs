using System;
using System.Threading;

namespace Banks
{
    public abstract class AccountWithInterest : Account
    {
        protected int itterationForInterest { get; set; } = 0;
        protected double _interestBalance = 0;
        protected DateTime DateForInterest;
        protected DateTime lastDateCalcInterest;
        public double InterestProcent { get; protected set; } = 0;

        protected abstract void InterestCalc();

        protected abstract void CalcForDay();

        protected AccountWithInterest(int BIC, bool isDoubtful, double Fee, long numbersAccount, double bankLimitForTransactions) 
            : base(BIC, isDoubtful, Fee, numbersAccount, bankLimitForTransactions)
        {
            
        }
    }
}