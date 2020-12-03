using System;
using System.Collections.Generic;

namespace Banks
{
    public abstract class Bank
    {
        public abstract string Name { get; }
        public abstract double LimitForTransactions { get; }
        public abstract double Fee { get; protected set; }
        public abstract double CreditFee { get; protected set; }
        public abstract double CreditLimit { get; protected set; }
        public abstract int BIC { get; }
        private List<Client> _clients = new List<Client>();
        private List<Account> _accounts = new List<Account>();
        private List<(Client client, Account account)> _clientsAndAccounts = new List<(Client client, Account account)>();
        private List<(int firstInterval, int secondInterval, double interest)> _conditions = new List<(int firstInterval, int secondInterval, double interest)>();
        public double Interest { get; private set; } = 3.5;

        public List<(int firstInterval, int secondInterval, double interest)> conditions
        {
            get => _conditions;
            protected set => _conditions = value;
        }
        public List<(Client client, Account account)> clientsAndAccounts
        {
            get => _clientsAndAccounts;
            protected set => _clientsAndAccounts = value;
        }

        public List<Client> clients
        {
            get => _clients;
            protected set => _clients = value;
        }
        public List<Account> accounts
        {
            get => _accounts;
            private set => _accounts = value;
        }

        public void ConnectClientAndAccount(Client client, Account account)
        {
            _clientsAndAccounts.Add((client, account));
        }
        
        public int CreateRandomAccountNumbers()
        {
            while (true)
            {
                Random rnd = new Random();
                int numbers = rnd.Next(10000, 99999);

                var account = _accounts.Find(account => account.NumbersAccount.ToString().LastIndexOf(numbers.ToString()) != -1);

                if (account is null)
                    return numbers;
            }
        }

        public void TopUpAccount(Account account, double money)
        {
            if (account.BIC == BIC)
            {
                account.TopUpBalance(money);
            }
            else
            {
                var moneyWithFee = money - (money * Fee / 100);
                account.TopUpBalance(moneyWithFee);
            }
        }

        public void WithDrawFromAccount(Account account, double money)
        {
            if (account.BIC == BIC)
            {
                account.WithDraw(money);
            }
            else
            {
                var moneyWithFee = money + (money * Fee / 100);
                account.WithDraw(moneyWithFee);
            }
        }

        public void Transfer(Account accountFrom, Account accountTo, double money)
        {
            if (accountFrom.BIC == BIC)
            {
                if (accountFrom.BIC == accountTo.BIC)
                {
                    accountFrom.WithDraw(money);
                    accountTo.TopUpBalance(money);
                }
                else
                {
                    var moneyWithFee = money + (money * accountFrom.BankFee / 100);
                    
                    accountFrom.WithDraw(moneyWithFee);
                    accountTo.TopUpBalance(money);
                }
            }
            else
            {
                var moneyWithFee = money + (money * Fee / 100);
                if (accountFrom.BIC == accountTo.BIC)
                {
                    accountFrom.WithDraw(moneyWithFee);
                    accountTo.TopUpBalance(money);
                }
                else
                {
                    moneyWithFee += money * accountFrom.BankFee / 100;
                    
                    accountFrom.WithDraw(moneyWithFee);
                    accountTo.TopUpBalance(money);
                }
            }
        }

    }
}