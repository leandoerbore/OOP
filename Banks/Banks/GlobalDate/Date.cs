using System;
using System.ComponentModel;
using System.Threading;

namespace Banks
{
    public static class Date
    {
        private static bool flag = true;
        public static DateTime globalDate = DateTime.Now.Date;
        private static Thread threadDate = new Thread(DayNow);

        static Date()
        {
            threadDate.Start();
        }

        public static void changeTime(DateTime date) 
        { 
            globalDate = date;
            Console.WriteLine("Сменил дату на " + globalDate.Date);
        }
        
        private static void DayNow()
        {
            while (flag)
            {
                globalDate = DateTime.Now.Date;
                //Thread.Sleep(10000);
                Thread.Sleep(TimeSpan.FromDays(1));
            }
        }
        
    }
}