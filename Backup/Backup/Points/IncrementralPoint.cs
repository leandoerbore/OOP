using System;
using System.Collections.Generic;

namespace Backup
{
    public class IncrementralPoint : RestorePoint
    {
        public int IndexOfDeltas { get; set; } = -1;
        public IncrementralPoint() : base()
        {
        }
        public IncrementralPoint(string path, long size, DateTime date, List<string> files, int number) : base(path, size, date, files, number)
        {
        }

        public IncrementralPoint(string path, long size, DateTime date, List<string> files, int number, string zipName) : base(path, size, date, files, number, zipName)
        {
        }

        
    }
}