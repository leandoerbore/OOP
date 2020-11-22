﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Backup
{
    public class FileBackup
    {
        public int flagForCleaning { get; set; } = 2;
        private int _number = 1;
        List<string> _listOfFiles = new List<string>();

        public int _id { get; }
        public DateTime _creationTime { get; }
        public long _backupSize { get; private set; } = 0;
        private List<IPoints> _restorePoints = new List<IPoints>();

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
                    _restorePoints.Add(new FullPoint(path, Sizing(_listOfFiles),  DateTime.Now, _listOfFiles, _number));
                    _backupSize += _restorePoints.Last()._size;

                    for (int i = 0; i < _listOfFiles.Count; ++i)
                    {
                        File.Copy(_listOfFiles[i],  path+_restorePoints.Last().files[i]);
                    }

                    ++_number;
                    break;
                
                case 2:
                    string zipName = path + "restore-point-" + _number + ".zip";
                    _restorePoints.Add(new FullPoint(zipName, Sizing(_listOfFiles),  DateTime.Now, _listOfFiles, _number, zipName));
                    
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

            Cleaning();
        }
        
        public void CreateDeltaRestorePoint(string path)
        {
            Console.WriteLine("1) Раздельно\n2) Архивом");
            var answer = int.Parse(Console.ReadLine());

            var fullPoint = FindLastFullRestorePoint();
            if (fullPoint == -1)
            {
                Console.WriteLine("Ну пизда что");
                return;
            }

            var difList = new List<string>();
            
            if (Path.GetExtension(_restorePoints[fullPoint]._path) == ".zip")
            {
                difList = FindDifferenceInZip(_restorePoints[fullPoint]);
            }
            else
            {
                difList = FindDifference(_restorePoints[fullPoint]);
            }

            if (difList.Count == 0)
            {
                Console.WriteLine("ТОже пизда, ничего нет");
                return;
            }
            
            switch (answer)
            {
                case 1:
                    _restorePoints.Add(new IncrementralPoint(path, Sizing(_listOfFiles),  DateTime.Now, difList, _number));
                    _backupSize += _restorePoints.Last()._size;

                    for (int i = 0; i < difList.Count; ++i)
                    {
                        File.Copy(difList[i],  path+_restorePoints.Last().files[i]);
                    }

                    _restorePoints[fullPoint].IndexOfDeltas += 1;
                    ++_number;
                    break;
                
                case 2:
                    string zipName = path + "restore-point-" + _number + ".zip";
                    _restorePoints.Add(new IncrementralPoint(zipName, Sizing(_listOfFiles),  DateTime.Now, difList, _number, zipName));
                    
                    Directory.CreateDirectory(path + @"\temp\");
                    
                    for (int i = 0; i < difList.Count; ++i)
                    {
                        File.Copy(_listOfFiles[i], path + @"\temp\" +_restorePoints.Last().files[i]);
                    }

                    ZipFile.CreateFromDirectory(path+"temp", zipName);
                    
                    Directory.Delete(path + @"\temp\", true);
                    
                    FileInfo fileInfo = new FileInfo(zipName);
                    _restorePoints.Last()._size = fileInfo.Length;
                    _backupSize += _restorePoints.Last()._size;
                    
                    _restorePoints[fullPoint].IndexOfDeltas += 1;
                    ++_number;
                    break;
                
            }

            Cleaning();
        }

        private int FindLastFullRestorePoint()
        {
            var fullPoint = 0;

            for (int i = _restorePoints.Count - 1; i >= 0; --i)
            {
                if (_restorePoints[i] is FullPoint)
                {
                    return i;
                }
            }

            return -1;
        }

        private List<string> FindDifference(IPoints point)
        {
            var dif = new List<string>();

            for (int i = 0; i < _listOfFiles.Count; ++i)
            {
                FileInfo fileInfoList = new FileInfo(_listOfFiles[i]);
                var FindFile =
                    from x in point.files
                    where x.LastIndexOf(fileInfoList.Name) != -1
                    select x;

                if (FindFile.Count() == 0)
                {
                    dif.Add(_listOfFiles[i]);
                    continue;
                }

                FileInfo fileInfoPoint = new FileInfo(point._path+FindFile.First());
                if (fileInfoList.Length - fileInfoPoint.Length != 0)
                {
                    dif.Add(_listOfFiles[i]);
                }
            }
            
            /*int answer;
            int num;
            if (_listOfFiles.Count > point.files.Count)
            {
                answer = 1;
                num = point.files.Count;
            }
            else 
            {
                answer = 2;
                num = 0;
            }*/
            
            /*switch (answer)
            {
                case 1:
                    for (int i = 0; i < point.files.Count; ++i)
                    {
                        FileInfo fileInfoList = new FileInfo(_listOfFiles[i]);
                        var FindFile =
                            from x in point.files
                            where x.LastIndexOf(fileInfoList.Name) != -1
                            select x;

                        if (FindFile.Count() == 0)
                        {
                            dif.Add(_listOfFiles[i]);
                            break;
                        }

                        FileInfo fileInfoPoint = new FileInfo(point._path+FindFile.First());

                        if (fileInfoList.Length - fileInfoPoint.Length != 0)
                        {
                            dif.Add(_listOfFiles[i]);
                        }
                    }

                    for (int i = num; i < _listOfFiles.Count; ++i)
                    {
                        dif.Add(_listOfFiles[i]);
                    }
                    
                    break;
                case 2:
                    for (int i = 0; i < _listOfFiles.Count; ++i)
                    {
                        FileInfo fileInfoList = new FileInfo(_listOfFiles[i]);
                        var FindFile =
                            from x in point.files
                            where x.LastIndexOf(fileInfoList.Name) != -1
                            select x;

                        if (FindFile.Count() == 0)
                        {
                            dif.Add(_listOfFiles[i]);
                            break;
                        }

                        FileInfo fileInfoPoint = new FileInfo(point._path+FindFile.First());
                        if (fileInfoList.Length - fileInfoPoint.Length != 0)
                        {
                            dif.Add(_listOfFiles[i]);
                        }
                    }

                    break;
                case 3:
                    for (int i = 0; i < _listOfFiles.Count; ++i)
                    {
                        FileInfo fileInfoList = new FileInfo(_listOfFiles[i]);
                        var FindFile =
                            from x in point.files
                            where x.LastIndexOf(fileInfoList.Name) != -1
                            select x;

                        if (FindFile.Count() == 0)
                        {
                            dif.Add(_listOfFiles[i]);
                            break;
                        }

                        FileInfo fileInfoPoint = new FileInfo(point._path+FindFile.First());
                        if (fileInfoList.Length - fileInfoPoint.Length != 0)
                        {
                            dif.Add(_listOfFiles[i]);
                        }
                    }
                    
                    break;
            }*/

            return dif;
        }
        private List<string> FindDifferenceInZip(IPoints point)
        {
            var dif = new List<string>();
            
            var currentDir = Path.GetDirectoryName(point._path);
            Directory.CreateDirectory(currentDir + @"\temp\");
            ZipFile.ExtractToDirectory(point._path, currentDir + @"\temp\");
            
            for (int i = 0; i < _listOfFiles.Count; ++i)
            {
                FileInfo fileInfoList = new FileInfo(_listOfFiles[i]);
                var FindFile =
                    from x in point.files
                    where x.LastIndexOf(fileInfoList.Name) != -1
                    select x;

                if (FindFile.Count() == 0)
                {
                    dif.Add(_listOfFiles[i]);
                    continue;
                }

                FileInfo fileInfoPoint = new FileInfo(currentDir + @"\temp\"+FindFile.First());
                if (fileInfoList.Length - fileInfoPoint.Length != 0)
                {
                    dif.Add(_listOfFiles[i]);
                }
            }
            
            /*int answer;
            int num;
            if (_listOfFiles.Count > point.files.Count)
            {
                answer = 1;
                num = point.files.Count;
            }
            else if (_listOfFiles.Count < point.files.Count)
            {
                answer = 2;
                num = _listOfFiles.Count;
            }
            else // if (_listOfFiles.Count == point.files.Count)
            {
                answer = 3;
                num = 0;
            }*/
            
            /*switch (answer)
            {
                case 1:
                    
                    for (int i = 0; i < point.files.Count; ++i)
                    {
                        FileInfo fileInfoList = new FileInfo(_listOfFiles[i]);
                        FileInfo fileInfoPoint = new FileInfo(currentDir + @"\temp\"+point.files[i]);
                        if (fileInfoList.Length - fileInfoPoint.Length != 0)
                        {
                            dif.Add(_listOfFiles[i]);
                        }
                    }

                    for (int i = num; i < _listOfFiles.Count; ++i)
                    {
                        dif.Add(_listOfFiles[i]);
                    }

                    break;
                case 2:
                    for (int i = 0; i < _listOfFiles.Count; ++i)
                    {
                        FileInfo fileInfoList = new FileInfo(_listOfFiles[i]);
                        FileInfo fileInfoPoint = new FileInfo(currentDir + @"\temp\"+point.files[i]);
                        if (fileInfoList.Length - fileInfoPoint.Length != 0)
                        {
                            dif.Add(_listOfFiles[i]);
                        }
                    }

                    break;
                case 3:
                    for (int i = 0; i < _listOfFiles.Count; ++i)
                    {
                        FileInfo fileInfoList = new FileInfo(_listOfFiles[i]);
                        FileInfo fileInfoPoint = new FileInfo(currentDir + @"\temp\"+point.files[i]);
                        if (fileInfoList.Length - fileInfoPoint.Length != 0)
                        {
                            dif.Add(_listOfFiles[i]);
                        }
                    }
                    
                    break;
            }*/

            Directory.Delete(currentDir + @"\temp\", true);
            return dif;
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
        
        private void Cleaning()
        {
            int count;
            switch (flagForCleaning)
            {
                case 1:
                    int len = 2;

                    if (_restorePoints.Count > len)
                    {
                        List<IPoints> SaveToDelbyLen = new List<IPoints>();
                        var FirstStepPointsToDeletebyLen =
                            from x in _restorePoints
                            where _restorePoints.IndexOf(x) < _restorePoints.Count - len
                            select x;
                        
                        {
                            var SecondStepPointsToDeletebyLen =
                                from x in FirstStepPointsToDeletebyLen
                                where x is FullPoint && x.IndexOfDeltas == 0
                                select x;

                            foreach (var point in SecondStepPointsToDeletebyLen)
                            {
                                SaveToDelbyLen.Add(point);
                            }
                        }
                        
                        var ThirdStepPointsToDeletebyLen =
                            from x in FirstStepPointsToDeletebyLen
                            where x is FullPoint && x.IndexOfDeltas > 0
                            select x;
                        
                        
                        count = 0;
                        foreach (var point in ThirdStepPointsToDeletebyLen)
                        {
                            for (int i = 1; i <= point.IndexOfDeltas; ++i)
                                if (_restorePoints.IndexOf(point) + i < _restorePoints.Count - len)
                                {
                                    SaveToDelbyLen.Add(_restorePoints[_restorePoints.IndexOf(point) + i]);
                                    count++;
                                }

                            point.IndexOfDeltas -= count;

                            if (point.IndexOfDeltas == 0)
                            {
                                SaveToDelbyLen.Add(point);
                            }
                        }

                        foreach (var point in SaveToDelbyLen)
                        {
                            _restorePoints.Remove(point);
                        }
                    }
                    break;
                case 2:
                    List<IPoints> SaveToDelbyDate = new List<IPoints>();
                    var dateTimeSpan = new TimeSpan().Days;
                    dateTimeSpan = 2;
                    var dateNow = DateTime.Now;
                    
                    var FirstStepPointsToDeletebyDate =
                        from x in _restorePoints
                        where (dateNow - x._date).Days > dateTimeSpan
                        select x; 
                    
                    {
                        var SecondStepPointsToDeletebyDate =
                        from x in FirstStepPointsToDeletebyDate
                        where x is FullPoint && x.IndexOfDeltas < 1
                        select x;
                        
                        foreach (var point in SecondStepPointsToDeletebyDate)
                        {
                            SaveToDelbyDate.Add(point);
                        }
                    }

                    var ThirdStepPointsToDeletebyDate =
                        from x in FirstStepPointsToDeletebyDate
                        where x is FullPoint && x.IndexOfDeltas > 0
                        select x;
                    
                    count = 0;
                    foreach (var point in ThirdStepPointsToDeletebyDate)
                    {
                        int indexOfFullPoint = _restorePoints.IndexOf(point);
                        for (int i = 1; i <= point.IndexOfDeltas; ++i)
                            if ((dateNow - _restorePoints[indexOfFullPoint + i]._date).Days > dateTimeSpan)
                            {
                                SaveToDelbyDate.Add(_restorePoints[indexOfFullPoint + i]);
                                count++;
                            }

                        point.IndexOfDeltas -= count;

                        if (point.IndexOfDeltas == 0)
                        {
                            SaveToDelbyDate.Add(point);
                        }
                    }

                    foreach (var point in SaveToDelbyDate)
                    {
                        _restorePoints.Remove(point);
                    }

                    break;
                case 3:
                    List<IPoints> SaveToDelbySize = new List<IPoints>();
                    
                    
                    

                    break;
            }
        }

    }
}