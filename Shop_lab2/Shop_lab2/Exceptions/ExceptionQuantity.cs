using System;

namespace Shop_lab2
{
    public class ExceptionQuantity : Exception
    {
        public ExceptionQuantity(string message)
        { 
            Console.WriteLine(message);
        }
    }
}