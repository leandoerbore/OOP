using System;

namespace Shop_lab2
{
    public class ExceptionZeroProducts : Exception
    {
        public ExceptionZeroProducts(string message)
        {
            Console.WriteLine(message);
        }
    }
}