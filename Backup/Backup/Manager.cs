using System;
using System.Collections.Generic;
using System.Linq;
using Backup.CreationRestorePointsAlgorithms;

namespace Backup
{
    public static class Manager
    {
        static List<FileBackup> _backups = new List<FileBackup>();

        public static void CreateBackup(CreationMode creationMode)
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
        
        public static int CreateBackup(List<string> files, CreationMode creationMode)
        {
            int id = _backups.Count;
            _backups.Add(new FileBackup(id, files, creationMode));

            return _backups.Count - 1;
        }

        public static int CreateRestorePoint(int idBackup, string path)
        {
            CheckIdBackup(idBackup);
            _backups[idBackup].CreateRestorePoint(path, new CreateFullRestorePoint());
            return _backups[idBackup].restorePoints.Count - 1;
        }

        public static void ChangeComboMode(int idBackup, Combo option)
        {
            CheckIdBackup(idBackup);
            _backups[idBackup].comboOption = option;
        }
        public static int CreateDeltaRestorePoint(int idBackup, string path)
        {
            CheckIdBackup(idBackup);
            _backups[idBackup].CreateDeltaRestorePoint(path, new CreateDeltaRestorePoint());
            return _backups[idBackup].restorePoints.Count - 1;
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

        public static List<string> GetRestorePoints(int idBackup)
        {
            CheckIdBackup(idBackup);
            List<string> pointsNames = new List<string>();
            foreach (var point in _backups[idBackup].restorePoints)
            {
                pointsNames.Add(point.RestorePointName);
            }

            return pointsNames;
        }
        public static void CleanRestorePoints(int idBackup, ICleaningAlgorithm algorithm)
        {
            CheckIdBackup(idBackup);
            
            _backups[idBackup].Cleaning(algorithm);
        }
        
        public static void CleanRestorePoints(int idBackup, List<ICleaningAlgorithm> algorithms)
        {
            CheckIdBackup(idBackup);
            
            _backups[idBackup].Cleaning(algorithms);
        }

        public static void ChangeCreationMode(int idBackup, CreationMode option)
        {
            CheckIdBackup(idBackup);
            
            _backups[idBackup].creationMode = option;
        }

        public static void ChangeParametrForCleaning(int idBackup, CleaningParametrs option, int param)
        {
            CheckIdBackup(idBackup);
            
            switch (option)
            {
                case CleaningParametrs.LENGTH:
                    _backups[idBackup].length = param;
                    break;
                
                case CleaningParametrs.SIZE:
                    _backups[idBackup].maxSize = param;
                    break;
                case CleaningParametrs.DATE:
                    _backups[idBackup].dateTimeSpan = param;
                    break;
            }
        }

        public static long GetBackupSize(int idBackup)
        {
            CheckIdBackup(idBackup);

            return _backups[idBackup].backupSize;
        }

        public static void ChangeTime(int idBackup, DateTime date)
        {
            _backups[idBackup].ChangeTime(date);
        }
        

        private static void CheckIdBackup(int idBackup)
        {
            if (idBackup > _backups.Count || idBackup < 0)
                throw new ExceptionInvalidIndexOfBackup("Не найдет бэкап по такому индексу");
        }
        
        
        
    }
}