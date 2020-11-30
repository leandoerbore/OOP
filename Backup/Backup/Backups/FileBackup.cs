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

        public int len { get; set; } = 1;
        public int dateTimeSpan = new TimeSpan(2,0,0,0).Days;
        public long maxSize = 150;
        

        private int creationMode = 1; // Для выбора архивом или раздельно
        public int Id { get; }
        public DateTime CreationTime { get; }
        public long BackupSize { get; private set; } = 0;
        private List<IPoints> _restorePoints = new List<IPoints>();

        public FileBackup(int id)
        {
            Id = id;
            CreationTime = DateTime.Now;
        }
        
        public FileBackup(int id, List<string> files)
        {
            Id = id;
            CreationTime = DateTime.Now;

            foreach (var file in files)
            {
                CheckFile(file);   
                AddFilesToBackup(file);
            }
        }
        
        public void CreateRestorePoint(string path)
        {
            CheckDirectory(path);

            switch (creationMode)
            {
                case 1:
                    _restorePoints.Add(new FullPoint(path, Sizing(_listOfFiles),  DateTime.Now, _listOfFiles, _number));
                    BackupSize += _restorePoints.Last()._size;

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
                    BackupSize += _restorePoints.Last()._size;
                    ++_number;
                    break;
            }

        }
        
        public void CreateDeltaRestorePoint(string path)
        {
            CheckDirectory(path);
            
            var fullPoint = FindLastFullRestorePoint();
            if (fullPoint == -1)
            {
                throw new ExceptionFullPointDontFound("Не найден последний FullPoint");
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
                Console.WriteLine("Нет изменений с последнего FullPoint");
                return;
            }
            
            switch (creationMode)
            {
                case 1:
                    _restorePoints.Add(new IncrementralPoint(path, Sizing(_listOfFiles),  DateTime.Now, difList, _number));
                    BackupSize += _restorePoints.Last()._size;

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
                    BackupSize += _restorePoints.Last()._size;
                    
                    _restorePoints[fullPoint].IndexOfDeltas += 1;
                    ++_number;
                    break;
                
            }
        }

        private int FindLastFullRestorePoint()
        {
            var fullPoint = _restorePoints.FindLastIndex(point => point is FullPoint);

            return fullPoint == -1 
                ? throw new ExceptionFullPointDontFound("Последний FullPoint не найден") 
                : fullPoint;
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
        
        
        
        
        
        
        
                                             
        /*public void CleaningDefault()
        {
            int count;
            switch (flagForCleaningDefault)
            {
                case 1:
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
                    
                    if (BackupSize < maxSize)
                        break;
                    var currentSize = BackupSize;

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
                    currentSize = BackupSize;
                    foreach (var point in ThirdStepPointsToDeletebySize)
                    {
                        int indexOfFullPoint = _restorePoints.IndexOf(point);
                        for (int i = 1; i <= point.IndexOfDeltas; ++i)
                        {
                            if (currentSize - _restorePoints[indexOfFullPoint + i]._size > BackupSize)
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
                        BackupSize -= point._size;
                        _restorePoints.Remove(point);
                    }
                    
                    break;
            }
        }*/
        
        /*public void CleaningHybrid()
        {
            int count;

            List<IPoints> SaveToDelbyLen = new List<IPoints>();
            List<IPoints> SaveToDelbyDate = new List<IPoints>();
            List<IPoints> SaveToDelbySize = new List<IPoints>();

            for(int j = 0; j < flagsForCleaningHybrid.Count; ++j)
            switch (flagsForCleaningHybrid[j])
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

                    break;
                case 3:
                    
                    if (BackupSize < maxSize)
                        break;
                    var currentSize = BackupSize;

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
                    currentSize = BackupSize;
                    foreach (var point in ThirdStepPointsToDeletebySize)
                    {
                        int indexOfFullPoint = _restorePoints.IndexOf(point);
                        for (int i = 1; i <= point.IndexOfDeltas; ++i)
                        {
                            if (currentSize - _restorePoints[indexOfFullPoint + i]._size > BackupSize)
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
                    break;
            }

            List<List<IPoints>> listPoints = new List<List<IPoints>>() { SaveToDelbyLen, SaveToDelbyDate, SaveToDelbySize };
            
            switch (combo)
            {
                case 1:
                    switch (minMax)
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
                                    BackupSize -= point._size;
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
                                    BackupSize -= point._size;
                                    _restorePoints.Remove(point);
                                }
                            }
                            break;
                    }
                    break;
                case 2:
                    switch (minMax)
                    {
                        case 1:
                            var MaxFull =
                                (from x in listPoints
                                    orderby x.Count descending
                                    select x).Take(1);


                            if (MaxFull.First().Count > 0)
                            {

                                if (flagsForCleaningHybrid.Contains(1) && flagsForCleaningHybrid.Contains(2)) // Len & Date
                                {
                                    var PointToDel =
                                        from x in MaxFull.First()
                                        where SaveToDelbyLen.Contains(x) && SaveToDelbyDate.Contains(x)
                                        select x; 
                                
                                    foreach (var point in PointToDel)
                                    {
                                        BackupSize -= point._size;
                                        _restorePoints.Remove(point);
                                    }
                                }
                                else if (flagsForCleaningHybrid.Contains(1) && flagsForCleaningHybrid.Contains(3)) // Len & Size
                                {
                                    var PointToDel =
                                        from x in MaxFull.First()
                                        where SaveToDelbyLen.Contains(x) && SaveToDelbySize.Contains(x)
                                        select x; 
                                
                                    foreach (var point in PointToDel)
                                    {
                                        BackupSize -= point._size;
                                        _restorePoints.Remove(point);
                                    }
                                }
                                else if (flagsForCleaningHybrid.Contains(2) && flagsForCleaningHybrid.Contains(2)) // Date & Size
                                {
                                    var PointToDel =
                                        from x in MaxFull.First()
                                        where SaveToDelbyDate.Contains(x) && SaveToDelbySize.Contains(x)
                                        select x; 
                                
                                    foreach (var point in PointToDel)
                                    {
                                        BackupSize -= point._size;
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
                                        BackupSize -= point._size;
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
                                if (flagsForCleaningHybrid.Contains(1) && flagsForCleaningHybrid.Contains(2)) // Len & Date
                                {
                                    var PointToDel =
                                        from x in MinFull.First()
                                        where SaveToDelbyLen.Contains(x) && SaveToDelbyDate.Contains(x)
                                        select x;

                                    foreach (var point in PointToDel)
                                    {
                                        BackupSize -= point._size;
                                        _restorePoints.Remove(point);
                                    }
                                }
                                else if (flagsForCleaningHybrid.Contains(1) && flagsForCleaningHybrid.Contains(3)) // Len & Size
                                {
                                    var PointToDel =
                                        from x in MinFull.First()
                                        where SaveToDelbyLen.Contains(x) && SaveToDelbySize.Contains(x)
                                        select x;

                                    foreach (var point in PointToDel)
                                    {
                                        BackupSize -= point._size;
                                        _restorePoints.Remove(point);
                                    }
                                }
                                else if (flagsForCleaningHybrid.Contains(2) && flagsForCleaningHybrid.Contains(3)) // Date & Size
                                {
                                    var PointToDel =
                                        from x in MinFull.First()
                                        where SaveToDelbyDate.Contains(x) && SaveToDelbySize.Contains(x)
                                        select x;

                                    foreach (var point in PointToDel)
                                    {
                                        BackupSize -= point._size;
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
                                        BackupSize -= point._size;
                                        _restorePoints.Remove(point);
                                    }
                                }
                            }
                            break;
                    }
                    break;
            }
            
        }*/

        
        public int combo { get; set; } = 1;  // 1 - нужно удалить точку, если вышла за хотя бы один установленный лимит
                                             // 2 - нужно удалить точку, если вышла за все установленные лимиты
        public int minMax { get; set; } = 1; // 1 - убирать по минимум точек
                                             // 2 - убирать по максимум точек
                                             
        public int cleaningHybridMode = 2;  // Size & Date - 1 ; Size & Len - 2; Date & Len - 3; Size & Date & Len
        
        
        public void Cleaning(string type)
        {
            List<IPoints> forRemove = new List<IPoints>();
            switch (type)
            {
                case "len":
                    forRemove = CleaningByLen();
                    foreach (var point in forRemove)
                    {
                        BackupSize -= point._size;
                        _restorePoints.Remove(point);
                    }

                    break;
                case "date":
                    forRemove = CleaningByDate();
                    foreach (var point in forRemove)
                    {
                        BackupSize -= point._size;
                        _restorePoints.Remove(point);
                    }

                    break;
                case "size":
                    forRemove = CleaningBySize();
                    foreach (var point in forRemove)
                    {
                        BackupSize -= point._size;
                        _restorePoints.Remove(point);
                    }
                    
                    break;
                case "hybrid":
                    List<IPoints> SaveToDelbyLen = new List<IPoints>();
                    List<IPoints> SaveToDelbyDate = new List<IPoints>();
                    List<IPoints> SaveToDelbySize = new List<IPoints>();
                    List<int> flagsForCleaningHybrid = new List<int>();
                    
                    switch (cleaningHybridMode)
                    {
                        case 1: // Len & Date
                            SaveToDelbyLen = CleaningByLen();
                            SaveToDelbyDate = CleaningByDate();
                            flagsForCleaningHybrid = new List<int>() {1, 2};   // Size - 1 ; Date - 2 ; Len - 3
                            
                            break;
                        case 2: // Len & Size
                            SaveToDelbyLen = CleaningByLen();
                            SaveToDelbySize = CleaningBySize();
                            flagsForCleaningHybrid = new List<int>() {1, 3};   // Size - 1 ; Date - 2 ; Len - 3
                            
                            break;
                        case 3: // Date & Size
                            SaveToDelbyDate = CleaningByDate();
                            SaveToDelbySize = CleaningByLen();
                            flagsForCleaningHybrid = new List<int>() {2, 3};   // Size - 1 ; Date - 2 ; Len - 3

                            break;
                        case 4: // Len & Date & Size
                            SaveToDelbyLen = CleaningByLen();
                            SaveToDelbyDate = CleaningByDate();
                            SaveToDelbySize = CleaningByLen();
                            flagsForCleaningHybrid = new List<int>() {1, 2, 3}; // Size - 1 ; Date - 2 ; Len - 3
                            
                            break;
                    }

                    
                    List<List<IPoints>> listPoints = new List<List<IPoints>>() 
                        { SaveToDelbyLen, SaveToDelbyDate, SaveToDelbySize };

                    switch (combo)
                    {
                        case 1:
                            switch (minMax)
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
                                            BackupSize -= point._size;
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
                                            BackupSize -= point._size;
                                            _restorePoints.Remove(point);
                                        }
                                    }
                                    break;
                            }
                            break;
                        case 2:
                            switch (minMax)
                            {
                                case 1:
                                    var MaxFull =
                                        (from x in listPoints
                                        orderby x.Count descending
                                        select x).Take(1);


                                    if (MaxFull.First().Count > 0)
                                    {
                                        if (flagsForCleaningHybrid.Contains(1) && flagsForCleaningHybrid.Contains(2)) // Len & Date
                                        { 
                                            var PointToDel =
                                                from x in MaxFull.First()
                                                where SaveToDelbyLen.Contains(x) && SaveToDelbyDate.Contains(x)
                                                select x; 
                                
                                            foreach (var point in PointToDel)
                                            {
                                                BackupSize -= point._size;
                                                _restorePoints.Remove(point);
                                            }
                                        }
                                        else if (flagsForCleaningHybrid.Contains(1) && flagsForCleaningHybrid.Contains(3)) // Len & Size
                                        {
                                            var PointToDel =
                                                from x in MaxFull.First()
                                                where SaveToDelbyLen.Contains(x) && SaveToDelbySize.Contains(x)
                                                select x; 
                                
                                        foreach (var point in PointToDel)
                                        {
                                            BackupSize -= point._size;
                                            _restorePoints.Remove(point);
                                        }
                                    }
                                    else if (flagsForCleaningHybrid.Contains(2) && flagsForCleaningHybrid.Contains(2)) // Date & Size
                                    {
                                        var PointToDel =
                                            from x in MaxFull.First()
                                            where SaveToDelbyDate.Contains(x) && SaveToDelbySize.Contains(x)
                                            select x; 
                                
                                        foreach (var point in PointToDel)
                                        {
                                            BackupSize -= point._size;
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
                                            BackupSize -= point._size;
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
                                        if (flagsForCleaningHybrid.Contains(1) && flagsForCleaningHybrid.Contains(2)) // Len & Date
                                        {
                                            var PointToDel =
                                                from x in MinFull.First()
                                                where SaveToDelbyLen.Contains(x) && SaveToDelbyDate.Contains(x)
                                                select x;

                                            foreach (var point in PointToDel)
                                            {
                                                BackupSize -= point._size;
                                                _restorePoints.Remove(point);
                                            }
                                        }
                                        else if (flagsForCleaningHybrid.Contains(1) && flagsForCleaningHybrid.Contains(3)) // Len & Size
                                        {
                                            var PointToDel =
                                                from x in MinFull.First()
                                                where SaveToDelbyLen.Contains(x) && SaveToDelbySize.Contains(x)
                                                select x;

                                            foreach (var point in PointToDel)
                                            {
                                                BackupSize -= point._size;
                                                _restorePoints.Remove(point);
                                            }
                                        }
                                        else if (flagsForCleaningHybrid.Contains(2) && flagsForCleaningHybrid.Contains(3)) // Date & Size
                                        {
                                            var PointToDel =
                                                from x in MinFull.First()
                                                where SaveToDelbyDate.Contains(x) && SaveToDelbySize.Contains(x)
                                                select x;

                                            foreach (var point in PointToDel)
                                            {
                                                BackupSize -= point._size;
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
                                                BackupSize -= point._size;
                                                _restorePoints.Remove(point);
                                            }
                                        }
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }
        
        public List<IPoints> CleaningByLen()
        {
            List<IPoints> SaveToDelbyLen = new List<IPoints>();
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


                int count = 0;
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
            }
            return SaveToDelbyLen;
        }
        public List<IPoints> CleaningByDate()
        {
            List<IPoints> SaveToDelbyDate = new List<IPoints>();
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

            int count = 0;
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

            return SaveToDelbyDate;
        }
        public List<IPoints> CleaningBySize()
        {
            List<IPoints> SaveToDelbySize = new List<IPoints>();
            if (BackupSize < maxSize)
                return SaveToDelbySize;
            var currentSize = BackupSize;

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

            int count = 0;
            currentSize = BackupSize;
            foreach (var point in ThirdStepPointsToDeletebySize)
            {
                int indexOfFullPoint = _restorePoints.IndexOf(point);
                for (int i = 1; i <= point.IndexOfDeltas; ++i)
                {
                    if (currentSize - _restorePoints[indexOfFullPoint + i]._size > BackupSize)
                    {
                        SaveToDelbySize.Add(_restorePoints[indexOfFullPoint + i]);
                        count++;
                    }
                }

                point.IndexOfDeltas -= count;
                
            }

            return SaveToDelbySize;
        }


        public void ChangeTime()
        {
            _restorePoints.First()._date = new DateTime(2020, 11, 20);
        }
        private void CheckDirectory(string path)
        {
            if (!Directory.Exists(path))
                throw new ExceptionDirectoryDoesNotExist("Нет такой директории");
        }
        
        private void CheckFile(string path)
        {
            if (!File.Exists(path))
                throw new ExceptionDirectoryDoesNotExist("Нет такой директории");
        }
        
    }
}