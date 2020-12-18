using System;

namespace Banks
{
    public abstract class Account 
    {
        public double BankLimitForTransactions { get; protected set; }
        public double LimitForTransactionsLeft { get; set; }
        public double BankFee { get; protected set; }
        public double Balance { get; protected set; } = 0;
        public bool IsDoubtful { get; set; }
        public long NumbersAccount { get; protected set; }
        
        public int BIC { get; protected set; }

        public Account(int BIC, bool isDoubtful, double Fee, long numbersAccount, double bankLimitForTransactions)
        {
            var temp = BIC.ToString() + numbersAccount.ToString();
            NumbersAccount = int.Parse(temp);

            IsDoubtful = isDoubtful;
            BankFee = Fee;
            this.BIC = BIC;
            BankLimitForTransactions = bankLimitForTransactions;
            LimitForTransactionsLeft = bankLimitForTransactions;
        }

        public void TopUpBalance(double money) => Balance += money;

        public abstract void WithDraw(double money);

        public void CancelTopUp(double money) => Balance -= money;
        public void CancelWithDraw(double money) => Balance += money;
        

        public virtual void Calc()
        {
            throw new NotImplementedException();
        }
    }
}