using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Backup.CleaningAlgorithms;

namespace Backup
{
    public class FileBackup
    {
        public int _number { get; private set; } = 1;
        List<string> _listOfFiles = new List<string>();
        public Combo comboOption = Combo.OR;

        public int length { get; set; } = 1;
        public int dateTimeSpan { get; set; } = 2;
        public long maxSize { get; set; } = 150;
        

        public CreationMode creationMode ; 
        public int id { get; }
        public DateTime creationTime { get; }
        public long backupSize { get; private set; } = 0;
        private List<IPoints> _restorePoints = new List<IPoints>();
        
        public List<string> listOfFiles
        {
            get => _listOfFiles;
            private set => _listOfFiles = value;
        }

        public FileBackup(int id, CreationMode creationModeOption)
        {
            id = id;
            creationTime = DateTime.Now;
            creationMode = creationModeOption;
        }
        
        public FileBackup(int id, List<string> files, CreationMode creationModeOption)
        {
            id = id;
            creationTime = DateTime.Now;

            foreach (var file in files)
            {
                CheckFile(file);   
                AddFilesToBackup(file);
            }
            creationMode = creationModeOption;
        }

        

        public void CreateRestorePoint(string path, ICreateRestorePoint creationAlgoritm)
        {
            CheckDirectory(path);

            switch (creationMode)
            {
                case CreationMode.SEPARATELY:
                    
                    creationAlgoritm.CreateRestorePointSeparately(path, this);
                    backupSize += _restorePoints.Last()._size;
                    ++_number;
                    break;
                
                case CreationMode.ZIP:
                    
                    creationAlgoritm.CreateRestorePointZip(path, this);
                    backupSize += _restorePoints.Last()._size;
                    ++_number;
                    break;
            }

        }
        
        public void CreateDeltaRestorePoint(string path, ICreateRestorePoint creationAlgorithm)
        {
            CheckDirectory(path);

            switch (creationMode)
            {
                case CreationMode.SEPARATELY:
                    creationAlgorithm.CreateRestorePointSeparately(path, this);
                    backupSize += restorePoints.Last()._size;
                    ++_number;

                    break;
                
                case CreationMode.ZIP:
                    creationAlgorithm.CreateRestorePointZip(path, this);
                    backupSize += _restorePoints.Last()._size;
                    ++_number;
                    
                    break;
            }
        }

        public void AddFilesToBackup(string path)
        {
            if (File.Exists(path))
                _listOfFiles.Add(path);
            else
                throw new ExceptionFileDoesNotExist("Нет такого файла");
        }
        public void DeleteFilesForBackup()
        {
            Console.WriteLine("Выберите какие файлы убрать из бэкапа");
            ShowAllFilesForBackup();
            var answer = int.Parse(Console.ReadLine());
            if(answer < 0 || answer > _listOfFiles.Count)
                throw new ExceptionFileDoesNotExist("Нет такого файла");
            _listOfFiles.Remove(_listOfFiles[--answer]);
        }
        public void ShowAllFilesForBackup()
        {
            if (_listOfFiles.Count == 0)
                Console.WriteLine("Нет файлов для бекапа");
            int i = 1;
            foreach (var path in _listOfFiles)
            {
                Console.WriteLine("{0}) {1}", i, path);
                ++i;
            }
        }
        public List<IPoints> restorePoints
        {
            get { return _restorePoints; }
        }

        public void ShowAllRestorePoints()
        {
            int i = 1;
            foreach (var point in _restorePoints)
            {
                Console.WriteLine("{0}) {1}", i, point.GetType());
                ++i;
            }
        }
        public void SetDateToPoint(DateTime date)
        {
            Console.WriteLine("Выбери у какой точки изменить дату");
            ShowAllRestorePoints();
            var answer = int.Parse(Console.ReadLine());
            _restorePoints[--answer]._date = date;
        }

        public void Cleaning(ICleaningAlgorithm cleaningAlgorithm)
        {
            List<IPoints> forRemove = new List<IPoints>();
            forRemove = cleaningAlgorithm.Cleaning(this);
            foreach (var point in forRemove)
            {
                backupSize -= point._size;
                _restorePoints.Remove(point);
            }
        }

        public void Cleaning(List<ICleaningAlgorithm> hybrid)
        {
            List<IPoints> forRemove = new List<IPoints>();
            forRemove = new CleaningHybrid(this, comboOption).Cleaning(hybrid);
            foreach (var point in forRemove)
            {
                backupSize -= point._size;
                _restorePoints.Remove(point);
            }
        }

        public void ChangeTime(DateTime date)
        {
            _restorePoints.First()._date = date;
        }
        private void CheckDirectory(string path)
        {
            if (!Directory.Exists(path))
                throw new ExceptionDirectoryDoesNotExist("Нет такой директории");
        }
        
        public void CheckFile(string path)
        {
            if (!File.Exists(path))
                throw new ExceptionDirectoryDoesNotExist("Нет такой директории");
        }
        
    }
}