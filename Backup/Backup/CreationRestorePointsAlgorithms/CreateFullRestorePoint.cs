using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Backup.CreationRestorePointsAlgorithms
{
    public class CreateFullRestorePoint : ICreateRestorePoint
    {
        public void CreateRestorePointSeparately(string path, FileBackup backup)
        {
            backup.restorePoints.Add(new FullPoint(path, Sizing(backup.listOfFiles),  DateTime.Now, backup.listOfFiles, backup._number));
                    

            for (int i = 0; i < backup.listOfFiles.Count; ++i)
            {
                if (File.Exists(path + backup.restorePoints))
                    throw new ExceptionFileExist("Такой файл уже существует");
                File.Copy(backup.listOfFiles[i],  path+backup.restorePoints.Last().files[i]);
            }
        }

        public void CreateRestorePointZip(string path, FileBackup backup)
        {
            string zipName = path + "restore-point-" + backup._number + ".zip";
            backup.restorePoints.Add(new FullPoint(zipName, Sizing(backup.listOfFiles),  DateTime.Now, backup.listOfFiles, backup._number, zipName));
                    
            if (Directory.Exists(path + @"\temp\"))
                throw new ExceptionDirectoryExist("Такая директория уже существует, ошибка создания архива");
            Directory.CreateDirectory(path + @"\temp\");
                    
            for (int i = 0; i < backup.listOfFiles.Count; ++i)
            {
                if (File.Exists(path + @"\temp\" +backup.restorePoints.Last().files[i]))
                    throw new ExceptionFileExist("Такой файл уже существует");
                File.Copy(backup.listOfFiles[i], path + @"\temp\" +backup.restorePoints.Last().files[i]);
            }

            ZipFile.CreateFromDirectory(path+"temp", zipName);
                    
            Directory.Delete(path + @"\temp\", true);
                    
            FileInfo fileInfo = new FileInfo(zipName);
            backup.restorePoints.Last()._size = fileInfo.Length;
        }

        public long Sizing(List<string> files)
        {
            throw new System.NotImplementedException();
        }
    }
}