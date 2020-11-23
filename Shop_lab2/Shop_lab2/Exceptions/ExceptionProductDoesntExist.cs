using System;

namespace Shop_lab2
{
    public class ExceptionProductDoesntExist : Exception
    {
        public ExceptionProductDoesntExist(string message)
        {
            Console.WriteLine(message);
        }
    }
}