using System;

namespace Shop_lab2
{
    public class ExceptionZeroShops : Exception
    {
        public ExceptionZeroShops(string message)
        {
            Console.WriteLine(message);
        }
    }
}