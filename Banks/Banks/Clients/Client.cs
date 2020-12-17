using System.Runtime.ConstrainedExecution;
using System;

namespace Banks
{
    public class Client
    {
        public string name { get; private set; }
        public string surname { get; private set; }
        public string adress { get; private set; }
        public long passport { get; private set; }

        public Client(string name, string surname, string adress=null, long passport=default)
        {
            this.name = name;
            this.surname = surname;
            this.adress = adress;
            this.passport = passport;
        }

        public void AddAdress(string adress)
        {
            adress = adress;
        }

        public void AddPassport(long passport)
        {
            passport = passport;
        }
    }
}