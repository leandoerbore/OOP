using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Backup.CreationRestorePointsAlgorithms
{
    public class CreateDeltaRestorePoint : ICreateRestorePoint
    {
        private List<string> difList;
        
        public void CreateRestorePointSeparately(string path,FileBackup backup)
        {
            var fullPoint = FindLastFullRestorePoint(backup.restorePoints);
            if (fullPoint == null)
            {
                throw new ExceptionFullPointDontFound("Не найден последний FullPoint");
            }
            difList = FindDifference(fullPoint, backup.listOfFiles);
            if (difList.Count == 0)
            {
                Console.WriteLine("Нет изменений с последнего FullPoint");
                return;
            }
            
            backup.restorePoints.Add(new IncrementralPoint(path, Sizing(backup.listOfFiles),  DateTime.Now, difList, backup._number));

            for (int i = 0; i < difList.Count; ++i)
            {
                File.Copy(difList[i],  path+ backup.restorePoints.Last().files[i]);
            }

            fullPoint.IndexOfDeltas += 1;
        }

        public void CreateRestorePointZip(string path,FileBackup backup)
        {
            var fullPoint = FindLastFullRestorePoint(backup.restorePoints);
            if (fullPoint == null)
            {
                throw new ExceptionFullPointDontFound("Не найден последний FullPoint");
            }
            difList = FindDifference(fullPoint, backup.listOfFiles);
            if (difList.Count == 0)
            {
                Console.WriteLine("Нет изменений с последнего FullPoint");
                return;
            }
            
            string zipName = path + "restore-point-" + backup._number + ".zip";
            backup.restorePoints.Add(new IncrementralPoint(zipName, Sizing(backup.listOfFiles),  DateTime.Now, difList, backup._number, zipName));
                    
            if (Directory.Exists(path + @"\temp\"))
                throw new ExceptionDirectoryExist("Такая директория уже существует, ошибка создания архива");
            Directory.CreateDirectory(path + @"\temp\");
                    
            for (int i = 0; i < difList.Count; ++i)
            {
                File.Copy(backup.listOfFiles[i], path + @"\temp\" + backup.restorePoints.Last().files[i]);
            }

            ZipFile.CreateFromDirectory(path+"temp", zipName);
                    
            Directory.Delete(path + @"\temp\", true);
                    
            FileInfo fileInfo = new FileInfo(zipName);
            backup.restorePoints.Last()._size = fileInfo.Length;
                    
                    
            fullPoint.IndexOfDeltas += 1;
        }
        
        public long Sizing(List<string> files)
        {
            long size = 0;

            foreach (var file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                size += fileInfo.Length;
            }

            return size;
        }
        
        public IPoints FindLastFullRestorePoint(List<IPoints> restorePoints)
        {
            var fullPoint = restorePoints.Find(point => point is FullPoint);

            return fullPoint == null
                ? throw new ExceptionFullPointDontFound("Последний FullPoint не найден") 
                : fullPoint;
        }
        
        public List<string> FindDifference(IPoints point, List<string> listOfFiles)
        {
            var dif = new List<string>();

            for (int i = 0; i < listOfFiles.Count; ++i)
            {
                FileInfo fileInfoList = new FileInfo(listOfFiles[i]);
                var FindFile =
                    from x in point.files
                    where x.LastIndexOf(fileInfoList.Name) != -1
                    select x;

                if (FindFile.Count() == 0)
                {
                    dif.Add(listOfFiles[i]);
                    continue;
                }

                FileInfo fileInfoPoint = new FileInfo(point._path+FindFile.First());
                if (fileInfoList.Length - fileInfoPoint.Length != 0)
                {
                    dif.Add(listOfFiles[i]);
                }
            }
            return dif;
        }

        public List<string> FindDifferenceInZip(IPoints point, FileBackup backup)
        {
            var dif = new List<string>();
            
            var currentDir = Path.GetDirectoryName(point._path);
            
            if (Directory.Exists(currentDir + @"\temp\"))
                throw new ExceptionDirectoryExist("Такая директория уже создана, ошибка с архивом");
            Directory.CreateDirectory(currentDir + @"\temp\");
            ZipFile.ExtractToDirectory(point._path, currentDir + @"\temp\");
            
            for (int i = 0; i < backup.listOfFiles.Count; ++i)
            {
                backup.CheckFile(backup.listOfFiles[i]);
                FileInfo fileInfoList = new FileInfo(backup.listOfFiles[i]);
                var FindFile =
                    from x in point.files
                    where x.LastIndexOf(fileInfoList.Name) != -1
                    select x;

                if (FindFile.Count() == 0)
                {
                    dif.Add(backup.listOfFiles[i]);
                    continue;
                }

                backup.CheckFile(currentDir + @"\temp\"+FindFile.First());
                FileInfo fileInfoPoint = new FileInfo(currentDir + @"\temp\"+FindFile.First());
                if (fileInfoList.Length - fileInfoPoint.Length != 0)
                {
                    dif.Add(backup.listOfFiles[i]);
                }
            }

            Directory.Delete(currentDir + @"\temp\", true);
            return dif;
        }
    }
}