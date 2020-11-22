using System;
using System.Collections.Generic;
using static Backup.Exceptions;

namespace Backup
{
    public class Manager
    {
        List<FileBackup> _backups = new List<FileBackup>();

        public void CreateBackup()
        {
            int id = _backups.Count;
            _backups.Add(new FileBackup(id));
            Console.WriteLine("Вы создали бэкап с идом {0}", id);
        }
        
        public void CreateBackup(List<string> files)
        {
            int id = _backups.Count;
            _backups.Add(new FileBackup(id, files));
            Console.WriteLine("Вы создали бэкап с идом {0}", id);
        }

        public void CreateRestorePoint(int idBackup, string path)
        {
            CheckIdBackup(idBackup);
            _backups[idBackup].CreateRestorePoint(path);
        }
        
        public void CreateDeltaRestorePoint(int idBackup, string path)
        {
            CheckIdBackup(idBackup);
            _backups[idBackup].CreateDeltaRestorePoint(path);
        }
        public void ShowSizeOfBackup(int idBackup)
        {
            CheckIdBackup(idBackup);
            Console.WriteLine(_backups[idBackup]._backupSize);
        }
        public void ShowCreationTime(int idBackup)
        {
            CheckIdBackup(idBackup);
            Console.WriteLine(_backups[idBackup]._creationTime);
        }
        public void AddFileToBackup(int idBackup, string path )
        {
            CheckIdBackup(idBackup);
            _backups[idBackup].AddFilesToBackup(path);
        }

        public void DeleteFileFromBackup(int idBackup)
        {
            CheckIdBackup(idBackup);
            _backups[idBackup].DeleteFilesForBackup();
        }
        public void ShowBackupFiles(int idBackup)
        {
            CheckIdBackup(idBackup);
            _backups[idBackup].ShowAllFilesForBackup();
        }

        public void ShowBackupRestorePoints(int idBackup)
        {
            CheckIdBackup(idBackup);
            foreach (var point in _backups[idBackup].restorePoints)
            {
                Console.WriteLine("---------------------------------------------------");
                for(int i = 0; i < point.files.Count; ++i)
                    Console.WriteLine("{0}) {1}", i+1, point.files[i]);
                Console.WriteLine("---------------------------------------------------");
            }
        }

        public void SetDateToPoint(int idBackup, DateTime date)
        {
            CheckIdBackup(idBackup);
            _backups[idBackup].SetDateToPoint(date);
        }


        public void CleanRestorePoints(int idBackup)
        {
            CheckIdBackup(idBackup);
            Console.WriteLine("Выберите тип очистки:");
            Console.WriteLine("1) По количеству");
            Console.WriteLine("2) По дате");
            Console.WriteLine("3) По размеру");
            Console.WriteLine("4) Гибритный алгоритм");
            int answer = int.Parse(Console.ReadLine());

            switch (answer)
            {
                case 1:
                    _backups[idBackup].Cleaning(1);
                    break;
                case 2:
                    _backups[idBackup].Cleaning(2);
                    break;
                case 3:
                    _backups[idBackup].Cleaning(3);
                    break;
                case 4:
                    Console.WriteLine("Выберите типы очистки:");
                    Console.WriteLine("Для выхода впишите -1");
                    List<int> answers = new List<int>();

                    int countOfTypes = 0;
                    while (true)
                    {
                        answer = int.Parse(Console.ReadLine());
                        if (answer == -1)
                            break;
                        if (answer < 1 || answer > 3)
                        {
                            Console.WriteLine("Нет такого типа очистки");
                            continue;
                        }
                        answers.Add(answer);
                        
                        if (answers.Count >= 3)
                        {
                            Console.WriteLine("Вы выбрали все типы");
                            break;
                        }
                    }
                    
                    _backups[idBackup].Cleaning(answers);



                    break;
            }
        }

        private void CheckIdBackup(int idBackup)
        {
            if (idBackup > _backups.Count || idBackup < 0)
                throw new ExceptionInvalidIndexOfBackup("Не найдет бэкап по такому индексу");
        }
    }
}