using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Backup
{
    public static class Manager
    {
        static List<FileBackup> _backups = new List<FileBackup>();

        public static void CreateBackup()
        {
            int id = _backups.Count;
            _backups.Add(new FileBackup(id));
            Console.WriteLine("Вы создали бэкап с идом {0}", id);
        }
        
        public static void CreateBackup(List<string> files)
        {
            int id = _backups.Count;
            _backups.Add(new FileBackup(id, files));
            Console.WriteLine("Вы создали бэкап с идом {0}", id);
        }

        public static void CreateRestorePoint(int idBackup, string path)
        {
            CheckIdBackup(idBackup);
            _backups[idBackup].CreateRestorePoint(path);
        }
        
        public static void CreateDeltaRestorePoint(int idBackup, string path)
        {
            CheckIdBackup(idBackup);
            _backups[idBackup].CreateDeltaRestorePoint(path);
        }
        public static void ShowSizeOfBackup(int idBackup)
        {
            CheckIdBackup(idBackup);
            Console.WriteLine(_backups[idBackup].BackupSize);
        }
        public static void ShowCreationTime(int idBackup)
        {
            CheckIdBackup(idBackup);
            Console.WriteLine(_backups[idBackup].CreationTime);
        }
        public static void AddFileToBackup(int idBackup, string path )
        {
            CheckIdBackup(idBackup);
            _backups[idBackup].AddFilesToBackup(path);
        }

        public static void DeleteFileFromBackup(int idBackup)
        {
            CheckIdBackup(idBackup);
            _backups[idBackup].DeleteFilesForBackup();
        }
        public static void ShowBackupFiles(int idBackup)
        {
            CheckIdBackup(idBackup);
            _backups[idBackup].ShowAllFilesForBackup();
        }

        public static void ShowBackupRestorePoints(int idBackup)
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
        public static void CleanRestorePoints(int idBackup, string type)
        {
            CheckIdBackup(idBackup);
            
            _backups[idBackup].Cleaning(type);
                    
        }

        public static void ChangeModeForCleaning(int idBackup)
        {
            Console.WriteLine("Выберите мод, который вы хотите изменить");
            Console.WriteLine("1) Алгоритм обычной очитски");
            Console.WriteLine("2) Алгоритм гибридной очистки");

            var answer = int.Parse(Console.ReadLine());

            switch (answer)
            {
                case 1:
                    Console.WriteLine("Выберите алгоритм в котором хотите сделать поправки");
                    Console.WriteLine("1) По кол-ву точек");
                    Console.WriteLine("2) По дате");
                    Console.WriteLine("3) По размеру");
                    answer = int.Parse(Console.ReadLine());
                    switch (answer)
                    {
                        case 1:
                            Console.WriteLine("Напишите максимум точек, которые могут храниться");
                            answer = int.Parse(Console.ReadLine());
                            _backups[idBackup].len = answer;
                            break;
                        case 2:
                            Console.WriteLine("Напишите самую позднюю дату(YY:MM:DD)");
                            var answerDate = DateTime.Parse(Console.ReadLine());
                            _backups[idBackup].dateTimeSpan = DateTime.Now.Day - answerDate.Day;
                            break;
                        case 3:
                            Console.WriteLine("Напишите максимальный размер в байтах");
                            long answerSize = long.Parse(Console.ReadLine());
                            _backups[idBackup].maxSize = answerSize;
                            break;
                    }
                    
                    break;
                case 2:
                    Console.WriteLine("Выберите алгоритмы очистки");
                    Console.WriteLine("1) По кол-ву точек");
                    Console.WriteLine("2) По дате");
                    Console.WriteLine("3) По размеру");
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
                        switch (answer)
                        {
                            case 1:
                                Console.WriteLine("Напишите максимум точек, которые могут храниться");
                                answer = int.Parse(Console.ReadLine());
                                _backups[idBackup].len = answer;
                                break;
                            case 2:
                                Console.WriteLine("Напишите самую позднюю дату(YY:MM:DD)");
                                var answerDate = DateTime.Parse(Console.ReadLine());
                                _backups[idBackup].dateTimeSpan = DateTime.Now.Day - answerDate.Day;
                                break;
                            case 3:
                                Console.WriteLine("Напишите максимальный размер в байтах");
                                long answerSize = long.Parse(Console.ReadLine());
                                _backups[idBackup].maxSize = answerSize;
                                break;
                        }
                        
                        if (answers.Count >= 3)
                        {
                            Console.WriteLine("Вы выбрали все типы");
                            break;
                        }
                    }

                    if (answers.Contains(1) && answers.Contains(2) && !answers.Contains(3) )
                    {
                        _backups[idBackup].cleaningHybridMode = 1;
                    }
                    else if (answers.Contains(1) && answers.Contains(3) && !answers.Contains(2))
                    {
                        _backups[idBackup].cleaningHybridMode = 2;
                    }
                    else if (answers.Contains(2) && answers.Contains(3) && !answers.Contains(1))
                    {
                        _backups[idBackup].cleaningHybridMode = 3;
                    }
                    else if (answers.Contains(1) && answers.Contains(2) && answers.Contains(3))
                    {
                        _backups[idBackup].cleaningHybridMode = 4;
                    }
                    else
                    {
                        throw new ExceptionBadMode("Не выбран правильно мод для гибрида");
                    }
                    
                    
                    Console.WriteLine("");
                    Console.WriteLine("Выберите как комбинировать");
                    Console.WriteLine("1) Нужно удалить точку, если вышла за хотя бы один установленный лимит\n2) Нужно удалить точку, если вышла за все установленные лимиты");

                    int Combo = int.Parse(Console.ReadLine());
                    if (Combo != 1 && Combo != 2)
                        throw new ExceptionBadMode("Не выбран лимит");
                    Console.WriteLine("Убирать по\n1) Максимум точек\n2) Минимум точек");
                    int MinMax = int.Parse(Console.ReadLine());
                    if (MinMax != 1 && MinMax != 2)
                        throw new ExceptionBadMode("Не выбран мод кол-ва точек");

                    _backups[idBackup].combo = Combo;
                    _backups[idBackup].minMax = MinMax;

                    break;
            }
        }

        public static void ChangeTime(int idBackup)
        {
            _backups[idBackup].ChangeTime();
            Console.WriteLine("Изменено время у первого бэкапа");
        }
        

        private static void CheckIdBackup(int idBackup)
        {
            if (idBackup > _backups.Count || idBackup < 0)
                throw new ExceptionInvalidIndexOfBackup("Не найдет бэкап по такому индексу");
        }
        
        
        
    }
}