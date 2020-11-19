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
            manager.CreateRestorePoint(0, backuppath);
            manager.CreateRestorePoint(0, backuppath);
            manager.ShowSizeOfBackup(0);
            manager.ShowBackupRestorePoints(0);
            manager.AddFileToBackup("hihihi", 0);
            
            Console.WriteLine("");
            
            
            
            
        }
    }
}