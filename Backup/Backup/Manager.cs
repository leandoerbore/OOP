using System;
using System.Collections.Generic;

namespace Backup
{
    public class Manager
    {
        List<FileBackup> _backups = new List<FileBackup>();

        public void CreateBackup()
        {
            int id = _backups.Count;
            _backups.Add(new FileBackup(id));
            Console.WriteLine("You create backup with id {0}", id);
        }
        
        public void CreateBackup(List<string> files)
        {
            int id = _backups.Count;
            _backups.Add(new FileBackup(id, files));
            Console.WriteLine("You create backup with id {0}", id);
        }

        public void CreateRestorePoint(int idBackup, string path)
        {
            _backups[idBackup].CreateRestorePoint(path);
        }
        
        public void CreateDeltaRestorePoint(int idBackup, string path)
        {
            _backups[idBackup].CreateDeltaRestorePoint(path);
        }
        public void ShowSizeOfBackup(int idBackup)
        {
            Console.WriteLine(_backups[idBackup]._backupSize);
        }
        public void ShowCreationTime(int idBackup)
        {
            Console.WriteLine(_backups[idBackup]._creationTime);
        }
        public void AddFileToBackup(int idBackup, string path )
        {
            _backups[idBackup].AddFilesToBackup(path);
        }

        public void DeleteFileFromBackup(int idBackup)
        {
            _backups[idBackup].DeleteFilesForBackup();
        }
        public void ShowBackupFiles(int idBackup)
        {
            _backups[idBackup].ShowAllFilesForBackup();
        }

        public void ShowBackupRestorePoints(int idBackup)
        {
            foreach (var point in _backups[idBackup].restorePoints)
            {
                Console.WriteLine("---------------------------------------------------");
                for(int i = 0; i < point.files.Count; ++i)
                    Console.WriteLine("{0}) {1}", i+1, point.files[i]);
                Console.WriteLine("---------------------------------------------------");
            }
        }
    }
}