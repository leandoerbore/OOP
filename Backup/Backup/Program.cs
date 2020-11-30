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
            string backupPath = @"D:\Test\backups\";
            
            // Case №1
            {
                Manager.CreateBackup(new List<string>() {@"D:\Test\file1.txt", @"D:\Test\file2.txt"});
                Manager.CreateRestorePoint(0, backupPath);
                
                Manager.CreateRestorePoint(0, backupPath);
                Manager.CleanRestorePoints(0, "len");
            }
            
            // Case №2
            {
                Manager.CreateBackup(new List<string>() {@"D:\Test\file3.txt", @"D:\Test\file4.txt"});
                Manager.CreateRestorePoint(1, backupPath);
                Manager.CreateRestorePoint(1, backupPath);
                Manager.CleanRestorePoints(1, "size");
            }
            
            // Case №3
            {
                Manager.CreateBackup(new List<string>() {@"D:\Test\file5.txt"});
                Manager.CreateRestorePoint(2, backupPath);
                
                Manager.AddFileToBackup(2, @"D:\Test\file6.txt");
                using (StreamWriter sw = new StreamWriter(@"D:\Test\file5.txt", true))
                {
                    sw.WriteLine("Add new text");
                }
                
                Manager.CreateDeltaRestorePoint(2, backupPath);
            }
            
            // Case №4
            {
                Manager.CreateBackup(new List<string>() {@"D:\Test\file7.txt", @"D:\Test\file8.txt"});
                Manager.CreateRestorePoint(3, backupPath);
                Manager.CreateRestorePoint(3, backupPath);
                Manager.CleanRestorePoints(3, "hybrid"); // Len(1) & Size(150) default
                
                Manager.CreateRestorePoint(3, backupPath);
                Manager.ChangeTime(3);
                Manager.ChangeModeForCleaning(3);
                Manager.CleanRestorePoints(3, "hybrid"); // Len & Date 
            }

        }
    }
}