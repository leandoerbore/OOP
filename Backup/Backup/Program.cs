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
            manager.CreateBackup(new List<string>() {@"D:\Test\file1.txt", @"D:\Test\file2.txt"});
            manager.CreateRestorePoint(0, backuppath); // Создал full
            manager.AddFileToBackup(0, @"D:\Test\file3.txt");
            manager.CreateDeltaRestorePoint(0, backuppath); // Создал delta
            manager.AddFileToBackup(0, @"D:\Test\file4.txt");
            manager.CreateDeltaRestorePoint(0, backuppath); // Создал delta
            manager.AddFileToBackup(0, @"D:\Test\file5.txt");
            manager.CreateDeltaRestorePoint(0, backuppath); // Создал delta
            
            manager.CreateRestorePoint(0, backuppath); // Создал full
            manager.CreateRestorePoint(0, backuppath); // Создал full
           
            
            
            
            
            
            //qq
            /*manager.CreateRestorePoint(0, backuppath); // Создал full
            manager.AddFileToBackup(0, @"D:\Test\file3.txt");
            manager.CreateDeltaRestorePoint(0, backuppath); // Создал delta
            manager.CreateRestorePoint(0, backuppath); // Создал full
            manager.AddFileToBackup(0, @"D:\Test\file4.txt");
            manager.CreateDeltaRestorePoint(0, backuppath); // Создал delta*/
            
            
            
            
            Console.WriteLine("");
            
            
            
            
        }
    }
}