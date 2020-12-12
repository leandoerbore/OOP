﻿using System;
using System.Collections.Generic;

namespace Backup
{
    public static class Manager
    {
        static List<FileBackup> _backups = new List<FileBackup>();

        public static void CreateBackup(int creationMode)
        {
            int id = _backups.Count;
            _backups.Add(new FileBackup(id, creationMode));
            Console.WriteLine("Вы создали бэкап с идом {0}", id);
        }

        public static (DateTime date, string path, long size, List<string> files) GetRestorePointInfo(int idBackUp, int idRestorePoint)
        {
            var backup = _backups[idBackUp];
            var restorePoint = backup.restorePoints[idRestorePoint];

            return (restorePoint._date, restorePoint._path, restorePoint._size, restorePoint.files);
        }
        
        public static void CreateBackup(List<string> files, int creationMode)
        {
            int id = _backups.Count;
            _backups.Add(new FileBackup(id, files, creationMode));
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
        public static List<string> GetBackupFiles(int idBackup)
        {
            CheckIdBackup(idBackup);
            _backups[idBackup].ShowAllFilesForBackup();

            return _backups[idBackup].listOfFiles;
        }
        public static void CleanRestorePoints(int idBackup, ICleaningAlgorithm algorithm)
        {
            CheckIdBackup(idBackup);
            
            _backups[idBackup].Cleaning(algorithm);
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
                            _backups[idBackup].length = answer;
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
                                _backups[idBackup].length = answer;
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