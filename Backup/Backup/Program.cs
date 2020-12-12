using System;
using System.Collections.Generic;
using System.IO;
using System.Transactions;
using Backup.CleaningAlgorithms;

namespace Backup
{
    class Program
    {
        static void Main(string[] args)
        {
            string backupPath = @"D:\Test\backups\";

            // Test
            {
                Manager.CreateBackup(new List<string>() {@"D:\Test\file1.txt", @"D:\Test\file2.txt"}, 1);
                Manager.CreateRestorePoint(0, backupPath);
                
                Manager.CreateRestorePoint(0, backupPath);
                Manager.CleanRestorePoints(0, new CleaningBySize());
                
            }

            /*// Case №1
            {
                Manager.CreateBackup(new List<string>() {@"D:\Test\file1.txt", @"D:\Test\file2.txt"}, 1);
                Manager.CreateRestorePoint(0, backupPath);
                
                Manager.CreateRestorePoint(0, backupPath);
                Manager.CleanRestorePoints(0, "len");
            }
            
            // Case №2
            {
                Manager.CreateBackup(new List<string>() {@"D:\Test\file3.txt", @"D:\Test\file4.txt"}, 1);
                Manager.CreateRestorePoint(1, backupPath);
                Manager.CreateRestorePoint(1, backupPath);
                Manager.CleanRestorePoints(1, "size");
            }
            
            // Case №3
            {
                Manager.CreateBackup(new List<string>() {@"D:\Test\file5.txt"}, 1);
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
                Manager.CreateBackup(new List<string>() {@"D:\Test\file7.txt", @"D:\Test\file8.txt"}, 1);
                Manager.CreateRestorePoint(3, backupPath);
                Manager.CreateRestorePoint(3, backupPath);
                Manager.CleanRestorePoints(3, "hybrid"); // Len(1) & Size(150) default
                
                Manager.CreateRestorePoint(3, backupPath);
                Manager.ChangeTime(3);
                Manager.ChangeModeForCleaning(3);
                Manager.CleanRestorePoints(3, "hybrid"); // Len & Date 
            }*/

        }

        public static void ParseRestorePointInfo(DateTime date, string path, long size, List<string> files)
        {
            int i = 0;
            Console.WriteLine("Дата создания: {0}\nПуть: {1}\nРазмер точки: {2}\nФайлы:", date, path, size);
            foreach (var file in files)
            {
                ++i;
                Console.WriteLine("{0}. {1}", i, file);
            }
        }
        
    }
}