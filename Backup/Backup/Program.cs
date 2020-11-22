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
            

            string backuppath = @"D:\Test\backups\";
            var manager = new Manager();
            manager.CreateBackup(new List<string>() {@"D:\Test\file1.txt"});
            manager.CreateRestorePoint(0, backuppath); // Создал full

            manager.AddFileToBackup(0,@"D:\Test\file3.txt" );
            manager.CreateRestorePoint(0, backuppath); // Создал full

            manager.CreateRestorePoint(0, backuppath); // Создал full

            manager.CleanRestorePoints(0);
            
            /*var date = new DateTime(2020, 11, 19);
            manager.SetDateToPoint(0, date);*/


            Console.WriteLine("");
        }
    }
}