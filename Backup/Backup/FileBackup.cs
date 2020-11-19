using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Backup
{
    public class FileBackup
    {
        private int _number = 1;
        List<string> _listOfFiles = new List<string>();

        public int _id { get; }
        public DateTime _creationTime { get; }
        public long _backupSize { get; private set; } = 0;
        private List<RestorePoint> _restorePoints = new List<RestorePoint>();

        public FileBackup(int id)
        {
            _id = id;
            _creationTime = DateTime.Now;
        }
        
        public FileBackup(int id, List<string> files)
        {
            _id = id;
            _creationTime = DateTime.Now;

            foreach (var file in files)
            {
                AddFilesToBackup(file);
            }
        }
        
        public void CreateRestorePoint(string path)
        {
            Console.WriteLine("1) Раздельно\n2) Архивом");
            var answer = int.Parse(Console.ReadLine());

            switch (answer)
            {
                case 1:
                    _restorePoints.Add(new RestorePoint(path, Sizing(_listOfFiles),  DateTime.Now, _listOfFiles, _number));
                    _backupSize += _restorePoints.Last()._size;

                    for (int i = 0; i < _listOfFiles.Count; ++i)
                    {
                        File.Copy(_listOfFiles[i],  path+_restorePoints.Last().files[i]);
                    }

                    ++_number;
                    break;
                
                case 2:
                    string zipName = path + "restore-point-" + _number + ".zip";
                    _restorePoints.Add(new RestorePoint(zipName, Sizing(_listOfFiles),  DateTime.Now, _listOfFiles, _number, zipName));
                    
                    Directory.CreateDirectory(path + @"\temp\");
                    
                    for (int i = 0; i < _listOfFiles.Count; ++i)
                    {
                        File.Copy(_listOfFiles[i], path + @"\temp\" +_restorePoints.Last().files[i]);
                    }

                    ZipFile.CreateFromDirectory(path+"temp", zipName);
                    
                    Directory.Delete(path + @"\temp\", true);
                    
                    FileInfo fileInfo = new FileInfo(zipName);
                    _restorePoints.Last()._size = fileInfo.Length;
                    _backupSize += _restorePoints.Last()._size;
                    ++_number;
                    break;
            }
            
            
        }

        private long Sizing(List<string> files)
        {
            long size = 0;

            foreach (var file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                size += fileInfo.Length;
            }

            return size;
        }
       
        public void AddFilesToBackup(string path)
        {
            if (Directory.Exists(path) || File.Exists(path))
                _listOfFiles.Add(path);
            else
                Console.WriteLine("Не существует такого файла или дирректории");
        }
        public void DeleteFilesForBackup()
        {
            Console.WriteLine("Выберите какие файлы убрать из бэкапа");
            ShowAllFilesForBackup();
            var answer = int.Parse(Console.ReadLine());
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

        public List<RestorePoint> restorePoints
        {
            get { return _restorePoints; }
        }

    }
}