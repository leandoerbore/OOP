using System;
using System.ComponentModel;
using System.Threading;

namespace Banks
{
    public class Date
    {
        private static Date instance;
        public DateTime globalDate { get; private set; }
        
        public Date()
        { 
            globalDate = DateTime.Now.Date;
        }

        public static Date date()
        {
            if (instance == null)
                instance = new Date();
            return instance;
        }

        public void changeTime(DateTime datet) 
        { 
            globalDate = datet;
            Console.WriteLine("Сменил дату на " + globalDate.Date);
        }
    }
}