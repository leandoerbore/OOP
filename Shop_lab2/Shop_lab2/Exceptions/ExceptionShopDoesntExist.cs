using System;

namespace Shop_lab2
{
    public class ExceptionShopDoesntExist : Exception
    {
        public ExceptionShopDoesntExist(string message)
        {
            Console.WriteLine(message);
        }
    }
}