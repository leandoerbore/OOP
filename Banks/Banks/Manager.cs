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
            
            
            Console.WriteLine("{0} {1} стал клиентом банка {2}", client.name, client.surname, bank.Name);
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
            Console.WriteLine("Для {0} {1} был создан депозитный счёт в банке {2} с номером : {3}", client.name, client.surname, bank.Name, account.NumbersAccount);

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
            Console.WriteLine("Для {0} {1} был создан дебетовый счёт в банке {2} с номером : {3}", client.name, client.surname, bank.Name, account.NumbersAccount);

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
            Console.WriteLine("Для {0} {1} был создан кредитный счёт в банке {2} с номером : {3}", client.name, client.surname, bank.Name, account.NumbersAccount);

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
        {
            var clientInfo = bank.clients[idClient];
            
            return (clientInfo.passport == default || clientInfo.adress == null) ? true : false;
        }

        public static void ShowAllClientsOfBank(string nameBank)
        {
            var bank = FindBank(nameBank);

            int i = 0;
            foreach (var client in bank.clients)
            {
                ++i;
                Console.WriteLine("{0}) {1} {2}", i, client.name, client.surname );
            }
        }

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

        public static void CheckBalance(string nameBank)
        {
            var bank = FindBank(nameBank);

            foreach (var account in bank.accounts)
            {
                Console.WriteLine("Account: {0}, balance: {1:f2}", account.NumbersAccount, account.Balance);
            }
        }

        public static void TopUpBalance (long numbersAccount, double money)
        {
            var bic = numbersAccount / 100000;
            
            var bank = banks.Find(bank => bank.BIC == bic);
            var account = bank.accounts.Find(account => account.NumbersAccount == numbersAccount);

            bank.TopUpAccount(account, money);
        }

        public static void WithDraw(string nameBank, int numbers, double money)
        {

            var bic = numbers / 100000;
            
            var bank = banks.Find(bank => bank.BIC == bic);
            var account = bank.accounts.Find(account => account.NumbersAccount == numbers);

            bank.WithDrawFromAccount(account, money);
        }

        public static void Transfer(int numbersAccountFrom, int numbersAccountTo, double money)
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

        public static void ShowAllAccountsOfClient(string nameBank)
        {
            var bank = FindBank(nameBank);
            Console.WriteLine("Чьи счета показать ?");
            int i = 0;
            foreach (var client in bank.clients)
            {
                ++i;
                Console.WriteLine("{0}) {1} {2}, паспорт: {3}, адресс: {4}", i, client.name, client.surname, client.passport, client.adress);
            }

            var idClient = int.Parse(Console.ReadLine());

            var theClient = bank.clients[idClient - 1];

            var accounts = bank.clientsAndAccounts.FindAll(connection => connection.client == theClient).Select(connection => connection.account);

            i = 0;
            foreach (var account in accounts)
            {
                ++i;
                Console.WriteLine("{0}) numbers: {1}, balance: {2}", i, account.NumbersAccount, account.Balance);
            }
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

        public static void CancleTransaction(string nameBank, long numbers, int transactionNumber)
        {
            var bank = FindBank(nameBank);
            
            bank.CancleTransaction(numbers, transactionNumber);
        }

        public static Date Date()
        {
            return Banks.Date.date();
        }

        public static void ChangeTime(int days)
        {
            for (int i = 0; i < days; ++i)
            {
                var dateTest = Banks.Date.date().globalDate + TimeSpan.FromDays(1);
                Banks.Date.date().changeTime(dateTest);
                Thread.Sleep(1000);
            }
        }

        public static Client GetClientInfo(string nameBank, int idClient)
        {
            var bank = FindBank(nameBank);

            return bank.clients[idClient];
        }

        public static Account GetAccountInfo(int numbers)
        {
            var bic = numbers / 100000;
            var requiredBankFrom = banks.Find(bank => bank.BIC == bic);
            return requiredBankFrom.accounts.Find(account => account.NumbersAccount == numbers);
        }
    }
}