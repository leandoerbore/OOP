using System;
using System.Collections.Generic;

namespace Banks
{
    public abstract class Bank
    {
        private static List<(Account accountFrom, Account accountTo, double WithDrawMoney, double TopUpMoney)> _logs 
            = new List<(Account accountFrom, Account accountTo, double WithDrawMoney, double TopUpMoney)>();
        
        public abstract string Name { get; protected set; }
        public abstract double LimitForTransactions { get; protected set; }
        public abstract double Fee { get; protected set; }
        public abstract double CreditFee { get; protected set; }
        public abstract double CreditLimit { get; protected set; }
        public abstract int BIC { get; protected set; }
        private List<Client> _clients = new List<Client>();
        private List<Account> _accounts = new List<Account>();
        private List<(Client client, Account account)> _clientsAndAccounts = new List<(Client client, Account account)>();
        private List<(int firstInterval, int secondInterval, double interest)> _conditions = new List<(int firstInterval, int secondInterval, double interest)>();
        public double Interest { get; private set; } = 3.5;

        public static List<(Account accountFrom, Account accountTo, double WithDrawMoney, double TopUpMoney)> Logs
        {
            get => _logs;
            set => _logs = value;
        }
        
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
                    
                    Logging(accountFrom, accountTo, money, money);
                }
                else
                {
                    var moneyWithFee = money + (money * accountFrom.BankFee / 100);
                    
                    accountFrom.WithDraw(moneyWithFee);
                    accountTo.TopUpBalance(money);
                    
                    Logging(accountFrom, accountTo, moneyWithFee, money);
                }
            }
            else
            {
                var moneyWithFee = money + (money * Fee / 100);
                if (accountFrom.BIC == accountTo.BIC)
                {
                    accountFrom.WithDraw(moneyWithFee);
                    accountTo.TopUpBalance(money);
                    
                    Logging(accountFrom, accountTo, moneyWithFee, money);
                }
                else
                {
                    moneyWithFee += money * accountFrom.BankFee / 100;
                    
                    accountFrom.WithDraw(moneyWithFee);
                    accountTo.TopUpBalance(money);
                    
                    Logging(accountFrom, accountTo, moneyWithFee, money);
                }
            }
        }

        private void Logging(Account accountFrom, Account accountTo, double WithDrawMoney, double TopUpMoney)
        {
            _logs.Add((accountFrom, accountTo, WithDrawMoney, TopUpMoney));
        }
        public void CancleTransaction(long numbers, int transactionNumber)
        {

            var logsAccount = _logs.FindAll(log => log.accountFrom.NumbersAccount == numbers);
            if(logsAccount is null)
                throw new ExceptionAccountDoesNotExist("Счет не найден");

            if(logsAccount.Count < transactionNumber)
                throw new ExceptionWrongTransactionNumber("Номер транзакции не найден");
            
            var canceledTransaction = logsAccount[transactionNumber];

            canceledTransaction.accountFrom.CancelWithDraw(canceledTransaction.WithDrawMoney);
            canceledTransaction.accountTo.CancelTopUp(canceledTransaction.TopUpMoney);
        }

        public void ShowTransactionOnAccount(long numbers)
        {
            var logsAccount = _logs.FindAll(log => log.accountFrom.NumbersAccount == numbers);
            if(logsAccount is null)
                throw new ExceptionAccountDoesNotExist("Счет не найден");
            
            int i = 0;
            foreach (var log in logsAccount)
            {
                ++i;
                Console.WriteLine("{0}) numbersFrom: {1} , numbersTo: {2} , withdraw: {3:f2} , topup: {4:f2}", 
                    i, log.accountFrom.NumbersAccount, log.accountTo.NumbersAccount, log.WithDrawMoney, log.TopUpMoney);
            }
        }
        
    }
}