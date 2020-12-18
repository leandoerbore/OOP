using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Banks
{
    public static class Manager
    {
        private static List<Bank> banks = new List<Bank>(){new TinkoffBank(), new SberBank(), new GazpromBank()};

        public static int CreateClientInBank(string nameBank, string name, string surname, string adress = null, long passport = default)
        {
            var bank = FindBank(nameBank);
            
            bank.clients.Add(new Client(name, surname, adress, passport));
            var client = bank.clients.Last();
            
            return bank.clients.Count - 1;
        }

        public static long CreateDepositAccount(string nameBank, int idClient)
        {
            var bank = FindBank(nameBank);
            Client client = FindClient(bank, idClient);
            var BIC = bank.BIC;
            var condition = bank.conditions;
            var fee = bank.Fee;
            var numbersAccount = bank.CreateRandomAccountNumbers();
            var isDoubrful = (client.passport == default || client.adress == null) ? true : false;
            var bankLimitForTransactions = bank.LimitForTransactions;
            
            bank.accounts.Add(new DepositAccount(BIC, isDoubrful, condition, fee, numbersAccount, 30, bankLimitForTransactions));

            var account = bank.accounts.Last();
                
            bank.ConnectClientAndAccount(client,account);

            return account.NumbersAccount;
        }
        
        public static long CreateDebitAccount(string nameBank, int idClient)
        {
            var bank = FindBank(nameBank);
            Client client = FindClient(bank, idClient);
            int BIC = bank.BIC;
            var interest = bank.Interest;
            var fee = bank.Fee;
            var numbersAccount = bank.CreateRandomAccountNumbers();
            var isDoubrful = (client.passport == default || client.adress == null) ? true : false;
            var bankLimitForTransactions = bank.LimitForTransactions;
            
            bank.accounts.Add(new DebitAccount(BIC, isDoubrful, interest, fee, numbersAccount, bankLimitForTransactions));

            var account = bank.accounts.Last();
                
            bank.ConnectClientAndAccount(client,account);

            return account.NumbersAccount;
        }
        
        public static long CreateCreditAccount(string nameBank, int idClient)
        {
            var bank = FindBank(nameBank);
            Client client = FindClient(bank, idClient);
            int BIC = bank.BIC;
            var fee = bank.Fee;
            var numbersAccount = bank.CreateRandomAccountNumbers();
            var isDoubrful = (client.passport == default || client.adress == null) ? true : false;
            var bankLimitForTransactions = bank.LimitForTransactions;
            var bankCreditLimit = bank.CreditLimit;
            var bankCreditFee = bank.CreditFee;
            
            bank.accounts.Add(new CreditAccount(BIC, isDoubrful, fee, numbersAccount, bankLimitForTransactions, bankCreditLimit, bankCreditFee ));
            
            var account = bank.accounts.Last();
                
            bank.ConnectClientAndAccount(client,account);

            return account.NumbersAccount;
        }
        
        

        public static void AddInformationAboutClient(string nameBank, int idClient, string adress=null, long passport=default)
        {
            var bank = FindBank(nameBank);
            Client client = FindClient(bank, idClient);
            
            client.AddAdress(adress);
            client.AddPassport(passport);
            var clientIsDoubtful = (client.passport == default || client.adress == null) ? true : false;
            var accounts = bank.clientsAndAccounts
                .FindAll(connection => connection.client == client)
                .Select(connection => connection.account);

            if (!clientIsDoubtful)
            {
                foreach (var account in accounts)
                {
                    account.IsDoubtful = false;
                }
            }
        }
        
        private static bool CheckForDoubtful(Bank bank, int idClient) 
            => ( bank.clients[idClient].passport == default ||  bank.clients[idClient].adress == null) ? true : false;

        public static List<Client> GetAllClientsOfBank(string nameBank) => FindBank(nameBank).clients;

        private static Bank FindBank(string nameBank)
        {
            var bank = banks.Find(bank => bank.Name == nameBank);
            if (bank is null)
                throw new ExceptionBankDoesNotExist("Такой банк не существует");
            else
            {
                return bank;
            }
        }

        public static List<(long numbers, double balance)> GetBalanceAllAccounts(string nameBank)
        {
            var bank = FindBank(nameBank);
            List<(long numbers, double balance)> accountsBalance = new List<(long numbers, double balance)>();

            foreach (var account in bank.accounts)
            {
                accountsBalance.Add((account.NumbersAccount, account.Balance));
            }

            return accountsBalance;
        }

        public static void TopUpBalance (string bank,long numbersAccount, double money)
        {
            var bic = numbersAccount / 100000;
            
            var reqBank = banks.Find(bank => bank.BIC == bic);
            var account = reqBank.accounts.Find(account => account.NumbersAccount == numbersAccount);

            reqBank.TopUpAccount(account, money);
        }

        public static void WithDraw(string nameBank, long numbers, double money)
        {

            var bic = numbers / 100000;
            
            var reqBank = banks.Find(bank => bank.BIC == bic);
            var account = reqBank.accounts.Find(account => account.NumbersAccount == numbers);

            reqBank.WithDrawFromAccount(account, money);
        }

        public static void Transfer(long numbersAccountFrom, long numbersAccountTo, double money)
        {
            var bicFrom = numbersAccountFrom / 100000;
            var bankFrom = banks.Find(bank => bank.BIC == bicFrom);
            var accountFrom = bankFrom.accounts.Find(account => account.NumbersAccount == numbersAccountFrom);
            
            var bicTo = numbersAccountTo / 100000;
            var bankTo = banks.Find(bank => bank.BIC == bicTo);
            var accountTo = bankTo.accounts.Find(account => account.NumbersAccount == numbersAccountTo);
            
            bankFrom.Transfer(accountFrom, accountTo, money);
            
            Console.WriteLine("Трансфер окончен");
        }

        public static List<Account> GetAllAccountsOfClient(string nameBank, string name, string surname)
        {
            var bank = FindBank(nameBank);
            var client = bank.clients.Find(client => client.name == name && client.surname == surname);

            var accounts = bank.clientsAndAccounts
                .FindAll(connection => connection.client == client)
                .Select(connection => connection.account);

            List<Account> accountsClient = new List<Account>();
            foreach (var account in accounts)
            {
                accountsClient.Add(account);
            }

            return accountsClient;
        }

        private static Client FindClient(Bank bank, int idClient)
        {
            Client client;
            if (bank.clients.Count > idClient && idClient > -1)
            {
                client = bank.clients[idClient];
                return client;
            }
            else
            {
                throw new ExceptionClientDoesNotExist("Нет такого клиента");
            }
        }

        public static void CancleTransaction(long numbers, int transactionNumber)
        {
            var BIC = numbers / 100000;
            var bank = banks.Find(bank => bank.BIC == BIC);
            
            bank.CancleTransaction(numbers, transactionNumber);
        }

        public static void ShowAccountTransactions(long numbers)
        {
            var BIC = numbers / 100000;
            var bank = banks.Find(bank => bank.BIC == BIC);
            
            bank.ShowTransactionOnAccount(numbers);
        }

        public static Date Date() => Banks.Date.date();

        public static void ChangeTime(int days)
        {
            for (int i = 0; i < days; ++i)
            {
                var dateTest = Banks.Date.date().globalDate + TimeSpan.FromDays(1);
                Banks.Date.date().changeTime(dateTest);
                Calc();
            }
        }

        public static Client GetClientInfo(string nameBank, int idClient) => FindBank(nameBank).clients[idClient];

        public static Account GetAccountInfo(long numbers)
        {
            var bic = numbers / 100000;
            var requiredBankFrom = banks.Find(bank => bank.BIC == bic);
            return requiredBankFrom.accounts.Find(account => account.NumbersAccount == numbers);
        }

        public static double GetAccountBalance(long numbers)
        {
            var bic = numbers / 100000;
            var requiredBankFrom = banks.Find(bank => bank.BIC == bic);
            var account = requiredBankFrom.accounts.Find(account => account.NumbersAccount == numbers);

            return account.Balance;
        }

        private static void Calc()
        {
            foreach (var bank in banks)
            {
                foreach (var account in bank.accounts)
                {
                    account.Calc();
                }
            }
        }
    }
}