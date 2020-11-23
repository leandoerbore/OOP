using System;
using System.Collections.Generic;

namespace Backup
{
    public class FullPoint : RestorePoint
    {
        public override int IndexOfDeltas { get; set; } = 0;

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