using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using Backup.CleaningAlgorithms;
using Backup.CreationRestorePointsAlgorithms;

namespace Backup
{
    class Program
    {
        static void Main(string[] args)
        {
            string backupPath = @"D:\Test\backups\";

            // Case №1
            {
                int casePoint = 0;
                
                Manager.CreateBackup(new List<string>() {@"D:\Test\file1.txt", @"D:\Test\file2.txt"}, CreationMode.SEPARATELY);
                Manager.CreateRestorePoint(0, backupPath);
                var pointInfo = Manager.GetRestorePointInfo(0, 0);
                if (pointInfo.files.Count == 2)
                    casePoint++;

                Manager.CreateRestorePoint(0, backupPath);
                
                Manager.ChangeParametrForCleaning(0, CleaningParametrs.LENGTH, 1);
                Manager.CleanRestorePoints(0, new CleaningByLength());
                var pointsName = Manager.GetRestorePoints(0);
                casePoint += pointsName.Count == 1 ? 1 : 0;

                Console.WriteLine(casePoint == 2 ? "Case 1 успешно пройден" : "Case 1 не пройден");
            }
            
            // Case №2
            {
                int casePoint = 0;
                Manager.CreateBackup(new List<string>() {@"D:\Test\file3.txt", @"D:\Test\file4.txt"}, CreationMode.SEPARATELY);
                Manager.CreateRestorePoint(1, backupPath);
                Manager.CreateRestorePoint(1, backupPath);

                casePoint += Manager.GetRestorePoints(1).Count == 2 ? 1 : 0;
                casePoint += Manager.GetBackupSize(1) == 200 ? 1 : 0;
                Manager.ChangeParametrForCleaning(1, CleaningParametrs.SIZE, 150);
                
                Manager.CleanRestorePoints(1, new CleaningBySize());

                casePoint += Manager.GetRestorePoints(1).Count == 1 ? 1 : 0;
                
                Console.WriteLine(casePoint == 3 ? "Case 2 успешно пройден" : "Case 2 не пройден");
            }
            
            // Case №3
            {
                int casePoint = 0;
                Manager.CreateBackup(new List<string>() {@"D:\Test\file5.txt"}, CreationMode.SEPARATELY);
                Manager.CreateRestorePoint(2, backupPath);
                
                Manager.AddFileToBackup(2, @"D:\Test\file6.txt");
                using (StreamWriter sw = new StreamWriter(@"D:\Test\file5.txt", true))
                {
                    sw.WriteLine("Add new text");
                }
                
                Manager.CreateDeltaRestorePoint(2, backupPath);
                casePoint += Manager.GetRestorePointInfo(2, 1).files.Count == 2 ? 1 : 0;
                
                Console.WriteLine(casePoint == 1 ? "Case 3 успешно пройден" : "Case 3 не пройден");

            }
            
            // Case №4
            {
                int casePoint = 0;
                Manager.CreateBackup(new List<string>() {@"D:\Test\file7.txt", @"D:\Test\file8.txt"}, CreationMode.SEPARATELY);
                Manager.CreateRestorePoint(3, backupPath);
                Manager.CreateRestorePoint(3, backupPath);
                Manager.CreateRestorePoint(3, backupPath);
                Manager.CreateRestorePoint(3, backupPath);
                
                Manager.ChangeParametrForCleaning(3, CleaningParametrs.SIZE, 300);
                Manager.ChangeParametrForCleaning(3, CleaningParametrs.LENGTH, 1);
                Manager.ChangeComboMode(3, Combo.AND);

                Manager.CleanRestorePoints(3, new List<ICleaningAlgorithm> {new CleaningByLength(), new CleaningBySize()});

                casePoint += Manager.GetRestorePoints(3).Count == 3 ? 1 : 0;
                
                Manager.CreateRestorePoint(3, backupPath);
                Manager.ChangeTime(3, new DateTime(2020,01, 01));
                Manager.ChangeParametrForCleaning(3, CleaningParametrs.SIZE,200);
                Manager.ChangeComboMode(3, Combo.OR);
                
                Manager.CleanRestorePoints(3, new List<ICleaningAlgorithm> {new CleaningByDate(), new CleaningBySize()});

                casePoint += Manager.GetRestorePoints(3).Count == 2 ? 1 : 0;
                
                Console.WriteLine(casePoint == 2 ? "Case 4 успешно пройден" : "Case 4 не пройден");
            }

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