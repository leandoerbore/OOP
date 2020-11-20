using System;
using System.Collections.Generic;
using System.IO;

namespace Backup
{
    public class FullPoint : RestorePoint
    {
        public int IndexOfDeltas { get; set; } = 0;
        /*private List<int> _indexOfDeltas = new List<int>();
        
        public List<int> IndexOfDeltas
        {
            get => _indexOfDeltas; 
            set => _indexOfDeltas = value; 
        } */

        public FullPoint(string path, long size, DateTime date, List<string> files, int number) 
            : base(path, size, date, files, number)
        {
        }

        public FullPoint(string path, long size, DateTime date, List<string> files, int number, string zipName) 
            : base(path, size, date, files, number, zipName)
        {
        }

        
    }
}