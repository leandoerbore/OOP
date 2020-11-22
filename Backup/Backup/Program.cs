using System;
using System.Collections.Generic;
using System.IO;
using System.Transactions;

namespace Backup
{
    class Program
    {
        static void Main(string[] args)
        {
            var date = new DateTime(2020, 11, 19);
            /*var datenow = DateTime.Now;
            Console.WriteLine(date.Date);
            Console.WriteLine(datenow);

            var answer = datenow - date;
            
            Console.WriteLine(answer.Days);*/

            string backuppath = @"D:\Test\backups\";
            var manager = new Manager();
            manager.CreateBackup(new List<string>() {@"D:\Test\file1.txt", @"D:\Test\file2.txt"});
            manager.CreateRestorePoint(0, backuppath); // Создал full

            manager.AddFileToBackup(0, @"D:\Test\file3.txt");
            manager.AddFileToBackup(0, @"D:\Test\file4.txt");
            manager.AddFileToBackup(0, @"D:\Test\file5.txt");
            manager.CreateDeltaRestorePoint(0, backuppath); // Создал delta
            
            manager.SetDateToPoint(0, date); // Поставил дату, да
            manager.SetDateToPoint(0, date); // Поставил дату, да
            manager.CreateRestorePoint(0, backuppath); // Создал full





            Console.WriteLine("");
            
            
            
            
        }
    }
}