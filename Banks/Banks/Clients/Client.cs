using System.Runtime.ConstrainedExecution;
using System;

namespace Banks
{
    public class Client
    {
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public string Adress { get; private set; }
        public long Passport { get; private set; }

        public Client(string name, string surname, string adress=null, long passport=default)
        {
            Name = name;
            Surname = surname;
            Adress = adress;
            Passport = passport;
        }

        public void AddAdress(string adress)
        {
            Adress = adress;
        }

        public void AddPassport(long passport)
        {
            Passport = passport;
        }
    }
}