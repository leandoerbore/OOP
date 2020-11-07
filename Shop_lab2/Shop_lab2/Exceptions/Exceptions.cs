using System;

namespace Shop_lab2
{
    public class Exceptions
    {
        public class ExceptionQuantity : Exception
        {
            public ExceptionQuantity(string message)
            {
                Console.WriteLine(message);
            }
        }

        public class ExceptionShopDoesntExist : Exception
        {
            public ExceptionShopDoesntExist(string message)
            {
                Console.WriteLine(message);
            }
        }
        public class ExceptionProductDoesntExist : Exception
        {
            public ExceptionProductDoesntExist(string message)
            {
                Console.WriteLine(message);
            }
        }
        public class ExceptionZeroShops : Exception
        {
            public ExceptionZeroShops(string message)
            {
                Console.WriteLine(message);
            }
        }
        public class ExceptionZeroProducts : Exception
        {
            public ExceptionZeroProducts(string message)
            {
                Console.WriteLine(message);
            }
        }
    }
}