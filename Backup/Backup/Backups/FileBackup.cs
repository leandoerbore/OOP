using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using static Backup.Exceptions;

namespace Backup
{
    public class FileBackup
    {
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
                CheckFile(file);   
                AddFilesToBackup(file);
            }
        }
        
        public void CreateRestorePoint(string path)
        {
            CheckDirectory(path);
            Console.WriteLine("1) Раздельно\n2) Архивом");
            var answer = int.Parse(Console.ReadLine());

            switch (answer)
            {
                case 1:
                    _restorePoints.Add(new FullPoint(path, Sizing(_listOfFiles),  DateTime.Now, _listOfFiles, _number));
                    _backupSize += _restorePoints.Last()._size;

                    for (int i = 0; i < _listOfFiles.Count; ++i)
                    {
                        if (File.Exists(path + _restorePoints))
                            throw new ExceptionFileExist("Такой файл уже существует");
                        File.Copy(_listOfFiles[i],  path+_restorePoints.Last().files[i]);
                    }

                    ++_number;
                    break;
                
                case 2:
                    string zipName = path + "restore-point-" + _number + ".zip";
                    _restorePoints.Add(new FullPoint(zipName, Sizing(_listOfFiles),  DateTime.Now, _listOfFiles, _number, zipName));
                    
                    if (Directory.Exists(path + @"\temp\"))
                        throw new ExceptionDirectoryExist("Такая директория уже существует, ошибка создания архива");
                    Directory.CreateDirectory(path + @"\temp\");
                    
                    for (int i = 0; i < _listOfFiles.Count; ++i)
                    {
                        if (File.Exists(path + @"\temp\" +_restorePoints.Last().files[i]))
                            throw new ExceptionFileExist("Такой файл уже существует");
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

            //Cleaning();
        }
        
        public void CreateDeltaRestorePoint(string path)
        {
            CheckDirectory(path);
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
                    
                    if (Directory.Exists(path + @"\temp\"))
                        throw new ExceptionDirectoryExist("Такая директория уже существует, ошибка создания архива");
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

            //Cleaning();
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
            return dif;
        }
        private List<string> FindDifferenceInZip(IPoints point)
        {
            var dif = new List<string>();
            
            var currentDir = Path.GetDirectoryName(point._path);
            
            if (Directory.Exists(currentDir + @"\temp\"))
                throw new ExceptionDirectoryExist("Такая директория уже создана, ошибка с архивом");
            Directory.CreateDirectory(currentDir + @"\temp\");
            ZipFile.ExtractToDirectory(point._path, currentDir + @"\temp\");
            
            for (int i = 0; i < _listOfFiles.Count; ++i)
            {
                CheckFile(_listOfFiles[i]);
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

                CheckFile(currentDir + @"\temp\"+FindFile.First());
                FileInfo fileInfoPoint = new FileInfo(currentDir + @"\temp\"+FindFile.First());
                if (fileInfoList.Length - fileInfoPoint.Length != 0)
                {
                    dif.Add(_listOfFiles[i]);
                }
            }

            Directory.Delete(currentDir + @"\temp\", true);
            return dif;
        }
        
        private long Sizing(List<string> files)
        {
            long size = 0;

            foreach (var file in files)
            {
                CheckFile(file);
                FileInfo fileInfo = new FileInfo(file);
                size += fileInfo.Length;
            }

            return size;
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
        
        public void Cleaning(int flagForCleaning)
        {
            int len = 2;
            var dateTimeSpan = new TimeSpan().Days;
            dateTimeSpan = 2;
            long maxSize = 200;
            switch (flagForCleaning)
            {
                case 1:
                    Console.WriteLine("Выберите максимальное кол-во точек");
                    len = int.Parse(Console.ReadLine());
                    break;
                case 2:
                    Console.WriteLine("Выберите максимальное кол-во дней, которое может хранится точка");
                    dateTimeSpan = int.Parse(Console.ReadLine());

                    break;
                case 3:
                    Console.WriteLine("Выберите максимальный размер бэкапа в байтах");
                    maxSize = int.Parse(Console.ReadLine());

                    break;
            }
            
            int count;
            switch (flagForCleaning)
            {
                case 1:
                    //int len = 2;

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
                    //var dateTimeSpan = new TimeSpan().Days;
                    //dateTimeSpan = 2;
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
                        {
                            if ((dateNow - _restorePoints[indexOfFullPoint + i]._date).Days > dateTimeSpan)
                            {
                                SaveToDelbyDate.Add(_restorePoints[indexOfFullPoint + i]);
                                count++;
                            }
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
                    
                    if (_backupSize < maxSize)
                        break;
                    var currentSize = _backupSize;

                    int countPoints = 0;
                    foreach (var point in _restorePoints)
                    {
                        currentSize -= point._size;
                        ++countPoints;

                        if (currentSize < maxSize)
                            break;
                    }

                    var FirstStepPointsToDeletebySize =
                        from x in _restorePoints
                        where x is FullPoint && _restorePoints.IndexOf(x) < countPoints
                        select x;

                    {
                        var SecondStepPointsToDeletebySize =
                            from x in FirstStepPointsToDeletebySize
                            where x.IndexOfDeltas == 0
                            select x;
                        
                        foreach (var point in SecondStepPointsToDeletebySize)
                        {
                            SaveToDelbySize.Add(point);
                        }
                    }

                    var ThirdStepPointsToDeletebySize =
                        from x in FirstStepPointsToDeletebySize
                        where x.IndexOfDeltas > 0
                        select x;

                    count = 0;
                    currentSize = _backupSize;
                    foreach (var point in ThirdStepPointsToDeletebySize)
                    {
                        int indexOfFullPoint = _restorePoints.IndexOf(point);
                        for (int i = 1; i <= point.IndexOfDeltas; ++i)
                        {
                            if (currentSize - _restorePoints[indexOfFullPoint + i]._size > _backupSize)
                            {
                                SaveToDelbySize.Add(_restorePoints[indexOfFullPoint + i]);
                                count++;
                            }
                        }
                        
                        point.IndexOfDeltas -= count;
                        
                        if (point.IndexOfDeltas == 0)
                        {
                            SaveToDelbySize.Add(point);
                        }
                    }
                    
                    foreach (var point in SaveToDelbySize)
                    {
                        _backupSize -= point._size;
                        _restorePoints.Remove(point);
                    }
                    
                    break;
            }
        }
        
        public void Cleaning(List<int> FlagsForCleaning)
        {
            Console.WriteLine("Выберите как комбинировать");
            Console.WriteLine("1) Нужно удалить точку, если вышла за хотя бы один установленный лимит\n2) Нужно удалить точку, если вышла за все установленные лимиты");

            int Combo = int.Parse(Console.ReadLine());
            Console.WriteLine("Убирать по\n1) Максимум точек\n2) Минимум точек");
            int MaxMin = int.Parse(Console.ReadLine());
            
            int count;

            List<IPoints> SaveToDelbyLen = new List<IPoints>();
            List<IPoints> SaveToDelbyDate = new List<IPoints>();
            List<IPoints> SaveToDelbySize = new List<IPoints>();
            
            int len = 2;
            var dateTimeSpan = new TimeSpan().Days;
            dateTimeSpan = 2;
            long maxSize = 200;
            
            for(int i = 0; i < FlagsForCleaning.Count; ++i)
            switch (FlagsForCleaning[i])
            {
                case 1:
                    Console.WriteLine("Выберите максимальное кол-во точек");
                    len = int.Parse(Console.ReadLine());
                    break;
                case 2:
                    Console.WriteLine("Выберите максимальное кол-во дней, которое может хранится точка");
                    dateTimeSpan = int.Parse(Console.ReadLine());

                    break;
                case 3:
                    Console.WriteLine("Выберите максимальный размер бэкапа в байтах");
                    maxSize = int.Parse(Console.ReadLine());

                    break;
            }
            
            for(int j = 0; j < FlagsForCleaning.Count; ++j)
            switch (FlagsForCleaning[j])
            {
                case 1:
                    if (_restorePoints.Count > len)
                    {
                        
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

                        /*foreach (var point in SaveToDelbyLen)
                        {
                            _restorePoints.Remove(point);
                        }*/
                    }
                    break;
                case 2:
                    
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
                        {
                            if ((dateNow - _restorePoints[indexOfFullPoint + i]._date).Days > dateTimeSpan)
                            {
                                SaveToDelbyDate.Add(_restorePoints[indexOfFullPoint + i]);
                                count++;
                            }
                        }
                        
                        point.IndexOfDeltas -= count;

                        if (point.IndexOfDeltas == 0)
                        {
                            SaveToDelbyDate.Add(point);
                        }
                    }

                    /*foreach (var point in SaveToDelbyDate)
                    {
                        _restorePoints.Remove(point);
                    }*/

                    break;
                case 3:
                    
                    if (_backupSize < maxSize)
                        break;
                    var currentSize = _backupSize;

                    int countPoints = 0;
                    foreach (var point in _restorePoints)
                    {
                        currentSize -= point._size;
                        ++countPoints;

                        if (currentSize < maxSize)
                            break;
                    }

                    var FirstStepPointsToDeletebySize =
                        from x in _restorePoints
                        where x is FullPoint && _restorePoints.IndexOf(x) < countPoints
                        select x;

                    {
                        var SecondStepPointsToDeletebySize =
                            from x in FirstStepPointsToDeletebySize
                            where x.IndexOfDeltas == 0
                            select x;
                        
                        foreach (var point in SecondStepPointsToDeletebySize)
                        {
                            SaveToDelbySize.Add(point);
                        }
                    }

                    var ThirdStepPointsToDeletebySize =
                        from x in FirstStepPointsToDeletebySize
                        where x.IndexOfDeltas > 0
                        select x;

                    count = 0;
                    currentSize = _backupSize;
                    foreach (var point in ThirdStepPointsToDeletebySize)
                    {
                        int indexOfFullPoint = _restorePoints.IndexOf(point);
                        for (int i = 1; i <= point.IndexOfDeltas; ++i)
                        {
                            if (currentSize - _restorePoints[indexOfFullPoint + i]._size > _backupSize)
                            {
                                SaveToDelbySize.Add(_restorePoints[indexOfFullPoint + i]);
                                count++;
                            }
                        }
                        
                        point.IndexOfDeltas -= count;
                        
                        if (point.IndexOfDeltas == 0)
                        {
                            SaveToDelbySize.Add(point);
                        }
                    }
                    
                    /*foreach (var point in SaveToDelbySize)
                    {
                        _backupSize -= point._size;
                        _restorePoints.Remove(point);
                    }*/
                    
                    break;
            }

            List<List<IPoints>> listPoints = new List<List<IPoints>>() { SaveToDelbyLen, SaveToDelbyDate, SaveToDelbySize };
            
            switch (Combo)
            {
                case 1:
                    switch (MaxMin)
                    {
                        case 1:
                            var MaxPoint =
                                (from x in listPoints
                                orderby x.Count descending
                                select x).Take(1);

                            if (MaxPoint.First().Count > 0)
                            {
                                foreach (var point in MaxPoint.First())
                                {
                                    _backupSize -= point._size;
                                    _restorePoints.Remove(point);
                                }
                            }
                            break;
                        case 2:
                            var MinPoint =
                                (from x in listPoints
                                    where x.Count != 0
                                    orderby x.Count 
                                    select x).Take(1);

                            if (MinPoint.First().Count > 0)
                            {
                                foreach (var point in MinPoint.First())
                                {
                                    _backupSize -= point._size;
                                    _restorePoints.Remove(point);
                                }
                            }
                            break;
                    }
                    break;
                case 2:
                    switch (MaxMin)
                    {
                        case 1:
                            var MaxFull =
                                (from x in listPoints
                                    orderby x.Count descending
                                    select x).Take(1);


                            if (MaxFull.First().Count > 0)
                            {

                                if (FlagsForCleaning.Contains(1) && FlagsForCleaning.Contains(2)) // Len & Date
                                {
                                    var PointToDel =
                                        from x in MaxFull.First()
                                        where SaveToDelbyLen.Contains(x) && SaveToDelbyDate.Contains(x)
                                        select x; 
                                
                                    foreach (var point in PointToDel)
                                    {
                                        _backupSize -= point._size;
                                        _restorePoints.Remove(point);
                                    }
                                }
                                else if (FlagsForCleaning.Contains(1) && FlagsForCleaning.Contains(3)) // Len & Size
                                {
                                    var PointToDel =
                                        from x in MaxFull.First()
                                        where SaveToDelbyLen.Contains(x) && SaveToDelbySize.Contains(x)
                                        select x; 
                                
                                    foreach (var point in PointToDel)
                                    {
                                        _backupSize -= point._size;
                                        _restorePoints.Remove(point);
                                    }
                                }
                                else if (FlagsForCleaning.Contains(2) && FlagsForCleaning.Contains(2)) // Date & Size
                                {
                                    var PointToDel =
                                        from x in MaxFull.First()
                                        where SaveToDelbyDate.Contains(x) && SaveToDelbySize.Contains(x)
                                        select x; 
                                
                                    foreach (var point in PointToDel)
                                    {
                                        _backupSize -= point._size;
                                        _restorePoints.Remove(point);
                                    }
                                }
                                else // Len & Date & Size
                                {
                                    var PointToDel =
                                        from x in MaxFull.First()
                                        where SaveToDelbyLen.Contains(x) && SaveToDelbyDate.Contains(x) && SaveToDelbySize.Contains(x)
                                        select x; 
                                
                                    foreach (var point in PointToDel)
                                    {
                                        _backupSize -= point._size;
                                        _restorePoints.Remove(point);
                                    }
                                }
                            }
                            break;
                        case 2:
                            var MinFull =
                                (from x in listPoints
                                    where x.Count != 0
                                    orderby x.Count 
                                    select x).Take(1);

                            if (MinFull.First().Count > 0)
                            {
                                if (FlagsForCleaning.Contains(1) && FlagsForCleaning.Contains(2)) // Len & Date
                                {
                                    var PointToDel =
                                        from x in MinFull.First()
                                        where SaveToDelbyLen.Contains(x) && SaveToDelbyDate.Contains(x)
                                        select x;

                                    foreach (var point in PointToDel)
                                    {
                                        _backupSize -= point._size;
                                        _restorePoints.Remove(point);
                                    }
                                }
                                else if (FlagsForCleaning.Contains(1) && FlagsForCleaning.Contains(3)) // Len & Size
                                {
                                    var PointToDel =
                                        from x in MinFull.First()
                                        where SaveToDelbyLen.Contains(x) && SaveToDelbySize.Contains(x)
                                        select x;

                                    foreach (var point in PointToDel)
                                    {
                                        _backupSize -= point._size;
                                        _restorePoints.Remove(point);
                                    }
                                }
                                else if (FlagsForCleaning.Contains(2) && FlagsForCleaning.Contains(3)) // Date & Size
                                {
                                    var PointToDel =
                                        from x in MinFull.First()
                                        where SaveToDelbyDate.Contains(x) && SaveToDelbySize.Contains(x)
                                        select x;

                                    foreach (var point in PointToDel)
                                    {
                                        _backupSize -= point._size;
                                        _restorePoints.Remove(point);
                                    }
                                }
                                else // Len & Date & Size
                                {
                                    var PointToDel =
                                        from x in MinFull.First()
                                        where SaveToDelbyLen.Contains(x) && SaveToDelbyDate.Contains(x) && SaveToDelbySize.Contains(x)
                                        select x;

                                    foreach (var point in PointToDel)
                                    {
                                        _backupSize -= point._size;
                                        _restorePoints.Remove(point);
                                    }
                                }
                            }
                            break;
                    }
                    break;
            }
            
        }

        private void CheckDirectory(string path)
        {
            if (!Directory.Exists(path))
                throw new Exceptions.ExceptionDirectoryDoesNotExist("Нет такой директории");
        }
        
        private void CheckFile(string path)
        {
            if (!File.Exists(path))
                throw new Exceptions.ExceptionDirectoryDoesNotExist("Нет такой директории");
        }
        
    }
}