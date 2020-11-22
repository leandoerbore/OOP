using System;
using System.Collections.Generic;
using System.IO;

namespace Backup
{
    public abstract class RestorePoint : IPoints
    {
        public abstract int IndexOfDeltas { get; set; }
        public DateTime _date { get; set; }
        public long _size { get; set; }
        public string _path { get; set; } 
        
        private List<string> _files = new List<string>();

        public List<string> files
        {
            get => _files; 
            set => _files = value; 
        }

        public RestorePoint()
        {
            
        }
        public RestorePoint(string path, long size, DateTime date, List<string> files, int number)
        {
            _date = date;
            _size = size;
            _path = path;
            _files = Rename(files, number);
        }
        
        public RestorePoint(string path, long size, DateTime date, List<string> files, int number, string zipName)
        {
            _date = date;
            _size = size;
            _path = path;
            _files = RenameZip(files, number);
        }

        private List<string> Rename(List<string> files, int number)
        {
            var RenamedFiles = new List<string>();
            
            foreach (var file in files)
            {
                var Name = Path.GetFileName(file);
                RenamedFiles.Add( "restore-point-" + number + "_" + Name);
            }
            
            return RenamedFiles;
        }
        
        private List<string> RenameZip(List<string> files, int number)
        {
            var RenamedFiles = new List<string>();
            
            foreach (var file in files)
            {
                var Name = Path.GetFileName(file);
                RenamedFiles.Add( "restore-point-" + number + "_" + Name);
            }
            
            return RenamedFiles;
        }

        
    }
}